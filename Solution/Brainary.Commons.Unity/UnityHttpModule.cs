namespace Brainary.Commons.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using Microsoft.Practices.Unity;

    public class UnityHttpModule : IHttpModule
    {
        #region Fields

        /// <summary>Backing field for the <see cref="ParentContainer"/> property.</summary>
        private IUnityContainer parentContainer;

        /// <summary>Backing field for the <see cref="ChildContainer"/> property.</summary>
        private IUnityContainer childContainer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent container out of the application state.
        /// </summary>
        private IUnityContainer ParentContainer
        {
            get { return parentContainer ?? (parentContainer = HttpContext.Current.Application.GetContainer()); }
        }

        /// <summary>
        ///  Gets/sets the child container for the current request.
        /// </summary>
        private IUnityContainer ChildContainer
        {
            get
            {
                return childContainer;
            }

            set
            {
                childContainer = value;
                HttpContext.Current.SetChildContainer(value);
            }
        }

        #endregion

        #region Implementation of IHttpModule

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/>
        /// that provides access to the methods, properties, and events 
        /// common to all application objects within an ASP.NET application.</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextOnBeginRequest;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.EndRequest += ContextOnEndRequest;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module
        /// that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Traverses through the control tree to build up the dependencies.
        /// </summary>
        /// <param name="root">The root control to traverse.</param>
        /// <returns>
        /// Any child controls to be processed.
        /// </returns>
        private static IEnumerable<Control> GetControlTree(Control root)
        {
            if (root.HasControls())
            {
                foreach (Control child in root.Controls)
                {
                    yield return child;
                    if (!child.HasControls()) continue;
                    foreach (var c in GetControlTree(child)) yield return c;
                }
            }
        }

        #endregion

        #region Life-cycle event handlers

        /// <summary>
        /// Initializes a new child container at the beginning of each request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextOnBeginRequest(object sender, EventArgs e)
        {
            ChildContainer = ParentContainer.CreateChildContainer();
        }

        /// <summary>
        /// Registers the injection event to fire when the page has been
        /// initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            // static content; no need for a container 
            if (HttpContext.Current.Handler == null) return;

            var handler = HttpContext.Current.Handler;
            ChildContainer.BuildUp(handler.GetType(), handler);

            // User controls are ready to be built up after the page initialization in complete
            var page = handler as Page;
            if (page != null) page.InitComplete += OnPageInitComplete;
        }

        /// <summary>
        /// Build-up each control in the page's control tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageInitComplete(object sender, EventArgs e)
        {
            var page = (Page)sender;

            foreach (var c in from c in GetControlTree(page) let type = c.GetType() let typeFullName = type.FullName ?? string.Empty let baseTypeFullName = type.BaseType != null ? type.BaseType.FullName : string.Empty where !typeFullName.StartsWith("System") || !baseTypeFullName.StartsWith("System") select c)
            {
                ChildContainer.BuildUp(c.GetType(), c);
            }
        }

        /// <summary>
        /// Ensures that the child container gets disposed of properly at the end
        /// of each request cycle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextOnEndRequest(object sender, EventArgs e)
        {
            if (ChildContainer != null) ChildContainer.Dispose();
        }

        #endregion
    }
}

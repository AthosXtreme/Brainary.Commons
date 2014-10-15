namespace Brainary.Commons.Web
{
    using System;
    using System.Web.Mvc;

    using Brainary.Commons.Domain;
    using Brainary.Commons.Domain.Contracts;

    public class EntityModelBinder<T> : DefaultModelBinder where T : Entity, new()
    {
        private readonly IRepository<T> repo;

        public EntityModelBinder(IRepository<T> repository)
        {
            repo = repository;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var id = TryGetValue<int>(bindingContext, "Id");
            if (id.HasValue) bindingContext.ModelMetadata.Model = repo.FindById(id.Value);

            return base.BindModel(controllerContext, bindingContext);
        }

        private static TU? TryGetValue<TU>(ModelBindingContext bindingContext, string key) where TU : struct
        {
            if (string.IsNullOrEmpty(key)) return null;

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix)
                valueResult = bindingContext.ValueProvider.GetValue(key);

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);

            if (valueResult == null) return null;

            try
            {
                return (TU?)valueResult.ConvertTo(typeof(TU));
            }
            catch (ArgumentNullException ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }
}
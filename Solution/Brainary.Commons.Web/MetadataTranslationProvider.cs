using System.ComponentModel.DataAnnotations;
using System.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Translates all ValidationAttribute using a Resource (.resx) type
    /// </summary>
    public class MetadataTranslationProvider : IValidationMetadataProvider
    {
        private readonly ResourceManager resourceManager;
        private readonly Type resourceType;

        /// <summary>
        /// Use default Resource messages
        /// </summary>
        public MetadataTranslationProvider() : this(typeof(Resources.DataAnnotations))
        {
        }

        /// <summary>
        /// Use custom Resource messages
        /// </summary>
        /// <param name="type">Resource type</param>
        public MetadataTranslationProvider(Type type)
        {
            resourceType = type;
            resourceManager = new ResourceManager(type);
        }

        /// <summary>
        /// Set validation metadata for current context
        /// </summary>
        /// <param name="context">Provider context</param>
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
            {
                if (attribute is ValidationAttribute tAttr)
                {
                    var name = tAttr.GetType().Name;
                    if (resourceManager.GetString(name) != null)
                    {
                        tAttr.ErrorMessageResourceType = resourceType;
                        tAttr.ErrorMessageResourceName = name;
                        tAttr.ErrorMessage = null;
                    }
                }
            }
        }
    }
}

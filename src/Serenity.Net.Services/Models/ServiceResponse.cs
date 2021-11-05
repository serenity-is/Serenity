using System.Collections.Generic;

namespace Serenity.Services
{
    public class ServiceResponse
    {
        public ServiceError Error { get; set; }

        /// <summary>
        /// A custom data dictionary. Please consider subclassing the service response
        /// before passing data via this property. This should be only used in limited 
        /// cases where subclassing is not feasible.
        /// Another option could be to add a [JsonExtensionData] but that could open
        /// way to the side effect ignoring typos.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }
    }
}
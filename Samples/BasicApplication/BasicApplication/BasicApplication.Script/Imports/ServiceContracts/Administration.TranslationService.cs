namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class TranslationService
    {
        public static jQueryXmlHttpRequest List(TranslationListRequest request, Action<ListResponse<TranslationItem>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Translation/List", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(TranslationUpdateRequest request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Translation/Update", request, onSuccess, options);
        }
    }
    
}


namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class LanguageService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<LanguageRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Language/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<LanguageRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Language/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Language/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<LanguageRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Language/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<LanguageRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/Language/List", request, onSuccess, options);
        }
    }
    
}


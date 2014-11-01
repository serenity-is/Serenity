namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class UserService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<UserRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<UserRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/Undelete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<UserRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<UserRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Administration/User/List", request, onSuccess, options);
        }
    }
    
}


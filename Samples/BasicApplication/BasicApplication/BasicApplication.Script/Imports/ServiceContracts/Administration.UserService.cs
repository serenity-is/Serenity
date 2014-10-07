using Serenity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BasicApplication.Administration
{
    public partial class UserService
    {
        public static void Create(SaveRequest<UserRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<UserRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<UserRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<UserRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Administration/User/List", request, onSuccess, options);
        }
    }
    
}


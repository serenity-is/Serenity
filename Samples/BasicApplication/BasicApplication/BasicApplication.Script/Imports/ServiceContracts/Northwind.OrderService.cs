namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class OrderService
    {
        public static void Create(SaveRequest<OrderRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<OrderRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<OrderRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<OrderRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Order/List", request, onSuccess, options);
        }
    }
    
}


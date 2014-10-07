namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class OrderDetailService
    {
        public static void Create(SaveRequest<OrderDetailRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<OrderDetailRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<OrderDetailRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<OrderDetailRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/OrderDetail/List", request, onSuccess, options);
        }
    }
    
}


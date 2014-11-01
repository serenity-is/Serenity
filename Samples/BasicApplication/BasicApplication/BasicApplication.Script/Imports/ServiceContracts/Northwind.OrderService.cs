namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class OrderService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<OrderRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Order/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<OrderRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Order/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Order/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<OrderRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Order/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<OrderRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Order/List", request, onSuccess, options);
        }
    }
    
}


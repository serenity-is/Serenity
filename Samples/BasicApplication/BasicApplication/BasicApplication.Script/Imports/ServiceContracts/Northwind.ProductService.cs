namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class ProductService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<ProductRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Product/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<ProductRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Product/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Product/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<ProductRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Product/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<ProductRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Product/List", request, onSuccess, options);
        }
    }
    
}


namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class ProductService
    {
        public static void Create(SaveRequest<ProductRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<ProductRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<ProductRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<ProductRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Product/List", request, onSuccess, options);
        }
    }
    
}


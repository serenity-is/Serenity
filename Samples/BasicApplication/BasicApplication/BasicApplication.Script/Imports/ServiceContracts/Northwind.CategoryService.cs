namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class CategoryService
    {
        public static void Create(SaveRequest<CategoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Category/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<CategoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Category/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Category/Delete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<CategoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Category/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<CategoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Category/List", request, onSuccess, options);
        }
    }
    
}


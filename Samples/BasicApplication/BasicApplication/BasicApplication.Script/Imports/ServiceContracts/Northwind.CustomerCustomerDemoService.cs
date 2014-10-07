namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class CustomerCustomerDemoService
    {
        public static void Create(SaveRequest<CustomerCustomerDemoRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<CustomerCustomerDemoRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<CustomerCustomerDemoRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<CustomerCustomerDemoRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerCustomerDemo/List", request, onSuccess, options);
        }
    }
    
}


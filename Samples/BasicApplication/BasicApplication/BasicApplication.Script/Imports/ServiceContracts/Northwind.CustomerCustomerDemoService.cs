namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class CustomerCustomerDemoService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<CustomerCustomerDemoRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/CustomerCustomerDemo/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<CustomerCustomerDemoRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/CustomerCustomerDemo/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/CustomerCustomerDemo/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<CustomerCustomerDemoRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/CustomerCustomerDemo/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<CustomerCustomerDemoRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/CustomerCustomerDemo/List", request, onSuccess, options);
        }
    }
    
}


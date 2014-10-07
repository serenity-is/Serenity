namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class CustomerDemographicService
    {
        public static void Create(SaveRequest<CustomerDemographicRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<CustomerDemographicRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<CustomerDemographicRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<CustomerDemographicRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/CustomerDemographic/List", request, onSuccess, options);
        }
    }
    
}


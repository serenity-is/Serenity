namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class TerritoryService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<TerritoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Territory/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<TerritoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Territory/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Territory/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<TerritoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Territory/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<TerritoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Territory/List", request, onSuccess, options);
        }
    }
    
}


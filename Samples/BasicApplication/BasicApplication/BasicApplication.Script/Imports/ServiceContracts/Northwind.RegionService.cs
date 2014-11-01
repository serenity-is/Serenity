namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class RegionService
    {
        public static jQueryXmlHttpRequest Create(SaveRequest<RegionRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Region/Create", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Update(SaveRequest<RegionRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Region/Update", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Region/Delete", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest Retrieve(RetrieveRequest request, Action<RetrieveResponse<RegionRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Region/Retrieve", request, onSuccess, options);
        }
    
        public static jQueryXmlHttpRequest List(ListRequest request, Action<ListResponse<RegionRow>> onSuccess, ServiceCallOptions options = null)
        {
            return Q.ServiceRequest("Northwind/Region/List", request, onSuccess, options);
        }
    }
    
}


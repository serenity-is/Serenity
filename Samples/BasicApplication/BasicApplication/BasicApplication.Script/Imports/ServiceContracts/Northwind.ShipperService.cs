namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class ShipperService
    {
        public static void Create(SaveRequest<ShipperRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Shipper/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<ShipperRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Shipper/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Shipper/Delete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<ShipperRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Shipper/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<ShipperRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Shipper/List", request, onSuccess, options);
        }
    }
    
}


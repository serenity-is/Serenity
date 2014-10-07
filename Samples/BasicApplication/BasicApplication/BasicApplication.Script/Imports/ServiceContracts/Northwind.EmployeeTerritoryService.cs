namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class EmployeeTerritoryService
    {
        public static void Create(SaveRequest<EmployeeTerritoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<EmployeeTerritoryRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/Delete", request, onSuccess, options);
        }
    
        public static void Undelete(UndeleteRequest request, Action<UndeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/Undelete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<EmployeeTerritoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<EmployeeTerritoryRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/EmployeeTerritory/List", request, onSuccess, options);
        }
    }
    
}


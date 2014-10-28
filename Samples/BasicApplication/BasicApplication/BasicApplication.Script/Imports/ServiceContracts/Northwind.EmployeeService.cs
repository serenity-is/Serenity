namespace BasicApplication.Northwind
{
    using Serenity;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class EmployeeService
    {
        public static void Create(SaveRequest<EmployeeRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Employee/Create", request, onSuccess, options);
        }
    
        public static void Update(SaveRequest<EmployeeRow> request, Action<SaveResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Employee/Update", request, onSuccess, options);
        }
    
        public static void Delete(DeleteRequest request, Action<DeleteResponse> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Employee/Delete", request, onSuccess, options);
        }
    
        public static void Retrieve(RetrieveRequest request, Action<RetrieveResponse<EmployeeRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Employee/Retrieve", request, onSuccess, options);
        }
    
        public static void List(ListRequest request, Action<ListResponse<EmployeeRow>> onSuccess, ServiceCallOptions options = null)
        {
            Q.ServiceRequest("Northwind/Employee/List", request, onSuccess, options);
        }
    }
    
}


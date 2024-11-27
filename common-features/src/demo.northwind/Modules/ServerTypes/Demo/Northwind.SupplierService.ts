﻿import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { SupplierRow } from "./Northwind.SupplierRow";

export namespace SupplierService {
    export const baseUrl = 'Northwind/Supplier';

    export declare function Create(request: SaveRequest<SupplierRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<SupplierRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<SupplierRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<SupplierRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<SupplierRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<SupplierRow>>;

    export const Methods = {
        Create: "Northwind/Supplier/Create",
        Update: "Northwind/Supplier/Update",
        Delete: "Northwind/Supplier/Delete",
        Retrieve: "Northwind/Supplier/Retrieve",
        List: "Northwind/Supplier/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>SupplierService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
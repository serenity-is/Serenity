import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { GetNextNumberRequest, GetNextNumberResponse } from "@serenity-is/extensions";
import { CustomerRow } from "./Northwind.CustomerRow";

export namespace CustomerService {
    export const baseUrl = 'Serenity.Demo.Northwind/Customer';

    export declare function Create(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function GetNextNumber(request: GetNextNumberRequest, onSuccess?: (response: GetNextNumberResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetNextNumberResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CustomerRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CustomerRow>>;

    export const Methods = {
        Create: "Serenity.Demo.Northwind/Customer/Create",
        Update: "Serenity.Demo.Northwind/Customer/Update",
        Delete: "Serenity.Demo.Northwind/Customer/Delete",
        GetNextNumber: "Serenity.Demo.Northwind/Customer/GetNextNumber",
        Retrieve: "Serenity.Demo.Northwind/Customer/Retrieve",
        List: "Serenity.Demo.Northwind/Customer/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'GetNextNumber', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>CustomerService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
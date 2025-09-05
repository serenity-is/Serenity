import { DeleteRequest, DeleteResponse, ListRequest, ListResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { GetNextNumberRequest, GetNextNumberResponse } from "@serenity-is/extensions";
import { CustomerRow } from "./Northwind.CustomerRow";

export namespace CustomerService {
    export const baseUrl = 'Northwind/Customer';

    export declare function Create(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function GetNextNumber(request: GetNextNumberRequest, onSuccess?: (response: GetNextNumberResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetNextNumberResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CustomerRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CustomerRow>>;

    export const Methods = {
        Create: "Northwind/Customer/Create",
        Update: "Northwind/Customer/Update",
        Delete: "Northwind/Customer/Delete",
        GetNextNumber: "Northwind/Customer/GetNextNumber",
        Retrieve: "Northwind/Customer/Retrieve",
        List: "Northwind/Customer/List"
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
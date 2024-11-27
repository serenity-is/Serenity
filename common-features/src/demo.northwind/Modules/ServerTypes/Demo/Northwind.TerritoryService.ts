import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { TerritoryRow } from "./Northwind.TerritoryRow";

export namespace TerritoryService {
    export const baseUrl = 'Northwind/Territory';

    export declare function Create(request: SaveRequest<TerritoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<TerritoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<TerritoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<TerritoryRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<TerritoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<TerritoryRow>>;

    export const Methods = {
        Create: "Northwind/Territory/Create",
        Update: "Northwind/Territory/Update",
        Delete: "Northwind/Territory/Delete",
        Retrieve: "Northwind/Territory/Retrieve",
        List: "Northwind/Territory/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>TerritoryService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { RegionRow } from "./Northwind.RegionRow";

export namespace RegionService {
    export const baseUrl = 'Serenity.Demo.Northwind/Region';

    export declare function Create(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<RegionRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<RegionRow>>;

    export const Methods = {
        Create: "Serenity.Demo.Northwind/Region/Create",
        Update: "Serenity.Demo.Northwind/Region/Update",
        Delete: "Serenity.Demo.Northwind/Region/Delete",
        Retrieve: "Serenity.Demo.Northwind/Region/Retrieve",
        List: "Serenity.Demo.Northwind/Region/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>RegionService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
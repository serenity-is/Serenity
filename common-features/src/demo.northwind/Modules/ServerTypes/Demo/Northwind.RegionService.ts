import { DeleteRequest, DeleteResponse, ListRequest, ListResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { RegionRow } from "./Northwind.RegionRow";

export namespace RegionService {
    export const baseUrl = 'Northwind/Region';

    export declare function Create(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<RegionRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<RegionRow>>;

    export const Methods = {
        Create: "Northwind/Region/Create",
        Update: "Northwind/Region/Update",
        Delete: "Northwind/Region/Delete",
        Retrieve: "Northwind/Region/Retrieve",
        List: "Northwind/Region/List"
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
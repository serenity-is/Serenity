import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { ShipperRow } from "./Northwind.ShipperRow";

export namespace ShipperService {
    export const baseUrl = 'Northwind/Shipper';

    export declare function Create(request: SaveRequest<ShipperRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<ShipperRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ShipperRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ShipperRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<ShipperRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ShipperRow>>;

    export const Methods = {
        Create: "Northwind/Shipper/Create",
        Update: "Northwind/Shipper/Update",
        Delete: "Northwind/Shipper/Delete",
        Retrieve: "Northwind/Shipper/Retrieve",
        List: "Northwind/Shipper/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>ShipperService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
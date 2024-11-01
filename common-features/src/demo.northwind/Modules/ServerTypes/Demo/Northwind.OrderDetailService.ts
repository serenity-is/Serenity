import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { OrderDetailRow } from "./Northwind.OrderDetailRow";

export namespace OrderDetailService {
    export const baseUrl = 'Serenity.Demo.Northwind/OrderDetail';

    export declare function Create(request: SaveRequest<OrderDetailRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<OrderDetailRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<OrderDetailRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<OrderDetailRow>>;

    export const Methods = {
        Create: "Serenity.Demo.Northwind/OrderDetail/Create",
        Update: "Serenity.Demo.Northwind/OrderDetail/Update",
        Delete: "Serenity.Demo.Northwind/OrderDetail/Delete",
        Retrieve: "Serenity.Demo.Northwind/OrderDetail/Retrieve",
        List: "Serenity.Demo.Northwind/OrderDetail/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>OrderDetailService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
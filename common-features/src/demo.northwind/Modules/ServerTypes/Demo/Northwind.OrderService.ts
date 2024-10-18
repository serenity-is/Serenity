import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { OrderListRequest } from "./Northwind.OrderListRequest";
import { OrderRow } from "./Northwind.OrderRow";

export namespace OrderService {
    export const baseUrl = 'Serenity.Demo.Northwind/Order';

    export declare function Create(request: SaveRequest<OrderRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<OrderRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<OrderRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<OrderRow>>;
    export declare function List(request: OrderListRequest, onSuccess?: (response: ListResponse<OrderRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<OrderRow>>;

    export const Methods = {
        Create: "Serenity.Demo.Northwind/Order/Create",
        Update: "Serenity.Demo.Northwind/Order/Update",
        Delete: "Serenity.Demo.Northwind/Order/Delete",
        Retrieve: "Serenity.Demo.Northwind/Order/Retrieve",
        List: "Serenity.Demo.Northwind/Order/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>OrderService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
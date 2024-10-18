import { RetrieveRequest, RetrieveResponse, ServiceOptions, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { OrderDetailRow } from "./Northwind.OrderDetailRow";

export namespace OrderDetailService {
    export const baseUrl = 'Serenity.Demo.Northwind/OrderDetail';

    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<OrderDetailRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<OrderDetailRow>>;

    export const Methods = {
        Retrieve: "Serenity.Demo.Northwind/OrderDetail/Retrieve",
        List: "Serenity.Demo.Northwind/OrderDetail/List"
    } as const;

    [
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>OrderDetailService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
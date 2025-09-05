import { DeleteRequest, DeleteResponse, ListRequest, ListResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { ProductRow } from "./Northwind.ProductRow";

export namespace ProductService {
    export const baseUrl = 'Northwind/Product';

    export declare function Create(request: SaveRequest<ProductRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<ProductRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ProductRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ProductRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<ProductRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ProductRow>>;

    export const Methods = {
        Create: "Northwind/Product/Create",
        Update: "Northwind/Product/Update",
        Delete: "Northwind/Product/Delete",
        Retrieve: "Northwind/Product/Retrieve",
        List: "Northwind/Product/List"
    } as const;

    [
        'Create',
        'Update',
        'Delete',
        'Retrieve',
        'List'
    ].forEach(x => {
        (<any>ProductService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
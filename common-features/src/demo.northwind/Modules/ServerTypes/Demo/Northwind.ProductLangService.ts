import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { ProductLangRow } from "./Northwind.ProductLangRow";

export namespace ProductLangService {
    export const baseUrl = 'Northwind/ProductLang';

    export declare function Create(request: SaveRequest<ProductLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<ProductLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ProductLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ProductLangRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<ProductLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ProductLangRow>>;

    export const Methods = {
        Create: "Northwind/ProductLang/Create",
        Update: "Northwind/ProductLang/Update",
        Delete: "Northwind/ProductLang/Delete",
        Retrieve: "Northwind/ProductLang/Retrieve",
        List: "Northwind/ProductLang/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>ProductLangService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
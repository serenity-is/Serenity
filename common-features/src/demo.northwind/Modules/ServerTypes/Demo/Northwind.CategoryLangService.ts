import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { CategoryLangRow } from "./Northwind.CategoryLangRow";

export namespace CategoryLangService {
    export const baseUrl = 'Serenity.Demo.Northwind/CategoryLang';

    export declare function Create(request: SaveRequest<CategoryLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<CategoryLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CategoryLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CategoryLangRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<CategoryLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CategoryLangRow>>;

    export const Methods = {
        Create: "Serenity.Demo.Northwind/CategoryLang/Create",
        Update: "Serenity.Demo.Northwind/CategoryLang/Update",
        Delete: "Serenity.Demo.Northwind/CategoryLang/Delete",
        Retrieve: "Serenity.Demo.Northwind/CategoryLang/Retrieve",
        List: "Serenity.Demo.Northwind/CategoryLang/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>CategoryLangService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
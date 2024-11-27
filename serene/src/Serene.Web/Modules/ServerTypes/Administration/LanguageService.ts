import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { LanguageRow } from "./LanguageRow";

export namespace LanguageService {
    export const baseUrl = 'Administration/Language';

    export declare function Create(request: SaveRequest<LanguageRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<LanguageRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<LanguageRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<LanguageRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<LanguageRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<LanguageRow>>;

    export const Methods = {
        Create: "Administration/Language/Create",
        Update: "Administration/Language/Update",
        Delete: "Administration/Language/Delete",
        Retrieve: "Administration/Language/Retrieve",
        List: "Administration/Language/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>LanguageService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
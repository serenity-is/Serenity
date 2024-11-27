import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { RoleRow } from "./RoleRow";

export namespace RoleService {
    export const baseUrl = 'Administration/Role';

    export declare function Create(request: SaveRequest<RoleRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<RoleRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<RoleRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<RoleRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<RoleRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<RoleRow>>;

    export const Methods = {
        Create: "Administration/Role/Create",
        Update: "Administration/Role/Update",
        Delete: "Administration/Role/Delete",
        Retrieve: "Administration/Role/Retrieve",
        List: "Administration/Role/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>RoleService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { UserListRequest } from "./UserListRequest";
import { UserRow } from "./UserRow";

export namespace UserService {
    export const baseUrl = 'Administration/User';

    export declare function Create(request: SaveRequest<UserRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<UserRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<UserRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<UserRow>>;
    export declare function List(request: UserListRequest, onSuccess?: (response: ListResponse<UserRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<UserRow>>;

    export const Methods = {
        Create: "Administration/User/Create",
        Update: "Administration/User/Update",
        Delete: "Administration/User/Delete",
        Retrieve: "Administration/User/Retrieve",
        List: "Administration/User/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>UserService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
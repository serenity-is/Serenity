import { SaveResponse, ServiceOptions, ListResponse, ServiceRequest, serviceRequest } from "@serenity-is/corelib";
import { UserPermissionListRequest } from "./UserPermissionListRequest";
import { UserPermissionRow } from "./UserPermissionRow";
import { UserPermissionUpdateRequest } from "./UserPermissionUpdateRequest";

export namespace UserPermissionService {
    export const baseUrl = 'Administration/UserPermission';

    export declare function Update(request: UserPermissionUpdateRequest, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function List(request: UserPermissionListRequest, onSuccess?: (response: ListResponse<UserPermissionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<UserPermissionRow>>;
    export declare function ListRolePermissions(request: UserPermissionListRequest, onSuccess?: (response: ListResponse<string>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<string>>;
    export declare function ListPermissionKeys(request: ServiceRequest, onSuccess?: (response: ListResponse<string>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<string>>;

    export const Methods = {
        Update: "Administration/UserPermission/Update",
        List: "Administration/UserPermission/List",
        ListRolePermissions: "Administration/UserPermission/ListRolePermissions",
        ListPermissionKeys: "Administration/UserPermission/ListPermissionKeys"
    } as const;

    [
        'Update', 
        'List', 
        'ListRolePermissions', 
        'ListPermissionKeys'
    ].forEach(x => {
        (<any>UserPermissionService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
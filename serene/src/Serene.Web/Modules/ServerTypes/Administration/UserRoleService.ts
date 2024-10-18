import { SaveResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { UserRoleListRequest } from "./UserRoleListRequest";
import { UserRoleListResponse } from "./UserRoleListResponse";
import { UserRoleUpdateRequest } from "./UserRoleUpdateRequest";

export namespace UserRoleService {
    export const baseUrl = 'Administration/UserRole';

    export declare function Update(request: UserRoleUpdateRequest, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function List(request: UserRoleListRequest, onSuccess?: (response: UserRoleListResponse) => void, opt?: ServiceOptions<any>): PromiseLike<UserRoleListResponse>;

    export const Methods = {
        Update: "Administration/UserRole/Update",
        List: "Administration/UserRole/List"
    } as const;

    [
        'Update', 
        'List'
    ].forEach(x => {
        (<any>UserRoleService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
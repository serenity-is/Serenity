import { ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { PermissionGrantListRequest } from "./PermissionGrantListRequest";
import { PermissionGrantListResponse } from "./PermissionGrantListResponse";

export namespace PermissionGrantService {
    export const baseUrl = 'Extensions/PermissionGrant';

    export declare function List(request: PermissionGrantListRequest, onSuccess?: (response: PermissionGrantListResponse) => void, opt?: ServiceOptions<any>): PromiseLike<PermissionGrantListResponse>;

    export const Methods = {
        List: "Extensions/PermissionGrant/List"
    } as const;

    [
        'List'
    ].forEach(x => {
        (<any>PermissionGrantService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
import { ServiceResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { UserPreferenceRetrieveRequest } from "./UserPreferenceRetrieveRequest";
import { UserPreferenceRetrieveResponse } from "./UserPreferenceRetrieveResponse";
import { UserPreferenceUpdateRequest } from "./UserPreferenceUpdateRequest";

export namespace UserPreferenceService {
    export const baseUrl = 'Extensions/UserPreference';

    export declare function Update(request: UserPreferenceUpdateRequest, onSuccess?: (response: ServiceResponse) => void, opt?: ServiceOptions<any>): PromiseLike<ServiceResponse>;
    export declare function Retrieve(request: UserPreferenceRetrieveRequest, onSuccess?: (response: UserPreferenceRetrieveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<UserPreferenceRetrieveResponse>;

    export const Methods = {
        Update: "Extensions/UserPreference/Update",
        Retrieve: "Extensions/UserPreference/Retrieve"
    } as const;

    [
        'Update', 
        'Retrieve'
    ].forEach(x => {
        (<any>UserPreferenceService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
import { ListResponse, ServiceOptions, SaveResponse, serviceRequest } from "@serenity-is/corelib";
import { TranslationItem } from "./TranslationItem";
import { TranslationListRequest } from "./TranslationListRequest";
import { TranslationUpdateRequest } from "./TranslationUpdateRequest";

export namespace TranslationService {
    export const baseUrl = 'Administration/Translation';

    export declare function List(request: TranslationListRequest, onSuccess?: (response: ListResponse<TranslationItem>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<TranslationItem>>;
    export declare function Update(request: TranslationUpdateRequest, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;

    export const Methods = {
        List: "Administration/Translation/List",
        Update: "Administration/Translation/Update"
    } as const;

    [
        'List', 
        'Update'
    ].forEach(x => {
        (<any>TranslationService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
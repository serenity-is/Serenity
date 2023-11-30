import { ServiceResponse } from "@serenity-is/base"

export interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
    request?: any;
    service?: string;
    blockUI?: boolean;
    onError?(response: TResponse): void;
    onSuccess?(response: TResponse): void;
    onCleanup?(): void;
}
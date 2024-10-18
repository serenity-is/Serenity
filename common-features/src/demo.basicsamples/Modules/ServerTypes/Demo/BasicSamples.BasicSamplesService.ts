import { ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { OrdersByShipperRequest } from "./BasicSamples.OrdersByShipperRequest";
import { OrdersByShipperResponse } from "./BasicSamples.OrdersByShipperResponse";

export namespace BasicSamplesService {
    export const baseUrl = 'Serenity.Demo.BasicSamples';

    export declare function OrdersByShipper(request: OrdersByShipperRequest, onSuccess?: (response: OrdersByShipperResponse) => void, opt?: ServiceOptions<any>): PromiseLike<OrdersByShipperResponse>;

    export const Methods = {
        OrdersByShipper: "Serenity.Demo.BasicSamples/OrdersByShipper"
    } as const;

    [
        'OrdersByShipper'
    ].forEach(x => {
        (<any>BasicSamplesService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
import { ListRequest } from "@serenity-is/corelib";

export interface OrderListRequest extends ListRequest {
    ProductID?: number;
}
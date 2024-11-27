import { ServiceResponse } from "@serenity-is/corelib";

export interface OrdersByShipperResponse extends ServiceResponse {
    Values?: { [key: string]: any }[];
    ShipperKeys?: string[];
    ShipperLabels?: string[];
}
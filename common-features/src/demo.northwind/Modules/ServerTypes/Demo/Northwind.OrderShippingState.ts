import { registerEnum } from "@serenity-is/corelib";

export enum OrderShippingState {
    NotShipped = 0,
    Shipped = 1
}
registerEnum(OrderShippingState, 'Serenity.Demo.Northwind.OrderShippingState', 'Northwind.OrderShippingState');
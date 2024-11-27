import { Decorators } from "@serenity-is/corelib";

export enum OrderShippingState {
    NotShipped = 0,
    Shipped = 1
}
Decorators.registerEnumType(OrderShippingState, 'Serenity.Demo.Northwind.OrderShippingState', 'Northwind.OrderShippingState');

import { registerEnum } from "../base";
import { EnumTypeRegistry } from "./enumtyperegistry";

describe("EnumTypeRegistry", () => {
    test('can find enum type by its key', function () {

        enum OrderShippingState {
            NotShipped = 0,
            Shipped = 1
        }
        registerEnum(OrderShippingState, 'Serenity.Demo.Northwind.OrderShippingState', 'Northwind.OrderShippingState');

        var type = EnumTypeRegistry.tryGet("Northwind.OrderShippingState");
        expect(type).toBeTruthy();
        expect(type === OrderShippingState).toBe(true);
    });
});
import { addCustomAttribute, EnumKeyAttribute, getGlobalTypeRegistry, registerEnum } from "../base";
import { EnumTypeRegistry } from "./enumtyperegistry";

beforeEach(() => {
    const typeRegistry = getGlobalTypeRegistry();
    Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
    EnumTypeRegistry.reset();
});

describe("EnumTypeRegistry", () => {

    it('can find enum type by its key', function () {

        enum OrderShippingState {
            NotShipped = 0,
            Shipped = 1
        }
        registerEnum(OrderShippingState, 'Serenity.Demo.Northwind.OrderShippingState', 'Northwind.OrderShippingState');

        var type = EnumTypeRegistry.tryGet("Northwind.OrderShippingState");
        expect(type).toBeTruthy();
        expect(type === OrderShippingState).toBe(true);
    });

    it('can find enum type by its full name', function () {
        enum OrderStatus {
            Pending = 0,
            Completed = 1
        }
        registerEnum(OrderStatus, 'MyProject.OrderStatus');

        const type = EnumTypeRegistry.tryGet("MyProject.OrderStatus");
        expect(type).toBe(OrderStatus);
    });

    it('returns undefined for non-existent enum', function () {
        const type = EnumTypeRegistry.tryGet("NonExistent.Enum");
        expect(type).toBeUndefined();
    });

    it('get throws error for non-existent enum', function () {
        expect(() => EnumTypeRegistry.get("NonExistent.Enum")).toThrow();
    });

    it('can find enum with EnumKeyAttribute', function () {
        enum PaymentMethod {
            Cash = 0,
            Card = 1
        }

        // Add EnumKeyAttribute to the enum
        addCustomAttribute(PaymentMethod, new EnumKeyAttribute("Payment.Method"));

        registerEnum(PaymentMethod, 'MyApp.PaymentMethod');

        const type = EnumTypeRegistry.tryGet("Payment.Method");
        expect(type).toBe(PaymentMethod);
    });

    it('prefers EnumKeyAttribute over registered key', function () {
        enum UserRole {
            Admin = 0,
            User = 1
        }

        registerEnum(UserRole, 'MyApp.UserRole', 'User.Role');

        // Add EnumKeyAttribute
        addCustomAttribute(UserRole, new EnumKeyAttribute("Role.User"));

        // Should find by EnumKeyAttribute, not by registered enumKey
        const type1 = EnumTypeRegistry.tryGet("Role.User");
        expect(type1).toBe(UserRole);

        // Should also find by registered name
        const type2 = EnumTypeRegistry.tryGet("MyApp.UserRole");
        expect(type2).toBe(UserRole);
    });

    it('reset clears the registry cache', function () {
        enum Status {
            Active = 0,
            Inactive = 1
        }
        registerEnum(Status, 'Test.Status');

        // First call should find it
        const type1 = EnumTypeRegistry.tryGet("Test.Status");
        expect(type1).toBe(Status);

        // Reset should clear cache
        EnumTypeRegistry.reset();

        // After reset, should still find it (since it's in the global registry)
        const type2 = EnumTypeRegistry.tryGet("Test.Status");
        expect(type2).toBe(Status);
    });

    it('tryGetOrLoad returns enum synchronously when found', function () {
        enum Priority {
            Low = 0,
            High = 1
        }
        registerEnum(Priority, 'Test.Priority');

        const type = EnumTypeRegistry.tryGetOrLoad("Test.Priority");
        expect(type).toBe(Priority);
    });

    it('getOrLoad returns enum when found', function () {
        enum Category {
            A = 0,
            B = 1
        }
        registerEnum(Category, 'Test.Category');

        const type = EnumTypeRegistry.getOrLoad("Test.Category");
        expect(type).toBe(Category);
    });

    it('getOrLoad throws error for non-existent enum', function () {
        expect(() => EnumTypeRegistry.getOrLoad("NonExistent.Enum")).toThrow();
    });

    it('handles enums with numeric string keys', function () {
        enum ErrorCode {
            NotFound = 404,
            Unauthorized = 401
        }
        registerEnum(ErrorCode, 'Http.ErrorCode');

        const type = EnumTypeRegistry.tryGet("Http.ErrorCode") as typeof ErrorCode;
        expect(type).toBe(ErrorCode);
        expect(type.NotFound).toBe(404);
        expect(type.Unauthorized).toBe(401);
    });

    it('handles enums with string values', function () {
        enum Color {
            Red = "RED",
            Blue = "BLUE"
        }
        registerEnum(Color, 'UI.Color');

        const type = EnumTypeRegistry.tryGet("UI.Color") as typeof Color;
        expect(type).toBe(Color);
        expect(type.Red).toBe("RED");
        expect(type.Blue).toBe("BLUE");
    });

    it('works with multiple registered enums', function () {
        enum Status1 {
            New = 0,
            Old = 1
        }

        enum Status2 {
            Active = 0,
            Inactive = 1
        }

        registerEnum(Status1, 'Test.Status1');
        registerEnum(Status2, 'Test.Status2');

        const type1 = EnumTypeRegistry.tryGet("Test.Status1");
        const type2 = EnumTypeRegistry.tryGet("Test.Status2");

        expect(type1).toBe(Status1);
        expect(type2).toBe(Status2);
        expect(type1).not.toBe(type2);
    });

    it('loadError provides helpful error message', function () {
        const consoleSpy = vi.fn();
        const originalError = console.error;
        console.error = consoleSpy;

        try {
            EnumTypeRegistry.get("Definitely.Not.Found.Enum");
            expect(true).toBe(false); // Should not reach here
        } catch (e: any) {
            expect(e.message).toContain("was not found");
            expect(e.message).toContain("Definitely.Not.Found.Enum");
            expect(e.message).toContain("dotnet sergen servertypings");
        } finally {
            console.error = originalError;
        }
    });

    it('handles enum with no namespace', function () {
        enum SimpleEnum {
            Value1 = 1,
            Value2 = 2
        }
        registerEnum(SimpleEnum, 'SimpleEnum');

        const type = EnumTypeRegistry.tryGet("SimpleEnum");
        expect(type).toBe(SimpleEnum);
    });

    it('can find type registered after initialization', function () {
        const typeRegistry = getGlobalTypeRegistry();
        Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
        enum TestEnum1 { Test = 1 }
        enum TestEnum2 { Test = 2 }
        registerEnum(TestEnum1, 'Test.MyEnum1');
        const type1 = EnumTypeRegistry.tryGet("Test.MyEnum1");
        expect(type1).toBe(TestEnum1);
        let type2 = EnumTypeRegistry.tryGet("Test.MyEnum2");
        expect(type2).toBeUndefined();
        registerEnum(TestEnum2, 'Test.MyEnum2');
        type2 = EnumTypeRegistry.tryGet("Test.MyEnum2");
        expect(type2).toBe(TestEnum2);
    });

});
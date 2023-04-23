import { Enum } from "@/q/system";
import { Decorators } from "@/decorators";

test('Enum.getValues returns correct values', function() {
    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
});

test('Enum.getValues returns correct values for registered enums', function() {

    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    Decorators.registerEnum(Test, 'EnumGetValuesRegisteredEnums.Test')

    expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
});
import { getType, registerEnum, isEnum, getTypeFullName } from "@Q/System";

function expectTypeDetails(enumObj: any, name: string) {
    expect(isEnum(enumObj)).toBe(true);
    expect(enumObj.__interface).toBeNull();

    expect(enumObj.__interfaces).toBeUndefined();
    expect(enumObj.__metadata).toBeUndefined();

    if (name != null) {
        expect(getTypeFullName(enumObj)).toBe(name);
        expect(enumObj.__typeName).toBe(name);
        expect(enumObj.__typeName$).toBe(name);
        expect(getType(name)).toStrictEqual(enumObj);
    }
    else {
        var fullName = getTypeFullName(enumObj);
        expect(fullName).toBe('Object');
        expect(enumObj.__typeName).toBeUndefined();
        expect(enumObj.__typeName$).toBeUndefined();
        expect(getType(fullName) == null).toBe(true);
    }

    expect(enumObj.__isAssignableFrom).toBeUndefined();
}

test('no name', function() {

    enum Test {
        A = 1,
        B = 2
    }

    const name = 'Test_Enum_NoName';
    registerEnum(Test, null);
    expectTypeDetails(Test, null);
});

test('with name', function() {

    enum Test {
        A = 1,
        B = 2
    }

    const name = 'Test_Enum_With_Name';
    registerEnum(Test, name);
});
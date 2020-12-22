import { registerType, TypeKind, isEnum, getTypeFullName } from "@Q/System";

test('registerType with Enum works properly', function() {

    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    registerType(Test, 'TestName', TypeKind.Enum);

    expect(isEnum(Test)).toBe(true);
    expect(getTypeFullName(Test as any)).toBe('TestName');

    // implementation details
    expect((Test as any).__register).toBeUndefined();
    expect((Test as any).__typeName).toBe('TestName');
    expect((Test as any).__typeName$).toBe('TestName');
});
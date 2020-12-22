import { getType, registerType, TypeKind, isEnum, getTypeFullName } from "@Q/System";

function expectTypeDetails(klass: any, name: string, kind: TypeKind, intf?: any[]) {
    if (kind !== TypeKind.Enum)
        expect(isEnum(klass)).toBe(false);
    else
        expect(isEnum(klass)).toBe(true);

    if (intf != null) {
        expect(klass.__interfaces).toStrictEqual(intf);
    }
    else
        expect(klass.__interfaces).toBeUndefined();

    expect(klass.__metadata).toBeUndefined();
    expect(klass.__typeKind).toBe(kind);
    
    if (name != null) {
        expect(klass.__register).toBeUndefined();
        expect(getTypeFullName(klass)).toBe(name);
        expect(klass.__typeName).toBe(name);
        expect(klass.__typeName$).toBe(name);

        expect(getType(name)).toStrictEqual(klass);
    }
    else {
        expect(klass.__register).toBe(true);
        var fullName = getTypeFullName(klass);
        if (typeof klass == "function")
            expect(fullName).toBe(klass.name);
        else
            expect(fullName).toBe('Object');
        expect(klass.__typeName).toBeUndefined();
        expect(klass.__typeName$).toBeUndefined();

        expect(getType(fullName) == null).toBe(true);
    }

    if (kind === TypeKind.Interface)
        expect(typeof klass.__isAssignableFrom).toBe("function");
    else 
        expect(klass.__isAssignableFrom).toBeUndefined();
}

test('no name and no kind', function() {

    class Test {
    }

    const name = 'Test_NoName_NoKind';
    registerType(Test, null);
    expectTypeDetails(Test, null, TypeKind.Class);
});

test('no name and interface kind', function() {

    class Test {
    }

    const name = 'Test_NoName_InterfaceKind';
    registerType(Test, null, TypeKind.Interface);
    expectTypeDetails(Test, null, TypeKind.Interface);
});

test('no name and class kind', function() {

    class Test {
    }

    const name = 'Test_NoName_ClassKind';
    registerType(Test, null, TypeKind.Class);
    expectTypeDetails(Test, null, TypeKind.Class);
});


test('no name and enum kind', function() {

    enum Test {
        A = 1
    }

    const name = 'Test_NoName_EnumKind';
    registerType(Test, null, TypeKind.Enum);
    expectTypeDetails(Test, null, TypeKind.Enum);
});

test('no kind', function() {

    class Test {
    }

    const name = 'Test_NoKind';
    registerType(Test, name);
    expectTypeDetails(Test, name, TypeKind.Class);
});

test('class kind', function() {

    class Test {
    }

    const name = 'Test_ClassKind';
    registerType(Test, name, TypeKind.Enum);
    expectTypeDetails(Test, name, TypeKind.Enum);
});

test('interface kind', function() {

    class Test {
    }

    const name = 'Test_InterfaceKind';
    registerType(Test, name, TypeKind.Enum);
    expectTypeDetails(Test, name, TypeKind.Enum);
});

test('enum kind', function() {

    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    const name = 'Test_EnumKind';
    registerType(Test, name, TypeKind.Enum);
    expectTypeDetails(Test, name, TypeKind.Enum);
});

test('with interfaces', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    const name = 'Test_WithInterfaces';
    registerType(Test, name, TypeKind.Class, [Intf1, Intf2]);
    expectTypeDetails(Test, name, TypeKind.Class, [Intf1, Intf2]);
});

test('derived class with null interface list null derives interfaces as is', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    class Derived extends Test {
    }

    const nameTest = 'Test_DerivedClassNullIntfDerivesInterfacesAsIs';
    const nameDerived = 'Derived_DerivedClassNullIntfDerivesInterfacesAsIs';
    registerType(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);

    registerType(Derived, nameDerived, TypeKind.Class, null);
    expectTypeDetails(Derived, nameDerived, TypeKind.Class, [Intf1, Intf2]);
    // should be the same reference
    expect((Derived as any).__interfaces === (Test as any).__interfaces).toBe(true);
});

test('derived class with empty interface list derives interfaces as a copy', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    class Derived extends Test {
    }

    const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsACopy';
    const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsACopy';
    registerType(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);

    registerType(Derived, nameDerived, TypeKind.Class, []);
    expectTypeDetails(Derived, nameDerived, TypeKind.Class, [Intf1, Intf2]);
    // should be the same reference
    expect((Derived as any).__interfaces === (Test as any).__interfaces).toBe(true);
});

test('derived class with extra interface list derives interfaces as a copy', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    class Intf3 {
    }

    class Derived extends Test {
    }

    const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsCopy';
    const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsCopy';
    registerType(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);

    registerType(Derived, nameDerived, TypeKind.Class, [Intf3]);
    expectTypeDetails(Derived, nameDerived, TypeKind.Class, [Intf1, Intf2, Intf3]);
    
    // should not be the same reference
    expect((Derived as any).__interfaces !== (Test as any).__interfaces).toBe(true);

    // check base class again to make sure its interfaces are not modified
    expectTypeDetails(Test, nameTest, TypeKind.Class, [Intf1, Intf2]);
});

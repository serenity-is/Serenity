import { getType, registerClass, isEnum, getTypeFullName } from "@Q/System";

function expectTypeDetails(klass: any, name: string, intf?: any[]) {
    expect(isEnum(klass)).toBe(false);

    if (intf != null) {
        expect(klass.__interfaces).toStrictEqual(intf);
    }
    else
        expect(klass.__interfaces).toBeUndefined();

    expect(klass.__metadata).toBeUndefined();
    expect(klass.__interface).toBe(false);

    if (name != null) {
        expect(getTypeFullName(klass)).toBe(name);
        expect(klass.__typeName).toBe(name);
        expect(klass.__typeName$).toBe(name);

        expect(getType(name)).toStrictEqual(klass);
    }
    else {
        var fullName = getTypeFullName(klass);
        expect(fullName).toBe(klass.name);
        expect(klass.__typeName).toBeUndefined();
        expect(klass.__typeName$).toBeUndefined();

        expect(getType(fullName) == null).toBe(true);
    }

    expect(klass.__isAssignableFrom).toBeUndefined();
}

test('no name', function() {

    class Test {
    }

    const name = 'Test_Class_NoName';
    registerClass(Test, null);
    expectTypeDetails(Test, null);
});

test('with name', function() {

    class Test {
    }

    const name = 'Test_Class_NoKind';
    registerClass(Test, name);
    expectTypeDetails(Test, name);
});

test('no name with interfaces', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    registerClass(Test, null, [Intf1, Intf2]);
    expectTypeDetails(Test, null, [Intf1, Intf2]);
});

test('with name and interfaces', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class Test {
    }

    const name = 'Test_Class_With_Name_And_Interfaces';
    registerClass(Test, name, [Intf1, Intf2]);
    expectTypeDetails(Test, name, [Intf1, Intf2]);
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
    registerClass(Test, nameTest, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, [Intf1, Intf2]);

    registerClass(Derived, nameDerived, null);
    expectTypeDetails(Derived, nameDerived, [Intf1, Intf2]);
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
    registerClass(Test, nameTest, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, [Intf1, Intf2]);

    registerClass(Derived, nameDerived, []);
    expectTypeDetails(Derived, nameDerived, [Intf1, Intf2]);
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
    registerClass(Test, nameTest, [Intf1, Intf2]);
    expectTypeDetails(Test, nameTest, [Intf1, Intf2]);

    registerClass(Derived, nameDerived, [Intf3]);
    expectTypeDetails(Derived, nameDerived, [Intf1, Intf2, Intf3]);
    
    // should not be the same reference
    expect((Derived as any).__interfaces !== (Test as any).__interfaces).toBe(true);

    // check base class again to make sure its interfaces are not modified
    expectTypeDetails(Test, nameTest, [Intf1, Intf2]);
});
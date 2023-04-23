import { getType, registerInterface, isEnum, getTypeFullName, getTypeNameProp } from "@/q/system";

function expectTypeDetails(klass: any, name: string, intf?: any[]) {
    expect(isEnum(klass)).toBe(false);

    if (intf != null) {
        expect(klass.__interfaces).toStrictEqual(intf);
    }
    else
        expect(klass.__interfaces).toBeUndefined();

    expect(klass.__metadata).toBeUndefined();
    expect(klass.__interface).toBe(true);

    if (name != null) {
        expect(getTypeFullName(klass)).toBe(name);
        expect(getTypeNameProp(klass)).toBe(name);

        expect(getType(name)).toStrictEqual(klass);
    }
    else {
        var fullName = getTypeFullName(klass);
        expect(fullName).toBe(klass.name);
        expect(getTypeNameProp(klass));

        expect(getType(fullName) == null).toBe(true);
    }

    expect(typeof klass.__isAssignableFrom).toBe("function");
}

test('no name', function() {

    class ITest {
    }

    const name = 'ITest_Interface_NoName';
    registerInterface(ITest, null);
    expectTypeDetails(ITest, null);
});

test('with name', function() {

    class ITest {
    }

    const name = 'ITest_Interface_NoKind';
    registerInterface(ITest, name);
    expectTypeDetails(ITest, name);
});

test('no name with interfaces', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class ITest {
    }

    registerInterface(ITest, null, [Intf1, Intf2]);
    expectTypeDetails(ITest, null, [Intf1, Intf2]);
});

test('with name and interfaces', function() {

    class Intf1 {
    }

    class Intf2 {
    }

    class ITest {
    }

    const name = 'Test_Interface_With_Name_And_Interfaces';
    registerInterface(ITest, name, [Intf1, Intf2]);
    expectTypeDetails(ITest, name, [Intf1, Intf2]);
});

test('derived class with null interface list null derives interfaces as is', function() {

    // NOTE: interfaces are not supposed to be derived

    class Intf1 {
    }

    class Intf2 {
    }

    class ITest {
    }

    class IDerived extends ITest {
    }

    const nameTest = 'Test_DerivedClassNullIntfDerivesInterfacesAsIs';
    const nameDerived = 'Derived_DerivedClassNullIntfDerivesInterfacesAsIs';
    registerInterface(ITest, nameTest, [Intf1, Intf2]);
    expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

    registerInterface(IDerived, nameDerived, null);
    expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2]);
    // should be the same reference
    expect((IDerived as any).__interfaces === (ITest as any).__interfaces).toBe(true);
});

test('derived class with empty interface list derives interfaces as a copy', function() {

    // NOTE: interfaces are not supposed to be derived classes
    // just testing copy of __interfaces here

    class Intf1 {
    }

    class Intf2 {
    }

    class ITest {
    }

    class IDerived extends ITest {
    }

    const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsACopy';
    const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsACopy';
    registerInterface(ITest, nameTest, [Intf1, Intf2]);
    expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

    registerInterface(IDerived, nameDerived, []);
    expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2]);
    // should be the same reference
    expect((IDerived as any).__interfaces === (ITest as any).__interfaces).toBe(true);
});

test('derived class with extra interface list derives interfaces as a copy', function() {

    // NOTE: interfaces are not supposed to be derived classes
    // just testing copy of __interfaces here

    class Intf1 {
    }

    class Intf2 {
    }

    class ITest {
    }

    class Intf3 {
    }

    class IDerived extends ITest {
    }

    const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsCopy';
    const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsCopy';
    registerInterface(ITest, nameTest, [Intf1, Intf2]);
    expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);

    registerInterface(IDerived, nameDerived, [Intf3]);
    expectTypeDetails(IDerived, nameDerived, [Intf1, Intf2, Intf3]);
    
    // should not be the same reference
    expect((IDerived as any).__interfaces !== (ITest as any).__interfaces).toBe(true);

    // check base class again to make sure its interfaces are not modified
    expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);
});
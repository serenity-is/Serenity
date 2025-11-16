import { implementedInterfacesSymbol, isAssignableFromSymbol, isInstanceOfTypeSymbol } from "./symbols";
import { CustomAttribute, EditorAttribute, Enum, ISlickFormatter, addCustomAttribute, classTypeInfo, editorTypeInfo, fieldsProxy, formatterTypeInfo, getBaseType, getCustomAttribute, getCustomAttributes, getInstanceType, getType, getTypeFullName, getTypeNameProp, getTypeShortName, hasCustomAttribute, initFormType, interfaceTypeInfo, isAssignableFrom, isEnum, isInstanceOfType, registerClass, registerEditor, registerEnum, registerFormatter, registerInterface, registerType, type InterfaceType, type InterfaceTypeInfo } from "./system";
import { ensureTypeInfo, getGlobalTypeRegistry, peekTypeInfo } from "./system-internal";

afterEach(() => {
    const typeRegistry = getGlobalTypeRegistry();
    Object.keys(typeRegistry).forEach(k => delete typeRegistry[k]);
});

describe("Enum.getValues", () => {
    it('returns correct values', function () {
        enum Test {
            Some = 1,
            Other = 5,
            Another = -1
        }

        expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
    });

    it('returns correct values for registered enums', function () {

        enum Test {
            Some = 1,
            Other = 5,
            Another = -1
        }

        registerEnum(Test, 'EnumGetValuesRegisteredEnums.Test')

        expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
    });
});

describe("Enum.toString", () => {
    enum Test {
        Some = 1,
        Other = 5,
        Another = -1
    }

    it('returns empty string for null', () => {
        expect(Enum.toString(Test, null)).toBe("");
        expect(Enum.toString(Test, undefined)).toBe("");
    });

    it('returns the value as string if value is not a number', () => {
        expect(Enum.toString(Test, "test" as any)).toBe("test");
        expect(Enum.toString(Test, "0" as any)).toBe("0");
    });

    it('returns names', function () {
        expect(Enum.toString(Test, 1)).toBe("Some");
        expect(Enum.toString(Test, 5)).toBe("Other");
        expect(Enum.toString(Test, -1)).toBe("Another");
    });

    it('returns number if non-existent', function () {
        expect(Enum.toString(Test, 0)).toBe("0");
        expect(Enum.toString(Test, -5)).toBe("-5");
    });

    it('works with flags', () => {
        enum Test2 {
            Some = 1,
            Other = 2,
            Another = 4
        }

        ensureTypeInfo(Test2 as any).enumFlags = true;
        expect(Enum.toString(Test2, Test2.Some + Test2.Another)).toBe("Some | Another");
        expect(Enum.toString(Test2, Test2.Some + Test2.Another + 16)).toBe("Some | Another | 16");
    });
});

namespace Module1 {
    export class ISome { 
        declare static [Symbol.typeInfo]: InterfaceTypeInfo<"ISome">;
    }

    registerInterface(ISome, "ISome");
}

namespace CopyModule1 {
    export class ISome { 
        declare static [Symbol.typeInfo]: InterfaceTypeInfo<"ISome">;
    }
    registerInterface(ISome, "ISome");
}

class Module1Class { }
registerClass(Module1Class, "SomeClassUsingCopy1", [Module1.ISome]);

class CopyModule1Class { }
registerClass(CopyModule1Class, "SomeClassUsingCopy2", [CopyModule1.ISome]);

describe("isAssignableFrom", () => {

    it("interfaces can also be matched by their registration names", function () {
        expect(isAssignableFrom(Module1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(Module1.ISome, CopyModule1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, Module1Class)).toBe(true);
        expect(isAssignableFrom(CopyModule1.ISome, CopyModule1Class)).toBe(true);
    });

    it("interfaces are matched by their registration names even when class name is different", function () {
        class ISomeMeSome { }
        registerInterface(ISomeMeSome, "ISome")
        expect(isAssignableFrom(ISomeMeSome, Module1Class)).toBe(true);
        expect(isAssignableFrom(ISomeMeSome, CopyModule1Class)).toBe(true);
    });

    it("interfaces with different class names and registration names won't match", function () {
        class IOther { }
        registerInterface(IOther, "IOther")
        expect(isAssignableFrom(IOther, Module1Class)).toBe(false);
        expect(isAssignableFrom(IOther, CopyModule1Class)).toBe(false);
    });

    it("interfaces with same class names but different registration names won't match", function () {
        class ISome { 
            declare static [Symbol.typeInfo]: InterfaceTypeInfo<"ISomeDiff">;
        }
        registerInterface(ISome, "ISomeDiff")
        class X { }
        registerClass(X, "X", [ISome])
        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });

    it("classes that are not registered as interfaces won't match", function () {
        class ISome extends CustomAttribute { 
        }
        class X { }
        registerClass(X, "X", [ISome])
        expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
    });

    it("returns true for same class", function () {
        expect(isAssignableFrom(String, String)).toBe(true);
    });

    it("returns true for sub class", function () {
        class A { }
        class B extends A { }
        expect(isAssignableFrom(A, B)).toBe(true);
        expect(isAssignableFrom(B, A)).toBe(false);
    });

});



describe("registerClass", () => {
    function expectClassDetails(klass: any, name: string, intf?: InterfaceType[], attrs?: CustomAttribute[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass[implementedInterfacesSymbol]).toStrictEqual(intf);
        }
        else
            expect(klass[implementedInterfacesSymbol]).toBeUndefined();

        if (attrs != null) {
            expect(peekTypeInfo(klass)?.customAttributes).toStrictEqual(attrs);
        }
        else
            expect(peekTypeInfo(klass)?.customAttributes).toBeUndefined();
        expect(peekTypeInfo(klass)?.enumFlags).toBeUndefined();

        if (name != null) {
            expect(getTypeFullName(klass)).toBe(name);
            expect(getTypeNameProp(klass)).toBe(name);
            expect(getType(name)).toStrictEqual(klass);
        }
        else {
            const fullName = getTypeFullName(klass);
            expect(fullName).toBe(klass.name);
            expect(getTypeNameProp(klass)).toBeUndefined();
            expect(getType(fullName) == null).toBe(true);
        }

        expect(klass[isAssignableFromSymbol]).toBeUndefined();
    }

    it('works with no name', function () {

        class Test {
        }

        const name = 'Test_Class_NoName';
        registerClass(Test, null);
        expectClassDetails(Test, null);
    });

    it('works with with name', function () {

        class Test {
        }

        const name = 'Test_Class_NoKind';
        registerClass(Test, name);
        expectClassDetails(Test, name);
    });

    it('works with no name with interfaces', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }
        
        class Test {
        }

        registerClass(Test, null, [Intf1, Intf2]);
        expectClassDetails(Test, null, [Intf1, Intf2]);
    });

    it('works with name and interfaces', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class Test {
        }

        const name = 'Test_Class_With_Name_And_Interfaces';
        registerClass(Test, name, [Intf1, Intf2]);
        expectClassDetails(Test, name, [Intf1, Intf2]);
    });

    it('works with derived class with null interface list null derives interfaces as is', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class Test {
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassNullIntfDerivesInterfacesAsIs';
        const nameDerived = 'Derived_DerivedClassNullIntfDerivesInterfacesAsIs';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, null);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((Derived as any)[implementedInterfacesSymbol] === (Test as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with empty interface list derives interfaces as a copy', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class Test {
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsACopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsACopy';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, []);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2]);
        // should be the same reference
        expect((Derived as any)[implementedInterfacesSymbol] === (Test as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with extra interface list derives interfaces as a copy', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class Test {
        }

        class Intf3 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf3");
        }

        class Derived extends Test {
        }

        const nameTest = 'Test_DerivedClassEmptyIntfDerivesInterfacesAsCopy';
        const nameDerived = 'Derived_DerivedEmptyIntfClassDerivesInterfacesAsCopy';
        registerClass(Test, nameTest, [Intf1, Intf2]);
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);

        registerClass(Derived, nameDerived, [Intf3]);
        expectClassDetails(Derived, nameDerived, [Intf1, Intf2, Intf3]);

        // should not be the same reference
        expect((Derived as any)[implementedInterfacesSymbol] !== (Test as any)[implementedInterfacesSymbol]).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectClassDetails(Test, nameTest, [Intf1, Intf2]);
    });

    it("works with types that already have a static [Symbol.typeInfo] property with typeName", () => {
        class MyClass {
            static [Symbol.typeInfo] = classTypeInfo("MyClassName");
        }

        registerClass(MyClass, null);
        expect(getType("MyClassName")).toBe(MyClass);
    });

    it('works with attribute instances', function () {
        class MyAttr extends CustomAttribute {
        }

        class Test {
        }

        const attr = new MyAttr();
        const name = 'Test_Class_With_Attr_Instance';
        registerClass(Test, name, [attr]);
        expectClassDetails(Test, name, undefined, [attr]);
    });

    it('works with attribute classes', function () {
        class MyAttr extends CustomAttribute {
        }

        class Test {
        }

        const name = 'Test_Class_With_Attr_Class';
        registerClass(Test, name, [MyAttr]);
        expectClassDetails(Test, name, undefined, [new MyAttr()]);
    });

    it('works with attribute factories', function () {
        class MyAttr extends CustomAttribute {
        }

        function myAttrFactory() {
            return new MyAttr();
        }
        myAttrFactory.isAttributeFactory = true;

        class Test {
        }

        const name = 'Test_Class_With_Attr_Factory';
        registerClass(Test, name, [myAttrFactory]);
        expectClassDetails(Test, name, undefined, [new MyAttr()]);
    });

    it('works with mixed interfaces and attributes', function () {
        class Intf {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf");
        }

        class MyAttr extends CustomAttribute {
        }

        class Test {
        }

        const attr = new MyAttr();
        const name = 'Test_Class_With_Intf_And_Attr';
        registerClass(Test, name, [Intf, attr]);
        expectClassDetails(Test, name, [Intf], [attr]);
    });
});

describe("registerEnum", () => {
    function expectTypeDetails(enumObj: any, name: string) {
        expect(isEnum(enumObj)).toBe(true);
        expect(enumObj[Symbol.typeInfo]).toBeDefined();
        expect(peekTypeInfo(enumObj)?.typeKind).toBe("enum");
        expect(enumObj[implementedInterfacesSymbol]).toBeUndefined();
        expect(peekTypeInfo(enumObj)?.enumFlags).toBeUndefined();

        if (name != null) {
            const fullName = getTypeNameProp(enumObj);
            expect(fullName).toBe(name);
            expect(enumObj[fullName]).toBe(name);
            expect(getType(name)).toStrictEqual(enumObj);
        }
        else {
            const fullName = getTypeFullName(enumObj);
            expect(fullName).toBe('Object');
            expect(enumObj[fullName]).toBeUndefined();
            expect(getType(fullName) == null).toBe(true);
        }

        expect(enumObj[isAssignableFromSymbol]).toBeUndefined();
    }

    it('works with no name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        registerEnum(Test, null);
        expectTypeDetails(Test, null);
    });

    it('works with name', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_With_Name';
        registerEnum(Test, name);
    });

    it('works with enum key', function () {

        enum Test {
            A = 1,
            B = 2
        }

        const name = 'Test_Enum_Name';
        const key = 'Test_Enum_Key';
        registerEnum(Test, name, key);
        expect(getType(name)).toBe(Test);
        expect(getType(key)).toBe(Test);
    });

    it('throws if enumType is not object or null', function () {
        expect(() => registerEnum("not object", "name")).toThrow("Enum type is required in registerEnum!");
        expect(() => registerEnum(null, "name")).toThrow("Enum type is required in registerEnum!");
    });

    it('throws if name ends with dot', function () {
        enum Test {
            A = 1
        }
        expect(() => registerEnum(Test, "name.")).toThrow("Enum name cannot end with a dot in registerEnum!");
    });
});

describe("registerEditor", () => {
    it('works with attributes', function () {
        class MyAttr extends CustomAttribute {
        }

        class TestEditor {
        }

        const name = 'Test_Editor_With_Attr';
        registerEditor(TestEditor, name, [MyAttr]);
        expect(hasCustomAttribute(TestEditor, EditorAttribute)).toBe(true);
        expect(hasCustomAttribute(TestEditor, MyAttr)).toBe(true);
    });
});

describe("registerFormatter", () => {
    it('works with attributes', function () {
        class MyAttr extends CustomAttribute {
        }

        class TestFormatter {
        }

        const name = 'Test_Formatter_With_Attr';
        registerFormatter(TestFormatter, name, [MyAttr]);
        expect(hasCustomAttribute(TestFormatter, MyAttr)).toBe(true);
        expect(isAssignableFrom(ISlickFormatter, TestFormatter)).toBe(true);
    });
});

describe("registerInterface", () => {
    function expectTypeDetails(klass: any, name: string, intf?: InterfaceType[]) {
        expect(isEnum(klass)).toBe(false);

        if (intf != null) {
            expect(klass[implementedInterfacesSymbol]).toStrictEqual(intf);
        }
        else
            expect(klass[implementedInterfacesSymbol]).toBeUndefined();

        expect(peekTypeInfo(klass)?.enumFlags).toBeUndefined();
        expect(peekTypeInfo(klass)?.typeKind).toBe("interface");

        if (name != null) {
            expect(getTypeFullName(klass)).toBe(name);
            expect(getTypeNameProp(klass)).toBe(name);

            expect(getType(name)).toStrictEqual(klass);
        }
        else {
            const fullName = getTypeFullName(klass);
            expect(fullName).toBe(klass.name);
            expect(getTypeNameProp(klass));

            expect(getType(fullName) == null).toBe(true);
        }

        expect(typeof klass[isAssignableFromSymbol]).toBe("function");
    }

    it('works with no name', function () {

        class ITest {
        }

        registerInterface(ITest, null);
        expectTypeDetails(ITest, null);
    });

    it('works with with name', function () {

        class ITest {
        }

        const name = 'ITest_Interface_NoKind';
        registerInterface(ITest, name);
        expectTypeDetails(ITest, name);
    });

    it('works with no name with interfaces', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class ITest {
        }

        registerInterface(ITest, null, [Intf1, Intf2]);
        expectTypeDetails(ITest, null, [Intf1, Intf2]);
    });

    it('works with name and interfaces', function () {

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class ITest {
        }

        const name = 'Test_Interface_With_Name_And_Interfaces';
        registerInterface(ITest, name, [Intf1, Intf2]);
        expectTypeDetails(ITest, name, [Intf1, Intf2]);
    });

    it('works with derived class with null interface list null derives interfaces as is', function () {

        // NOTE: interfaces are not supposed to be derived

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
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
        expect((IDerived as any)[implementedInterfacesSymbol] === (ITest as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with empty interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of [implementedInterfacesSymbol] here

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");

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
        expect((IDerived as any)[implementedInterfacesSymbol] === (ITest as any)[implementedInterfacesSymbol]).toBe(true);
    });

    it('works with derived class with extra interface list derives interfaces as a copy', function () {

        // NOTE: interfaces are not supposed to be derived classes
        // just testing copy of [implementedInterfacesSymbol] here

        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        class Intf2 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf2");
        }

        class ITest {
        }

        class Intf3 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf3");
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
        expect((IDerived as any)[implementedInterfacesSymbol] !== (ITest as any)[implementedInterfacesSymbol]).toBe(true);

        // check base class again to make sure its interfaces are not modified
        expectTypeDetails(ITest, nameTest, [Intf1, Intf2]);
    });
});

describe("registerType", () => {
    it("throws for null or undefined", () => {
        expect(() => registerType(null)).toThrow();
        expect(() => registerType(undefined)).toThrow();
    });

    it("throws if the class does not have a typeInfo property", () => {
        class Test {
        }

        expect(() => registerType(Test as any)).toThrow();
    });

    it("registers type as a class for a class typeInfo", () => {
        class Test {
            static [Symbol.typeInfo] = classTypeInfo("MyTestName");
        }

        registerType(Test);
        expect(getTypeFullName(Test)).toBe("MyTestName");
        expect(Test[Symbol.typeInfo].typeKind).toBe("class");
    });

    it("registers type as interface for an interface typeInfo", () => {
        class Test {
            static [Symbol.typeInfo] = interfaceTypeInfo("MyTestName");
        }

        registerType(Test);
        expect(getTypeFullName(Test)).toBe("MyTestName");
        expect(Test[Symbol.typeInfo].typeKind).toBe("interface");
    });

    it("registers type as editor for an editor typeInfo", () => {
        class Test {
            static [Symbol.typeInfo] = editorTypeInfo("MyTestName");
        }

        registerType(Test);
        expect(getTypeFullName(Test)).toBe("MyTestName");
        expect(Test[Symbol.typeInfo].typeKind).toBe("class");
        expect(Test[Symbol.typeInfo].customAttributes?.some(x => x instanceof EditorAttribute)).toBe(true);
    });

    it("registers type as formatter for a formatter typeInfo", () => {
        class Test {
            static [Symbol.typeInfo] = formatterTypeInfo("MyTestName");
        }

        registerType(Test);
        expect(getTypeFullName(Test)).toBe("MyTestName");
        expect(Test[Symbol.typeInfo].typeKind).toBe("class");
        expect(Test[Symbol.typeInfo].interfaces).toContain(ISlickFormatter);
        expect(isAssignableFrom(ISlickFormatter, Test)).toBe(true);
    });
});

describe("isInstanceOfType", () => {
    it("returns false if instance is null or undefined", () => {
        expect(isInstanceOfType(null, String)).toBe(false);
        expect(isInstanceOfType(undefined, Object)).toBe(false);
    });

    it("uses isInstanceOfTypeSymbol function if available", () => {
        class Test1 {
            static [isInstanceOfTypeSymbol](type: any) { return typeof type === "string" && type.startsWith("t"); }
        }

        class Test2 {
            static [isInstanceOfTypeSymbol] = true;
        }

        expect(isInstanceOfType("test", Test1)).toBe(true);
        expect(isInstanceOfType("vest", Test1)).toBe(false);
        expect(isInstanceOfType("test", Test2)).toBe(false);
        expect(isInstanceOfType("vest", Test2)).toBe(false);
    });
});

describe("getInstanceType", () => {
    it("throws for null or undefined", () => {
        expect(() => getInstanceType(undefined)).toThrow();
        expect(() => getInstanceType(null)).toThrow();
    });

    it("returns Object if can't read constructor", () => {
        let a = {
            get constructor() { throw "test"; }
        }
        expect(getInstanceType(a)).toBe(Object);
    });
});

describe("getBaseType", () => {
    it("returns null for null, Object, NaN, and interfaces", () => {
        expect(getBaseType(null)).toBeNull();
        expect(getBaseType(undefined)).toBeNull();
        expect(getBaseType(Object)).toBeNull();
        expect(getBaseType(NaN)).toBeNull();

        class ITest {
        }
        registerInterface(ITest, "getBaseType.ITest");
        expect(getBaseType(ITest)).toBeNull();
    });

    it("uses getPrototypeOf for others", () => {

        class Test {
        }

        class Sub extends Test {
        }

        expect(getBaseType(Test)).toBe(Object);
        expect(getBaseType(Sub)).toBe(Test);
    });
});

describe("initFormType", () => {
    it("uses w function", () => {
        class TestForm {
            w(name: string, widget: string) {
                return name + "_" + widget;
            }
        }

        initFormType(TestForm, ["test1", "1", "test2", "2", "test3", "3"]);
        const form = new TestForm();
        expect((form as any).test1).toBe("test1_1");
        expect((form as any).test2).toBe("test2_2");
        expect((form as any).test3).toBe("test3_3");
    });
});

describe("fieldsProxy", () => {
    it("returns same instance every time", () => {
        class Row1Fields {
        }

        class Row2Fields {
        }

        expect(fieldsProxy<Row1Fields>()).toBe(fieldsProxy<Row2Fields>());
    });

    it("returns property name as is", () => {
        class Fields {
            declare A: string;
            declare B: string;
        }

        const proxy = fieldsProxy<Fields>();
        expect(proxy.A).toBe("A");
        expect(proxy.B).toBe("B");

    });
});

describe("getType", () => {
    it("can return type from a root object", () => {
        let root = {
            A: {
                B: class {
                }
            }
        }

        expect(getType("A.B", root)).toBe(root.A.B);
    });
});

describe("getTypeShortName", () => {
    it("returns part after last dot", () => {
        class Test {
        }
        registerClass(Test, "NamespaceOf.C.IsD")

        expect(getTypeShortName(Test)).toBe("IsD");
    });

    it("returns as is if no dot", () => {
        class Test {
        }
        registerClass(Test, "Me")

        expect(getTypeShortName(Test)).toBe("Me");
    });
});

describe("addCustomAttribute", () => {
    it("can add custom attribute", () => {
        class MyAttribute extends CustomAttribute {
        }
        registerClass(MyAttribute, "MyAttribute");
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr);
    });

    it("can add multiple attribute", () => {
        class MyAttribute1 extends CustomAttribute {
        }
        class MyAttribute2 extends CustomAttribute {
        }
        class MyClass {
        }
        const attr1 = new MyAttribute1();
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute2();
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttribute(MyClass, MyAttribute1)).toBe(attr1);
        expect(getCustomAttribute(MyClass, MyAttribute2)).toBe(attr2);
    });

    it("can add multiple attribute of same type", () => {
        class MyAttribute extends CustomAttribute {
            constructor(public value: boolean) {
                super();
            }
        }
        class MyClass {
        }
        const attr1 = new MyAttribute(false);
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute(true);
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr2); // returns last added one
        let attrs = getCustomAttributes(MyClass, MyAttribute);
        expect(attrs?.length).toBe(2);
        expect(attrs[0]).toBe(attr2); // returns last added one first
        expect(attrs[1]).toBe(attr1);
    });
});

describe("getCustomAttribute", () => {
    class MyAttribute extends CustomAttribute {
    }
    it("returns null if type is null or undefined", () => {
        expect(getCustomAttribute(null, MyAttribute)).toBeNull();
        expect(getCustomAttribute(undefined, MyAttribute)).toBeNull();
    });

    it("returns null if attribute type is null or undefined", () => {
        class MyClass {
        }
        addCustomAttribute(MyClass, new MyAttribute());
        expect(getCustomAttribute(MyClass, null)).toBeNull();
        expect(getCustomAttribute(MyClass, undefined)).toBeNull();
    });

    it("returns empty array if no custom attribute", () => {
        class MyClass {
        }
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([]);
    });

    it("returns the attribute if exists", () => {
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr);
    });

    it("does not return another attribute type", () => {
        class MyClass {
        }
        class OtherAttribute extends CustomAttribute {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(MyClass, OtherAttribute)).toBeUndefined();
    });

    it("returns the last added attribute if multiple of same type of attribute is added", () => {
        class MyClass {
        }
        const attr1 = new MyAttribute();
        const attr2 = new MyAttribute();
        addCustomAttribute(MyClass, attr1);
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr2);
    });

    it("returns inherited attribute types", () => {
        class MyDerivedAttr extends MyAttribute {
        }
        class MyClass {
        }
        const attr = new MyDerivedAttr();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(MyClass, MyAttribute)).toBe(attr);
    });

    it("returns inherited attributes from subclasses", () => {
        class MyClass {
        }
        class SubClass extends MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(SubClass, MyAttribute)).toBe(attr);
    });

    it("returns the subclass attribute if both the class and the subclass has same attribute", () => {
        class MyClass {
        }
        class SubClass extends MyClass {
        }
        const attr1 = new MyAttribute();
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute();
        addCustomAttribute(SubClass, attr2);
        expect(getCustomAttribute(SubClass, MyAttribute)).toBe(attr2);
    });


    it("does not return inherited attributes from subclasses if inherited is false", () => {
        class MyClass {
        }
        class SubClass extends MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttribute(SubClass, MyAttribute, false)).toBeUndefined();
    });

});

describe("getCustomAttributes", () => {
    class MyAttribute extends CustomAttribute {
    }

    it("returns empty array if type is null or undefined", () => {
        expect(getCustomAttributes(null, null)).toStrictEqual([]);
        expect(getCustomAttributes(undefined, undefined)).toStrictEqual([]);
    });

    it("returns empty array if attribute type is null", () => {
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttributes(MyClass, null)).toStrictEqual([]);
    });

    it("returns all attributes if attribute type is undefined", () => {
        class MyClass {
        }
        const attr1 = new MyAttribute();
        addCustomAttribute(MyClass, attr1);
        const attr2 = new MyAttribute();
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttributes(MyClass, void 0)).toStrictEqual([attr1, attr2]);
    });

    it("returns empty array if no custom attribute", () => {
        class MyClass {
        }
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([]);
    });

    it("returns array if custom attribute", () => {
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([attr]);
    });

    it("returns inherited attribute types", () => {
        class MyDerivedAttr extends MyAttribute {
        }
        class MyClass {
        }
        const attr = new MyDerivedAttr();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([attr]);
    });

    it("returns the attributes in descending order", () => {
        class MyClass {
        }
        const attr1 = new MyAttribute();
        const attr2 = new MyAttribute();
        addCustomAttribute(MyClass, attr1);
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([attr2, attr1]);
    });

    it("returns inherited attributes from subclasses", () => {
        class MyClass {
        }
        class SubClass extends MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttributes(SubClass, MyAttribute)).toStrictEqual([attr]);
    });

    it("does not return inherited attributes from subclasses if inherited is false", () => {
        class MyClass {
        }
        class SubClass extends MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(getCustomAttributes(SubClass, MyAttribute, false)).toStrictEqual([]);
    });

    it("returns the attributes in descending order", () => {
        class MyClass {
        }
        const attr1 = new MyAttribute();
        const attr2 = new MyAttribute();
        addCustomAttribute(MyClass, attr1);
        addCustomAttribute(MyClass, attr2);
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([attr2, attr1]);
    });

    it("returns the base class attributes last", () => {
        class MyClass {
        }
        class MyDerivedClass extends MyClass {
        }
        const attr1 = new MyAttribute();
        const attr2 = new MyAttribute();
        const attr3 = new MyAttribute();
        addCustomAttribute(MyDerivedClass, attr1);
        addCustomAttribute(MyClass, attr2);
        addCustomAttribute(MyDerivedClass, attr3);
        expect(getCustomAttributes(MyDerivedClass, MyAttribute)).toStrictEqual([attr3, attr1, attr2]);
        expect(getCustomAttributes(MyClass, MyAttribute)).toStrictEqual([attr2]);
    });
});

describe("hasCustomAttribute", () => {
    it("returns false if no custom attribute", () => {
        class MyAttribute extends CustomAttribute {
        }
        class MyClass {
        }
        expect(hasCustomAttribute(MyClass, MyAttribute)).toBe(false);
    });

    it("returns true if custom attribute", () => {
        class MyAttribute extends CustomAttribute {
        }
        class MyClass {
        }
        const attr = new MyAttribute();
        addCustomAttribute(MyClass, attr);
        expect(hasCustomAttribute(MyClass, MyAttribute)).toBe(true);
    });
});

describe("typeInfo functions", () => {
    it("classTypeInfo creates correct type info", () => {
        class MyAttr extends CustomAttribute {
        }

        class Intf {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf");
        }

        const typeInfo = classTypeInfo("MyClass", [Intf, MyAttr]);
        expect(typeInfo.typeKind).toBe("class");
        expect(typeInfo.typeName).toBe("MyClass");
        expect(typeInfo.interfaces).toStrictEqual([Intf]);
        expect(typeInfo.customAttributes).toHaveLength(1);
        expect(typeInfo.customAttributes[0]).toBeInstanceOf(MyAttr);
    });

    it("editorTypeInfo creates correct type info with EditorAttribute", () => {
        class MyAttr extends CustomAttribute {
        }

        const typeInfo = editorTypeInfo("MyEditor", [MyAttr]);
        expect(typeInfo.typeKind).toBe("class");
        expect(typeInfo.typeName).toBe("MyEditor");
        expect(typeInfo.customAttributes).toHaveLength(2);
        expect(typeInfo.customAttributes.some(attr => attr instanceof EditorAttribute)).toBe(true);
        expect(typeInfo.customAttributes.some(attr => attr instanceof MyAttr)).toBe(true);
    });

    it("formatterTypeInfo creates correct type info with ISlickFormatter", () => {
        class MyAttr extends CustomAttribute {
        }

        const typeInfo = formatterTypeInfo("MyFormatter", [MyAttr]);
        expect(typeInfo.typeKind).toBe("class");
        expect(typeInfo.typeName).toBe("MyFormatter");
        expect(typeInfo.interfaces).toContain(ISlickFormatter);
        expect(typeInfo.customAttributes).toHaveLength(1);
        expect(typeInfo.customAttributes[0]).toBeInstanceOf(MyAttr);
    });

    it("interfaceTypeInfo creates correct type info", () => {
        class Intf1 {
            static [Symbol.typeInfo] = interfaceTypeInfo("Intf1");
        }

        const typeInfo = interfaceTypeInfo("MyInterface", [Intf1]);
        expect(typeInfo.typeKind).toBe("interface");
        expect(typeInfo.typeName).toBe("MyInterface");
        expect(typeInfo.interfaces).toStrictEqual([Intf1]);
    });
});

describe("isEnum", () => {
    it("correctly identifies enum types", function () {
        enum TestEnum {
            A = 1,
            B = 2
        }

        class TestClass {
        }

        registerEnum(TestEnum, 'Test.TestEnum');
        try {

            expect(isEnum(TestEnum)).toBe(true);
            expect(isEnum(TestClass)).toBe(false);
            expect(isEnum({})).toBe(false);
            expect(isEnum(null)).toBe(false);
            expect(isEnum(undefined)).toBe(false);
        } finally {
        }
    });
});


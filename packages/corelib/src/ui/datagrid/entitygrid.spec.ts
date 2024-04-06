import { addCustomAttribute, classTypeInfo } from "../../base";
import { IdPropertyAttribute, IsActivePropertyAttribute, LocalTextPrefixAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { EntityGrid } from "./entitygrid";

function getIdProperty(grid: EntityGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('EntityGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid({});
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('can be overridden in subclass', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addCustomAttribute(AttrGrid, new IdPropertyAttribute("attrId"));

        var grid = new AttrGrid({});
        expect(getIdProperty(grid)).toBe("attrId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("");
    });
});

function getIsActiveProperty(grid: EntityGrid<any, any>): string {
    return grid["getIsActiveProperty"]();
}

describe('EntityGrid.getIsActiveProperty', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid({});
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addCustomAttribute(AttrGrid, new IsActivePropertyAttribute("attrIsActive"));

        var grid = new AttrGrid({});
        expect(getIsActiveProperty(grid)).toBe("attrIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });
});

function getLocalTextDbPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextDbPrefix"]();
}

describe('EntityGrid.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly typeInfo = classTypeInfo('MyProject.TestModule.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addCustomAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.attrPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});

function getLocalTextPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextPrefix"]();
}

describe('EntityGrid.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("Default");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly typeInfo = classTypeInfo('MyProject.TestModule.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addCustomAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid({});
        expect(getLocalTextPrefix(grid)).toBe("attrPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});
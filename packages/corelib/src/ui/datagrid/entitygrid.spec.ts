import { Decorators, IdPropertyAttribute, IsActivePropertyAttribute, LocalTextPrefixAttribute } from "../../decorators";
import { addAttribute } from "../../q/system";
import { EntityGrid } from "./entitygrid";

function getIdProperty(grid: EntityGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('EntityGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('can be overridden in subclass', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new IdPropertyAttribute("attrId"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("attrId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
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

        var grid = new DefaultGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new IsActivePropertyAttribute("attrIsActive"));

        var grid = new AttrGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("attrIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
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

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly __typeName = 'MyProject.TestModule.DefaultGrid';
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.attrPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
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

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("Default");
    });

    it('returns class identifier based on typeName property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static readonly __typeName = 'MyProject.TestModule.DefaultGrid';
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        @Decorators.registerClass('MyProject.MyModule.Some.DefaultGrid')
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('can be set via attribute', () => {
        class AttrGrid extends EntityGrid<any, any> {
        }
        addAttribute(AttrGrid, new LocalTextPrefixAttribute("attrPrefix"));

        var grid = new AttrGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("attrPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid($('<div/>'));
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});
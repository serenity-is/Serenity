import { Column, initColumnProps, titleize } from "../../src/core/column";

describe("initColumnProps", () => {
    it('should set defaults of the columns', () => {
        const column: Column = {};

        initColumnProps([column], {
            width: 200,
            cssClass: 'test'
        });

        expect(column.width).toBe(200);
        expect(column.cssClass).toBe('test');
    });

    it('should create new id if column id is used', () => {
        const columns: Column[] = [
            { id: 'test', name: 'test' },
            { id: 'test', name: 'test' },
            { id: 'test', name: 'test' },
        ];

        initColumnProps(columns, {});

        expect(columns[0].id).toBe('test');
        expect(columns[1].id).toBe('test_1');
        expect(columns[2].id).toBe('test_2');
    });

    it('should generate id from field if id is null', () => {
        const column: Column = { id: null, field: 'test' };

        initColumnProps([column], {});

        expect(column.id).toBe('test');
    });

    it('should generate id as col if id and field are null', () => {
        const column: Column = { id: null, field: null };

        initColumnProps([column], {});

        expect(column.id).toBe('col');
    });

    it('should use field instead of id when generating unique col id if id is null', () => {
        const columns: Column[] = [
            { id: 'test', name: 'test' },
            { id: null, field: 'test', name: 'test' },
            { id: null, field: 'test', name: 'test' },
        ];

        initColumnProps(columns, {});

        expect(columns[0].id).toBe('test');
        expect(columns[1].id).toBe('test_1');
        expect(columns[2].id).toBe('test_2');
    });

    it('should use generated column id correctly when field and id are null', () => {
        const columns: Column[] = [
            { id: null, field: null, name: 'test' },
            { id: null, field: null, name: 'test' },
            { id: null, field: null, name: 'test' },
        ];

        initColumnProps(columns, {});

        expect(columns[0].id).toBe('col');
        expect(columns[1].id).toBe('col_1');
        expect(columns[2].id).toBe('col_2');
    });

    it('should be able to constrain the minWidth of the column', () => {
        const column: Column = { minWidth: 100, width: 50 };

        initColumnProps([column], {});
        expect(column.width).toBe(100);
    });

    it('should be able to constrain the maxWidth of the column', () => {
        const column: Column = { maxWidth: 100, width: 200 };

        initColumnProps([column], {});
        expect(column.width).toBe(100);
    });

    it('should not override already existing properties', () => {
        const columns: Column[] = [
            { id: 'c1', name: 'c1', width: 100 },
            { id: 'c2', name: 'c2', minWidth: 50 },
            { id: 'c3', name: 'c3', maxWidth: 200 },
        ];

        initColumnProps(columns, {
            width: 200,
            minWidth: 100,
            maxWidth: 300
        });

        expect(columns[0].width).toBe(100);
        expect(columns[0].minWidth).toBe(100);
        expect(columns[0].maxWidth).toBe(300);

        expect(columns[1].width).toBe(200);
        expect(columns[1].minWidth).toBe(50);
        expect(columns[1].maxWidth).toBe(300);

        expect(columns[2].width).toBe(200);
        expect(columns[2].minWidth).toBe(100);
        expect(columns[2].maxWidth).toBe(200);
    });

    it('should use titleized field or id to generate name when its undefined', () => {
        const columns: Column[] = [
            { id: 'test', name: 'test abc' },
            { id: 'test abc', name: undefined, field: null },
            { id: null, name: undefined, field: 'test abcdef' },
            { id: null, name: undefined, field: null }
        ];

        initColumnProps(columns, {});

        expect(columns[0].name).toBe('test abc');
        expect(columns[1].name).toBe('Test Abc');
        expect(columns[2].name).toBe('Test Abcdef');
        expect(columns[3].name).toBe('Col'); // auto-generated id
    });
});

describe('titleize', () => {
    it('titleize should work with empty values', () => {
        expect(titleize(null)).toBe(null);
        expect(titleize(undefined)).toBe(undefined);
        expect(titleize("")).toBe("");
        expect(titleize(0 as any)).toBe(0);
    })

    it('titleize should handle camelCase correctly', () => {
        expect(titleize('camelCase')).toBe('Camel Case');
        expect(titleize('XMLHttpRequest')).toBe('Xml Http Request');
        expect(titleize('simple')).toBe('Simple');
        expect(titleize('already_spaced')).toBe('Already Spaced');
        expect(titleize('with-dashes')).toBe('With Dashes');
        expect(titleize('mixed_case-andSpaces')).toBe('Mixed Case And Spaces');
    });

    it('titleize should handle various string patterns', () => {
        expect(titleize('mixed-case_underscore and spaces')).toBe('Mixed Case Underscore And Spaces');
        expect(titleize('PascalCase')).toBe('Pascal Case');
        expect(titleize('HTMLParser')).toBe('Html Parser');
        expect(titleize('JSONData')).toBe('Json Data');
        expect(titleize('userID')).toBe('User Id');
        expect(titleize('APIKey')).toBe('Api Key');
        expect(titleize('HTTPSConnection')).toBe('Https Connection');
        expect(titleize('databaseURL')).toBe('Database Url');
        expect(titleize('maxLength')).toBe('Max Length');
        expect(titleize('minWidth')).toBe('Min Width');
        expect(titleize('backgroundColor')).toBe('Background Color');
        expect(titleize('borderTopWidth')).toBe('Border Top Width');
        expect(titleize('test123Value')).toBe('Test123 Value');
        expect(titleize('value1Test')).toBe('Value1 Test');
        expect(titleize('multiple   spaces')).toBe('Multiple Spaces');
        expect(titleize('tabs\tand\tspaces')).toBe('Tabs And Spaces');
        expect(titleize('hyphen-ated')).toBe('Hyphen Ated');
        expect(titleize('under_score')).toBe('Under Score');
    });

    it('titleize should handle edge cases and performance', () => {
        // Test with many consecutive uppercase letters (potential ReDoS case)
        const manyCaps = 'A'.repeat(10) + 'b';
        expect(titleize(manyCaps)).toBe('Aaaaaaaaa Ab'); // Should split due to regex match

        // Test with many alternating case changes
        let alternating = '';
        for (let i = 0; i < 10; i++) {
            alternating += i % 2 === 0 ? 'a' : 'A';
        }
        expect(() => titleize(alternating)).not.toThrow();

        // Test with numbers
        expect(titleize('test123')).toBe('Test123');
        expect(titleize('123test')).toBe('123test');
        expect(titleize('test123Value')).toBe('Test123 Value');
    });
});

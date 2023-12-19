import { Culture } from "@serenity-is/base";
import { DecimalEditor } from "./decimaleditor";

describe("DecimalEditor", () => {
    it("adds default values to autonumeric", () => {
        Culture.decimalSeparator = ".";
        ($.fn as any).autoNumeric = jest.fn().mockImplementationOnce((options) => {
            expect(options.vMin).toBe('0.00');
            expect(options.vMax).toBe('999999999999.99');
            expect(options.aPad).toBe(true);
            expect(options.aDec).toBe('.');
            expect(options.altDec).toBe(',');
            expect(options.aSep).toBe(',');
            return null;
        })
    });

    it("gets current value using autonumeric if exists", () => {
        var editor = new DecimalEditor();

        ($.fn as any).autoNumeric = jest.fn().mockImplementationOnce((request) => {
            expect(request).toBe('get');
            return '123.45';
        });

        expect(editor.value).toBe(123.45);
    });

    it("gets current value directly if autonumeric not exists", () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        ($.fn as any).autoNumeric = undefined;

        var editor = new DecimalEditor();
        
        editor.element.val('543.21');
        expect(editor.value).toBe(543.21);
    });


    it("sets current value using autonumeric if exists", () => {
        var editor = new DecimalEditor();

        ($.fn as any).autoNumeric = jest.fn().mockImplementationOnce((request, value) => {
            expect(request).toBe('set');
            expect(value).toBe(123.45);
        });

        editor.value = 123.45;
    });

    it("sets current value directly if autonumeric not exists", () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        ($.fn as any).autoNumeric = undefined;

        var editor = new DecimalEditor();
        
        editor.value = 123.45;

        expect(editor.element.val()).toBe('123.45');
    });

    it("sets element value directly if value is null or empty", () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        ($.fn as any).autoNumeric = undefined;

        var editor = new DecimalEditor();
        
        editor.value = null;
        expect(editor.element.val()).toBe('');

        (editor.value as any) = '';
        expect(editor.element.val()).toBe('');
    });

    it("returns true from isvalid if value is valid", () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        ($.fn as any).autoNumeric = undefined;

        var editor = new DecimalEditor();
        
        editor.element.val('');
        expect(editor.get_isValid()).toBe(true);
        editor.element.val('123');
        expect(editor.get_isValid()).toBe(true);
    });

    it("returns false from isvalid if value is invalid", () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        ($.fn as any).autoNumeric = undefined;

        var editor = new DecimalEditor();
        
        editor.element.val('not valid');
        expect(editor.get_isValid()).toBe(false);
    });
});
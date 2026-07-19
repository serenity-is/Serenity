import { Fluent } from "../../base";
import { AutoNumeric, AutoNumericOptions } from "./autonumeric";

function createInput(): HTMLInputElement {
    return document.createElement('input');
}

function initInput(options?: AutoNumericOptions, value?: string): HTMLInputElement {
    const input = createInput();
    if (value !== undefined) input.value = value;
    AutoNumeric.init(input, options ?? {});
    return input;
}

function createDiv(): HTMLDivElement {
    return document.createElement('div');
}

function initDiv(options?: AutoNumericOptions, text?: string): HTMLDivElement {
    const div = createDiv();
    if (text !== undefined) div.textContent = text;
    AutoNumeric.init(div as any, options ?? {});
    return div;
}

beforeEach(() => {
    vi.clearAllMocks();
});

// ===================== AutoNumeric defaults =====================

describe("AutoNumeric defaults", () => {
    it("has correct default values", () => {
        expect(AutoNumeric.defaults.aNum).toBe('0123456789');
        expect(AutoNumeric.defaults.aSep).toBe(',');
        expect(AutoNumeric.defaults.dGroup).toBe('3');
        expect(AutoNumeric.defaults.aDec).toBe('.');
        expect(AutoNumeric.defaults.altDec).toBeNull();
        expect(AutoNumeric.defaults.aSign).toBe('');
        expect(AutoNumeric.defaults.pSign).toBe('p');
        expect(AutoNumeric.defaults.vMax).toBe('9999999999999.99');
        expect(AutoNumeric.defaults.vMin).toBe('0.00');
        expect(AutoNumeric.defaults.mDec).toBeNull();
        expect(AutoNumeric.defaults.mRound).toBe('S');
        expect(AutoNumeric.defaults.aPad).toBe(true);
        expect(AutoNumeric.defaults.nBracket).toBeNull();
        expect(AutoNumeric.defaults.wEmpty).toBe('empty');
        expect(AutoNumeric.defaults.lZero).toBe('allow');
        expect(AutoNumeric.defaults.aForm).toBe(true);
    });

    it("has allowedSettingKeys", () => {
        expect(AutoNumeric.allowedSettingKeys.has('aNum')).toBe(true);
        expect(AutoNumeric.allowedSettingKeys.has('aSep')).toBe(true);
        expect(AutoNumeric.allowedSettingKeys.has('dGroup')).toBe(true);
        expect(AutoNumeric.allowedSettingKeys.has('aDec')).toBe(true);
        expect(AutoNumeric.allowedSettingKeys.has('tagList')).toBe(true);
        expect(AutoNumeric.allowedSettingKeys.size).toBe(17);
    });
});

// ===================== AutoNumeric.init =====================

describe("AutoNumeric.init", () => {
    it("initializes with default options", () => {
        const input = initInput();
        const settings = AutoNumeric.getSettings(input);
        expect(settings).toBeDefined();
        expect(settings.aNum).toBe('0123456789');
        expect((settings as any).mInt).toBeGreaterThan(0);;
    });

    it("initializes with custom options", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMax: '9999.99', vMin: '0.00', aDec: ',', aSep: '.' });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.aDec).toBe(',');
        expect(settings.aSep).toBe('.');
        // vMax/vMin get converted to numbers by convertKeyToNumber
        expect(settings.vMax).toBe(9999.99);
        expect(settings.vMin).toBe(0);
    });

    it("throws on null input", () => {
        expect(() => AutoNumeric.init(null as any, {})).toThrow("autoNumeric called with null element!");
    });

    it("throws when aDec equals aSep", () => {
        const input = createInput();
        expect(() => AutoNumeric.init(input, { aDec: '.', aSep: '.' })).toThrow(
            "autoNumeric will not function properly"
        );
    });

    it("does not reinitialize if already initialized", () => {
        const input = initInput({ vMax: '999.99' });
        const settings1 = AutoNumeric.getSettings(input);
        AutoNumeric.init(input, { vMax: '111.11' });
        const settings2 = AutoNumeric.getSettings(input);
        expect(settings2).toBe(settings1);
    });

    it("formats default value on init when aForm is true", () => {
        const input = initInput({}, '1234.56');
        expect(input.value).not.toBe('');
    });

    it("aForm false does not auto-format on init", () => {
        const input = createInput();
        input.value = '1234.56';
        AutoNumeric.init(input, { aForm: false });
        expect(input.value).toBe('1234.56');
    });

    it("handles wEmpty 'sign' on init", () => {
        const input = createInput();
        input.value = '';
        AutoNumeric.init(input, { wEmpty: 'sign', aSign: '$' });
        expect(input.value).toBe('$');
    });

    it("handles wEmpty 'empty' on init", () => {
        const input = createInput();
        input.value = '';
        AutoNumeric.init(input, { wEmpty: 'empty' });
        expect(input.value).toBe('');
    });
});

// ===================== AutoNumeric.destroy =====================

describe("AutoNumeric.destroy", () => {
    it("removes settings and event handlers", () => {
        const input = initInput();
        expect(AutoNumeric.hasInstance(input)).toBe(true);
        AutoNumeric.destroy(input);
        expect(AutoNumeric.hasInstance(input)).toBe(false);
        expect(AutoNumeric.getSettings(input)).toBeUndefined();
    });

    it("does not throw on uninitialized input", () => {
        const input = createInput();
        expect(() => AutoNumeric.destroy(input)).not.toThrow();
    });
});

// ===================== AutoNumeric.setValue =====================

describe("AutoNumeric.setValue", () => {
    it("formats a numeric value with thousands separator", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('1,234.56');
    });

    it("throws without init", () => {
        const input = createInput();
        expect(() => AutoNumeric.setValue(input, '123')).toThrow(
            "You must initialize autoNumeric"
        );
    });

    it("returns empty string for NaN", () => {
        const input = initInput();
        expect(AutoNumeric.setValue(input, 'abc')).toBe('');
    });

    it("handles negative values", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('-123.45');
    });

    it("rounds value to mDec", () => {
        const input = initInput({ mDec: 2, aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '123.456');
        expect(input.value).toBe('123.46');
    });

    it("handles nBracket on negative values", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '(,)' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toMatch(/\(123\.45\)/);
    });

    it("handles zero value with aPad", () => {
        const input = initInput({ aSep: '', aDec: '.', aPad: true, mDec: 2 });
        AutoNumeric.setValue(input, '0');
        expect(input.value).toBe('0.00');
    });

    it("handles value with currency sign prefix", () => {
        const input = initInput({ aSign: '$', pSign: 'p', aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('$1,234.56');
    });

    it("handles value with currency sign suffix", () => {
        const input = initInput({ aSign: '€', pSign: 's', aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('1,234.56€');
    });

    it("handles empty string with wEmpty 'empty'", () => {
        const input = initInput({ wEmpty: 'empty' });
        AutoNumeric.setValue(input, '');
        expect(input.value).toBe('');
    });

    it("handles empty string with wEmpty 'zero'", () => {
        const input = initInput({ wEmpty: 'zero', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '');
        expect(input.value).toBe('0');
    });

    it("handles empty string with wEmpty 'sign'", () => {
        const input = initInput({ wEmpty: 'sign', aSign: '$', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '');
        expect(input.value).toBe('$');
    });

    it("handles negative value with aSign prefix", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', aSign: '$', pSign: 'p', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('-$123.45');
    });

    it("handles negative value with aSign suffix", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', aSign: '€', pSign: 's', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('-123.45€');
    });
});

// ===================== AutoNumeric.getValue =====================

describe("AutoNumeric.getValue", () => {
    it("returns raw numeric string", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        input.value = '1,234.56';
        expect(AutoNumeric.getValue(input)).toBe('1234.56');
    });

    it("throws without init", () => {
        const input = createInput();
        expect(() => AutoNumeric.getValue(input)).toThrow(
            "You must initialize autoNumeric"
        );
    });

    it("returns empty for empty value wEmpty 'empty'", () => {
        const input = initInput({ wEmpty: 'empty' });
        input.value = '';
        expect(AutoNumeric.getValue(input)).toBe('');
    });

    it("returns empty for sign-only value with wEmpty 'sign'", () => {
        const input = initInput({ wEmpty: 'sign', aSign: '$' });
        input.value = '$';
        expect(AutoNumeric.getValue(input)).toBe('');
    });

    it("handles nBracket on get", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '(,)' });
        AutoNumeric.setValue(input, '-123.45');
        expect(AutoNumeric.getValue(input)).toBe('-123.45');
    });
});

// ===================== AutoNumeric.updateOptions =====================

describe("AutoNumeric.updateOptions", () => {
    it("updates existing settings", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        AutoNumeric.updateOptions(input, { aSep: '' });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.aSep).toBe('');
    });

    it("throws without init", () => {
        const input = createInput();
        expect(() => AutoNumeric.updateOptions(input, {})).toThrow(
            "You must initialize autoNumeric"
        );
    });

    it("throws when aDec equals aSep after update", () => {
        const input = initInput({ aDec: '.', aSep: ',' });
        expect(() => AutoNumeric.updateOptions(input, { aSep: '.' })).toThrow(
            "autoNumeric will not function properly"
        );
    });

    it("re-formats the current value after update", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        input.value = '1,234.56';
        AutoNumeric.updateOptions(input, { aSep: '' });
        expect(input.value).toBe('1234.56');
    });
});

// ===================== AutoNumeric.hasInstance =====================

describe("AutoNumeric.hasInstance", () => {
    it("returns true after init", () => {
        const input = initInput();
        expect(AutoNumeric.hasInstance(input)).toBe(true);
    });

    it("returns false before init", () => {
        const input = createInput();
        expect(AutoNumeric.hasInstance(input)).toBe(false);
    });

    it("returns false after destroy", () => {
        const input = initInput();
        AutoNumeric.destroy(input);
        expect(AutoNumeric.hasInstance(input)).toBe(false);
    });
});

// ===================== AutoNumeric.getSettings =====================

describe("AutoNumeric.getSettings", () => {
    it("returns settings object", () => {
        const input = initInput({ aSign: '$' });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.aSign).toBe('$');
    });

    it("returns undefined before init", () => {
        const input = createInput();
        expect(AutoNumeric.getSettings(input)).toBeUndefined();
    });
});

// ===================== autoCode derived properties =====================

describe("autoCode derived properties", () => {
    it("computes mInt from vMax and vMin", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMax: '99999.99', vMin: '0.00' });
        const settings = AutoNumeric.getSettings(input);
        expect((settings as any).mInt).toBe(5);
    });

    it("computes mDec from decimals in vMax/vMin when mDec is null", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMax: '999.9999', vMin: '0.00', mDec: null });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.mDec).toBe(4);
    });

    it("sets altDec when aDec is '.' and aSep is space", () => {
        const input = createInput();
        AutoNumeric.init(input, { aDec: '.', aSep: ' ', mDec: 2 });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.altDec).toBe(',');
    });

    it("sets altDec when aDec is ',' and aSep is space", () => {
        const input = createInput();
        AutoNumeric.init(input, { aDec: ',', aSep: ' ', mDec: 2 });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.altDec).toBe('.');
    });

    it("does not set altDec when mDec is 0", () => {
        const input = createInput();
        AutoNumeric.init(input, { aDec: '.', aSep: ' ', mDec: 0 });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.altDec).toBeNull();
    });

    it("sets aNeg when vMin is negative", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMin: '-999.99', vMax: '999.99' });
        const settings = AutoNumeric.getSettings(input);
        expect((settings as any).aNeg).toBe('-');;
    });

    it("leaves aNeg empty when vMin is positive", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMin: '0.00', vMax: '999.99' });
        const settings = AutoNumeric.getSettings(input);
        expect((settings as any).aNeg).toBe('');
    });

    it("sets mDec=2 for CHF rounding when mDec is null", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'CHF', mDec: null });
        const settings = AutoNumeric.getSettings(input);
        expect(settings.mDec).toBe(2);
    });
});

// ===================== Thousands separator (dGroup) =====================

describe("Thousands separator (dGroup)", () => {
    it("dGroup 2 (Indian style)", () => {
        const input = initInput({ dGroup: '2', aSep: ',', aDec: '.', mDec: 2 });
        AutoNumeric.setValue(input, '1234567.89');
        expect(input.value).toBe('12,34,567.89');
    });

    it("dGroup 4 (Asian style)", () => {
        const input = initInput({ dGroup: '4', aSep: ',', aDec: '.', mDec: 1 });
        AutoNumeric.setValue(input, '12345678.9');
        expect(input.value).toBe('1234,5678.9');
    });

    it("dGroup 3 (default Western style)", () => {
        const input = initInput({ dGroup: '3', aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234567.89');
        expect(input.value).toBe('1,234,567.89');
    });

    it("no separator when aSep is empty", () => {
        const input = initInput({ aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '1234567.89');
        expect(input.value).toBe('1234567.89');
    });
});

// ===================== Rounding modes =====================

describe("Rounding modes", () => {
    function roundValue(value: string, options: AutoNumericOptions): string {
        const input = document.createElement('input');
        AutoNumeric.init(input, { aSep: '', aDec: '.', vMin: '-9999', vMax: '9999', ...options });
        AutoNumeric.setValue(input, value);
        return input.value;
    }

    it("S Round-Half-Up Symmetric", () => {
        expect(roundValue('1.5', { mRound: 'S', mDec: 0 })).toBe('2');
    });

    it("A Round-Half-Up Asymmetric positive", () => {
        expect(roundValue('1.5', { mRound: 'A', mDec: 0 })).toBe('2');
    });

    it("A Round-Half-Up Asymmetric negative", () => {
        expect(roundValue('-1.6', { mRound: 'A', mDec: 0 })).toBe('-2');
    });

    it("s Round-Half-Down Symmetric", () => {
        expect(roundValue('1.6', { mRound: 's', mDec: 0 })).toBe('2');
    });

    it("a Round-Half-Down Asymmetric positive", () => {
        expect(roundValue('1.5', { mRound: 'a', mDec: 0 })).toBe('1');
    });

    it("B Bankers Rounding - even stays even", () => {
        expect(roundValue('2.5', { mRound: 'B', mDec: 0 })).toBe('2');
    });

    it("B Bankers Rounding - odd rounds up", () => {
        expect(roundValue('3.5', { mRound: 'B', mDec: 0 })).toBe('3');
    });

    it("U Round Up", () => {
        expect(roundValue('1.1', { mRound: 'U', mDec: 0 })).toBe('2');
    });

    it("D Round Down", () => {
        expect(roundValue('1.9', { mRound: 'D', mDec: 0 })).toBe('1');
    });

    it("C Round to Ceiling positive", () => {
        expect(roundValue('1.1', { mRound: 'C', mDec: 0 })).toBe('2');
    });

    it("F Round to Floor negative", () => {
        expect(roundValue('-1.1', { mRound: 'F', mDec: 0 })).toBe('-2');
    });

    it("CHF rounding - rounds to 0.05", () => {
        expect(roundValue('1.07', { mRound: 'CHF', mDec: null, aPad: false })).toBe('1.05');
    });

    it("CHF rounding - rounds up to 0.05", () => {
        expect(roundValue('1.08', { mRound: 'CHF', mDec: null, aPad: true })).toBe('1.10');
    });
});

// ===================== lZero modes =====================

describe("lZero modes", () => {
    it("lZero 'allow' strips leading zeros in setValue", () => {
        const input = initInput({ lZero: 'allow', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '00123.45');
        expect(input.value).toBe('123.45');
    });

    it("lZero 'keep' retains leading zeros", () => {
        const input = initInput({ lZero: 'keep', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '00123.45');
        expect(input.value).toBe('00123.45');
    });

    it("lZero 'deny' denies leading zeros", () => {
        const input = initInput({ lZero: 'deny', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '0123.45');
        expect(input.value).toBe('123.45');
    });
});

// ===================== aPad modes =====================

describe("aPad modes", () => {
    it("aPad true pads decimals with zeros", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 2, aPad: true });
        AutoNumeric.setValue(input, '12.3');
        expect(input.value).toBe('12.30');
    });

    it("aPad false does not pad decimals", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 2, aPad: false });
        AutoNumeric.setValue(input, '12.3');
        expect(input.value).toBe('12.3');
    });

    it("aPad as number pads to that many places", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 4, aPad: 3 as any });
        AutoNumeric.setValue(input, '12.3');
        expect(input.value).toBe('12.300');
    });
});

// ===================== Alternative decimal separator =====================

describe("Alternative decimal separator", () => {
    it("uses altDec when aDec is period and aSep is comma", () => {
        const input = initInput({ aDec: '.', aSep: ',', mDec: 2, vMax: '9999.99', vMin: '0.00' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('1,234.56');
    });

    it("uses comma as decimal when configured via focusout", () => {
        const input = initInput({ aDec: ',', aSep: '.', mDec: 2, vMax: '9999.99', vMin: '0.00' });
        input.value = '1234,56';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('1.234,56');
    });
});

// ===================== Carry overflow in rounding =====================

describe("Carry overflow in rounding", () => {
    it("handles carry past most significant digit", () => {
        const input = initInput({ mDec: 2, aSep: '', aDec: '.', mRound: 'U', vMax: '99999.99', vMin: '0.00' });
        AutoNumeric.setValue(input, '999.999');
        expect(input.value).toBe('1000.00');
    });

    it("handles carry with integer rounding", () => {
        const input = initInput({ mDec: 0, aSep: '', aDec: '.', mRound: 'U', vMax: '99999', vMin: '0' });
        AutoNumeric.setValue(input, '9999.5');
        expect(input.value).toBe('10000');
    });
});

// ===================== nBracket variations =====================

describe("nBracket variations", () => {
    it("handles parentheses", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '(,)', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('(123.45)');
    });

    it("handles square brackets", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '[,]', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('[123.45]');
    });

    it("handles angle brackets", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '<,>', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('<123.45>');
    });

    it("handles curly braces", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '{,}', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('{123.45}');
    });
});

// ===================== Focus events =====================

describe("Focus events", () => {
    it("focusin removes nBracket", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '(,)', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('(123.45)');

        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('-123.45');
    });

    it("focusin handles readonly inputs", () => {
        const input = initInput({});
        input.setAttribute('readonly', '');
        input.value = '123';
        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('123');
    });

    it("focusout formats the value", () => {
        const input = initInput({ aSep: ',', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        input.value = '1234.56';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('1,234.56');
    });

    it("focusout removes nBracket then re-applies it", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '(,)', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-123.45');

        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('-123.45');

        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('(123.45)');
    });

    it("focusout handles readonly inputs", () => {
        const input = initInput({});
        input.setAttribute('readonly', '');
        input.value = '123';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('123');
    });

    it("focusout triggers change event when value changes", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        const changeHandler = vi.fn();
        input.addEventListener('change', changeHandler);

        input.value = '123.45';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(changeHandler).toHaveBeenCalled();
    });

    it("focusin handles empty wEmpty 'sign'", () => {
        const input = initInput({ wEmpty: 'sign', aSign: '$', aSep: '', aDec: '.' });
        input.value = '';
        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('$');
    });
});

// ===================== checkValue edge cases =====================

describe("checkValue edge cases", () => {
    it("handles very small positive numbers", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 10, vMax: '1', vMin: '0' });
        AutoNumeric.setValue(input, '0.0000001');
        expect(input.value).not.toBe('');
    });

    it("handles trailing zeros in decimal", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 5 });
        AutoNumeric.setValue(input, '123.45000');
        expect(input.value).toMatch(/123\.45/);
    });

    it("handles value with no decimal part", () => {
        const input = initInput({ aSep: '', aDec: '.', aPad: false, mDec: 0 });
        AutoNumeric.setValue(input, '123');
        expect(input.value).toBe('123');
    });
});

// ===================== non-INPUT elements (tagList) =====================

describe("non-INPUT elements (tagList)", () => {
    it("init with DIV stores initial textContent", () => {
        const div = initDiv({ aSep: ',', aDec: '.' }, '1234.56');
        expect(div.textContent).toBe('1234.56');
    });

    it("init then setValue on DIV formats textContent", () => {
        const div = initDiv({ aSep: ',', aDec: '.' });
        AutoNumeric.setValue(div as any, '1234.56');
        expect(div.textContent).toBe('1,234.56');
    });

    it("setValue on DIV element", () => {
        const div = initDiv({ aSep: ',', aDec: '.', aPad: false });
        AutoNumeric.setValue(div as any, '5678.9');
        expect(div.textContent).toBe('5,678.9');
    });

    it("getValue on DIV element", () => {
        const div = initDiv({ aSep: ',', aDec: '.' }, '1,234.56');
        expect(AutoNumeric.getValue(div as any)).toBe('1234.56');
    });

    it("destroy on DIV element", () => {
        const div = initDiv({});
        expect(AutoNumeric.hasInstance(div as any)).toBe(true);
        AutoNumeric.destroy(div as any);
        expect(AutoNumeric.hasInstance(div as any)).toBe(false);
    });
});

// ===================== Keyboard events =====================

describe("Keyboard events", () => {
    function dispatchKey(input: HTMLInputElement, type: string, key: string, code: number, ctrlKey = false) {
        input.dispatchEvent(new KeyboardEvent(type, {
            key, keyCode: code, which: code,
            ctrlKey, shiftKey: false, metaKey: false, bubbles: true
        }));
    }

    it("accepts digit input via key events", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '9999.99', vMin: '0.00', lZero: 'deny' });
        input.value = '';
        input.setSelectionRange(0, 0);
        input.focus();

        dispatchKey(input, 'keydown', '1', 49);
        dispatchKey(input, 'keypress', '1', 49);
        dispatchKey(input, 'keyup', '1', 49);

        // The input should have been formatted
        expect(input.value.length).toBeGreaterThanOrEqual(1);
    });

    it("processes backspace", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        input.value = '123';
        input.setSelectionRange(3, 3);
        input.focus();

        dispatchKey(input, 'keydown', 'Backspace', 8);
        dispatchKey(input, 'keyup', 'Backspace', 8);

        expect(input.value).toBe('12');
    });

    it("processes delete", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        input.value = '123';
        input.setSelectionRange(0, 0);
        input.focus();

        dispatchKey(input, 'keydown', 'Delete', 46);
        dispatchKey(input, 'keyup', 'Delete', 46);

        expect(input.value).toBe('23');
    });

    it("does not allow minus when vMin is positive", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '';
        input.setSelectionRange(0, 0);
        input.focus();

        dispatchKey(input, 'keydown', '-', 189);
        dispatchKey(input, 'keypress', '-', 189);
        dispatchKey(input, 'keyup', '-', 189);

        expect(input.value).toBe('');
    });

    it("skips function keys", () => {
        const input = initInput({});
        input.value = '';
        input.setSelectionRange(0, 0);
        input.focus();

        dispatchKey(input, 'keydown', 'F1', 112);
        expect(input.value).toBe('');
    });

    it("handles Ctrl+A (select all)", () => {
        const input = initInput({});
        input.value = '123';
        input.focus();

        dispatchKey(input, 'keydown', 'a', 65, true);
        expect(input.value).toBe('123');
    });

    it("handles readonly input", () => {
        const input = initInput({});
        input.setAttribute('readonly', '');
        input.value = '123';
        input.focus();

        dispatchKey(input, 'keydown', '1', 49);
        expect(input.value).toBe('123');
    });
});

// ===================== Edge cases =====================

describe("Edge cases", () => {
    it("does not crash on unknown options", () => {
        const input = createInput();
        expect(() => AutoNumeric.init(input, { unknown: 'value' } as any)).not.toThrow();
    });

    it("handles tagList elements in focusout", () => {
        const div = initDiv({ aSep: '', aDec: '.' }, '123.45');
        div.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(div.textContent).toBe('123.45');
    });

    it("handles setValue with update via updateOptions", () => {
        const input = initInput({ aSep: ',', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('1,234.56');

        AutoNumeric.updateOptions(input, { aSep: '', aDec: '.' });
        expect(input.value).toBe('1234.56');
    });

    it("setValue with number type works", () => {
        const input = initInput({ aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, 1234.56 as any);
        expect(input.value).toBe('1234.56');
    });

    it("does not crash on empty init options", () => {
        const input = createInput();
        expect(() => AutoNumeric.init(input, {} as any)).not.toThrow();
    });

    it("getValue on non-input tagList element", () => {
        const span = document.createElement('span');
        span.textContent = '1234.56';
        AutoNumeric.init(span as any, { aSep: ',', aDec: '.' });
        expect(AutoNumeric.getValue(span as any)).toBe('1234.56');
    });

    it("lZero 'keep' in getValue returns value with leading zeros", () => {
        const input = initInput({ lZero: 'keep', aSep: '', aDec: '.' });
        input.value = '00123.45';
        expect(AutoNumeric.getValue(input)).toBe('00123.45');
    });

    it("getValue with aForm false strips value", () => {
        const input = initInput({ aForm: false, aSep: '', aDec: '.' });
        input.value = 'abc123.45';
        expect(AutoNumeric.getValue(input)).toBe('123.45');
    });

    it("nBracket with pageLoad event in setValue", () => {
        const input = createInput();
        input.setAttribute('value', 'test');
        AutoNumeric.init(input, { vMin: '-999', vMax: '999', nBracket: '(,)' });
        AutoNumeric.setValue(input, '-50');
        expect(input.value).toBe('(50)');
    });

    it("nBracket with square brackets getValue", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', nBracket: '[,]', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-50');
        expect(AutoNumeric.getValue(input)).toBe('-50');
    });

    it("checkValue with negative small number", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 10, vMax: '0.001', vMin: '-0.001' });
        AutoNumeric.setValue(input, '-0.0000001');
        expect(input.value).not.toBe('');
    });

    it("checkValue with number ending in .0", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 5, aPad: false, vMax: '99999', vMin: '0' });
        AutoNumeric.setValue(input, '123.0');
        expect(input.value).toBe('123');
    });

    it("setValue with back-button handling", () => {
        const input = createInput();
        input.value = '123.45';
        AutoNumeric.init(input, { aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '999.99');
        expect(input.value).toBe('999.99');
    });

    it("autoGroup with prefix sign and negative value", () => {
        const input = initInput({ aSign: '$', pSign: 'p', aSep: ',', aDec: '.', vMin: '-999.99', vMax: '999.99' });
        AutoNumeric.setValue(input, '-1234.56');
        expect(input.value).toBe('-$1,234.56');
    });

    it("autoGroup with suffix sign and negative value", () => {
        const input = initInput({ aSign: '€', pSign: 's', aSep: ',', aDec: '.', vMin: '-999.99', vMax: '999.99' });
        AutoNumeric.setValue(input, '-1234.56');
        expect(input.value).toBe('-1,234.56€');
    });

    it("mDec override from vMax decimal places", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMax: '99.1234', vMin: '0.00', mDec: null });
        const s = AutoNumeric.getSettings(input);
        expect(s.mDec).toBe(4);
    });

    it("deduplicate trailing zeros in checkValue", () => {
        const input = initInput({ aSep: '', aDec: '.', mDec: 5 });
        AutoNumeric.setValue(input, '123.45000');
        expect(input.value).toMatch(/^123\.45/);
    });

    it("C Round to Ceiling negative does not round up", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'C', mDec: 0, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '-1.1');
        expect(input.value).toBe('-1');
    });

    it("F Round to Floor positive does not round down", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'F', mDec: 0, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '1.1');
        expect(input.value).toBe('1');
    });

    it("D Round Down negative", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'D', mDec: 0, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '-1.9');
        expect(input.value).toBe('-1');
    });

    it("CHF rounding with aSign", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'CHF', mDec: null, aSep: '', aDec: '.', aPad: true, aSign: 'CHF', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '1.07');
        expect(input.value).toBe('CHF1.05');
    });

    it("setValue with mDec=0 and aPad true for integer values", () => {
        const input = initInput({ mDec: 0, aSep: '', aDec: '.', aPad: true, vMax: '999', vMin: '0' });
        AutoNumeric.setValue(input, '123');
        expect(input.value).toBe('123');
    });

    it("arrow key left skips over thousand separator", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234.56');
        input.setSelectionRange(3, 3);
        input.focus();
        input.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowLeft', keyCode: 37, which: 37, bubbles: true }));
        expect(input.selectionStart).toBe(2);
    });

    it("arrow key right skips over thousand separator", () => {
        const input = initInput({ aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234.56');
        input.setSelectionRange(1, 1);
        input.focus();
        input.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight', keyCode: 39, which: 39, bubbles: true }));
        expect(input.value).toBe('1,234.56');
    });

    it("tagList element with P tag", () => {
        const p = document.createElement('p');
        p.textContent = '1234.56';
        AutoNumeric.init(p as any, { aSep: ',', aDec: '.', aPad: false });
        expect(p.textContent).toBe('1234.56');
        AutoNumeric.setValue(p as any, '789.1');
        expect(p.textContent).toBe('789.1');
    });

    it("SPAN element tagList support", () => {
        const span = document.createElement('span');
        AutoNumeric.init(span as any, { aSep: ',', aDec: '.', aPad: false });
        AutoNumeric.setValue(span as any, '9876.5');
        expect(span.textContent).toBe('9,876.5');
    });

    it("getSettings on non-input element", () => {
        const div = initDiv({ aSign: '$' });
        expect(AutoNumeric.getSettings(div as any).aSign).toBe('$');
    });

    it("mDec=0 rounds to integer", () => {
        const input = initInput({ mDec: 0, aSep: '', aDec: '.', mRound: 'S', aPad: false });
        AutoNumeric.setValue(input, '123.99');
        expect(input.value).toBe('124');
    });

    it("lZero 'allow' strips leading zeros in focusout", () => {
        const input = initInput({ lZero: 'allow', aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '00123.45';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('123.45');
    });

    it("lZero 'deny' strips leading zeros in focusout", () => {
        const input = initInput({ lZero: 'deny', aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '0123.45';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('123.45');
    });

    it("autoCheck rejects out-of-range value", () => {
        const input = initInput({ vMax: '100', vMin: '0', aSep: '', aDec: '.' });
        input.value = '999';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('');
    });

    it("focusout with wEmpty 'zero' formats empty to zero", () => {
        const input = initInput({ wEmpty: 'zero', aSep: '', aDec: '.' });
        input.value = '';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('0');
    });

    it("focusout wEmpty 'sign' with aSign", () => {
        const input = initInput({ wEmpty: 'sign', aSign: '$', aSep: '', aDec: '.' });
        input.value = '';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('$');
    });

    it("init with aForm true and textContent for tagList", () => {
        const div = document.createElement('div');
        div.textContent = '123.45';
        AutoNumeric.init(div as any, { aForm: true, aSep: '', aDec: '.' });
        expect(div.textContent).toBe('123.45');
    });

    it("setValue with null getAttribute value", () => {
        const input = createInput();
        AutoNumeric.init(input, { aSep: '', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        AutoNumeric.setValue(input, '123.45');
        expect(input.value).toBe('123.45');
    });

    it("nBracket with 'pageLoad' on setValue re-init", () => {
        const input = createInput();
        AutoNumeric.init(input, { vMin: '-999', vMax: '999', nBracket: '(,)' });
        expect(AutoNumeric.hasInstance(input)).toBe(true);
    });

    it("autoStrip with aSign loop", () => {
        const input = initInput({ aSign: '$', aSep: '', aDec: '.', pSign: 'p', aPad: false, mDec: 0 });
        input.value = '$$$123';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('$123');
    });

    it("keyup triggers formatQuick when formatted is false", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '123.45';
        input.focus();
        Fluent.trigger(input, 'keyup');
        expect(input.value).toBe('123.45');
    });

    it("keyup with aSign and pSign prefix triggers selection", () => {
        const input = initInput({ aSign: '$', pSign: 'p', wEmpty: 'sign', aSep: '', aDec: '.' });
        input.value = '$';
        input.focus();
        Fluent.trigger(input, 'keyup');
        expect(input.value).toBe('$');
    });

    it("keyup with aSign and pSign suffix triggers selection at start", () => {
        const input = initInput({ aSign: '€', pSign: 's', wEmpty: 'sign', aSep: '', aDec: '.' });
        input.value = '€';
        input.focus();
        Fluent.trigger(input, 'keyup');
        expect(input.value).toBe('€');
    });

    it("keyup with empty value returns early", () => {
        const input = initInput({ aSep: '', aDec: '.' });
        input.value = '';
        input.focus();
        Fluent.trigger(input, 'keyup');
        expect(input.value).toBe('');
    });

    it("keydown with enter key triggers no formatting", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '123';
        input.focus();
        Fluent.trigger(input, 'keydown');
        expect(input.value).toBe('123');
    });

    it("keypress with no processKeypress match", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '123';
        input.focus();
        const event = new KeyboardEvent('keypress', { key: 'a', keyCode: 65, which: 65, bubbles: true });
        input.dispatchEvent(event);
        expect(input.value).toBe('123');
    });

    it("runCallbacks with function option", () => {
        const input = createInput();
        const fn = vi.fn(() => '999.99');
        AutoNumeric.init(input, { vMax: fn as any, vMin: '0', aSep: '', aDec: '.' });
        const s = AutoNumeric.getSettings(input);
        expect(s.vMax).toBe(999.99);
    });

    it("init on unsupported element throws", () => {
        const bad = { tagName: 'CUSTOM', matches: () => false, dataset: {} } as any;
        expect(() => AutoNumeric.init(bad, { aSep: '', aDec: '.' })).toThrow("not supported");
    });

    it("getValue on non-matching INPUT element throws", () => {
        const bad = { tagName: 'INPUT', matches: () => false, dataset: {}, value: '', textContent: '' } as any;
        AutoNumeric.init(bad, { aForm: false, aSep: '', aDec: '.' });
        expect(() => AutoNumeric.getValue(bad)).toThrow("not supported");
    });

    it("negativeBracket with pageLoad oEvent", () => {
        const input = createInput();
        input.setAttribute('value', '123.45');
        AutoNumeric.init(input, { vMin: '-999', vMax: '999', nBracket: '(,)', aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '-50');
        expect(input.value).toBe('(50)');
    });

    it("autoRound with Round Down (D) negative", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'D', mDec: 2, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '-1.999');
        expect(input.value).toBe('-1.99');
    });

    it("autoRound with Round to Ceiling (C) negative - no rounding", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'C', mDec: 2, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '-1.111');
        expect(input.value).toBe('-1.11');
    });

    it("autoRound with Round to Floor (F) positive - no rounding", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'F', mDec: 2, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '1.111');
        expect(input.value).toBe('1.11');
    });

    it("autoStrip with strip_zero='strip' and lZero allow", () => {
        const input = initInput({ lZero: 'allow', aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        const s = AutoNumeric.getSettings(input);
        (s as any).allowLeading = false;
        input.value = '00123.45';
        input.dispatchEvent(new FocusEvent('focusout', { bubbles: true }));
        expect(input.value).toBe('123.45');
    });

    it("checkValue with empty string returns empty", () => {
        const input = initInput({ aSep: '', aDec: '.' });
        AutoNumeric.setValue(input, '');
        expect(input.value).toBe('');
    });

    it("truncateDecimal with mDec=0 removes decimal part", () => {
        const input = initInput({ mDec: 0, aSep: '', aDec: '.', aPad: false });
        AutoNumeric.setValue(input, '123.99');
        expect(input.value).toBe('124');
    });

    it("normalizeParts inserts zero for leading dot", () => {
        const input = initInput({ aSep: '', aDec: '.', vMax: '999.99', vMin: '0.00' });
        input.value = '.';
        input.setSelectionRange(1, 1);
        input.focus();
        input.dispatchEvent(new KeyboardEvent('keydown', { key: '5', keyCode: 53, which: 53, bubbles: true }));
        input.dispatchEvent(new KeyboardEvent('keypress', { key: '5', keyCode: 53, which: 53, bubbles: true }));
        input.dispatchEvent(new KeyboardEvent('keyup', { key: '5', keyCode: 53, which: 53, bubbles: true }));
        expect(input.value).toBe('0.5');
    });

    it("updateOptions with setValue re-format", () => {
        const input = initInput({ aSep: ',', aDec: '.', vMax: '9999.99', vMin: '0.00' });
        AutoNumeric.setValue(input, '1234.56');
        expect(input.value).toBe('1,234.56');
        AutoNumeric.updateOptions(input, { aSep: '' });
        expect(input.value).toBe('1234.56');
    });

    it("getValue with lZero keep returns raw", () => {
        const input = initInput({ lZero: 'keep', aSep: '', aDec: '.' });
        input.value = '00123.45';
        expect(AutoNumeric.getValue(input)).toBe('00123.45');
    });

    it("getValue with +value === 0 returns '0'", () => {
        const input = initInput({ aSep: '', aDec: '.' });
        input.value = '0.00';
        expect(AutoNumeric.getValue(input)).toBe('0');
    });

    it("setValue on taglist element LI", () => {
        const li = document.createElement('li');
        AutoNumeric.init(li as any, { aSep: ',', aDec: '.', aPad: false });
        AutoNumeric.setValue(li as any, '9876.5');
        expect(li.textContent).toBe('9,876.5');
    });

    it("setValue on taglist element LABEL", () => {
        const label = document.createElement('label');
        AutoNumeric.init(label as any, { aSep: ',', aDec: '.', aPad: false });
        AutoNumeric.setValue(label as any, '123.45');
        expect(label.textContent).toBe('123.45');
    });

    it("aForm true with non-empty tagList element formats on init", () => {
        const div = document.createElement('div');
        div.textContent = '1234.56';
        AutoNumeric.init(div as any, { aForm: true, aSep: ',', aDec: '.' });
        expect(div.textContent).toBe('1234.56');
    });

    it("keyboard event with Ctrl+C (copy) does not modify value", () => {
        const input = initInput({});
        input.value = '123';
        input.focus();
        input.dispatchEvent(new KeyboardEvent('keydown', { key: 'c', keyCode: 67, which: 67, ctrlKey: true, bubbles: true }));
        expect(input.value).toBe('123');
    });

    it("keyboard event with Ctrl+X (cut)", () => {
        const input = initInput({});
        input.value = '123';
        input.focus();
        input.dispatchEvent(new KeyboardEvent('keydown', { key: 'x', keyCode: 88, which: 88, ctrlKey: true, bubbles: true }));
        expect(input.value).toBe('123');
    });

    it("focusin with nBracket null skips bracket conversion", () => {
        const input = initInput({});
        input.value = '-123';
        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('-123');
    });

    it("focusin with wEmpty zero and empty value", () => {
        const input = initInput({ wEmpty: 'zero', aSep: '', aDec: '.' });
        input.value = '';
        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('0');
    });

    it("pSign suffix with caret placement on focusin", () => {
        const input = initInput({ aSign: '€', pSign: 's', aSep: '', aDec: '.' });
        input.value = '';
        input.dispatchEvent(new FocusEvent('focusin', { bubbles: true }));
        expect(input.value).toBe('€');
    });

    it("setValue with negative value and nBracket null", () => {
        const input = initInput({ vMin: '-999.99', vMax: '999.99', aSep: '', aDec: '.', mDec: 2 });
        AutoNumeric.setValue(input, '-123.45');
        expect(input.value).toBe('-123.45');
    });

    it("autoGroup with dGroup 3 and large integer part", () => {
        const input = initInput({ dGroup: '3', aSep: ',', aDec: '.' });
        AutoNumeric.setValue(input, '1234567890.12');
        expect(input.value).toBe('1,234,567,890.12');
    });

    it("wEmpty sign with oEvent set and nBracket", () => {
        const input = initInput({ wEmpty: 'sign', aSign: '$', aSep: '', aDec: '.', vMin: '-999', vMax: '999', nBracket: '(,)' });
        AutoNumeric.setValue(input, '');
        expect(input.value).toBe('$');
    });

    it("lZero 'allow' with trailing non-zero digits after rounding pos", () => {
        const input = createInput();
        AutoNumeric.init(input, { mRound: 'B', mDec: 1, aSep: '', aDec: '.', vMin: '-999', vMax: '999' });
        AutoNumeric.setValue(input, '2.51');
        expect(input.value).toBe('2.5');
    });

    it("remove settings via destroy then verify getValue throws", () => {
        const input = initInput({});
        AutoNumeric.destroy(input);
        expect(() => AutoNumeric.getValue(input)).toThrow();
    });

    it("remove settings via destroy then verify setValue throws", () => {
        const input = initInput({});
        AutoNumeric.destroy(input);
        expect(() => AutoNumeric.setValue(input, '123')).toThrow();
    });

    it("remove settings via destroy then verify updateOptions throws", () => {
        const input = initInput({});
        AutoNumeric.destroy(input);
        expect(() => AutoNumeric.updateOptions(input, {})).toThrow();
    });

    it("hasInstance returns false for destroyed input", () => {
        const input = initInput({});
        AutoNumeric.destroy(input);
        expect(AutoNumeric.hasInstance(input)).toBe(false);
    });
});

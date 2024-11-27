import { Fluent } from "../../base";

/**
* Adapted from autoNumeric.js 1.9.22 by Bob Knothe / Sokolov Yura, https://github.com/BobKnothe/autoNumeric, removing jQuery dependency
*/
function getElementSelection(that: HTMLInputElement) {
    var position: { start?: number, end?: number, length?: number } = {};
    if (that.selectionStart === undefined) {
        that.focus();
        var select = (document as any).selection.createRange();
        position.length = select.text.length;
        select.moveStart('character', -that.value.length);
        position.end = select.text.length;
        position.start = position.end - position.length;
    } else {
        position.start = that.selectionStart;
        position.end = that.selectionEnd;
        position.length = position.end - position.start;
    }
    return position;
}

/**
 * Cross browser routine for setting selected range/cursor position
 */
function setElementSelection(that: HTMLInputElement, start: number, end: number) {
    if (that.selectionStart === undefined) {
        that.focus();
        var r = (that as any).createTextRange();
        r.collapse(true);
        r.moveEnd('character', end);
        r.moveStart('character', start);
        r.select();
    } else {
        that.selectionStart = start;
        that.selectionEnd = end;
    }
}

export interface AutoNumericOptions {
    aDec?: string;
    allowedAutoStrip?: RegExp;
    allowLeading?: boolean;
    altDec?: string;
    aForm?: boolean;
    aNum?: string;
    aNeg?: string;
    aSep?: string;
    aSign?: string;
    aNegRegAutoStrip?: string;
    aPad?: boolean;
    dGroup?: string;
    /** internal */
    holder?: any;
    lZero?: string;
    mDec?: number;
    mInt?: number;
    mRound?: string;
    nBracket?: string;
    numRegAutoStrip?: RegExp;
    oEvent?: any;
    pSign?: string;
    /** internal */
    runOnce?: boolean;
    skipFirstAutoStrip?: RegExp;
    skipLastAutoStrip?: RegExp;
    tagList?: string[];
    vMax?: any;
    vMin?: any;
    wEmpty?: string;
}

/**
 * run callbacks in parameters if any
 * any parameter could be a callback:
 * - a function, which invoked with jQuery element, parameters and this parameter name and returns parameter value
 * - a name of function, attached to $(selector).autoNumeric.functionName(){} - which was called previously
 */
function runCallbacks(input: HTMLInputElement & { autoNumeric?: AutoNumericOptions }, settings: AutoNumericOptions) {
    /**
     * loops through the settings object (option array) to find the following
     * k = option name example k=aNum
     * val = option value example val=0123456789
     */
    Object.keys(settings).forEach(function(k) {
        var val = (settings as any)[k];
        if (typeof val === 'function') {
            (settings as any)[k] = val(input, settings, k);
        } else if (typeof (AutoNumeric.getSettings(input) as any)?.[val] === 'function') {
            /**
             * calls the attached function from the html5 data example: data-a-sign="functionName"
             */
            (settings as any)[k] = (AutoNumeric.getSettings(input) as any)?.[val](input, settings, k);
        }
    });
}

function convertKeyToNumber(settings: Record<string, any>, key: string) {
    if (typeof (settings[key]) === 'string') {
        (settings as any)[key] *= 1;
    }
}

/**
 * Preparing user defined options for further usage
 * merge them with defaults appropriately
 */
function autoCode(input: HTMLInputElement, settings: AutoNumericOptions) {
    runCallbacks(input, settings);
    settings.oEvent = null;
    settings.tagList = ['B', 'CAPTION', 'CITE', 'CODE', 'DD', 'DEL', 'DIV', 'DFN', 'DT', 'EM', 'H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'INS', 'KDB', 'LABEL', 'LI', 'OUTPUT', 'P', 'Q', 'S', 'SAMPLE', 'SPAN', 'STRONG', 'TD', 'TH', 'U', 'VAR'];
    var vmax = settings.vMax.toString().split('.'),
        vmin = (!settings.vMin && settings.vMin !== 0) ? [] : settings.vMin.toString().split('.');
    convertKeyToNumber(settings, 'vMax');
    convertKeyToNumber(settings, 'vMin');
    convertKeyToNumber(settings, 'mDec'); /** set mDec if not defined by user */
    settings.mDec = (settings.mRound === 'CHF') ? 2 : settings.mDec;
    settings.allowLeading = true;
    settings.aNeg = settings.vMin < 0 ? '-' : '';
    vmax[0] = vmax[0].replace('-', '');
    vmin[0] = vmin[0].replace('-', '');
    settings.mInt = Math.max(vmax[0].length, vmin[0].length, 1);
    if (settings.mDec === null) {
        var vmaxLength = 0,
            vminLength = 0;
        if (vmax[1]) {
            vmaxLength = vmax[1].length;
        }
        if (vmin[1]) {
            vminLength = vmin[1].length;
        }
        settings.mDec = Math.max(vmaxLength, vminLength);
    } /** set alternative decimal separator key */
    if (settings.altDec === null && settings.mDec > 0) {
        if (settings.aDec === '.' && settings.aSep !== ',') {
            settings.altDec = ',';
        } else if (settings.aDec === ',' && settings.aSep !== '.') {
            settings.altDec = '.';
        }
    }
    /** cache regexps for autoStrip */
    var aNegReg = settings.aNeg ? '([-\\' + settings.aNeg + ']?)' : '(-?)';
    settings.aNegRegAutoStrip = aNegReg;
    settings.skipFirstAutoStrip = new RegExp(aNegReg + '[^-' + (settings.aNeg ? '\\' + settings.aNeg : '') + '\\' + settings.aDec + '\\d]' + '.*?(\\d|\\' + settings.aDec + '\\d)');
    settings.skipLastAutoStrip = new RegExp('(\\d\\' + settings.aDec + '?)[^\\' + settings.aDec + '\\d]\\D*$');
    var allowed = '-' + settings.aNum + '\\' + settings.aDec;
    settings.allowedAutoStrip = new RegExp('[^' + allowed + ']', 'gi');
    settings.numRegAutoStrip = new RegExp(aNegReg + '(?:\\' + settings.aDec + '?(\\d+\\' + settings.aDec + '\\d+)|(\\d*(?:\\' + settings.aDec + '\\d*)?))');
    return settings;
}

/**
 * strip all unwanted characters and leave only a number alert
 */
function autoStrip(s: string, settings: AutoNumericOptions, strip_zero?: string | boolean) {
    if (settings.aSign) { /** remove currency sign */
        while (s.indexOf(settings.aSign) > -1) {
            s = s.replace(settings.aSign, '');
        }
    }
    s = s.replace(settings.skipFirstAutoStrip, '$1$2'); /** first replace anything before digits */
    s = s.replace(settings.skipLastAutoStrip, '$1'); /** then replace anything after digits */
    s = s.replace(settings.allowedAutoStrip, ''); /** then remove any uninterested characters */
    if (settings.altDec) {
        s = s.replace(settings.altDec, settings.aDec);
    } /** get only number string */
    var m = s.match(settings.numRegAutoStrip);
    s = m ? [m[1], m[2], m[3]].join('') : '';
    if ((settings.lZero === 'allow' || settings.lZero === 'keep') && strip_zero !== 'strip') {
        var parts = [],
            nSign = '';
        parts = s.split(settings.aDec);
        if (parts[0].indexOf('-') !== -1) {
            nSign = '-';
            parts[0] = parts[0].replace('-', '');
        }
        if (parts[0].length > settings.mInt && parts[0].charAt(0) === '0') { /** strip leading zero if need */
            parts[0] = parts[0].slice(1);
        }
        s = nSign + parts.join(settings.aDec);
    }
    if ((strip_zero && settings.lZero === 'deny') || (strip_zero && settings.lZero === 'allow' && settings.allowLeading === false)) {
        var strip_reg = new RegExp('^' + settings.aNegRegAutoStrip + '0*(\\d' + (strip_zero === 'leading' ? ')' : '|$)'));
        s = s.replace(strip_reg, '$1$2');
    }
    return s;
}

/**
 * places or removes brackets on negative values
 */
function negativeBracket(s: string, nBracket: string, oEvent: string) { /** oEvent = settings.oEvent */
    var pBracket = nBracket.split(',');
    if (oEvent === 'set' || oEvent === 'focusout') {
        s = s.replace('-', '');
        s = pBracket[0] + s + pBracket[1];
    } else if ((oEvent === 'get' || oEvent === 'focusin' || oEvent === 'pageLoad') && s.charAt(0) === nBracket[0]) {
        s = s.replace(pBracket[0], '-');
        s = s.replace(pBracket[1], '');
    }
    return s;
}

/**
 * truncate decimal part of a number
 */
function truncateDecimal(s: string, aDec: string, mDec: number) {
    if (aDec && mDec) {
        var parts = s.split(aDec);
        /** truncate decimal part to satisfying length
         * cause we would round it anyway */
        if (parts[1] && parts[1].length > mDec) {
            if (mDec > 0) {
                parts[1] = parts[1].substring(0, mDec);
                s = parts.join(aDec);
            } else {
                s = parts[0];
            }
        }
    }
    return s;
}

/**
 * prepare number string to be converted to real number
 */
function fixNumber(s: string, aDec: string, aNeg: string) {
    if (aDec && aDec !== '.') {
        s = s.replace(aDec, '.');
    }
    if (aNeg && aNeg !== '-') {
        s = s.replace(aNeg, '-');
    }
    if (!s.match(/\d/)) {
        s += '0';
    }
    return s;
}

/**
 * function to handle numbers less than 0 that are stored in Exponential notation ex: .0000001 stored as 1e-7
 */
function checkValue(value: any, settings: AutoNumericOptions) {
    if (value) {
        var checkSmall = +value;
        if (checkSmall < 0.000001 && checkSmall > -1) {
            value = +value;
            if (value < 0.000001 && value > 0) {
                value = (value + 10).toString();
                value = value.substring(1);
            }
            if (value < 0 && value > -1) {
                value = (value - 10).toString();
                value = '-' + value.substring(2);
            }
            value = value.toString();
        } else {
            var parts = value.split('.');
            if (parts[1] !== undefined) {
                if (+parts[1] === 0) {
                    value = parts[0];
                } else {
                    parts[1] = parts[1].replace(/0*$/, '');
                    value = parts.join('.');
                }
            }
        }
    }
    return (settings.lZero === 'keep') ? value : value.replace(/^0*(\d)/, '$1');
}

/**
 * prepare real number to be converted to our format
 */
function presentNumber(s: string, aDec: string, aNeg: string) {
    if (aNeg && aNeg !== '-') {
        s = s.replace('-', aNeg);
    }
    if (aDec && aDec !== '.') {
        s = s.replace('.', aDec);
    }
    return s;
}

/**
 * checking that number satisfy format conditions
 * and lays between settings.vMin and settings.vMax
 * and the string length does not exceed the digits in settings.vMin and settings.vMax
 */
function autoCheck(s: string, settings: AutoNumericOptions) {
    s = autoStrip(s, settings);
    s = truncateDecimal(s, settings.aDec, settings.mDec);
    s = fixNumber(s, settings.aDec, settings.aNeg);
    var value = +s;
    return value >= settings.vMin && value <= settings.vMax;
}

/**
 * private function to check for empty value
 */
function checkEmpty(iv: string, settings: AutoNumericOptions, signOnEmpty?: boolean) {
    if (iv === '' || iv === settings.aNeg) {
        if (settings.wEmpty === 'zero') {
            return iv + '0';
        }
        if (settings.wEmpty === 'sign' || signOnEmpty) {
            return iv + settings.aSign;
        }
        return iv;
    }
    return null;
}

/**
 * private function that formats our number
 */
function autoGroup(iv: string, settings: AutoNumericOptions) {
    iv = autoStrip(iv, settings);
    var testNeg = iv.replace(',', '.'),
        empty = checkEmpty(iv, settings, true);
    if (empty !== null) {
        return empty;
    }
    var digitalGroup: any = '';
    if (settings.dGroup == "2") {
        digitalGroup = /(\d)((\d)(\d{2}?)+)$/;
    } else if (settings.dGroup == "4") {
        digitalGroup = /(\d)((\d{4}?)+)$/;
    } else {
        digitalGroup = /(\d)((\d{3}?)+)$/;
    } /** splits the string at the decimal string */
    var ivSplit = iv.split(settings.aDec);
    if (settings.altDec && ivSplit.length === 1) {
        ivSplit = iv.split(settings.altDec);
    } /** assigns the whole number to the a varibale (s) */
    var s = ivSplit[0];
    if (settings.aSep) {
        while (digitalGroup.test(s)) { /** re-inserts the thousand sepparator via a regualer expression */
            s = s.replace(digitalGroup, '$1' + settings.aSep + '$2');
        }
    }
    if (settings.mDec !== 0 && ivSplit.length > 1) {
        if (ivSplit[1].length > settings.mDec) {
            ivSplit[1] = ivSplit[1].substring(0, settings.mDec);
        } /** joins the whole number with the deciaml value */
        iv = s + settings.aDec + ivSplit[1];
    } else { /** if whole numbers only */
        iv = s;
    }
    if (settings.aSign) {
        var has_aNeg = iv.indexOf(settings.aNeg) !== -1;
        iv = iv.replace(settings.aNeg, '');
        iv = settings.pSign === 'p' ? settings.aSign + iv : iv + settings.aSign;
        if (has_aNeg) {
            iv = settings.aNeg + iv;
        }
    }
    if (settings.oEvent === 'set' && (testNeg as any) < 0 && settings.nBracket !== null) { /** removes the negative sign and places brackets */
        iv = negativeBracket(iv, settings.nBracket, settings.oEvent);
    }
    return iv;
}

/**
 * round number after setting by pasting or $().autoNumericSet()
 * private function for round the number
 * please note this handled as text - JavaScript math function can return inaccurate values
 * also this offers multiple rounding methods that are not easily accomplished in JavaScript
 */
function autoRound(iv: string, settings: AutoNumericOptions) { /** value to string */
    iv = (iv === '') ? '0' : iv.toString();
    convertKeyToNumber(settings, 'mDec'); /** set mDec to number needed when mDec set by 'update method */
    if (settings.mRound === 'CHF') {
        iv = (Math.round(iv as any * 20) / 20).toString();
    }
    var ivRounded = '',
        i = 0,
        nSign = '',
        rDec = (typeof (settings.aPad) === 'boolean' || settings.aPad === null) ? (settings.aPad ? settings.mDec : 0) : +settings.aPad;
    var truncateZeros = function (ivRounded: string) { /** truncate not needed zeros */
        var regex = (rDec === 0) ? (/(\.(?:\d*[1-9])?)0*$/) : rDec === 1 ? (/(\.\d(?:\d*[1-9])?)0*$/) : new RegExp('(\\.\\d{' + rDec + '}(?:\\d*[1-9])?)0*$');
        ivRounded = ivRounded.replace(regex, '$1'); /** If there are no decimal places, we don't need a decimal point at the end */
        if (rDec === 0) {
            ivRounded = ivRounded.replace(/\.$/, '');
        }
        return ivRounded;
    };
    if (iv.charAt(0) === '-') { /** Checks if the iv (input Value)is a negative value */
        nSign = '-';
        iv = iv.replace('-', ''); /** removes the negative sign will be added back later if required */
    }
    if (!iv.match(/^\d/)) { /** append a zero if first character is not a digit (then it is likely to be a dot)*/
        iv = '0' + iv;
    }
    if (nSign === '-' && +iv === 0) { /** determines if the value is zero - if zero no negative sign */
        nSign = '';
    }
    if ((+iv > 0 && settings.lZero !== 'keep') || (iv.length > 0 && settings.lZero === 'allow')) { /** trims leading zero's if needed */
        iv = iv.replace(/^0*(\d)/, '$1');
    }
    var dPos = iv.lastIndexOf('.'), /** virtual decimal position */
        vdPos = (dPos === -1) ? iv.length - 1 : dPos, /** checks decimal places to determine if rounding is required */
        cDec = (iv.length - 1) - vdPos; /** check if no rounding is required */
    if (cDec <= settings.mDec) {
        ivRounded = iv; /** check if we need to pad with zeros */
        if (cDec < rDec) {
            if (dPos === -1) {
                ivRounded += '.';
            }
            var zeros = '000000';
            while (cDec < rDec) {
                zeros = zeros.substring(0, rDec - cDec);
                ivRounded += zeros;
                cDec += zeros.length;
            }
        } else if (cDec > rDec) {
            ivRounded = truncateZeros(ivRounded);
        } else if (cDec === 0 && rDec === 0) {
            ivRounded = ivRounded.replace(/\.$/, '');
        }
        if (settings.mRound !== 'CHF') {
            return (+ivRounded === 0) ? ivRounded : nSign + ivRounded;
        }
        if (settings.mRound === 'CHF') {
            dPos = ivRounded.lastIndexOf('.');
            iv = ivRounded;
        }

    } /** rounded length of the string after rounding */
    var rLength = dPos + settings.mDec,
        tRound = +iv.charAt(rLength + 1),
        ivArray: any[] = iv.substring(0, rLength + 1).split(''),
        odd = (iv.charAt(rLength) === '.') ? ((iv.charAt(rLength - 1) as any) % 2) : ((iv.charAt(rLength) as any) % 2),
        onePass = true;
    odd = (odd === 0 && (iv.substring(rLength + 2, iv.length) as any > 0)) ? 1 : 0;
    if ((tRound > 4 && settings.mRound === 'S') || /** Round half up symmetric */
            (tRound > 4 && settings.mRound === 'A' && nSign === '') || /** Round half up asymmetric positive values */
            (tRound > 5 && settings.mRound === 'A' && nSign === '-') || /** Round half up asymmetric negative values */
            (tRound > 5 && settings.mRound === 's') || /** Round half down symmetric */
            (tRound > 5 && settings.mRound === 'a' && nSign === '') || /** Round half down asymmetric positive values */
            (tRound > 4 && settings.mRound === 'a' && nSign === '-') || /** Round half down asymmetric negative values */
            (tRound > 5 && settings.mRound === 'B') || /** Round half even "Banker's Rounding" */
            (tRound === 5 && settings.mRound === 'B' && odd === 1) || /** Round half even "Banker's Rounding" */
            (tRound > 0 && settings.mRound === 'C' && nSign === '') || /** Round to ceiling toward positive infinite */
            (tRound > 0 && settings.mRound === 'F' && nSign === '-') || /** Round to floor toward negative infinite */
            (tRound > 0 && settings.mRound === 'U') ||
            (settings.mRound === 'CHF')) { /** round up away from zero */
        for (i = (ivArray.length - 1); i >= 0; i -= 1) { /** Round up the last digit if required, and continue until no more 9's are found */
            if (ivArray[i] !== '.') {
                if (settings.mRound === 'CHF' && ivArray[i] <= 2 && onePass) {
                    ivArray[i] = 0;
                    onePass = false;
                    break;
                }
                if (settings.mRound === 'CHF' && ivArray[i] <= 7 && onePass) {
                    ivArray[i] = 5;
                    onePass = false;
                    break;
                }
                if (settings.mRound === 'CHF' && onePass) {
                    ivArray[i] = 10;
                    onePass = false;
                } else {
                    ivArray[i] = +ivArray[i] + 1;
                }
                if (ivArray[i] < 10) {
                    break;
                }
                if (i > 0) {
                    ivArray[i] = '0';
                }
            }
        }
    }
    ivArray = ivArray.slice(0, rLength + 1); /** Reconstruct the string, converting any 10's to 0's */
    ivRounded = truncateZeros(ivArray.join('')); /** return rounded value */
    return (+ivRounded === 0) ? ivRounded : nSign + ivRounded;
}

/**
 * Holder object for field properties
 */
class AutoNumericHolder {

    ctrlKey: boolean;
    cmdKey: boolean;
    dirty: boolean;
    formatted: boolean;
    kdCode: number;
    processed: boolean;
    selection: any;
    settings: AutoNumericOptions;
    settingsClone: AutoNumericOptions;
    shiftKey: boolean;
    that: HTMLInputElement;
    value: string;
    valuePartsBeforePaste: string[];
    which: number;

    constructor(that: HTMLInputElement, settings: AutoNumericOptions) {
        this.settings = settings;
        this.that = that;
        this.formatted = false;
        this.settingsClone = autoCode(this.that, this.settings);
        this.value = that.value;
    }
    
    init(e: KeyboardEvent) {
        this.value = this.that.value;
        this.settingsClone = autoCode(this.that, this.settings);
        this.ctrlKey = e.ctrlKey;
        this.cmdKey = e.metaKey;
        this.shiftKey = e.shiftKey;
        this.selection = getElementSelection(this.that); /** keypress event overwrites meaningful value of e.keyCode */
        if (e.type === 'keydown' || e.type === 'keyup') {
            this.kdCode = (e as any).keyCode;
        }
        this.which = (e as any).which;
        this.processed = false;
        this.formatted = false;
    }

    setSelection(start: number, end: number, setReal?: boolean) {
        start = Math.max(start, 0);
        end = Math.min(end, this.that.value.length);
        this.selection = {
            start: start,
            end: end,
            length: end - start
        };
        if (setReal === undefined || setReal) {
            setElementSelection(this.that, start, end);
        }
    }

    setPosition(pos: number, setReal?: boolean) {
        this.setSelection(pos, pos, setReal);
    }

    getBeforeAfter() {
        var value = this.value,
            left = value.substring(0, this.selection.start),
            right = value.substring(this.selection.end, value.length);
        return [left, right];
    }

    getBeforeAfterStriped() {
        var parts = this.getBeforeAfter();
        parts[0] = autoStrip(parts[0], this.settingsClone);
        parts[1] = autoStrip(parts[1], this.settingsClone);
        return parts;
    }

    /**
     * strip parts from excess characters and leading zeroes
     */
    normalizeParts(left: string, right: string) {
        var settingsClone = this.settingsClone;
        right = autoStrip(right, settingsClone); /** if right is not empty and first character is not aDec, */
        /** we could strip all zeros, otherwise only leading */
        var strip = right.match(/^\d/) ? true : 'leading';
        left = autoStrip(left, settingsClone, strip); /** prevents multiple leading zeros from being entered */
        if ((left === '' || left === settingsClone.aNeg) && settingsClone.lZero === 'deny') {
            if (right > '') {
                right = right.replace(/^0*(\d)/, '$1');
            }
        }
        var new_value = left + right; /** insert zero if has leading dot */
        if (settingsClone.aDec) {
            var m = new_value.match(new RegExp('^' + settingsClone.aNegRegAutoStrip + '\\' + settingsClone.aDec));
            if (m) {
                left = left.replace(m[1], m[1] + '0');
                new_value = left + right;
            }
        } /** insert zero if number is empty and io.wEmpty == 'zero' */
        if (settingsClone.wEmpty === 'zero' && (new_value === settingsClone.aNeg || new_value === '')) {
            left += '0';
        }
        return [left, right];
    }

    /**
     * set part of number to value keeping position of cursor
     */
    setValueParts(left: string, right: string) {
        var settingsClone = this.settingsClone,
            parts = this.normalizeParts(left, right),
            new_value = parts.join(''),
            position = parts[0].length;
        this.dirty = true;
        if (new_value.trim() === '') {
            new_value = '';
        }
        else if (autoCheck(new_value, settingsClone)) {
            new_value = truncateDecimal(new_value, settingsClone.aDec, settingsClone.mDec);
        }
        else {
            return false;
        }

        if (position > new_value.length) {
            position = new_value.length;
        }
        this.value = new_value;
        this.setPosition(position, false);
        return true;
    }

    /**
     * helper function for expandSelectionOnSign
     * returns sign position of a formatted value
     */
    signPosition() {
        var settingsClone = this.settingsClone,
            aSign = settingsClone.aSign,
            that = this.that;
        if (aSign) {
            var aSignLen = aSign.length;
            if (settingsClone.pSign === 'p') {
                var hasNeg = settingsClone.aNeg && that.value && that.value.charAt(0) === settingsClone.aNeg;
                return hasNeg ? [1, aSignLen + 1] : [0, aSignLen];
            }
            var valueLen = that.value.length;
            return [valueLen - aSignLen, valueLen];
        }
        return [1000, -1];
    }

    /**
     * expands selection to cover whole sign
     * prevents partial deletion/copying/overwriting of a sign
     */
    expandSelectionOnSign(setReal?: boolean) {
        var sign_position = this.signPosition(),
            selection = this.selection;
        if (selection.start < sign_position[1] && selection.end > sign_position[0]) { /** if selection catches something except sign and catches only space from sign */
            if ((selection.start < sign_position[0] || selection.end > sign_position[1]) && this.value.substring(Math.max(selection.start, sign_position[0]), Math.min(selection.end, sign_position[1])).match(/^\s*$/)) { /** then select without empty space */
                if (selection.start < sign_position[0]) {
                    this.setSelection(selection.start, sign_position[0], setReal);
                } else {
                    this.setSelection(sign_position[1], selection.end, setReal);
                }
            } else { /** else select with whole sign */
                this.setSelection(Math.min(selection.start, sign_position[0]), Math.max(selection.end, sign_position[1]), setReal);
            }
        }
    }

    /**
     * try to strip pasted value to digits
     */
    checkPaste() {
        if (this.valuePartsBeforePaste !== undefined) {
            var parts = this.getBeforeAfter(),
                oldParts = this.valuePartsBeforePaste;
            delete this.valuePartsBeforePaste; /** try to strip pasted value first */
            parts[0] = parts[0].substring(0, oldParts[0].length) + autoStrip(parts[0].substring(oldParts[0].length), this.settingsClone);
            if (!this.setValueParts(parts[0], parts[1])) {
                this.value = oldParts.join('');
                this.setPosition(oldParts[0].length, false);
            }
        }
    }

    /**
     * process pasting, cursor moving and skipping of not interesting keys
     * if returns true, futher processing is not performed
     */
    skipAllways(e: Event) {
        var kdCode = this.kdCode,
            which = this.which,
            ctrlKey = this.ctrlKey,
            cmdKey = this.cmdKey,
            shiftKey = this.shiftKey; /** catch the ctrl up on ctrl-v */
        if (((ctrlKey || cmdKey) && e.type === 'keyup' && this.valuePartsBeforePaste !== undefined) || (shiftKey && kdCode === 45)) {
            this.checkPaste();
            return false;
        }
        /** codes are taken from http://www.cambiaresearch.com/c4/702b8cd1-e5b0-42e6-83ac-25f0306e3e25/Javascript-Char-Codes-Key-Codes.aspx
         * skip Fx keys, windows keys, other special keys
         */
        if ((kdCode >= 112 && kdCode <= 123) || (kdCode >= 91 && kdCode <= 93) || (kdCode >= 9 && kdCode <= 31) || (kdCode < 8 && (which === 0 || which === kdCode)) || kdCode === 144 || kdCode === 145 || kdCode === 45) {
            return true;
        }
        if ((ctrlKey || cmdKey) && kdCode === 65) { /** if select all (a=65)*/
            return true;
        }
        if ((ctrlKey || cmdKey) && (kdCode === 67 || kdCode === 86 || kdCode === 88)) { /** if copy (c=67) paste (v=86) or cut (x=88) */
            if (e.type === 'keydown') {
                this.expandSelectionOnSign();
            }
            if (kdCode === 86 || kdCode as any === 45) { /** try to prevent wrong paste */
                if (e.type === 'keydown' || e.type === 'keypress') {
                    if (this.valuePartsBeforePaste === undefined) {
                        this.valuePartsBeforePaste = this.getBeforeAfter();
                    }
                } else {
                    this.checkPaste();
                }
            }
            return e.type === 'keydown' || e.type === 'keypress' || kdCode === 67;
        }
        if (ctrlKey || cmdKey) {
            return true;
        }
        if (kdCode === 37 || kdCode === 39) { /** jump over thousand separator */
            var aSep = this.settingsClone.aSep,
                start = this.selection.start,
                value = this.that.value;
            if (e.type === 'keydown' && aSep && !this.shiftKey) {
                if (kdCode === 37 && value.charAt(start - 2) === aSep) {
                    this.setPosition(start - 1);
                } else if (kdCode === 39 && value.charAt(start + 1) === aSep) {
                    this.setPosition(start + 1);
                }
            }
            return true;
        }
        if (kdCode >= 34 && kdCode <= 40) {
            return true;
        }
        return false;
    }

    /**
     * process deletion of characters
     * returns true if processing performed
     */
    processAllways() {
        var parts; /** process backspace or delete */
        if (this.kdCode === 8 || this.kdCode === 46) {
            if (!this.selection.length) {
                parts = this.getBeforeAfterStriped();
                if (this.kdCode === 8) {
                    parts[0] = parts[0].substring(0, parts[0].length - 1);
                } else {
                    parts[1] = parts[1].substring(1, parts[1].length);
                }
                this.setValueParts(parts[0], parts[1]);
            } else {
                this.expandSelectionOnSign(false);
                parts = this.getBeforeAfterStriped();
                this.setValueParts(parts[0], parts[1]);
            }
            return true;
        }
        return false;
    }

    /**
     * process insertion of characters
     * returns true if processing performed
     */
    processKeypress() {
        var settingsClone = this.settingsClone,
            cCode = String.fromCharCode(this.which),
            parts = this.getBeforeAfterStriped(),
            left = parts[0],
            right = parts[1]; /** start rules when the decimal character key is pressed */
        /** always use numeric pad dot to insert decimal separator */
        if (cCode === settingsClone.aDec || (settingsClone.altDec && cCode === settingsClone.altDec) || ((cCode === '.' || cCode === ',') && this.kdCode === 110)) { /** do not allow decimal character if no decimal part allowed */
            if (!settingsClone.mDec || !settingsClone.aDec) {
                return true;
            } /** do not allow decimal character before aNeg character */
            if (settingsClone.aNeg && right.indexOf(settingsClone.aNeg) > -1) {
                return true;
            } /** do not allow decimal character if other decimal character present */
            if (left.indexOf(settingsClone.aDec) > -1) {
                return true;
            }
            if (right.indexOf(settingsClone.aDec) > 0) {
                return true;
            }
            if (right.indexOf(settingsClone.aDec) === 0) {
                right = right.substring(1);
            }
            this.setValueParts(left + settingsClone.aDec, right);
            return true;
        } /** start rule on negative sign */

        if (cCode === '-' || cCode === '+') { /** prevent minus if not allowed */
            if (!settingsClone.aNeg) {
                return true;
            } /** caret is always after minus */
            if (left === '' && right.indexOf(settingsClone.aNeg) > -1) {
                left = settingsClone.aNeg;
                right = right.substring(1, right.length);
            } /** change sign of number, remove part if should */
            if (left.charAt(0) === settingsClone.aNeg) {
                left = left.substring(1, left.length);
            } else {
                left = (cCode === '-') ? settingsClone.aNeg + left : left;
            }
            this.setValueParts(left, right);
            return true;
        } /** digits */
        if (cCode >= '0' && cCode <= '9') { /** if try to insert digit before minus */
            if (settingsClone.aNeg && left === '' && right.indexOf(settingsClone.aNeg) > -1) {
                left = settingsClone.aNeg;
                right = right.substring(1, right.length);
            }
            if (settingsClone.vMax <= 0 && settingsClone.vMin < settingsClone.vMax && this.value.indexOf(settingsClone.aNeg) === -1 && cCode !== '0') {
                left = settingsClone.aNeg + left;
            }
            this.setValueParts(left + cCode, right);
            return true;
        } /** prevent any other character */
        return true;
    }

    /**
     * formatting of just processed value with keeping of cursor position
     */
    formatQuick() {
        var settingsClone = this.settingsClone,
            parts = this.getBeforeAfterStriped(),
            leftLength = this.value;
        if ((settingsClone.aSep === '' || (settingsClone.aSep !== '' && leftLength.indexOf(settingsClone.aSep) === -1)) && (settingsClone.aSign === '' || (settingsClone.aSign !== '' && leftLength.indexOf(settingsClone.aSign) === -1))) {
            var subParts = [],
                nSign = '';
            subParts = leftLength.split(settingsClone.aDec);
            if (subParts[0].indexOf('-') > -1) {
                nSign = '-';
                subParts[0] = subParts[0].replace('-', '');
                parts[0] = parts[0].replace('-', '');
            }
            if (subParts[0].length > settingsClone.mInt && parts[0].charAt(0) === '0') { /** strip leading zero if need */
                parts[0] = parts[0].slice(1);
            }
            parts[0] = nSign + parts[0];
        }
        var value = autoGroup(this.value, this.settingsClone),
            position = value.length;
        if (value) {
            /** prepare regexp which searches for cursor position from unformatted left part */
            var left_ar = parts[0].split(''),
                i = 0;
            for (i; i < left_ar.length; i += 1) { /** thanks Peter Kovari */
                if (!left_ar[i].match('\\d')) {
                    left_ar[i] = '\\' + left_ar[i];
                }
            }
            var leftReg = new RegExp('^.*?' + left_ar.join('.*?'));
            /** search cursor position in formatted value */
            var newLeft = value.match(leftReg);
            if (newLeft) {
                position = newLeft[0].length;
                /** if we are just before sign which is in prefix position */
                if (((position === 0 && value.charAt(0) !== settingsClone.aNeg) || (position === 1 && value.charAt(0) === settingsClone.aNeg)) && settingsClone.aSign && settingsClone.pSign === 'p') {
                    /** place carret after prefix sign */
                    position = this.settingsClone.aSign.length + (value.charAt(0) === '-' ? 1 : 0);
                }
            } else if (settingsClone.aSign && settingsClone.pSign === 's') {
                /** if we could not find a place for cursor and have a sign as a suffix */
                /** place carret before suffix currency sign */
                position -= settingsClone.aSign.length;
            }
        }
        this.that.value = value;
        this.setPosition(position);
        this.formatted = true;
    }
}

/** thanks to Anthony & Evan C */
function autoGet(obj: any) {
    if (typeof obj === 'string') {
        obj = obj.replace(/\[/g, "\\[").replace(/\]/g, "\\]");
        obj = '#' + obj.replace(/(:|\.)/g, '\\$1');
        /** obj = '#' + obj.replace(/([;&,\.\+\*\~':"\!\^#$%@\[\]\(\)=>\|])/g, '\\$1'); */
        /** possible modification to replace the above 2 lines */
        return document.querySelector(obj);
    }
    return obj;
}

function getHolder(that: HTMLInputElement, settings?: AutoNumericOptions, update?: boolean) {
    var data = AutoNumeric.getSettings(that);
    if (!data) {
        data = {};
        (that as any).autoNumeric = data;
    }
    var holder = data.holder;
    if ((holder === undefined && settings) || update) {
        holder = new AutoNumericHolder(that, settings);
        data.holder = holder;
    }
    return holder;
}

export class AutoNumeric {

    static init(input: HTMLInputElement, options: AutoNumericOptions): void {
        if (!input) {
            throw new Error("autoNumeric called with null element!");
        }
        var settings: AutoNumericOptions = AutoNumeric.getSettings(input), /** attempt to grab 'autoNumeric' settings, if they don't exist returns "undefined". */
            tagData: Record<string, string> = {}; /** attempt to grab HTML5 data, if they don't exist we'll get "undefined".*/
            Object.keys(input.dataset).forEach(key => tagData[key] = input.dataset[key]);
        if (typeof settings !== 'object') { /** If we couldn't grab settings, create them from defaults and passed options. */
            var defaults: AutoNumericOptions = {
                /** allowed numeric values
                 * please do not modify
                 */
                aNum: '0123456789',
                /** allowed thousand separator characters
                 * comma = ','
                 * period "full stop" = '.'
                 * apostrophe is escaped = '\''
                 * space = ' '
                 * none = ''
                 * NOTE: do not use numeric characters
                 */
                aSep: ',',
                /** digital grouping for the thousand separator used in Format
                 * dGroup: '2', results in 99,99,99,999 common in India for values less than 1 billion and greater than -1 billion
                 * dGroup: '3', results in 999,999,999 default
                 * dGroup: '4', results in 9999,9999,9999 used in some Asian countries
                 */
                dGroup: '3',
                /** allowed decimal separator characters
                 * period "full stop" = '.'
                 * comma = ','
                 */
                aDec: '.',
                /** allow to declare alternative decimal separator which is automatically replaced by aDec
                 * developed for countries the use a comma ',' as the decimal character
                 * and have keyboards\numeric pads that have a period 'full stop' as the decimal characters (Spain is an example)
                 */
                altDec: null,
                /** allowed currency symbol
                 * Must be in quotes aSign: '$', a space is allowed aSign: '$ '
                 */
                aSign: '',
                /** placement of currency sign
                 * for prefix pSign: 'p',
                 * for suffix pSign: 's',
                 */
                pSign: 'p',
                /** maximum possible value
                 * value must be enclosed in quotes and use the period for the decimal point
                 * value must be larger than vMin
                 */
                vMax: '9999999999999.99',
                /** minimum possible value
                 * value must be enclosed in quotes and use the period for the decimal point
                 * value must be smaller than vMax
                 */
                vMin: '0.00',
                /** max number of decimal places = used to override decimal places set by the vMin & vMax values
                 * value must be enclosed in quotes example mDec: '3',
                 * This can also set the value via a call back function mDec: 'css:#
                 */
                mDec: null,
                /** method used for rounding
                 * mRound: 'S', Round-Half-Up Symmetric (default)
                 * mRound: 'A', Round-Half-Up Asymmetric
                 * mRound: 's', Round-Half-Down Symmetric (lower case s)
                 * mRound: 'a', Round-Half-Down Asymmetric (lower case a)
                 * mRound: 'B', Round-Half-Even "Bankers Rounding"
                 * mRound: 'U', Round Up "Round-Away-From-Zero"
                 * mRound: 'D', Round Down "Round-Toward-Zero" - same as truncate
                 * mRound: 'C', Round to Ceiling "Toward Positive Infinity"
                 * mRound: 'F', Round to Floor "Toward Negative Infinity"
                 */
                mRound: 'S',
                /** controls decimal padding
                 * aPad: true - always Pad decimals with zeros
                 * aPad: false - does not pad with zeros.
                 * aPad: `some number` - pad decimals with zero to number different from mDec
                 * thanks to Jonas Johansson for the suggestion
                 */
                aPad: true,
                /** places brackets on negative value -$ 999.99 to (999.99)
                 * visible only when the field does NOT have focus the left and right symbols should be enclosed in quotes and seperated by a comma
                 * nBracket: null, nBracket: '(,)', nBracket: '[,]', nBracket: '<,>' or nBracket: '{,}'
                 */
                nBracket: null,
                /** Displayed on empty string
                 * wEmpty: 'empty', - input can be blank
                 * wEmpty: 'zero', - displays zero
                 * wEmpty: 'sign', - displays the currency sign
                 */
                wEmpty: 'empty',
                /** controls leading zero behavior
                 * lZero: 'allow', - allows leading zeros to be entered. Zeros will be truncated when entering additional digits. On focusout zeros will be deleted.
                 * lZero: 'deny', - allows only one leading zero on values less than one
                 * lZero: 'keep', - allows leading zeros to be entered. on fousout zeros will be retained.
                 */
                lZero: 'allow',
                /** determine if the default value will be formatted on page ready.
                 * true = automatically formats the default value on page ready
                 * false = will not format the default value
                 */
                aForm: true
            };
            settings = Object.assign({}, defaults, tagData, options); /** Merge defaults, tagData and options */
            if (settings.aDec === settings.aSep) {
                throw new Error("autoNumeric will not function properly when the decimal character aDec: '" + settings.aDec + "' and thousand separator aSep: '" + settings.aSep + "' are the same character");
            }
            (input as any).autoNumeric = settings; /** Save our new settings */
        } else {
            return;
        }
        settings.runOnce = false;
        var holder = getHolder(input, settings);
        if (!settings.tagList?.includes(input.tagName) && input.tagName !== 'INPUT') {
            throw new Error("The <" + input.tagName + "> is not supported by autoNumeric()");
        }
        if (settings.runOnce === false && settings.aForm) {/** routine to format default value on page load */
            if (input.matches('input[type=text], input[type=hidden], input[type=tel], input:not([type])')) {
                var setValue = true;
                if (input.value === '' && settings.wEmpty === 'empty') {
                    input.value = '';
                    setValue = false;
                }
                if (input.value === '' && settings.wEmpty === 'sign') {
                    input.value = settings.aSign;
                    setValue = false;
                }
                if (setValue) {
                    AutoNumeric.setValue(input, input.value);
                }
            }
            if (!settings.tagList?.includes(input.tagName) && input.textContent !== '') {
                AutoNumeric.setValue(input, input.textContent);
            }
        }

        settings.runOnce = true;

        if (input.matches('input[type=text], input[type=hidden], input[type=tel], input:not([type])')) { /**added hidden type */
            Fluent.on(input, 'keydown.autoNumeric', function (e) {
                holder = getHolder(input);
                if (holder.settings.aDec === holder.settings.aSep) {
                    throw new Error("autoNumeric will not function properly when the decimal character aDec: '" + holder.settings.aDec + "' and thousand separator aSep: '" + holder.settings.aSep + "' are the same character");
                }
                if (holder.that.readOnly) {
                    holder.processed = true;
                    return true;
                }
                /** The below streamed code / comment allows the "enter" keydown to throw a change() event */
                /** if (e.keyCode === 13 && holder.inVal !== $this.val()){
                    $this.change();
                    holder.inVal = $this.val();
                }*/
                holder.init(e);
                holder.settings.oEvent = 'keydown';
                if (holder.skipAllways(e)) {
                    holder.processed = true;
                    return true;
                }
                if (holder.processAllways()) {
                    holder.processed = true;
                    holder.formatQuick();
                    e.preventDefault();
                    return false;
                }
                holder.formatted = false;
                return true;
            });
            
            Fluent.on(input, 'keypress.autoNumeric', function (e) {
                var holder = getHolder(input),
                    processed = holder.processed;
                holder.init(e);
                holder.settings.oEvent = 'keypress';
                if (holder.skipAllways(e)) {
                    return true;
                }
                if (processed) {
                    e.preventDefault();
                    return false;
                }
                if (holder.processAllways() || holder.processKeypress()) {
                    holder.formatQuick();
                    e.preventDefault();
                    return false;
                }
                holder.formatted = false;
            });

            Fluent.on(input, 'keyup.autoNumeric', function (e) {
                var holder = getHolder(input);
                holder.init(e);
                holder.settings.oEvent = 'keyup';
                var skip = holder.skipAllways(e);
                holder.kdCode = 0;
                delete holder.valuePartsBeforePaste;
                if (input.value === holder.settings.aSign) { /** added to properly place the caret when only the currency is present */
                    if (holder.settings.pSign === 's') {
                        setElementSelection(input, 0, 0);
                    } else {
                        setElementSelection(input, holder.settings.aSign.length, holder.settings.aSign.length);
                    }
                }
                if (skip) {
                    return true;
                }
                if (input.value === '') {
                    return true;
                }
                if (!holder.formatted) {
                    holder.formatQuick();
                }
            });

            Fluent.on(input, 'focusin.autoNumeric', function () {

                if (input.matches('[readonly]') || input.matches('[disabled]'))
                    return;

                var holder = getHolder(input);
                holder.settingsClone.oEvent = 'focusin';
                if (holder.settingsClone.nBracket !== null) {
                    var checkVal = input.value;
                    input.value = negativeBracket(checkVal, holder.settingsClone.nBracket, holder.settingsClone.oEvent);
                }
                holder.inVal = input.value;
                holder.dirty = false;
                var onempty = checkEmpty(holder.inVal, holder.settingsClone, true);
                if (onempty !== null) {
                    input.value = onempty ?? "";
                    if (holder.settings.pSign === 's') {
                        setElementSelection(input, 0, 0);
                    } else {
                        setElementSelection(input, holder.settings.aSign.length, holder.settings.aSign.length);
                    }
                }
            });

            Fluent.on(input, 'focusout.autoNumeric', function () {

                if (input.matches('[readonly]') || input.matches('[disabled]'))
                    return;

                var holder = getHolder(input),
                    settingsClone = holder.settingsClone,
                    value = input.value,
                    origValue = value;
                holder.settingsClone.oEvent = 'focusout';
                var strip_zero = ''; /** added to control leading zero */
                if (settingsClone.lZero === 'allow') { /** added to control leading zero */
                    settingsClone.allowLeading = false;
                    strip_zero = 'leading';
                }
                if (value !== '') {
                    value = autoStrip(value, settingsClone, strip_zero);
                    if (checkEmpty(value, settingsClone) === null && (
                        (!holder.dirty && holder.inVal == origValue) || autoCheck(value, settingsClone))) {
                        value = fixNumber(value, settingsClone.aDec, settingsClone.aNeg);
                        value = autoRound(value, settingsClone);
                        value = presentNumber(value, settingsClone.aDec, settingsClone.aNeg);
                    } else {
                        value = '';
                    }
                }
                var groupedValue = checkEmpty(value, settingsClone, false);
                if (groupedValue === null) {
                    groupedValue = autoGroup(value, settingsClone);
                }
                if (groupedValue !== origValue) {
                    input.value = groupedValue;
                }
                if (groupedValue !== holder.inVal) {
                    Fluent.trigger(input, "change");
                    delete holder.inVal;
                }
                if (settingsClone.nBracket !== null && AutoNumeric.getValue(input) as any < 0) {
                    holder.settingsClone.oEvent = 'focusout';
                    input.value = negativeBracket(input.value, settingsClone.nBracket, settingsClone.oEvent);
                }
            });
        }
    }

    /** method to remove settings and stop autoNumeric() */
    static destroy(input: HTMLInputElement) {
        Fluent.off(input, '.autoNumeric');
        delete (input as any)?.autoNumeric;
    }

    /** method to update settings - can call as many times */
    static updateOptions(input: HTMLInputElement, options: AutoNumericOptions) {
        var settings = AutoNumeric.getSettings(input);
        if (typeof settings !== 'object') {
            throw new Error("You must initialize autoNumeric('init', {options}) prior to calling the 'update' method");
        }
        var strip = AutoNumeric.getValue(input);
        settings = Object.assign(settings, options);
        getHolder(input, settings, true);
        if (settings.aDec === settings.aSep) {
            throw new Error("autoNumeric will not function properly when the decimal character aDec: '" + settings.aDec + "' and thousand separator aSep: '" + settings.aSep + "' are the same character");
        }
        (input as any).autoNumeric = settings;
        if (input.value !== '' || input.textContent !== '') {
            AutoNumeric.setValue(input, strip);
        }
        return;
    }

    /** returns a formatted strings for "input:text" fields Uses jQuery's .val() method*/
    static setValue(input: HTMLInputElement, valueIn: number | string) {
        var settings = AutoNumeric.getSettings(input),
            value = valueIn.toString(),
            testValue = valueIn.toString();
        if (typeof settings !== 'object') {
            throw new Error("You must initialize autoNumeric('init', {options}) prior to calling the 'set' method");
        }
        /** routine to handle page re-load from back button */
        if (testValue !== input.getAttribute('value') && input.tagName === 'INPUT' && settings.runOnce === false) {
            value = (settings.nBracket !== null) ? negativeBracket(input.value, settings.nBracket, 'pageLoad') : value;
            value = autoStrip(value, settings);
        }
        /** allows locale decimal separator to be a comma */
        if ((testValue === input.getAttribute('value') || testValue === input.textContent) && settings.runOnce === false) {
            value = value.replace(',', '.');
        }
        /** returns a empty string if the value being 'set' contains non-numeric characters and or more than decimal point (full stop) and will not be formatted */
        if (isNaN(value as any)) {
            return '';
        }
        value = checkValue(value, settings);
        settings.oEvent = 'set';
        value.toString();
        if (value !== '') {
            value = autoRound(value, settings);
        }
        value = presentNumber(value, settings.aDec, settings.aNeg);
        value = autoGroup(value, settings);
        if (input.matches('input[type=text], input[type=hidden], input[type=tel], input:not([type])')) { /**added hidden type */
            input.value = value;
        }
        if (!settings.tagList?.includes(input.tagName)) {
            return input.textContent = value;
        }
        throw new Error("The <" + input.tagName + "> is not supported by autoNumeric()");
    }

    /** method to get the unformatted value from a specific input field, returns a numeric value */
    static getValue(input: HTMLInputElement): string {
        var settings = AutoNumeric.getSettings(input);
        if (typeof settings !== 'object') {
            throw new Error("You must initialize autoNumeric('init', {options}) prior to calling the 'get' method");
        }
        settings.oEvent = 'get';
        var getValue = '';
        /** determine the element type then use .eq(0) selector to grab the value of the first element in selector */
        if (input.matches('input[type=text], input[type=hidden], input[type=tel], input:not([type])')) { /**added hidden type */
            getValue = input.value;
        } else if (settings.tagList?.includes(input.tagName)) {
            getValue = input.textContent;
        } else {
            throw new Error("The <" + input.tagName + "> is not supported by autoNumeric()");
        }
        if ((getValue === '' && settings.wEmpty === 'empty') || (getValue === settings.aSign && (settings.wEmpty === 'sign' || settings.wEmpty === 'empty'))) {
            return '';
        }
        if (settings.nBracket !== null && getValue !== '') {
            getValue = negativeBracket(getValue, settings.nBracket, settings.oEvent);
        }
        if (settings.runOnce || settings.aForm === false) {
            getValue = autoStrip(getValue, settings);
        }
        getValue = fixNumber(getValue, settings.aDec, settings.aNeg);
        if (+getValue === 0 && settings.lZero !== 'keep') {
            getValue = '0';
        }
        if (settings.lZero === 'keep') {
            return getValue;
        }
        getValue = checkValue(getValue, settings);
        return getValue; /** returned Numeric String */
    }

    /** returns the settings object for those who need to look under the hood */
    static getSettings(input: HTMLInputElement): AutoNumericOptions {
        return (input as any)?.autoNumeric;
    }

    static hasInstance(input: HTMLInputElement) {
        return typeof (input as any)?.autoNumeric === "object";
    }
}


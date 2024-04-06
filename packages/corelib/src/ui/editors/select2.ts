/** 
 * Adapted from 3.5.x version of Select2 (https://github.com/select2/select2), removing jQuery dependency
 */

import { Fluent, localText, serviceCall, stringFormat } from "../../base";

export type Select2Element = HTMLInputElement | HTMLSelectElement;
export type Select2FormatResult = string | Element | DocumentFragment;

export interface Select2QueryOptions {
    element?: Select2Element;
    term?: string;
    page?: number;
    context?: any;
    callback?: (p1: Select2Result) => void;
    matcher?: (p1: any, p2: any, p3?: any) => boolean;
}

export interface Select2Item {
    id?: string;
    text?: string;
    source?: any;
    children?: Select2Item[];
    disabled?: boolean;
    locked?: boolean;
}

export interface Select2Result {
    hasError?: boolean;
    errorInfo?: any;
    results: Select2Item[];
    more?: boolean;
    context?: any;
}

export interface Select2AjaxOptions extends RequestInit {
    headers?: Record<string, string>;
    url?: string | ((term: string, page: number, context: any) => string);
    quietMillis?: number;
    data?: (p1: string, p2: number, p3: any) => any;
    results?: (p1: any, p2: number, p3: any) => any;
    params?: (() => any) | any;
    onError?(response: any, info?: any): void | boolean;
    onSuccess?(response: any): void;
}

export interface Select2Options {
    element?: Select2Element;
    width?: any;
    minimumInputLength?: number;
    maximumInputLength?: number;
    minimumResultsForSearch?: number;
    maximumSelectionSize?: any;
    placeholder?: string;
    placeholderOption?: any;
    separator?: string;
    allowClear?: boolean;
    multiple?: boolean;
    closeOnSelect?: boolean;
    openOnEnter?: boolean;
    id?: (p1: any) => string;
    matcher?: (p1: string, p2: string, p3: HTMLElement) => boolean;
    sortResults?: (p1: any, p2: HTMLElement, p3: any) => any;
    formatAjaxError?: (p1: any, p2: any) => Select2FormatResult;
    formatMatches?: (matches: number) => Select2FormatResult;
    formatSelection?: (p1: any, p2: HTMLElement, p3: (p1: string) => string) => Select2FormatResult;
    formatResult?: (p1: any, p2: HTMLElement, p3: any, p4: (p1: string) => string) => Select2FormatResult;
    formatResultCssClass?: (p1: any) => string;
    formatSelectionCssClass?: (item: Select2Item, container: HTMLElement) => string;
    formatNoMatches?: (input: string) => Select2FormatResult;
    formatLoadMore?: (pageNumber: number) => Select2FormatResult;
    formatSearching?: () => Select2FormatResult;
    formatInputTooLong?: (input: string, max: number) => Select2FormatResult;
    formatInputTooShort?: (input: string, min: number) => Select2FormatResult;
    formatSelectionTooBig?: (p1: number) => Select2FormatResult;
    createSearchChoice?: (p1: string) => Select2Item;
    createSearchChoicePosition?: string | ((list: Select2Item[], item: Select2Item) => void);
    initSelection?: (p1: HTMLElement, p2: (p1: any) => void) => void;
    tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
    tokenSeparators?: any;
    query?: (p1: Select2QueryOptions) => void;
    ajax?: Select2AjaxOptions;
    data?: any;
    tags?: ((string | Select2Item)[]) | (() => (string | Select2Item)[]);
    containerCss?: any;
    containerCssClass?: any;
    dropdownCss?: any;
    dropdownCssClass?: any;
    dropdownAutoWidth?: boolean;
    dropdownParent?: (input: HTMLElement) => HTMLElement;
    adaptContainerCssClass?: (p1: string) => string;
    adaptDropdownCssClass?: (p1: string) => string;
    escapeMarkup?: (p1: string) => string;
    searchInputPlaceholder?: string;
    selectOnBlur?: boolean;
    blurOnChange?: boolean;
    loadMorePadding?: number;
    nextSearchTerm?: (p1: any, p2: string) => string;
    populateResults?: (container: HTMLElement, results: Select2Item[], query: Select2QueryOptions) => void
    shouldFocusInput?: (p1: any) => boolean;
}

interface Select2Data {
    text?: string;
}

var lastMousePosition = { x: 0, y: 0 };

const KEY = {
    TAB: 9,
    ENTER: 13,
    ESC: 27,
    SPACE: 32,
    LEFT: 37,
    UP: 38,
    RIGHT: 39,
    DOWN: 40,
    SHIFT: 16,
    CTRL: 17,
    ALT: 18,
    PAGE_UP: 33,
    PAGE_DOWN: 34,
    HOME: 36,
    END: 35,
    BACKSPACE: 8,
    DELETE: 46,
    isArrow: function (k: any) {
        k = k.which ? k.which : k;
        switch (k) {
            case KEY.LEFT:
            case KEY.RIGHT:
            case KEY.UP:
            case KEY.DOWN:
                return true;
        }
        return false;
    },
    isControl: function (e: any) {
        var k = e.which;
        switch (k) {
            case KEY.SHIFT:
            case KEY.CTRL:
            case KEY.ALT:
                return true;
        }

        if (e.metaKey) return true;

        return false;
    },
    isFunctionKey: function (k: any) {
        k = k.which ? k.which : k;
        return k >= 112 && k <= 123;
    }
}

//MEASURE_SCROLLBAR_TEMPLATE = "<div class='select2-measure-scrollbar'></div>",

const DIACRITICS: Record<string, string> = { "\u24B6": "A", "\uFF21": "A", "\u00C0": "A", "\u00C1": "A", "\u00C2": "A", "\u1EA6": "A", "\u1EA4": "A", "\u1EAA": "A", "\u1EA8": "A", "\u00C3": "A", "\u0100": "A", "\u0102": "A", "\u1EB0": "A", "\u1EAE": "A", "\u1EB4": "A", "\u1EB2": "A", "\u0226": "A", "\u01E0": "A", "\u00C4": "A", "\u01DE": "A", "\u1EA2": "A", "\u00C5": "A", "\u01FA": "A", "\u01CD": "A", "\u0200": "A", "\u0202": "A", "\u1EA0": "A", "\u1EAC": "A", "\u1EB6": "A", "\u1E00": "A", "\u0104": "A", "\u023A": "A", "\u2C6F": "A", "\uA732": "AA", "\u00C6": "AE", "\u01FC": "AE", "\u01E2": "AE", "\uA734": "AO", "\uA736": "AU", "\uA738": "AV", "\uA73A": "AV", "\uA73C": "AY", "\u24B7": "B", "\uFF22": "B", "\u1E02": "B", "\u1E04": "B", "\u1E06": "B", "\u0243": "B", "\u0182": "B", "\u0181": "B", "\u24B8": "C", "\uFF23": "C", "\u0106": "C", "\u0108": "C", "\u010A": "C", "\u010C": "C", "\u00C7": "C", "\u1E08": "C", "\u0187": "C", "\u023B": "C", "\uA73E": "C", "\u24B9": "D", "\uFF24": "D", "\u1E0A": "D", "\u010E": "D", "\u1E0C": "D", "\u1E10": "D", "\u1E12": "D", "\u1E0E": "D", "\u0110": "D", "\u018B": "D", "\u018A": "D", "\u0189": "D", "\uA779": "D", "\u01F1": "DZ", "\u01C4": "DZ", "\u01F2": "Dz", "\u01C5": "Dz", "\u24BA": "E", "\uFF25": "E", "\u00C8": "E", "\u00C9": "E", "\u00CA": "E", "\u1EC0": "E", "\u1EBE": "E", "\u1EC4": "E", "\u1EC2": "E", "\u1EBC": "E", "\u0112": "E", "\u1E14": "E", "\u1E16": "E", "\u0114": "E", "\u0116": "E", "\u00CB": "E", "\u1EBA": "E", "\u011A": "E", "\u0204": "E", "\u0206": "E", "\u1EB8": "E", "\u1EC6": "E", "\u0228": "E", "\u1E1C": "E", "\u0118": "E", "\u1E18": "E", "\u1E1A": "E", "\u0190": "E", "\u018E": "E", "\u24BB": "F", "\uFF26": "F", "\u1E1E": "F", "\u0191": "F", "\uA77B": "F", "\u24BC": "G", "\uFF27": "G", "\u01F4": "G", "\u011C": "G", "\u1E20": "G", "\u011E": "G", "\u0120": "G", "\u01E6": "G", "\u0122": "G", "\u01E4": "G", "\u0193": "G", "\uA7A0": "G", "\uA77D": "G", "\uA77E": "G", "\u24BD": "H", "\uFF28": "H", "\u0124": "H", "\u1E22": "H", "\u1E26": "H", "\u021E": "H", "\u1E24": "H", "\u1E28": "H", "\u1E2A": "H", "\u0126": "H", "\u2C67": "H", "\u2C75": "H", "\uA78D": "H", "\u24BE": "I", "\uFF29": "I", "\u00CC": "I", "\u00CD": "I", "\u00CE": "I", "\u0128": "I", "\u012A": "I", "\u012C": "I", "\u0130": "I", "\u00CF": "I", "\u1E2E": "I", "\u1EC8": "I", "\u01CF": "I", "\u0208": "I", "\u020A": "I", "\u1ECA": "I", "\u012E": "I", "\u1E2C": "I", "\u0197": "I", "\u24BF": "J", "\uFF2A": "J", "\u0134": "J", "\u0248": "J", "\u24C0": "K", "\uFF2B": "K", "\u1E30": "K", "\u01E8": "K", "\u1E32": "K", "\u0136": "K", "\u1E34": "K", "\u0198": "K", "\u2C69": "K", "\uA740": "K", "\uA742": "K", "\uA744": "K", "\uA7A2": "K", "\u24C1": "L", "\uFF2C": "L", "\u013F": "L", "\u0139": "L", "\u013D": "L", "\u1E36": "L", "\u1E38": "L", "\u013B": "L", "\u1E3C": "L", "\u1E3A": "L", "\u0141": "L", "\u023D": "L", "\u2C62": "L", "\u2C60": "L", "\uA748": "L", "\uA746": "L", "\uA780": "L", "\u01C7": "LJ", "\u01C8": "Lj", "\u24C2": "M", "\uFF2D": "M", "\u1E3E": "M", "\u1E40": "M", "\u1E42": "M", "\u2C6E": "M", "\u019C": "M", "\u24C3": "N", "\uFF2E": "N", "\u01F8": "N", "\u0143": "N", "\u00D1": "N", "\u1E44": "N", "\u0147": "N", "\u1E46": "N", "\u0145": "N", "\u1E4A": "N", "\u1E48": "N", "\u0220": "N", "\u019D": "N", "\uA790": "N", "\uA7A4": "N", "\u01CA": "NJ", "\u01CB": "Nj", "\u24C4": "O", "\uFF2F": "O", "\u00D2": "O", "\u00D3": "O", "\u00D4": "O", "\u1ED2": "O", "\u1ED0": "O", "\u1ED6": "O", "\u1ED4": "O", "\u00D5": "O", "\u1E4C": "O", "\u022C": "O", "\u1E4E": "O", "\u014C": "O", "\u1E50": "O", "\u1E52": "O", "\u014E": "O", "\u022E": "O", "\u0230": "O", "\u00D6": "O", "\u022A": "O", "\u1ECE": "O", "\u0150": "O", "\u01D1": "O", "\u020C": "O", "\u020E": "O", "\u01A0": "O", "\u1EDC": "O", "\u1EDA": "O", "\u1EE0": "O", "\u1EDE": "O", "\u1EE2": "O", "\u1ECC": "O", "\u1ED8": "O", "\u01EA": "O", "\u01EC": "O", "\u00D8": "O", "\u01FE": "O", "\u0186": "O", "\u019F": "O", "\uA74A": "O", "\uA74C": "O", "\u01A2": "OI", "\uA74E": "OO", "\u0222": "OU", "\u24C5": "P", "\uFF30": "P", "\u1E54": "P", "\u1E56": "P", "\u01A4": "P", "\u2C63": "P", "\uA750": "P", "\uA752": "P", "\uA754": "P", "\u24C6": "Q", "\uFF31": "Q", "\uA756": "Q", "\uA758": "Q", "\u024A": "Q", "\u24C7": "R", "\uFF32": "R", "\u0154": "R", "\u1E58": "R", "\u0158": "R", "\u0210": "R", "\u0212": "R", "\u1E5A": "R", "\u1E5C": "R", "\u0156": "R", "\u1E5E": "R", "\u024C": "R", "\u2C64": "R", "\uA75A": "R", "\uA7A6": "R", "\uA782": "R", "\u24C8": "S", "\uFF33": "S", "\u1E9E": "S", "\u015A": "S", "\u1E64": "S", "\u015C": "S", "\u1E60": "S", "\u0160": "S", "\u1E66": "S", "\u1E62": "S", "\u1E68": "S", "\u0218": "S", "\u015E": "S", "\u2C7E": "S", "\uA7A8": "S", "\uA784": "S", "\u24C9": "T", "\uFF34": "T", "\u1E6A": "T", "\u0164": "T", "\u1E6C": "T", "\u021A": "T", "\u0162": "T", "\u1E70": "T", "\u1E6E": "T", "\u0166": "T", "\u01AC": "T", "\u01AE": "T", "\u023E": "T", "\uA786": "T", "\uA728": "TZ", "\u24CA": "U", "\uFF35": "U", "\u00D9": "U", "\u00DA": "U", "\u00DB": "U", "\u0168": "U", "\u1E78": "U", "\u016A": "U", "\u1E7A": "U", "\u016C": "U", "\u00DC": "U", "\u01DB": "U", "\u01D7": "U", "\u01D5": "U", "\u01D9": "U", "\u1EE6": "U", "\u016E": "U", "\u0170": "U", "\u01D3": "U", "\u0214": "U", "\u0216": "U", "\u01AF": "U", "\u1EEA": "U", "\u1EE8": "U", "\u1EEE": "U", "\u1EEC": "U", "\u1EF0": "U", "\u1EE4": "U", "\u1E72": "U", "\u0172": "U", "\u1E76": "U", "\u1E74": "U", "\u0244": "U", "\u24CB": "V", "\uFF36": "V", "\u1E7C": "V", "\u1E7E": "V", "\u01B2": "V", "\uA75E": "V", "\u0245": "V", "\uA760": "VY", "\u24CC": "W", "\uFF37": "W", "\u1E80": "W", "\u1E82": "W", "\u0174": "W", "\u1E86": "W", "\u1E84": "W", "\u1E88": "W", "\u2C72": "W", "\u24CD": "X", "\uFF38": "X", "\u1E8A": "X", "\u1E8C": "X", "\u24CE": "Y", "\uFF39": "Y", "\u1EF2": "Y", "\u00DD": "Y", "\u0176": "Y", "\u1EF8": "Y", "\u0232": "Y", "\u1E8E": "Y", "\u0178": "Y", "\u1EF6": "Y", "\u1EF4": "Y", "\u01B3": "Y", "\u024E": "Y", "\u1EFE": "Y", "\u24CF": "Z", "\uFF3A": "Z", "\u0179": "Z", "\u1E90": "Z", "\u017B": "Z", "\u017D": "Z", "\u1E92": "Z", "\u1E94": "Z", "\u01B5": "Z", "\u0224": "Z", "\u2C7F": "Z", "\u2C6B": "Z", "\uA762": "Z", "\u24D0": "a", "\uFF41": "a", "\u1E9A": "a", "\u00E0": "a", "\u00E1": "a", "\u00E2": "a", "\u1EA7": "a", "\u1EA5": "a", "\u1EAB": "a", "\u1EA9": "a", "\u00E3": "a", "\u0101": "a", "\u0103": "a", "\u1EB1": "a", "\u1EAF": "a", "\u1EB5": "a", "\u1EB3": "a", "\u0227": "a", "\u01E1": "a", "\u00E4": "a", "\u01DF": "a", "\u1EA3": "a", "\u00E5": "a", "\u01FB": "a", "\u01CE": "a", "\u0201": "a", "\u0203": "a", "\u1EA1": "a", "\u1EAD": "a", "\u1EB7": "a", "\u1E01": "a", "\u0105": "a", "\u2C65": "a", "\u0250": "a", "\uA733": "aa", "\u00E6": "ae", "\u01FD": "ae", "\u01E3": "ae", "\uA735": "ao", "\uA737": "au", "\uA739": "av", "\uA73B": "av", "\uA73D": "ay", "\u24D1": "b", "\uFF42": "b", "\u1E03": "b", "\u1E05": "b", "\u1E07": "b", "\u0180": "b", "\u0183": "b", "\u0253": "b", "\u24D2": "c", "\uFF43": "c", "\u0107": "c", "\u0109": "c", "\u010B": "c", "\u010D": "c", "\u00E7": "c", "\u1E09": "c", "\u0188": "c", "\u023C": "c", "\uA73F": "c", "\u2184": "c", "\u24D3": "d", "\uFF44": "d", "\u1E0B": "d", "\u010F": "d", "\u1E0D": "d", "\u1E11": "d", "\u1E13": "d", "\u1E0F": "d", "\u0111": "d", "\u018C": "d", "\u0256": "d", "\u0257": "d", "\uA77A": "d", "\u01F3": "dz", "\u01C6": "dz", "\u24D4": "e", "\uFF45": "e", "\u00E8": "e", "\u00E9": "e", "\u00EA": "e", "\u1EC1": "e", "\u1EBF": "e", "\u1EC5": "e", "\u1EC3": "e", "\u1EBD": "e", "\u0113": "e", "\u1E15": "e", "\u1E17": "e", "\u0115": "e", "\u0117": "e", "\u00EB": "e", "\u1EBB": "e", "\u011B": "e", "\u0205": "e", "\u0207": "e", "\u1EB9": "e", "\u1EC7": "e", "\u0229": "e", "\u1E1D": "e", "\u0119": "e", "\u1E19": "e", "\u1E1B": "e", "\u0247": "e", "\u025B": "e", "\u01DD": "e", "\u24D5": "f", "\uFF46": "f", "\u1E1F": "f", "\u0192": "f", "\uA77C": "f", "\u24D6": "g", "\uFF47": "g", "\u01F5": "g", "\u011D": "g", "\u1E21": "g", "\u011F": "g", "\u0121": "g", "\u01E7": "g", "\u0123": "g", "\u01E5": "g", "\u0260": "g", "\uA7A1": "g", "\u1D79": "g", "\uA77F": "g", "\u24D7": "h", "\uFF48": "h", "\u0125": "h", "\u1E23": "h", "\u1E27": "h", "\u021F": "h", "\u1E25": "h", "\u1E29": "h", "\u1E2B": "h", "\u1E96": "h", "\u0127": "h", "\u2C68": "h", "\u2C76": "h", "\u0265": "h", "\u0195": "hv", "\u24D8": "i", "\uFF49": "i", "\u00EC": "i", "\u00ED": "i", "\u00EE": "i", "\u0129": "i", "\u012B": "i", "\u012D": "i", "\u00EF": "i", "\u1E2F": "i", "\u1EC9": "i", "\u01D0": "i", "\u0209": "i", "\u020B": "i", "\u1ECB": "i", "\u012F": "i", "\u1E2D": "i", "\u0268": "i", "\u0131": "i", "\u24D9": "j", "\uFF4A": "j", "\u0135": "j", "\u01F0": "j", "\u0249": "j", "\u24DA": "k", "\uFF4B": "k", "\u1E31": "k", "\u01E9": "k", "\u1E33": "k", "\u0137": "k", "\u1E35": "k", "\u0199": "k", "\u2C6A": "k", "\uA741": "k", "\uA743": "k", "\uA745": "k", "\uA7A3": "k", "\u24DB": "l", "\uFF4C": "l", "\u0140": "l", "\u013A": "l", "\u013E": "l", "\u1E37": "l", "\u1E39": "l", "\u013C": "l", "\u1E3D": "l", "\u1E3B": "l", "\u017F": "l", "\u0142": "l", "\u019A": "l", "\u026B": "l", "\u2C61": "l", "\uA749": "l", "\uA781": "l", "\uA747": "l", "\u01C9": "lj", "\u24DC": "m", "\uFF4D": "m", "\u1E3F": "m", "\u1E41": "m", "\u1E43": "m", "\u0271": "m", "\u026F": "m", "\u24DD": "n", "\uFF4E": "n", "\u01F9": "n", "\u0144": "n", "\u00F1": "n", "\u1E45": "n", "\u0148": "n", "\u1E47": "n", "\u0146": "n", "\u1E4B": "n", "\u1E49": "n", "\u019E": "n", "\u0272": "n", "\u0149": "n", "\uA791": "n", "\uA7A5": "n", "\u01CC": "nj", "\u24DE": "o", "\uFF4F": "o", "\u00F2": "o", "\u00F3": "o", "\u00F4": "o", "\u1ED3": "o", "\u1ED1": "o", "\u1ED7": "o", "\u1ED5": "o", "\u00F5": "o", "\u1E4D": "o", "\u022D": "o", "\u1E4F": "o", "\u014D": "o", "\u1E51": "o", "\u1E53": "o", "\u014F": "o", "\u022F": "o", "\u0231": "o", "\u00F6": "o", "\u022B": "o", "\u1ECF": "o", "\u0151": "o", "\u01D2": "o", "\u020D": "o", "\u020F": "o", "\u01A1": "o", "\u1EDD": "o", "\u1EDB": "o", "\u1EE1": "o", "\u1EDF": "o", "\u1EE3": "o", "\u1ECD": "o", "\u1ED9": "o", "\u01EB": "o", "\u01ED": "o", "\u00F8": "o", "\u01FF": "o", "\u0254": "o", "\uA74B": "o", "\uA74D": "o", "\u0275": "o", "\u01A3": "oi", "\u0223": "ou", "\uA74F": "oo", "\u24DF": "p", "\uFF50": "p", "\u1E55": "p", "\u1E57": "p", "\u01A5": "p", "\u1D7D": "p", "\uA751": "p", "\uA753": "p", "\uA755": "p", "\u24E0": "q", "\uFF51": "q", "\u024B": "q", "\uA757": "q", "\uA759": "q", "\u24E1": "r", "\uFF52": "r", "\u0155": "r", "\u1E59": "r", "\u0159": "r", "\u0211": "r", "\u0213": "r", "\u1E5B": "r", "\u1E5D": "r", "\u0157": "r", "\u1E5F": "r", "\u024D": "r", "\u027D": "r", "\uA75B": "r", "\uA7A7": "r", "\uA783": "r", "\u24E2": "s", "\uFF53": "s", "\u00DF": "s", "\u015B": "s", "\u1E65": "s", "\u015D": "s", "\u1E61": "s", "\u0161": "s", "\u1E67": "s", "\u1E63": "s", "\u1E69": "s", "\u0219": "s", "\u015F": "s", "\u023F": "s", "\uA7A9": "s", "\uA785": "s", "\u1E9B": "s", "\u24E3": "t", "\uFF54": "t", "\u1E6B": "t", "\u1E97": "t", "\u0165": "t", "\u1E6D": "t", "\u021B": "t", "\u0163": "t", "\u1E71": "t", "\u1E6F": "t", "\u0167": "t", "\u01AD": "t", "\u0288": "t", "\u2C66": "t", "\uA787": "t", "\uA729": "tz", "\u24E4": "u", "\uFF55": "u", "\u00F9": "u", "\u00FA": "u", "\u00FB": "u", "\u0169": "u", "\u1E79": "u", "\u016B": "u", "\u1E7B": "u", "\u016D": "u", "\u00FC": "u", "\u01DC": "u", "\u01D8": "u", "\u01D6": "u", "\u01DA": "u", "\u1EE7": "u", "\u016F": "u", "\u0171": "u", "\u01D4": "u", "\u0215": "u", "\u0217": "u", "\u01B0": "u", "\u1EEB": "u", "\u1EE9": "u", "\u1EEF": "u", "\u1EED": "u", "\u1EF1": "u", "\u1EE5": "u", "\u1E73": "u", "\u0173": "u", "\u1E77": "u", "\u1E75": "u", "\u0289": "u", "\u24E5": "v", "\uFF56": "v", "\u1E7D": "v", "\u1E7F": "v", "\u028B": "v", "\uA75F": "v", "\u028C": "v", "\uA761": "vy", "\u24E6": "w", "\uFF57": "w", "\u1E81": "w", "\u1E83": "w", "\u0175": "w", "\u1E87": "w", "\u1E85": "w", "\u1E98": "w", "\u1E89": "w", "\u2C73": "w", "\u24E7": "x", "\uFF58": "x", "\u1E8B": "x", "\u1E8D": "x", "\u24E8": "y", "\uFF59": "y", "\u1EF3": "y", "\u00FD": "y", "\u0177": "y", "\u1EF9": "y", "\u0233": "y", "\u1E8F": "y", "\u00FF": "y", "\u1EF7": "y", "\u1E99": "y", "\u1EF5": "y", "\u01B4": "y", "\u024F": "y", "\u1EFF": "y", "\u24E9": "z", "\uFF5A": "z", "\u017A": "z", "\u1E91": "z", "\u017C": "z", "\u017E": "z", "\u1E93": "z", "\u1E95": "z", "\u01B6": "z", "\u0225": "z", "\u0240": "z", "\u2C6C": "z", "\uA763": "z", "\u0386": "\u0391", "\u0388": "\u0395", "\u0389": "\u0397", "\u038A": "\u0399", "\u03AA": "\u0399", "\u038C": "\u039F", "\u038E": "\u03A5", "\u03AB": "\u03A5", "\u038F": "\u03A9", "\u03AC": "\u03B1", "\u03AD": "\u03B5", "\u03AE": "\u03B7", "\u03AF": "\u03B9", "\u03CA": "\u03B9", "\u0390": "\u03B9", "\u03CC": "\u03BF", "\u03CD": "\u03C5", "\u03CB": "\u03C5", "\u03B0": "\u03C5", "\u03C9": "\u03C9", "\u03C2": "\u03C3" };
const nextUid = (function () { var counter = 1; return function () { return counter++; }; }());


function reinsertElement(element: HTMLElement) {
    var placeholder = document.createTextNode('');
    Fluent(placeholder).insertBefore(element);
    Fluent(element).insertBefore(placeholder as any);
    placeholder.remove();
}

function indexOf(value: any, array: any[]) {
    var i = 0, l = array.length;
    for (; i < l; i = i + 1) {
        if (equal(value, array[i])) return i;
    }
    return -1;
}

let scrollBarDimensions: { width: number, height: number } = null;

function getOffset(el: Element) {
    var box = el.getBoundingClientRect();
    var docElem = document.documentElement;
    return {
        top: box.top + window.scrollY - docElem.clientTop,
        left: box.left + window.scrollX - docElem.clientLeft
    };
}

function txt(s: string) { 
    return localText("Controls.SelectEditor." + s); 
}

function fmt(s: string, ...prm: any[]) {
    return stringFormat(localText("Controls.SelectEditor." + s), prm);
}

function measureScrollbar() {
    var $template = document.createElement("div");
    $template.classList.add("select2-measure-scrollbar");
    document.body.appendChild($template);

    var dim = {
        width: $template.offsetWidth - $template.clientWidth,
        height: $template.offsetHeight - $template.clientHeight
    };
    $template.remove();

    return dim;
}

/**
 * Compares equality of a and b
 * @param a
 * @param b
 */
function equal(a: any, b: any): boolean {
    if (a === b) return true;
    if (a === undefined || b === undefined) return false;
    if (a === null || b === null) return false;
    // Check whether 'a' or 'b' is a string (primitive or object).
    // The concatenation of an empty string (+'') converts its argument to a string's primitive.
    if (a.constructor === String) return a + '' === b + ''; // a+'' - in case 'a' is a String object
    if (b.constructor === String) return b + '' === a + ''; // b+'' - in case 'b' is a String object
    return false;
}

/**
 * Splits the string into an array of values, trimming each value. An empty array is returned for nulls or empty
 * strings
 * @param string
 * @param separator
 */
function splitVal(string: string, separator: string) {
    var val, i, l;
    if (string === null || string.length < 1) return [];
    val = string.split(separator);
    for (i = 0, l = val.length; i < l; i = i + 1) val[i] = val[i].trim();
    return val;
}

function parsePx(str: string) {
    var value = parseFloat(str);
    if (isNaN(value))
        return 0;
    return value;
}

function getSideBorderPadding(el: Element) {
    if (!el)
        return 0;

    var style = getComputedStyle(el);
    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    var delta = 0;
    for (var val of p)
        delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
}

function getOuterHeightWithMargins(el: Element) {
    const style = getComputedStyle(el);

    return (
        el.getBoundingClientRect().height +
        parseFloat(style.marginTop) +
        parseFloat(style.marginBottom)
    );
}

function installKeyUpChangeEvent(element: HTMLElement) {
    var key = "keyupChangeValue";
    Fluent.on(element, "keydown", function () {
        if (element.dataset[key] === undefined) {
            element.dataset[key] = (element as any).value;
        }
    });
    Fluent.on(element, "keyup", function () {
        var val = element.dataset[key];
        if (val !== undefined && (element as any).value !== val) {
            delete element.dataset[key];
            Fluent.trigger(element, "keyup-change");
        }
    });
}


/**
 * filters mouse events so an event is fired only if the mouse moved.
 *
 * filters out mouse events that occur when mouse is stationary but
 * the elements under the pointer are scrolled.
 */
function installFilteredMouseMove(element: HTMLElement) {
    Fluent.on(element, "mousemove", function (e) {
        var lastpos = lastMousePosition;
        if (lastpos === undefined || lastpos.x !== e.pageX || lastpos.y !== e.pageY) {
            Fluent.trigger(e.target, "mousemove-filtered", { pageX: e.pageX, pageY: e.pageY });
        }
    });
}

/**
 * Debounces a function. Returns a function that calls the original fn function only if no invocations have been made
 * within the last quietMillis milliseconds.
 *
 * @param quietMillis number of milliseconds to wait before invoking fn
 * @param fn function to be debounced
 * @param ctx object to be used as this reference within fn
 * @return debounced version of fn
 */
function debounce(quietMillis: number, fn: any, ctx?: any) {
    ctx = ctx || undefined;
    var timeout: number;
    return function () {
        var args = arguments;
        window.clearTimeout(timeout);
        timeout = window.setTimeout(function () {
            fn.apply(ctx, args);
        }, quietMillis);
    } as any;
}

function installDebouncedScroll(threshold: number, element: Element) {
    var notify = debounce(threshold, function (args?: any) { Fluent.trigger(element, "scroll-debounced", args); });
    Fluent.on(element, "scroll", function (e) {
        if (e.target === element) notify();
    });
}

function focus($el: HTMLElement) {
    if ($el === document.activeElement) return;

    /* set the focus in a 0 timeout - that way the focus is set after the processing
        of the current event has finished - which seems like the only reliable way
        to set focus */
    window.setTimeout(function () {
        var el = $el, pos = ($el as any).value?.length || 0, range;

        $el.focus();

        /* make sure el received focus so we do not error out when trying to manipulate the caret.
            sometimes modals or others listeners may steal it after its set */
        var isVisible = (el.offsetWidth > 0 || el.offsetHeight > 0);
        if (isVisible && el === document.activeElement) {

            /* after the focus is set move the caret to the end, necessary when we val()
                just before setting focus */
            if ((el as any).setSelectionRange) {
                (el as any).setSelectionRange(pos, pos);
            }
            else if ((el as any).createTextRange) {
                range = (el as any).createTextRange();
                range.collapse(false);
                range.select();
            }
        }
    }, 0);
}

function getCursorInfo(el: HTMLElement) {
    var offset = 0;
    var length = 0;
    if ('selectionStart' in el) {
        offset = (el as any).selectionStart;
        length = (el as any).selectionEnd - offset;
    } else if ('selection' in document) {
        el.focus();
        var sel = (document.selection as any).createRange();
        length = (document.selection as any).createRange().text.length;
        sel.moveStart('character', -(el as any).value.length);
        offset = sel.text.length - length;
    }
    return { offset: offset, length: length };
}

function killEvent(event: Event) {
    event.preventDefault();
    event.stopPropagation();
}
function killEventImmediately(event: Event) {
    event.preventDefault();
    event.stopImmediatePropagation();
}

let sizer: HTMLDivElement;

function measureTextWidth(e: Element) {
    if (!sizer) {
        var style = getComputedStyle(e, null);
        sizer = document.createElement("div");
        Object.assign(sizer.style, {
            position: "absolute",
            left: "-10000px",
            top: "-10000px",
            display: "none",
            fontSize: style.fontSize,
            fontFamily: style.fontFamily,
            fontStyle: style.fontStyle,
            fontWeight: style.fontWeight,
            letterSpacing: style.letterSpacing,
            textTransform: style.textTransform,
            whiteSpace: "nowrap"
        });
        sizer.setAttribute("class", "select2-sizer");
        document.body.appendChild(sizer);
    }
    sizer.textContent = (e as any).value;
    return sizer.offsetWidth;
}

function syncCssClasses(dest: Element, src: Element, adapter: (kls: string) => string) {
    var classes: string, replacements: string[] = [], adapted;

    classes = dest.getAttribute("class")?.trim();

    if (classes) {
        classes = '' + classes; // for IE which returns object

        classes.split(/\s+/).forEach(function (kls) {
            if (kls.indexOf("select2-") === 0) {
                replacements.push(kls);
            }
        });
    }

    classes = src.getAttribute("class")?.trim();

    if (classes) {
        classes = '' + classes; // for IE which returns object

        classes.split(/\s+/).forEach(function (kls) {
            if (kls.indexOf("select2-") !== 0) {
                adapted = adapter(kls);

                if (adapted) {
                    replacements.push(adapted);
                }
            }
        });
    }

    dest.setAttribute("class", replacements.join(" "));
}


function markMatch(text: string, term: string, markup: string[], escapeMarkup: (s: string) => string) {
    var match = Select2.stripDiacritics(text.toUpperCase()).indexOf(Select2.stripDiacritics(term.toUpperCase())),
        tl = term.length;

    if (match < 0) {
        markup.push(escapeMarkup(text));
        return;
    }

    markup.push(escapeMarkup(text.substring(0, match)));
    markup.push("<span class='select2-match'>");
    markup.push(escapeMarkup(text.substring(match, match + tl)));
    markup.push("</span>");
    markup.push(escapeMarkup(text.substring(match + tl, text.length)));
}

function defaultEscapeMarkup(markup: string) {
    var replace_map: Record<string, string> = {
        '\\': '&#92;',
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#39;',
        "/": '&#47;'
    };

    return String(markup).replace(/[&<>"'\/\\]/g, function (match) {
        return replace_map[match];
    });
}

export class Select2 {

    private el: Select2Element;
    
    constructor(opts?: Select2Options)
    constructor(opts?: Select2Options, create: boolean = true) {

        this.el = opts.element;
        if (!create) {
            return;
        }

        opts = Object.assign({}, opts);

        var multiple: boolean;
        if (opts.element.tagName.toLowerCase() === "select") {
            multiple = opts.element.multiple;
        } else {
            multiple = opts.multiple || false;
            if ("tags" in opts) { opts.multiple = multiple = true; }
        }

        var select2 = multiple ? new MultiSelect2() : new SingleSelect2();
        select2.init(opts);
    }

    private get instance(): AbstractSelect2 {
        if (!this.el)
            return null;
        return (this.el as any).select2;
    }

    close(): void {
        this.instance?.close();
    }

    get container(): HTMLElement {
        return this.instance?.container;
    }

    get dropdown(): HTMLElement {
        return this.instance?.dropdown;
    }

    destroy(): void {
        this.instance?.destroy();
    }

    get data(): (Select2Item | Select2Item[]) {
        return this.instance?.data() as (Select2Item | Select2Item[]);
    }

    set data(value: Select2Item | Select2Item[]) {
        this.instance?.data(value);
    }

    disable(): void {
        this.instance?.disable();
    }

    enable(enabled?: boolean): void {
        this.instance?.enable(enabled);
    }

    focus(): void {
        this.instance?.focus();
    }

    get isFocused(): boolean {
        return this.instance?.isFocused();
    }

    get isMultiple(): boolean {
        return this.instance instanceof MultiSelect2;
    }

    get opened(): boolean {
        return this.instance?.opened();
    }

    open(): boolean {
        return this.instance?.open();
    }

    positionDropdown() {
        this.instance?.positionDropdown();
    }

    readonly(value?: boolean): void {
        this.instance?.readonly(value);
    }

    get search(): HTMLInputElement {
        return this.instance?.search;
    }
   
    get val(): (string | string[]) {
        return this.instance?.val();
    }

    set val(value: string[]) {
        this.instance?.val(value);
    }

    static getInstance(el: Select2Element): Select2 {
        if (!el || !(el as any).select2)
            return null;
        return new (Select2 as any)({ element: el }, false);
    }

    static readonly ajaxDefaults: Select2AjaxOptions = {
        params: {
            method: "GET",
            cache: false,
            dataType: "json"
        }
    }

    // plugin defaults, accessible to users
    static readonly defaults: Select2Options = {
        width: "copy",
        loadMorePadding: 0,
        closeOnSelect: true,
        openOnEnter: true,
        containerCss: {},
        dropdownCss: {},
        containerCssClass: "",
        dropdownCssClass: "",
        dropdownParent: (element) => {
            return element?.closest(".modal") ?? document.body;
        },
        formatAjaxError: () => txt("AjaxError"),
        formatInputTooLong: (input: string, max: number) => fmt("InputTooLong", input.length - max, max, input.length),
        formatInputTooShort: (input: string, min: number) => fmt("InputTooShort", min - input.length, min, input.length),
        formatLoadMore: (pageNumber: number) => fmt("LoadMore", pageNumber),
        formatMatches: (matches: number) => matches === 1 ? txt("SingleMatch") : fmt("MultipleMatches", matches),
        formatNoMatches: () => txt("NoMatches"),
        formatResult: function (result, _, query, escapeMarkup) {
            var markup: string[] = [];
            markMatch(result.text, query.term, markup, escapeMarkup);
            return markup.join("");
        },
        formatResultCssClass: function (data) { return data.css; },
        formatSearching: () => txt("Searching"),
        formatSelection: (data, _, escapeMarkup) => data ? escapeMarkup(data.text) : undefined,
        formatSelectionCssClass: function () { return undefined; },
        formatSelectionTooBig: (limit: number) => fmt("SelectionTooBig", limit),
        sortResults: results => results,
        minimumResultsForSearch: 0,
        minimumInputLength: 0,
        maximumInputLength: null,
        maximumSelectionSize: 0,
        id: function (e) { return e == undefined ? null : e.id; },
        matcher: (term, text) => Select2.stripDiacritics('' + text).toUpperCase().indexOf(Select2.stripDiacritics('' + term).toUpperCase()) >= 0,
        separator: ",",
        tokenSeparators: [],
        tokenizer: defaultTokenizer,
        escapeMarkup: defaultEscapeMarkup,
        blurOnChange: false,
        selectOnBlur: false,
        adaptContainerCssClass: function (c) { return c; },
        adaptDropdownCssClass: function (c) { return null; },
        nextSearchTerm: function (selectedObject, currentSearchTerm) { return undefined; },
        searchInputPlaceholder: '',
        createSearchChoicePosition: 'top',
        shouldFocusInput: function (instance) {
            // Attempt to detect touch devices
            var supportsTouchEvents = (('ontouchstart' in window) ||
                ('msMaxTouchPoints' in navigator));

            // Only devices which support touch events should be special cased
            if (!supportsTouchEvents) {
                return true;
            }

            // Never focus the input if search is disabled
            if (instance.opts.minimumResultsForSearch < 0) {
                return false;
            }

            return true;
        }
    };

    static stripDiacritics(str: string) {
        // Used 'uni range + named function' from http://jsperf.com/diacritics/18
        function match(a: string) {
            return DIACRITICS[a] || a;
        }
    
        return str.replace(/[^\u0000-\u007E]/g, match);
    }
}

/**
 * Produces an ajax-based query function
 *
 * @param options object containing configuration parameters
 * @param options.params parameter map for the transport ajax call, can contain such options as cache, jsonpCallback, etc. see $.ajax
 * @param options.transport function that will be used to execute the ajax request. must be compatible with parameters supported by $.ajax
 * @param options.url url for the data
 * @param options.data a function(searchTerm, pageNumber, context) that should return an object containing query string parameters for the above url.
 * @param options.dataType request data type: ajax, jsonp, other datatypes supported by jQuery's $.ajax function or the transport function if specified
 * @param options.quietMillis (optional) milliseconds to wait before making the ajaxRequest, helps debounce the ajax function if invoked too often
 * @param options.results a function(remoteData, pageNumber, query) that converts data returned form the remote request to the format expected by Select2.
 *      The expected format is an object containing the following keys:
 *      results array of objects that will be used as choices
 *      more (optional) boolean indicating whether there are more results available
 *      Example: {results:[{id:1, text:'Red'},{id:2, text:'Blue'}], more:true}
 */
function ajax(options: Select2AjaxOptions) {
    var timeout: number, // current scheduled but not yet executed request
        quietMillis = options.quietMillis || 100,
        ajaxUrl = options.url,
        self = this;

    return function (query: Select2QueryOptions) {
        window.clearTimeout(timeout);
        timeout = window.setTimeout(function () {
            var data = options.data, // ajax data function
                url = ajaxUrl, // ajax url string or function
                // deprecated - to be removed in 4.0  - use params instead
                deprecated = {
                    cache: options.cache || false,
                },
                params = Object.assign({}, Select2.ajaxDefaults.params, deprecated);

            data = data ? data.call(self, query.term, query.page, query.context) : null;
            url = (typeof url === 'function') ? url.call(self, query.term, query.page, query.context) : url;

            if (options.params) {
                if (typeof options.params === "function") {
                    Object.assign(params, options.params.call(self));
                } else {
                    Object.assign(params, options.params);
                }
            }

            Object.assign(params, {
                url: url,
                data: data,
                onSuccess: function (response: any) {
                    // TODO - replace query.page with query so users have access to term, page, etc.
                    // added query as third paramter to keep backwards compatibility
                    var results = options.results(response, query.page, query);
                    query.callback(results);
                },
                onError: function (response: any, info: any) {
                    var results: Select2Result = {
                        errorInfo: info,
                        hasError: true,
                        results: response
                    };

                    query.callback(results);
                }
            });
            serviceCall(params);
        }, quietMillis);
    };
}

/**
 * Produces a query function that works with a local array
 *
 * @param options object containing configuration parameters. The options parameter can either be an array or an
 * object.
 *
 * If the array form is used it is assumed that it contains objects with 'id' and 'text' keys.
 *
 * If the object form is used it is assumed that it contains 'data' and 'text' keys. The 'data' key should contain
 * an array of objects that will be used as choices. These objects must contain at least an 'id' key. The 'text'
 * key can either be a String in which case it is expected that each element in the 'data' array has a key with the
 * value of 'text' which will be used to match choices. Alternatively, text can be a function(item) that can extract
 * the text.
 */
function local(options: any) {
    var data: any = options, // data elements
        dataText: string,
        tmp: any,
        text = function (item: Select2Item) { return "" + item.text; }; // function used to retrieve the text portion of a data item that is matched against the search

    if (Array.isArray(data)) {
        tmp = data;
        data = { results: tmp };
    }

    if (typeof data !== "function") {
        tmp = data;
        data = function () { return tmp; };
    }

    var dataItem = data();
    if (dataItem.text) {
        text = dataItem.text;
        // if text is not a function we assume it to be a key name
        if (typeof text !== "function") {
            dataText = dataItem.text; // we need to store this in a separate variable because in the next step data gets reset and data.text is no longer available
            text = function (item: any) { return item[dataText]; };
        }
    }

    return function (query: Select2QueryOptions) {
        var t = query.term, filtered = { results: <any[]>[] }, process: any;
        if (t === "") {
            query.callback(data());
            return;
        }

        process = function (datum: any, collection: any[]) {
            var group: any, attr: string;
            datum = datum[0];
            if (datum.children) {
                group = {};
                for (attr in datum) {
                    if (datum.hasOwnProperty(attr)) group[attr] = datum[attr];
                }
                group.children = [];
                datum.children.forEach(function (childDatum: any) { process(childDatum, group.children); });
                if (group.children.length || query.matcher(t, text(group), datum)) {
                    collection.push(group);
                }
            } else {
                if (query.matcher(t, text(datum), datum)) {
                    collection.push(datum);
                }
            }
        };

        data().results.forEach(function(datum: any) { process(datum, filtered.results); });
        query.callback(filtered);
    };
}

// TODO javadoc
function tags(data: any) {
    var isFunc = typeof data === "function";
    return function (query: Select2QueryOptions) {
        var t = query.term, filtered = { results: <any[]>[] };
        var result = isFunc ? data(query) : data;
        if (Array.isArray(result)) {
            result.forEach(function (item: Select2Item) {
                var isObject = item.text !== undefined,
                    text = isObject ? item.text : item;
                if (t === "" || query.matcher(t, text)) {
                    filtered.results.push(isObject ? item : { id: item, text: item });
                }
            });
            query.callback(filtered);
        }
    };
}

/**
 * Checks if the formatter function should be used.
 *
 * Throws an error if it is not a function. Returns true if it should be used,
 * false if no formatting should be performed.
 *
 * @param formatter
 */
function checkFormatter(formatter: any, formatterName: string) {
    if (typeof formatter === "function") return true;
    if (!formatter) return false;
    if (typeof (formatter) === 'string') return true;
    throw new Error(formatterName + " must be a string, function, or falsy value");
}

/**
 * Returns a given value
 * If given a function, returns its output
 *
 * @param val string|function
 * @param context value of "this" to be passed to function
 * @returns {*}
 */
function evaluate(val: any, context: any, ..._: any[]): any {
    if (typeof val === "function") {
        var args = Array.prototype.slice.call(arguments, 2);
        return val.apply(context, args);
    }
    return val;
}

function countResults(results: any[]) {
    var count = 0;
    results.forEach(function (item) {
        if (item.children) {
            count += countResults(item.children);
        } else {
            count++;
        }
    });
    return count;
}

/**
 * Default tokenizer. This function uses breaks the input on substring match of any string from the
 * opts.tokenSeparators array and uses opts.createSearchChoice to create the choice object. Both of those
 * two options have to be defined in order for the tokenizer to work.
 *
 * @param input text user has typed so far or pasted into the search field
 * @param selection currently selected choices
 * @param selectCallback function(choice) callback tho add the choice to selection
 * @param opts select2's opts
 * @return undefined/null to leave the current input unchanged, or a string to change the input to the returned value
 */
function defaultTokenizer(input: string, selection: string | any[], selectCallback: (arg0: any) => void, opts: { createSearchChoice: { call: (arg0: any, arg1: any, arg2: any) => any; }; tokenSeparators: string | any[]; id: (arg0: any) => null; }) {
    var original = input, // store the original so we can compare and know if we need to tell the search to update its text
        dupe = false, // check for whether a token we extracted represents a duplicate selected choice
        token, // token
        index, // position at which the separator was found
        i, l, // looping variables
        separator; // the matched separator

    if (!opts.createSearchChoice || !opts.tokenSeparators || opts.tokenSeparators.length < 1) return undefined;

    while (true) {
        index = -1;

        for (i = 0, l = opts.tokenSeparators.length; i < l; i++) {
            separator = opts.tokenSeparators[i];
            index = input.indexOf(separator);
            if (index >= 0) break;
        }

        if (index < 0) break; // did not find any token separator in the input string, bail

        token = input.substring(0, index);
        input = input.substring(index + separator.length);

        if (token.length > 0) {
            token = opts.createSearchChoice.call(this, token, selection);
            if (token !== undefined && token !== null && opts.id(token) !== undefined && opts.id(token) !== null) {
                dupe = false;
                for (i = 0, l = selection.length; i < l; i++) {
                    if (equal(opts.id(token), opts.id(selection[i]))) {
                        dupe = true; break;
                    }
                }

                if (!dupe) selectCallback(token);
            }
        }
    }

    if (original !== input) return input;
}

function cleanupJQueryElements() {
    var self = this;

    Array.from(arguments).forEach(function (element) {
        Fluent.remove(self[element]);
        self[element] = null;
    });
}

abstract class AbstractSelect2 {

    private _enabled: boolean;
    private _readonly: boolean;
    private _sync: () => void;
    private _touchEvent: boolean;
    private _touchMoved: boolean;

    protected autofocus: boolean;
    container: HTMLElement;
    protected containerId: string;
    protected containerEventName: string;
    protected context: any;
    dropdown: HTMLElement;
    protected elementTabIndex: string;
    protected enabledInterface: boolean;
    protected id: (item: any) => string;
    protected nextSearchTerm: string;
    protected opts: Select2Options;
    protected propertyObserver: MutationObserver;
    protected queryCount: number;
    protected results: HTMLElement;
    protected resultsPage: number;
    search: HTMLInputElement;
    protected selection: HTMLElement;
    protected showSearchInput: boolean;

    protected abstract createContainer(): HTMLElement;
    abstract data(value?: Select2Item | Select2Item[], triggerChange?: boolean): Select2Item | Select2Item[] | void;
    abstract focus(): void;
    abstract isFocused(): boolean;
    protected abstract initContainer(): void;
    protected abstract initSelection(): void;
    protected abstract onSelect(item: Select2Item, options?: { noFocus: boolean }): void;
    protected abstract postprocessResults(data: Select2Result, initial: boolean, noHighlightUpdate?: boolean): void;
    abstract val(value?: string | string[]): string | string[];

    init(opts: Select2Options) {
        var results: HTMLElement, search: HTMLInputElement, resultsSelector = ".select2-results";

        // prepare options
        this.opts = opts = this.prepareOpts(opts);

        this.id = opts.id;

        // destroy if called on an existing component
        if ((opts.element as any).select2 != null) {
            (opts.element as any).select2.destroy?.();
        }

        this.container = this.createContainer();

        this.containerId = "s2id_" + (opts.element.getAttribute("id") || "autogen" + nextUid());
        this.containerEventName = this.containerId
            .replace(/([.])/g, '_')
            .replace(/([;&,\-\.\+\*\~':"\!\^#$%@\[\]\(\)=>\|])/g, '\\$1');
        this.container.setAttribute("id", this.containerId);

        this.container.setAttribute("title", opts.element.getAttribute("title") ?? "");

        syncCssClasses(this.container, this.opts.element, this.opts.adaptContainerCssClass);

        this.container.setAttribute("style", opts.element.getAttribute("style"));
        Object.assign(this.container.style, evaluate(opts.containerCss, this.opts.element));
        Fluent.addClass(this.container, evaluate(opts.containerCssClass, this.opts.element));

        this.elementTabIndex = this.opts.element.getAttribute("tabindex");

        // swap container for the element
        (this.opts.element as any).select2 = this;
        this.opts.element.setAttribute("tabindex", "-1");
        Fluent(this.container).insertBefore(this.opts.element);
        Fluent.on(this.opts.element, "click.select2", killEvent); // do not leak click events
        (this.container as any).select2 = this;

        this.dropdown = this.container.querySelector(".select2-drop");

        syncCssClasses(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass);

        Fluent.addClass(this.dropdown, evaluate(opts.dropdownCssClass, this.opts.element));
        (this.dropdown as any).select2 = this;
        Fluent.on(this.dropdown, "click", killEvent);

        this.results = results = this.container.querySelector(resultsSelector);
        this.search = search = this.container.querySelector("input.select2-input");

        this.queryCount = 0;
        this.resultsPage = 0;
        this.context = null;

        // initialize the container
        this.initContainer();

        Fluent.on(this.container, "click", killEvent);

        installFilteredMouseMove(this.results);

        Fluent.on(this.dropdown, "mousemove-filtered", resultsSelector, this.highlightUnderEvent.bind(this));
        
        ["touchstart", "touchmove", "touchend"].forEach(ev => 
            Fluent.on(this.dropdown, "touchstart touchmove touchend", resultsSelector, (event: Event) => {
                this._touchEvent = true;
                this.highlightUnderEvent(event);
            })
        );

        Fluent.on(this.dropdown, "touchmove", resultsSelector, this.touchMoved.bind(this));
        ["touchstart", "touchend"].forEach(ev => Fluent.on(this.dropdown, ev, resultsSelector, this.clearTouchMoved.bind(this)));

        // Waiting for a click event on touch devices to select option and hide dropdown
        // otherwise click will be triggered on an underlying element
        Fluent.on(this.dropdown, 'click', event => {
            if (this._touchEvent) {
                this._touchEvent = false;
                this.selectHighlighted();
            }
        });

        installDebouncedScroll(80, this.results);
        Fluent.on(this.dropdown, "scroll-debounced", resultsSelector, this.loadMoreIfNeeded.bind(this));

        // do not propagate change event from the search field out of the component
        Fluent.on(this.container, "change", ".select2-input", function (e: Event) { e.stopPropagation(); });
        Fluent.on(this.dropdown, "change", ".select2-input", function (e: Event) { e.stopPropagation(); });

        // if jquery.mousewheel plugin is installed we can prevent out-of-bounds scrolling of results via mousewheel
        //if ($.fn.mousewheel) {
        //    results.mousewheel(function (e, delta, deltaX, deltaY) {
        //        var top = results.scrollTop();
        //        if (deltaY > 0 && top - deltaY <= 0) {
        //            results.scrollTop(0);
        //            killEvent(e);
        //        } else if (deltaY < 0 && results.get(0).scrollHeight - results.scrollTop() + deltaY <= results.height()) {
        //            results.scrollTop(results.get(0).scrollHeight - results.height());
        //            killEvent(e);
        //        }
        //    });
        //}

        installKeyUpChangeEvent(search);
        ["keyup-change", "input", "paste"].forEach(ev => Fluent.on(search, ev, this.updateResults.bind(this)));
        Fluent.on(search, "focus", function () { search.classList.add("select2-focused"); });
        Fluent.on(search, "blur", function () { search.classList.remove("select2-focused"); });

        Fluent.on(this.dropdown, "mouseup", resultsSelector, (e: Event) => {
            if ((e.target as HTMLElement).closest(".select2-result-selectable")) {
                this.highlightUnderEvent(e);
                this.selectHighlighted(e);
            }
        });

        // trap all mouse events from leaving the dropdown. sometimes there may be a modal that is listening
        // for mouse events outside of itself so it can close itself. since the dropdown is now outside the select2's
        // dom it will trigger the popup close, which is not what we want
        // focusin can cause focus wars between modals and select2 since the dropdown is outside the modal.
        ["click mouseup mousedown touchstart touchend focusin"].forEach(ev => Fluent.on(this.dropdown, ev, function (e) { e.stopPropagation(); }));

        this.nextSearchTerm = undefined;

        if (typeof this.opts.initSelection === "function") {
            // initialize selection based on the current value of the source element
            this.initSelection();

            // if the user has provided a function that can set selection based on the value of the source element
            // we monitor the change event on the element and trigger it, allowing for two way synchronization
            this.monitorSource();
        }

        if (opts.maximumInputLength !== null) {
            this.search.setAttribute("maxlength", "" + opts.maximumInputLength);
        }

        var disabled = opts.element.disabled;
        if (disabled === undefined) disabled = false;
        this.enable(!disabled);

        var readonly = (opts.element as any).readOnly;
        if (readonly === undefined) readonly = false;
        this.readonly(readonly);

        // Calculate size of scrollbar
        scrollBarDimensions = scrollBarDimensions || measureScrollbar();

        this.autofocus = (opts.element as any).autofocus;
        (opts.element as any).autofocus = false;
        if (this.autofocus) this.focus();

        this.search.setAttribute("placeholder", opts.searchInputPlaceholder);
    }

    destroy() {
        var element = this.opts.element, select2 = (element as any)?.select2, self = this;

        this.close();

        if (element && (element as any).detachEvent) {
            (element as any).detachEvent("onpropertychange", self._sync);
        }
        if (this.propertyObserver) {
            this.propertyObserver.disconnect();
            this.propertyObserver = null;
        }
        this._sync = null;

        if (select2 !== undefined) {
            select2.container?.remove();
            select2.dropdown?.remove();
            if (element) {
                element.classList.remove("select2-offscreen");
                delete (element as any).select2;
                Fluent.off(element, ".select2");
                element.autofocus = this.autofocus || false;
            }
            if (this.elementTabIndex) {
                element.setAttribute("tabindex", this.elementTabIndex);
            } else {
                element.removeAttribute("tabindex");
            }
            delete element.style.display;
        }

        cleanupJQueryElements.call(this,
            "container",
            "dropdown",
            "results",
            "search"
        );
    }

    optionToData(element: HTMLOptionElement | HTMLOptGroupElement): Select2Item {
        if (element instanceof HTMLOptionElement) {
            return {
                id: element.value,
                text: element.textContent,
                element: element,
                css: element.getAttribute("class"),
                disabled: element.disabled,
                locked: equal(element.getAttribute("locked"), "locked") || equal(element.getAttribute("locked"), true)
            } as any;
        } else if (element instanceof HTMLOptGroupElement) {
            return {
                text: element.getAttribute("label"),
                children: [],
                element: element,
                css: element.getAttribute("class")
            } as any;
        }
    }

    protected select: HTMLSelectElement;

    protected prepareOpts(opts: Select2Options): Select2Options {
        var element: HTMLInputElement | HTMLSelectElement, select: HTMLSelectElement, idKey: string, ajaxUrl: string, self = this;

        element = opts.element;

        if (element.tagName.toLowerCase() === "select") {
            this.select = select = opts.element as HTMLSelectElement;
        }

        if (select) {
            // these options are not allowed when attached to a select because they are picked up off the element itself
            ["id", "multiple", "ajax", "query", "createSearchChoice", "initSelection", "data", "tags"].forEach(function (opt) {
                if (opt in opts) {
                    throw new Error("Option '" + this + "' is not allowed for Select2 when attached to a <select> element.");
                }
            });
        }

        opts = Object.assign({}, <Select2Options>{
            populateResults: function(this: AbstractSelect2, container, results, query) {
                var id = this.opts.id;

                let populate = function (results: Select2Item[], container: HTMLElement, depth: number) {

                    var i, l, result, selectable, disabled, compound, node, label, innerContainer, formatted;

                    results = opts.sortResults(results, container, query);

                    // collect the created nodes for bulk append
                    var nodes: HTMLElement[] = [];
                    for (i = 0, l = results.length; i < l; i = i + 1) {

                        result = results[i];

                        disabled = (result.disabled === true);
                        selectable = (!disabled) && (id(result) !== undefined);

                        compound = result.children && result.children.length > 0;

                        node = document.createElement("li");
                        node.classList.add("select2-results-dept-" + depth);
                        node.classList.add("select2-result");
                        node.classList.add(selectable ? "select2-result-selectable" : "select2-result-unselectable");
                        if (disabled) { node.classList.add("select2-disabled"); }
                        if (compound) { node.classList.add("select2-result-with-children"); }
                        Fluent.addClass(node, self.opts.formatResultCssClass(result));
                        node.setAttribute("role", "presentation");

                        label = document.createElement("div");
                        label.classList.add("select2-result-label");
                        label.setAttribute("id", "select2-result-label-" + nextUid());
                        label.setAttribute("role", "option");

                        formatted = opts.formatResult(result, label, query, self.opts.escapeMarkup);
                        if (formatted !== undefined) {
                            if (formatted instanceof Node) {
                                Fluent.empty(label);
                                label.appendChild(formatted);
                            }
                            else 
                                label.innerHTML = formatted ?? "";
                            node.append(label);
                        }

                        if (compound) {

                            innerContainer = document.createElement("ul");
                            innerContainer.classList.add("select2-result-sub");
                            populate(result.children, innerContainer, depth + 1);
                            node.append(innerContainer);
                        }

                        (node as any).select2data = result;
                        nodes.push(node);
                    }

                    // bulk append the created nodes
                    container.append(...nodes);
                };

                populate(results, container, 0);
            }
        }, Select2.defaults, opts);

        if (typeof (opts.id) !== "function") {
            idKey = opts.id;
            opts.id = function (e) { return e[idKey]; };
        }

        if (opts.element.dataset.select2Tags) {
            if ("tags" in opts) {
                throw "tags specified as both an attribute 'data-select2-tags' and in options of Select2 " + opts.element.getAttribute("id");
            }
            opts.tags = JSON.parse(opts.element.dataset.select2Tags);
        }

        if (select) {
            opts.query = (query: Select2QueryOptions) => {
                var data: Select2Result = { results: [], more: false },
                    term = query.term,
                    children;

                let process = function (element: HTMLElement, collection: Select2Item[]) {
                    var group: Select2Item;
                    if (element instanceof HTMLOptionElement) {
                        if (query.matcher(term, element.textContent, element)) {
                            collection.push(self.optionToData(element));
                        }
                    } else if (element instanceof HTMLOptGroupElement) {
                        group = self.optionToData(element);
                        Array.from(element.children).forEach(function (elm) { process(elm as HTMLElement, group.children); });
                        if (group.children.length > 0) {
                            collection.push(group);
                        }
                    }
                };

                children = Array.from(element.children);

                // ignore the placeholder option if there is one
                if (this.getPlaceholder() !== undefined && children.length > 0) {
                    var placeholderOption = this.getPlaceholderOption();
                    if (placeholderOption) {
                        children = children.filter(x => x !== placeholderOption);
                    }
                }

                children.forEach(elm => process(elm as HTMLElement, data.results));

                query.callback(data);
            };
            // this is needed because inside val() we construct choices from options and their id is hardcoded
            opts.id = function (e) { return e.id; };
        } else {
            if (!("query" in opts)) {

                if ("ajax" in opts) {
                    ajaxUrl = opts.element.dataset.ajaxUrl;
                    if (ajaxUrl && ajaxUrl.length > 0) {
                        opts.ajax.url = ajaxUrl;
                    }
                    opts.query = ajax.call(opts.element, opts.ajax);
                } else if ("data" in opts) {
                    opts.query = local(opts.data);
                } else if ("tags" in opts) {
                    opts.query = tags(opts.tags);
                    if (opts.createSearchChoice === undefined) {
                        opts.createSearchChoice = function (term) { return { id: term?.trim(), text: term?.trim() }; };
                    }
                    if (opts.initSelection === undefined) {
                        opts.initSelection = function (element, callback) {
                            var data: Select2Item[] = [];
                            splitVal((element as any).value, opts.separator).forEach(function (id) {
                                var obj = { id: id, text: id },
                                    tags = opts.tags;
                                if (typeof tags === "function") tags = tags();
                                tags.forEach(function (tag: any) { if (equal(tag.id, obj.id)) { obj = tag; return false; } });
                                data.push(obj);
                            });

                            callback(data);
                        };
                    }
                }
            }
        }
        if (typeof (opts.query) !== "function") {
            throw "query function not defined for Select2 " + opts.element.getAttribute("id");
        }

        if (opts.createSearchChoicePosition === 'top') {
            opts.createSearchChoicePosition = function (list, item) { list.unshift(item); };
        }
        else if (opts.createSearchChoicePosition === 'bottom') {
            opts.createSearchChoicePosition = function (list, item) { list.push(item); };
        }
        else if (typeof (opts.createSearchChoicePosition) !== "function") {
            throw "invalid createSearchChoicePosition option must be 'top', 'bottom' or a custom function";
        }

        return opts;
    }

    /**
     * Monitor the original element for changes and update select2 accordingly
     */
    protected monitorSource(): void {
        var el = this.opts.element, observer, self = this;

        Fluent.on(el, "change.select2", function (this: AbstractSelect2, e: Event) {
            if (this.opts.element.dataset.select2ChangeTriggered !== "true") {
                this.initSelection();
            }
        }.bind(this));

        Fluent.on(el, "focus.select2", function (this: AbstractSelect2, e: Event) {
            this.focus();
        }.bind(this));

        this._sync = () => {

            // sync enabled state
            var disabled = el.disabled;
            if (disabled === undefined) disabled = false;
            this.enable(!disabled);

            var readonly = (el as any).readOnly;
            if (readonly === undefined) readonly = false;
            this.readonly(readonly);

            syncCssClasses(this.container, this.opts.element, this.opts.adaptContainerCssClass);
            Fluent.addClass(this.container, evaluate(this.opts.containerCssClass, this.opts.element));

            syncCssClasses(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass);
            Fluent.addClass(this.dropdown, evaluate(this.opts.dropdownCssClass, this.opts.element));

        };

        // IE8-10 (IE9/10 won't fire propertyChange via attachEventListener)
        if (el && (el as any).attachEvent) {
            (el as any).attachEvent("onpropertychange", self._sync);
        }

        // safari, chrome, firefox, IE11
        observer = window.MutationObserver;
        if (observer !== undefined) {
            if (this.propertyObserver) { delete this.propertyObserver; this.propertyObserver = null; }
            this.propertyObserver = new observer(function (mutations) {
                mutations.forEach(self._sync);
            });
            this.propertyObserver.observe(el, { attributes: true, subtree: false });
        }
    }

    protected triggerSelect(data: Select2Item): boolean {
        var evt = { val: this.id(data), object: data, choice: data };
        var event = Fluent.trigger(this.opts.element, "select2-selecting", evt);
        return !Fluent.isDefaultPrevented(event);
    }

    /**
     * Triggers the change event on the source element
     */
    protected triggerChange(details?: any): void {

        details = details || {};
        details = Object.assign({}, details, { val: this.val() });
        // prevents recursive triggering
        this.opts.element.dataset.select2ChangeTriggered = "true";
        Fluent.trigger(this.opts.element, "change", details);
        delete this.opts.element.dataset.select2ChangeTriggered;

        // some validation frameworks ignore the change event and listen instead to keyup, click for selects
        // so here we trigger the click event manually
        this.opts.element.click();

        // ValidationEngine ignores the change event and listens instead to blur
        // so here we trigger the blur event manually if so desired
        if (this.opts.blurOnChange)
            this.opts.element.blur();
    }

    protected isInterfaceEnabled(): boolean {
        return this.enabledInterface === true;
    }

    protected enableInterface(): boolean {
        var enabled = this._enabled && !this._readonly,
            disabled = !enabled;

        if (enabled === this.enabledInterface) return false;

        this.container.classList.toggle("select2-container-disabled", disabled);
        this.close();
        this.enabledInterface = enabled;

        return true;
    }

    enable(enabled?: boolean): void {
        if (enabled === undefined) enabled = true;
        if (this._enabled === enabled) return;
        this._enabled = enabled;

        this.opts.element.disabled = !enabled;
        this.enableInterface();
    }

    disable() {
        this.enable(false);
    }

    readonly(enabled?: boolean): void {
        if (enabled === undefined) enabled = false;
        if (this._readonly === enabled) return;
        this._readonly = enabled;

        (this.opts.element as any).readOnly = enabled;
        this.enableInterface();
    }

    opened(): boolean {
        return (this.container) ? this.container.classList.contains("select2-dropdown-open") : false;
    }

    positionDropdown() {
        var dropdown = this.dropdown,
            offset = getOffset(this.container),
            height = this.container.offsetHeight,
            width = this.container.offsetWidth,
            dropHeight = dropdown.offsetHeight,
            windowWidth = window.innerWidth,
            windowHeight = window.innerHeight,
            viewPortRight = window.scrollX + windowWidth,
            viewportBottom = window.scrollY + windowHeight,
            dropTop = offset.top + height,
            dropLeft = offset.left,
            enoughRoomBelow = dropTop + dropHeight <= viewportBottom,
            enoughRoomAbove = (offset.top - dropHeight) >= window.scrollY,
            dropWidth = dropdown.offsetWidth,
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight,
            aboveNow = dropdown.classList.contains("select2-drop-above"),
            bodyOffset,
            above,
            changeDirection,
            resultsListNode;

        // always prefer the current above/below alignment, unless there is not enough room
        if (aboveNow) {
            above = true;
            if (!enoughRoomAbove && enoughRoomBelow) {
                changeDirection = true;
                above = false;
            }
        } else {
            above = false;
            if (!enoughRoomBelow && enoughRoomAbove) {
                changeDirection = true;
                above = true;
            }
        }

        //if we are changing direction we need to get positions when dropdown is hidden;
        if (changeDirection) {
            Fluent.toggleClass(dropdown, "select2-display-none", true);
            offset = getOffset(this.container);
            height = this.container.offsetHeight;
            width = this.container.offsetWidth;
            dropHeight = dropdown.offsetHeight;
            viewPortRight = window.scrollX + windowWidth;
            viewportBottom = window.scrollY + windowHeight;
            dropTop = offset.top + height;
            dropLeft = offset.left;
            dropWidth = dropdown.offsetWidth;
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight;
            Fluent.toggleClass(dropdown, "select2-display-none", false);

            // fix so the cursor does not move to the left within the search-textbox in IE
            this.focusSearch();
        }

        if (this.opts.dropdownAutoWidth) {
            resultsListNode = dropdown.querySelector('.select2-results');
            dropdown.classList.add('select2-drop-auto-width');
            dropdown.style.width = "";
            // Add scrollbar width to dropdown if vertical scrollbar is present
            dropWidth = dropdown.offsetWidth + (resultsListNode.scrollHeight === resultsListNode.clientHeight ? 0 : scrollBarDimensions.width);
            dropWidth > width ? width = dropWidth : dropWidth = width;
            dropHeight = dropdown.offsetHeight;
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight;
        }
        else {
            this.container.classList.remove('select2-drop-auto-width');
        }

        //console.log("below/ droptop:", dropTop, "dropHeight", dropHeight, "sum", (dropTop+dropHeight)+" viewport bottom", viewportBottom, "enough?", enoughRoomBelow);
        //console.log("above/ offset.top", offset.top, "dropHeight", dropHeight, "top", (offset.top-dropHeight), "scrollTop", this.body.scrollTop(), "enough?", enoughRoomAbove);

        // fix positioning when body has an offset and is not position: static
        if (getComputedStyle(document.body).position !== 'static') {
            bodyOffset = getOffset(document.body);
            dropTop -= bodyOffset.top;
            dropLeft -= bodyOffset.left;
        }

        if (!enoughRoomOnRight) {
            dropLeft = offset.left + this.container.offsetWidth - dropWidth;
        }

        let css: Record<string, any> = {
            left: dropLeft + "px",
            width: width + "px"
        };

        if (above) {
            css.top = (offset.top - dropHeight) + "px";
            css.bottom = 'auto';
            this.container.classList.add("select2-drop-above");
            dropdown.classList.add("select2-drop-above");
        }
        else {
            css.top = dropTop + "px";
            css.bottom = 'auto';
            this.container.classList.remove("select2-drop-above");
            dropdown.classList.remove("select2-drop-above");
        }
        css = Object.assign(css, evaluate(this.opts.dropdownCss, this.opts.element));

        Object.assign(dropdown.style, css);
    }

    protected shouldOpen(): boolean {

        if (this.opened()) return false;

        if (this._enabled === false || this._readonly === true) return false;

        var event = Fluent.trigger(this.opts.element, "select2-opening");
        return !Fluent.isDefaultPrevented(event);
    }

    protected clearDropdownAlignmentPreference() {
        // clear the classes used to figure out the preference of where the dropdown should be opened
        this.container.classList.remove("select2-drop-above");
        this.dropdown.classList.remove("select2-drop-above");
    }

    /**
     * Opens the dropdown
     *
     * @return {Boolean} whether or not dropdown was opened. This method will return false if, for example,
     * the dropdown is already open, or if the 'open' event listener on the element called preventDefault().
     */
    open(): boolean {

        if (!this.shouldOpen()) return false;

        this.opening();

        // Only bind the document mousemove when the dropdown is visible
        Fluent.on(document, "mousemove.select2Event", function (e: MouseEvent) {
            lastMousePosition.x = e.pageX;
            lastMousePosition.y = e.pageY;
        });

        return true;
    }

    /**
     * Performs the opening of the dropdown
     */
    protected opening() {
        var cid = this.containerEventName,
            scroll = "scroll." + cid,
            resize = "resize." + cid,
            orient = "orientationchange." + cid,
            mask: HTMLElement;

        this.container.classList.add("select2-dropdown-open", "select2-container-active");

        this.clearDropdownAlignmentPreference();

        var dropdownParent = (typeof this.opts.dropdownParent === "function" ? 
            this.opts.dropdownParent(this.opts.element) : null) ?? this.dropdown.parentElement ?? document.body;

        if (dropdownParent && this.dropdown !== dropdownParent.lastElementChild) {
            dropdownParent.appendChild(this.dropdown);
        }

        // create the dropdown mask if doesn't already exist
        mask = document.getElementById("select2-drop-mask");
        if (!mask) {
            mask = document.createElement("div");
            mask.setAttribute("id", "select2-drop-mask");
            mask.setAttribute("class", "select2-drop-mask");
            mask.style.display = "none";
            document.body.appendChild(mask);
            ["mousedown", "touchstart", "click"].forEach(ev => Fluent.on(mask, ev, function (e) {
                // Prevent IE from generating a click event on the body
                reinsertElement(mask);

                var dropdown = document.getElementById("select2-drop"), self;
                if (dropdown) {
                    self = (dropdown as any).select2;
                    if (self.opts.selectOnBlur) {
                        self.selectHighlighted({ noFocus: true });
                    }
                    self.close();
                    e.preventDefault();
                    e.stopPropagation();
                }
            }));
        }

        // ensure the mask is always right before the dropdown
        if (this.dropdown.previousElementSibling != mask) {
            Fluent(mask).insertBefore(this.dropdown);
        }

        // move the global id to the correct dropdown
        document.getElementById("select2-drop")?.removeAttribute("id");
        this.dropdown.setAttribute("id", "select2-drop");

        // show the elements
        Fluent.toggle(mask, true);

        this.positionDropdown();
        Fluent.toggleClass(this.dropdown, "select2-display-none", false);
        this.positionDropdown();

        this.dropdown.classList.add("select2-drop-active");

        // attach listeners to events that can change the position of the container and thus require
        // the position of the dropdown to be updated as well so it does not come unglued from the container
        var that = this;

        var parent = this.container.parentElement;
        var parents = [];
        while (parent) {
            parents.push(parent);
            parent = parent.parentElement;
        }
        parents.push(window);
        
        parents.forEach(parent => {
            [resize, scroll, orient].forEach(ev => {
                Fluent.on(parent, ev, function() { 
                    if (that.opened()) that.positionDropdown();
                });
            });
        });
    }

    close(): void {
        if (!this.opened()) return;

        var cid = this.containerEventName,
            scroll = "scroll." + cid,
            resize = "resize." + cid,
            orient = "orientationchange." + cid;

        var parent = this.container.parentElement;
        var parents = [];
        while (parent) {
            parents.push(parent);
            parent = parent.parentElement;
        }
        parents.push(window);
        parents.forEach(parent => {
            Fluent.off(parent, scroll);
            Fluent.off(parent, resize);
            Fluent.off(parent, orient);
        });

        this.clearDropdownAlignmentPreference();

        Fluent(document.getElementById("select2-drop-mask")).hide();
        this.dropdown.removeAttribute("id"); // only the active dropdown has the select2-drop id
        Fluent.toggleClass(this.dropdown, "select2-display-none", true);
        this.container.classList.remove("select2-dropdown-open", "select2-container-active");
        Fluent.empty(this.results);

        // Now that the dropdown is closed, unbind the global document mousemove event
        Fluent.off(document, "mousemove.select2Event");

        this.clearSearch();
        this.search.classList.remove("select2-active");
        this.search.parentElement?.classList.remove("select2-active");
        Fluent.trigger(this.opts.element, "select2-close");
    }

    /**
     * Opens control, sets input value, and updates results.
     */
    protected externalSearch(term: string) {
        this.open();
        this.search.value = term;
        this.updateResults(false);
    }

    protected clearSearch() {

    }

    protected getMaximumSelectionSize() {
        return evaluate(this.opts.maximumSelectionSize, this.opts.element);
    }

    protected ensureHighlightVisible() {
        var results = this.results, index, child, hb, rb, y, more, topOffset;

        index = this.highlight();

        if (index < 0) return;

        if (index == 0) {

            // if the first element is highlighted scroll all the way to the top,
            // that way any unselectable headers above it will also be scrolled
            // into view

            results.scrollTop = 0;
            return;
        }

        var children = this.findHighlightableChoices().map(x => x.querySelector('.select2-result-label'));

        child = children[index] as HTMLElement;
        if (!child)
            return;

        topOffset = (getOffset(child) || {}).top || 0;

        hb = topOffset + getOuterHeightWithMargins(child);

        // if this is the last child lets also make sure select2-more-results is visible
        if (index === children.length - 1) {
            more = results.querySelector("li.select2-more-results");
            if (more) {
                hb = getOffset(more).top + getOuterHeightWithMargins(more);
            }
        }

        rb = getOffset(results).top + getOuterHeightWithMargins(results);
        if (hb > rb) {
            results.scrollTop = results.scrollTop + (hb - rb);
        }
        y = topOffset - getOffset(results).top;

        // make sure the top of the element is visible
        if (y < 0 && getComputedStyle(child).display != 'none') {
            results.scrollTop = results.scrollTop + y; // y is negative
        }
    }

    protected findHighlightableChoices() {
        return Array.from(this.results.querySelectorAll(".select2-result-selectable:not(.select2-disabled):not(.select2-selected)"));
    }

    protected moveHighlight(delta: number) {
        var choices = this.findHighlightableChoices(),
            index = this.highlight();

        while (index > -1 && index < choices.length) {
            index += delta;
            var choice = choices[index];
            if (choice && choice.classList.contains("select2-result-selectable") && !choice.classList.contains("select2-disabled") && !choice.classList.contains("select2-selected")) {
                this.highlight(index);
                break;
            }
        }
    }

    protected highlight(index?: number) {
        var choices = this.findHighlightableChoices(),
            choice,
            data;

        if (arguments.length === 0) {
            return choices.findIndex(x => x.classList.contains("select2-highlighted"));
        }

        if (index >= choices.length) index = choices.length - 1;
        if (index < 0) index = 0;

        this.removeHighlight();

        choice = choices[index];
        choice?.classList.add("select2-highlighted");

        // ensure assistive technology can determine the active choice
        this.search.setAttribute("aria-activedescendant", choice?.querySelector(".select2-result-label")?.getAttribute("id"));

        this.ensureHighlightVisible();

        data = (choice as any)?.select2data;
        if (data) {
            Fluent.trigger(this.opts.element, "select2-highlight", { val: this.id(data), choice: data });
        }
    }

    protected removeHighlight() {
        this.results.querySelectorAll(".select2-highlighted").forEach(x => x.classList.remove("select2-highlighted"));
    }

    protected touchMoved() {
        this._touchMoved = true;
    }

    protected clearTouchMoved() {
        this._touchMoved = false;
    }

    protected countSelectableResults() {
        return this.findHighlightableChoices().length;
    }

    protected highlightUnderEvent(event: Event) {
        var el = (event.target as any)?.closest?.(".select2-result-selectable") as HTMLElement;
        if (el && !el.classList.contains("select2-highlighted")) {
            var choices = this.findHighlightableChoices();
            this.highlight(choices.indexOf(el));
        } else if (!el) {
            // if we are over an unselectable item remove all highlights
            this.removeHighlight();
        }
    }

    protected loadMoreIfNeeded() {
        var results = this.results,
            more = results.querySelector<HTMLElement>("li.select2-more-results"),
            below, // pixels the element is below the scroll fold, below==0 is when the element is starting to be visible
            page = this.resultsPage + 1,
            self = this,
            term = this.search.value,
            context = this.context;

        if (!more) return;
        below = getOffset(more).top - getOffset(results).top - results.getBoundingClientRect().height;

        if (below <= this.opts.loadMorePadding) {
            more.classList.add("select2-active");
            this.opts.query({
                element: this.opts.element,
                term: term,
                page: page,
                context: context,
                matcher: this.opts.matcher,
                callback: data => {

                    // ignore a response if the select2 has been closed before it was received
                    if (!self.opened()) return;


                    self.opts.populateResults.call(this, results, data.results, { term: term, page: page, context: context });
                    self.postprocessResults(data, false, false);

                    if (data.more === true) {
                        results.appendChild(more);
                        var loadMore = evaluate(self.opts.formatLoadMore, self.opts.element, page + 1);
                        Fluent.empty(more);
                        if (loadMore instanceof Node)
                            more.appendChild(loadMore);
                        else
                            more.textContent = loadMore ?? "";
                        window.setTimeout(function () { self.loadMoreIfNeeded(); }, 10);
                    } else {
                        more.remove();
                    }
                    self.positionDropdown();
                    self.resultsPage = page;
                    self.context = data.context;
                    Fluent.trigger(this.opts.element, "select2-loaded", { items: data });
                }
            });
        }
    }

    /**
     * Default tokenizer function which does nothing
     */
    protected tokenize(): string {
        return undefined;
    }

    /**
     * @param initial whether or not this is the call to this method right after the dropdown has been opened
     */
    protected updateResults(initial?: boolean) {
        var search = this.search,
            results = this.results,
            opts = this.opts,
            data,
            self = this,
            input,
            term = search.value,
            lastTerm = this.container.dataset.select2LastTerm,
            // sequence number used to drop out-of-order responses
            queryNumber: number;

        // prevent duplicate queries against the same term
        if (initial !== true && lastTerm && equal(term, lastTerm)) return;

        this.container.dataset.select2LastTerm = term;

        // if the search is currently hidden we do not alter the results
        if (initial !== true && (this.showSearchInput === false || !this.opened())) {
            return;
        }

        function postRender() {
            search.classList.remove("select2-active");
            search.parentElement?.classList.remove("select2-active");
            self.positionDropdown();
        }

        function createLi(klass: string, html?: Select2FormatResult) {
            if (klass == null && html === undefined)
                return;
            var li = document.createElement("li");
            li.classList.add(klass);
            if (html instanceof Node)
                li.appendChild(html);
            else
                li.innerHTML = html ?? "";
            return li;
        }


        function render(klass: string, html?: Select2FormatResult) {
            Fluent.empty(results);
            var li = createLi(klass, html);
            if (li != null)
                results.appendChild(li);
            postRender();
        }

        queryNumber = ++this.queryCount;

        var maxSelSize = this.getMaximumSelectionSize();
        if (maxSelSize >= 1) {
            data = this.data();
            if (Array.isArray(data) && data.length >= maxSelSize && checkFormatter(opts.formatSelectionTooBig, "formatSelectionTooBig")) {
                render("select2-selection-limit", evaluate(opts.formatSelectionTooBig, opts.element, maxSelSize));
                return;
            }
        }

        if (search.value?.length < opts.minimumInputLength) {
            if (checkFormatter(opts.formatInputTooShort, "formatInputTooShort")) {
                render("select2-no-results", evaluate(opts.formatInputTooShort, opts.element, search.value, opts.minimumInputLength));
            } else {
                render("");
            }
            if (initial && (this as any).showSearch) (this as any).showSearch(true);
            return;
        }

        if (opts.maximumInputLength && search.value?.length > opts.maximumInputLength) {
            if (checkFormatter(opts.formatInputTooLong, "formatInputTooLong")) {
                render("select2-no-results", evaluate(opts.formatInputTooLong, opts.element, search.value, opts.maximumInputLength));
            } else {
                render("");
            }
            return;
        }

        if (opts.formatSearching && this.findHighlightableChoices().length === 0) {
            render("select2-searching", evaluate(opts.formatSearching, opts.element) + "</li>");
        }

        search.classList.add("select2-active");
        search.parentElement?.classList.add("select2-active");

        this.removeHighlight();

        // give the tokenizer a chance to pre-process the input
        input = this.tokenize();
        if (input != undefined && input != null) {
            search.value = input ?? "";
        }

        this.resultsPage = 1;

        opts.query({
            element: opts.element,
            term: search.value,
            page: this.resultsPage,
            context: null,
            matcher: opts.matcher,
            callback: (data: Select2Result) => {
                var def: Select2Item; // default choice

                // ignore old responses
                if (queryNumber != this.queryCount) {
                    return;
                }

                // ignore a response if the select2 has been closed before it was received
                if (!this.opened()) {
                    this.search.classList.remove("select2-active");
                    return;
                }

                // handle ajax error
                if (data.hasError !== undefined && checkFormatter(opts.formatAjaxError, "formatAjaxError")) {
                    render("select2-ajax-error", evaluate(opts.formatAjaxError, opts.element, data));
                    return;
                }

                // save context, if any
                this.context = (data.context === undefined) ? null : data.context;
                // create a default choice and prepend it to the list
                if (this.opts.createSearchChoice && search.value !== "") {
                    def = this.opts.createSearchChoice.call(self, search.value, data.results);
                    if (def !== undefined && def !== null && self.id(def) !== undefined && self.id(def) !== null) {
                        if (data.results.filter(
                            function () {
                                return equal(self.id(this), self.id(def));
                            }).length === 0) {
                            (this.opts.createSearchChoicePosition as any)(data.results, def);
                        }
                    }
                }

                if (data.results.length === 0 && checkFormatter(opts.formatNoMatches, "formatNoMatches")) {
                    render("select2-no-results", evaluate(opts.formatNoMatches, opts.element, search.value));
                    return;
                }

                Fluent.empty(results);
                self.opts.populateResults.call(this, results, data.results, { term: search.value, page: this.resultsPage, context: null });

                if (data.more === true && checkFormatter(opts.formatLoadMore, "formatLoadMore")) {
                    results.appendChild(createLi("select2-more-results", evaluate(opts.formatLoadMore, opts.element, this.resultsPage)));
                    window.setTimeout(function () { self.loadMoreIfNeeded(); }, 10);
                }

                this.postprocessResults(data, initial);

                postRender();

                Fluent.trigger(this.opts.element, "select2-loaded", { items: data });
            }
        });
    }

    protected cancel(e?: Event) {
        this.close();
    }

    protected blur() {
        // if selectOnBlur == true, select the currently highlighted option
        if (this.opts.selectOnBlur)
            this.selectHighlighted({ noFocus: true });

        this.close();
        this.container.classList.remove("select2-container-active");
        // synonymous to .is(':focus'), which is available in jquery >= 1.6
        if (this.search === document.activeElement) { this.search.blur(); }
        this.clearSearch();
        this.selection.querySelectorAll(".select2-search-choice-focus").forEach(x => x.classList.remove("select2-search-choice-focus"));
    }

    protected focusSearch() {
        focus(this.search);
    }

    protected selectHighlighted(options?: any) {
        if (this._touchMoved) {
            this.clearTouchMoved();
            return;
        }
        var index = this.highlight(),
            highlighted = this.results.querySelector(".select2-highlighted"),
            data = (highlighted?.closest('.select2-result') as any)?.select2data;

        if (data) {
            this.highlight(index);
            this.onSelect(data, options);
        } else if (options && options.noFocus) {
            this.close();
        }
    }

    protected getPlaceholder() {
        var placeholderOption;
        return this.opts.element.getAttribute("placeholder") ||
            this.opts.element.dataset.placeholder ||
            this.opts.placeholder ||
            ((placeholderOption = this.getPlaceholderOption()) !== undefined ? placeholderOption?.textContent : undefined);
    }

    protected getPlaceholderOption(): HTMLOptionElement {
        if (this.select) {
            var firstOption = this.select.querySelector<HTMLOptionElement>(':scope > option');
            if (this.opts.placeholderOption !== undefined) {
                //Determine the placeholder option based on the specified placeholderOption setting
                return (this.opts.placeholderOption === "first" && firstOption) ||
                    (typeof this.opts.placeholderOption === "function" && this.opts.placeholderOption(this.select));
            } else if (firstOption.textContent?.trim() === "" && firstOption.value === "") {
                //No explicit placeholder option specified, use the first if it's blank
                return firstOption;
            }
        }
    }

    /**
     * Get the desired width for the container element.  This is
     * derived first from option `width` passed to select2, then
     * the inline 'style' on the original element, and finally
     * falls back to the jQuery calculated element width.
     */
    protected initContainerWidth() {
        function resolveContainerWidth(this: AbstractSelect2) {
            var style, attrs, matches, i, l, attr;

            if (this.opts.width === "off") {
                return null;
            } else if (this.opts.width === "element") {
                return this.opts.element.offsetWidth === 0 ? 'auto' : this.opts.element.offsetWidth + 'px';
            } else if (this.opts.width === "copy" || this.opts.width === "resolve") {
                // check if there is inline style on the element that contains width
                style = this.opts.element.getAttribute('style');
                if (style) {
                    attrs = style.split(';');
                    for (i = 0, l = attrs.length; i < l; i = i + 1) {
                        attr = attrs[i].replace(/\s/g, '');
                        matches = attr.match(/^width:(([-+]?([0-9]*\.)?[0-9]+)(px|em|ex|%|in|cm|mm|pt|pc))/i);
                        if (matches !== null && matches.length >= 1)
                            return matches[1];
                    }
                }

                if (this.opts.width === "resolve") {
                    // next check if css('width') can resolve a width that is percent based, this is sometimes possible
                    // when attached to input type=hidden or elements hidden via css
                    style = getComputedStyle(this.opts.element).width;
                    if (style.indexOf("%") > 0) return style;

                    // finally, fallback on the calculated width of the element
                    return (this.opts.element.offsetWidth === 0 ? 'auto' : this.opts.element.offsetWidth + 'px');
                }

                return null;
            } else if (typeof this.opts.width === "function") {
                return this.opts.width();
            } else {
                return this.opts.width;
            }
        };

        var width = resolveContainerWidth.call(this);
        if (width !== null) {
            this.container.style.width = width;
        }
    }
}

class SingleSelect2 extends AbstractSelect2 {

    protected focusser: HTMLInputElement;

    createContainer() {
        var container = document.createElement("div");
        container.classList.add("select2-container");
        container.innerHTML = 
            "<a href='javascript:void(0)' class='select2-choice' tabindex='-1'>" +
                "<span class='select2-chosen'>&#160;</span><abbr class='select2-search-choice-close'></abbr>" +
                "<span class='select2-arrow' role='presentation'><b role='presentation'></b></span>" +
            "</a>" +
            "<label for='' class='select2-offscreen'></label>" + 
            "<input class='select2-focusser select2-offscreen' type='text' aria-haspopup='true' role='button' />" + 
            "<div class='select2-drop select2-display-none'>" + 
                "<div class='select2-search'>" + 
                    "<label for='' class='select2-offscreen'></label>" + 
                    "<input type='text' autocomplete='off' autocorrect='off' autocapitalize='off' spellcheck='false' class='select2-input' role='combobox' aria-expanded='true'" +
                        "aria-autocomplete='list' />" + 
                "</div>" + 
                "<ul class='select2-results' role='listbox'>" + 
                "</ul>" +
            "</div>";
        return container;
    }

    protected enableInterface() {
        var result = super.enableInterface();
        if (result) {
            this.focusser.disabled = !this.isInterfaceEnabled();
        }
        return result;
    }

    protected opening() {
        var el, range, len;

        if (this.opts.minimumResultsForSearch >= 0) {
            this.showSearch(true);
        }

        super.opening();

        if (this.showSearchInput !== false) {
            // IE appends focusser.val() at the end of field :/ so we manually insert it at the beginning using a range
            // all other browsers handle this just fine

            this.search.value = this.focusser.value;
        }
        if (this.opts.shouldFocusInput(this)) {
            this.search.focus();
            // move the cursor to the end after focussing, otherwise it will be at the beginning and
            // new text will appear *before* focusser.val()
            el = this.search;
            if ((el as any).createTextRange) {
                range = (el as any).createTextRange();
                range.collapse(false);
                range.select();
            } else if (el.setSelectionRange) {
                len = this.search.value.length;
                el.setSelectionRange(len, len);
            }
        }

        // initializes search's value with nextSearchTerm (if defined by user)
        // ignore nextSearchTerm if the dropdown is opened by the user pressing a letter
        if (this.search.value === "") {
            if (this.nextSearchTerm != undefined) {
                this.search.value = this.nextSearchTerm ?? "";
                this.search.select();
            }
        }

        this.focusser.disabled = true;
        this.focusser.value = "";
        this.updateResults(true);
        Fluent.trigger(this.opts.element, "select2-open");
    }

    override close() {
        if (!this.opened()) return;
        super.close();

        this.focusser.disabled = false;

        if (this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }
    }

    override focus() {
        if (this.opened()) {
            this.close();
        } else {
            this.focusser.disabled = false;
            if (this.opts.shouldFocusInput(this)) {
                this.focusser.focus();
            }
        }
    }

    override isFocused(): boolean {
        return this.container.classList.contains("select2-container-active");
    }

    protected cancel(e?: Event) {
        super.cancel(e);
        this.focusser.disabled = false;

        if (this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }
    }

    override destroy() {
        document.querySelector("label[for='" + this.focusser.getAttribute('id') + "']")?.setAttribute(
            'for', this.opts.element.getAttribute("id"));
        super.destroy();

        cleanupJQueryElements.call(this,
            "selection",
            "focusser"
        );
    }

    override initContainer() {

        var selection: HTMLElement,
            container = this.container,
            dropdown = this.dropdown,
            idSuffix = nextUid(),
            elementLabel;

        if (this.opts.minimumResultsForSearch < 0) {
            this.showSearch(false);
        } else {
            this.showSearch(true);
        }

        this.selection = selection = container.querySelector(".select2-choice");

        this.focusser = container.querySelector(".select2-focusser");

        // add aria associations
        selection.querySelector(".select2-chosen")?.setAttribute("id", "select2-chosen-" + idSuffix);
        this.focusser.setAttribute("aria-labelledby", "select2-chosen-" + idSuffix);
        this.results.setAttribute("id", "select2-results-" + idSuffix);
        this.search.setAttribute("aria-owns", "select2-results-" + idSuffix);

        // rewrite labels from original element to focusser
        this.focusser.setAttribute("id", "s2id_autogen" + idSuffix);

        elementLabel = document.querySelector("label[for='" + this.opts.element.getAttribute("id") + "']");

        this.focusser.previousElementSibling.textContent = elementLabel?.textContent ?? "";
        this.focusser.previousElementSibling.setAttribute('for', this.focusser.getAttribute('id'));

        // Ensure the original element retains an accessible name
        var originalTitle = this.opts.element.getAttribute("title");
        this.opts.element.setAttribute("title", (originalTitle || (elementLabel?.textContent ?? "")));

        this.focusser.setAttribute("tabindex", this.elementTabIndex);

        // write label for search field using the label from the focusser element
        this.search.setAttribute("id", this.focusser.getAttribute('id') + '_search');

        this.search.previousElementSibling.textContent = document.querySelector("label[for='" + this.focusser.getAttribute('id') + "']")?.textContent ?? "";
        this.search.previousElementSibling.setAttribute('for', this.search.getAttribute('id'));

        Fluent.on(this.search, "keydown", (e) => {
            if (!this.isInterfaceEnabled()) return;

            // filter 229 keyCodes (input method editor is processing key input)
            if (229 == e.keyCode) return;

            if (e.which === KEY.PAGE_UP || e.which === KEY.PAGE_DOWN) {
                // prevent the page from scrolling
                killEvent(e);
                return;
            }

            switch (e.which) {
                case KEY.UP:
                case KEY.DOWN:
                    this.moveHighlight((e.which === KEY.UP) ? -1 : 1);
                    killEvent(e);
                    return;
                case KEY.ENTER:
                    this.selectHighlighted();
                    killEvent(e);
                    return;
                case KEY.TAB:
                    this.selectHighlighted({ noFocus: true });
                    return;
                case KEY.ESC:
                    this.cancel(e);
                    killEvent(e);
                    return;
            }
        });

        Fluent.on(this.search, "blur", e => {
            // a workaround for chrome to keep the search field focussed when the scroll bar is used to scroll the dropdown.
            // without this the search field loses focus which is annoying
            if (document.activeElement === document.body) {
                window.setTimeout(() => {
                    if (this.opened()) {
                        this.search.focus();
                    }
                }, 0);
            }
        });

        Fluent.on(this.focusser, "keydown", e => {
            if (!this.isInterfaceEnabled()) return;

            if (e.which === KEY.TAB || KEY.isControl(e) || KEY.isFunctionKey(e) || e.which === KEY.ESC) {
                return;
            }

            if (this.opts.openOnEnter === false && e.which === KEY.ENTER) {
                killEvent(e);
                return;
            }

            if (e.which == KEY.DOWN || e.which == KEY.UP
                || (e.which == KEY.ENTER && this.opts.openOnEnter)) {

                if (e.altKey || e.ctrlKey || e.shiftKey || e.metaKey) return;

                this.open();
                killEvent(e);
                return;
            }

            if (e.which == KEY.DELETE || e.which == KEY.BACKSPACE) {
                if (this.opts.allowClear) {
                    this.clear();
                }
                killEvent(e);
                return;
            }
        });


        installKeyUpChangeEvent(this.focusser);
        ["keyup-change", "input"].forEach(ev => Fluent.on(this.focusser, ev, e => {
            if (this.opts.minimumResultsForSearch >= 0) {
                e.stopPropagation();
                if (this.opened()) return;
                this.open();
            }
        }));

        ["mousedown", "touchstart"].forEach(ev => Fluent.on(selection, ev, "abbr", (e: Event) => {
            if (!this.isInterfaceEnabled()) return;
            this.clear();
            killEventImmediately(e);
            this.close();
            this.selection.focus();
        }));

        Fluent.on(selection, "dragstart", e => { e.preventDefault(); return false });

        Fluent.on(selection, "click", e => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }

            if (this.opened()) {
                this.close();
            } else if (this.isInterfaceEnabled()) {
                this.open();
            }

            killEvent(e);
        });

        ["mousedown", "touchstart"].forEach(ev => Fluent.on(dropdown, ev, () => {
            if (this.opts.shouldFocusInput(this)) {
                this.search.focus();
            }
        }));

        Fluent.on(selection, "focus", e => {
            killEvent(e);
        });

        Fluent.on(this.focusser, "focus", () => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
        });

        Fluent.on(this.focusser, "blur", () => {
            if (!this.opened()) {
                this.container.classList.remove("select2-container-active");
                Fluent.trigger(this.opts.element, "select2-blur");
            }
        });

        Fluent.on(this.search, "focus", () => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
        });

        this.initContainerWidth();
        this.opts.element.classList.add("select2-offscreen");
        this.setPlaceholder();
    }

    protected clear(triggerChange?: boolean) {
        var data = (this.selection as any)?.select2data;
        if (data) { // guard against queued quick consecutive clicks
            var evt = Fluent.trigger(this.opts.element, "select2-clearing");
            if (Fluent.isDefaultPrevented(evt)) {
                return;
            }
            var placeholderOption = this.getPlaceholderOption();
            this.opts.element.value = placeholderOption ? placeholderOption.value : "";
            Fluent.empty(this.selection.querySelector(".select2-chosen"));
            delete (this.selection as any).select2data;
            this.setPlaceholder();

            if (triggerChange !== false) {
                Fluent.trigger(this.opts.element, "select2-removed", { val: this.id(data), choice: data });
                this.triggerChange({ removed: data });
            }
        }
    }

    /**
     * Sets selection based on source element's value
     */
    protected initSelection() {
        if (this.isPlaceholderOptionSelected()) {
            this.updateSelection(null);
            this.close();
            this.setPlaceholder();
        } else {
            var self = this;
            this.opts.initSelection.call(null, this.opts.element, function (selected: Select2Item) {
                if (!self.container)
                    return;
                if (selected !== undefined && selected !== null) {
                    self.updateSelection(selected);
                    self.close();
                    self.setPlaceholder();
                    self.nextSearchTerm = self.opts.nextSearchTerm(selected, self.search.value);
                }
            });
        }
    }

    protected isPlaceholderOptionSelected() {
        var placeholderOption;
        if (this.getPlaceholder() === undefined) return false; // no placeholder specified so no option should be considered
        return ((placeholderOption = this.getPlaceholderOption()) !== undefined && placeholderOption.selected)
            || (this.opts.element.value === "")
            || (this.opts.element.value === undefined)
            || (this.opts.element.value === null);
    }

    protected prepareOpts(opts: Select2Options) {
        opts = super.prepareOpts(opts);
        var self = this;

        if (opts.element.tagName.toLowerCase() === "select") {
            // install the selection initializer
            opts.initSelection = function (element, callback) {
                var selected = element.querySelector<HTMLOptionElement>("option:checked:not(:disabled)");
                // a single select box always has a value, no need to null check 'selected'
                callback(self.optionToData(selected));
            };
        } else if ("data" in opts) {
            // install default initSelection when applied to hidden input and data is local
            opts.initSelection = opts.initSelection || function (element, callback) {
                var id = (element as HTMLInputElement).value;
                //search in data by id, storing the actual matching item
                var match: Select2Item = null;
                opts.query({
                    matcher: function (term, text, el) {
                        var is_match = equal(id, opts.id(el));
                        if (is_match) {
                            match = el;
                        }
                        return is_match;
                    },
                    callback: typeof callback !== "function" ? () => {} : function () {
                        callback(match);
                    }
                });
            };
        }

        return opts;
    }

    protected getPlaceholder() {
        // if a placeholder is specified on a single select without a valid placeholder option ignore it
        if (this.select) {
            if (this.getPlaceholderOption() === undefined) {
                return undefined;
            }
        }

        return super.getPlaceholder();
    }

    protected setPlaceholder() {
        var placeholder = this.getPlaceholder();

        if (this.isPlaceholderOptionSelected() && placeholder !== undefined) {

            // check for a placeholder option if attached to a select
            if (this.select && this.getPlaceholderOption() === undefined) return;

            var chosen = this.selection.querySelector(".select2-chosen");
            chosen && (chosen.textContent = placeholder);

            this.selection.classList.add("select2-default");

            this.container.classList.remove("select2-allowclear");
        }
    }

    protected override postprocessResults(data: Select2Result, initial: boolean, noHighlightUpdate?: boolean) {
        var selected = 0, self = this;

        // find the selected element in the result list

        this.findHighlightableChoices().forEach((elm, i) => {
            if (equal(self.id((elm as any).select2data), self.opts.element.value)) {
                selected = i;
                return false;
            }
        });

        // and highlight it
        if (noHighlightUpdate !== false) {
            if (initial === true && selected >= 0) {
                this.highlight(selected);
            } else {
                this.highlight(0);
            }
        }

        // hide the search box if this is the first we got the results and there are enough of them for search

        if (initial === true) {
            var min = this.opts.minimumResultsForSearch;
            if (min >= 0) {
                this.showSearch(countResults(data.results) >= min);
            }
        }
    }

    protected showSearch(showSearchInput: boolean) {
        if (this.showSearchInput === showSearchInput) return;

        this.showSearchInput = showSearchInput;

        this.dropdown.querySelector(".select2-search")?.classList.toggle("select2-search-hidden", !showSearchInput);
        this.dropdown.querySelector(".select2-search")?.classList.toggle("select2-offscreen", !showSearchInput);
        //add "select2-with-searchbox" to the container if search box is shown
        this.dropdown.classList.toggle("select2-with-searchbox", showSearchInput);
        this.container.classList.toggle("select2-with-searchbox", showSearchInput);
    }

    protected onSelect(data: Select2Item, options: any) {

        if (!this.triggerSelect(data)) { return; }

        var old = this.opts.element.value,
            oldData = this.data();

        this.opts.element.value = this.id(data);
        this.updateSelection(data);

        Fluent.trigger(this.opts.element, "select2-selected", { val: this.id(data), choice: data });

        this.nextSearchTerm = this.opts.nextSearchTerm(data, this.search.value);
        this.close();

        if ((!options || !options.noFocus) && this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }

        if (!equal(old, this.id(data))) {
            this.triggerChange({ added: data, removed: oldData });
        }
    }

    protected updateSelection(data: Select2Item) {

        if (!this.selection)
            return;
        var container = this.selection.querySelector<HTMLElement>(".select2-chosen"), formatted, cssClass;

        (this.selection as any).select2data = data;

        Fluent.empty(container);
        if (data !== null) {
            formatted = this.opts.formatSelection(data, container, this.opts.escapeMarkup);
        }
        if (formatted !== undefined) {
            if (formatted instanceof Node)
                container.appendChild(formatted);
            else
                container.innerHTML = formatted;
        }
        cssClass = this.opts.formatSelectionCssClass(data, container);
        if (cssClass !== undefined) {
            container?.classList.add(cssClass);
        }

        this.selection.classList.remove("select2-default");

        if (this.opts.allowClear && this.getPlaceholder() !== undefined) {
            this.container?.classList.add("select2-allowclear");
        }
    }

    override val(val?: string, triggerChange?: boolean): string {
        var triggerChange = false,
            data = null,
            self = this,
            oldData = this.data();

        if (arguments.length === 0) {
            return this.opts.element.value;
        }

        if (arguments.length > 1) {
            triggerChange = arguments[1];
        }

        if (this.select) {
            this.select.value = val ?? "";
            var selected = this.select.querySelector<HTMLOptionElement>("option:checked");
            if (selected)
                data = self.optionToData(selected);
            this.updateSelection(data);
            this.setPlaceholder();
            if (triggerChange) {
                this.triggerChange({ added: data, removed: oldData });
            }
        } else {
            // val is an id. !val is true for [undefined,null,'',0] - 0 is legal
            if (!val && val as any !== 0) {
                this.clear(triggerChange);
                return;
            }
            if (this.opts.initSelection === undefined) {
                throw new Error("cannot call val() if initSelection() is not defined");
            }
            this.opts.element.value = val;
            this.opts.initSelection(this.opts.element, function (data) {
                self.opts.element.value = !data ? "" : self.id(data);
                self.updateSelection(data);
                self.setPlaceholder();
                if (triggerChange) {
                    self.triggerChange({ added: data, removed: oldData });
                }
            });
        }
    }

    protected clearSearch() {
        this.search.value = "";
        this.focusser.value = "";
    }

    data(value?: Select2Item): (void | Select2Item) {
        var data,
            triggerChange = false;

        if (arguments.length === 0) {
            data = (this.selection as any)?.select2data;
            if (data == undefined) data = null;
            return data;
        } else {
            if (arguments.length > 1) {
                triggerChange = arguments[1];
            }
            if (!value) {
                this.clear(triggerChange);
            } else {
                data = this.data();
                this.opts.element.value = !value ? "" : this.id(value);
                this.updateSelection(value);
                if (triggerChange) {
                    this.triggerChange({ added: value, removed: data });
                }
            }
        }
    }
}

class MultiSelect2 extends AbstractSelect2 {

    protected createContainer() {
        var container = document.createElement("div");
        container.classList.add("select2-container", "select2-container-multi");
        container.innerHTML = (
            "<ul class='select2-choices'>" + 
                "<li class='select2-search-field'>" + 
                    "<label for='' class='select2-offscreen'></label>" +
                    "<input type='text' autocomplete='off' autocorrect='off' autocapitalize='off' spellcheck='false' class='select2-input'>" +
                "</li>" + 
            "</ul>" + 
            "<div class='select2-drop select2-drop-multi select2-display-none'>" + 
                "<ul class='select2-results'></ul>" + 
            "</div>");
        return container;
    }

    protected prepareOpts(opts: Select2Options) {
        opts = super.prepareOpts(opts);
        var self = this;

        // TODO validate placeholder is a string if specified

        if (opts.element.tagName.toLowerCase() === "select") {
            // install the selection initializer
            opts.initSelection = function (element, callback) {

                var data: Select2Item[] = [];

                element.querySelectorAll<HTMLOptionElement>("option:checked:not(:disabled)").forEach(elm => {
                    data.push(self.optionToData(elm));
                });
                callback(data);
            };
        } else if ("data" in opts) {
            // install default initSelection when applied to hidden input and data is local
            opts.initSelection = opts.initSelection || function (element: HTMLInputElement, callback) {
                var ids = splitVal(element.value, opts.separator);
                //search in data by array of ids, storing matching items in a list
                var matches: Select2Item[] = [];
                opts.query({
                    matcher: function (term, text, el) {
                        var is_match = ids.some(id => equal(id, opts.id(el)));
                        if (is_match) {
                            matches.push(el);
                        }
                        return is_match;
                    },
                    callback: typeof callback !== "function" ? () => {} : function () {
                        // reorder matches based on the order they appear in the ids array because right now
                        // they are in the order in which they appear in data array
                        var ordered = [];
                        for (var i = 0; i < ids.length; i++) {
                            var id = ids[i];
                            for (var j = 0; j < matches.length; j++) {
                                var match = matches[j];
                                if (equal(id, opts.id(match))) {
                                    ordered.push(match);
                                    matches.splice(j, 1);
                                    break;
                                }
                            }
                        }
                        callback(ordered);
                    }
                });
            };
        }

        return opts;
    }

    protected selectChoice(choice: HTMLElement) {

        var selected = this.container.querySelector(".select2-search-choice-focus");
        if (selected && choice && choice === selected) {

        } else {
            if (selected) {
                Fluent.trigger(this.opts.element, "choice-deselected", { choice: selected });
                selected.classList.remove("select2-search-choice-focus");
            }
            if (choice) {
                this.close();
                choice.classList.add("select2-search-choice-focus");
                Fluent.trigger(this.opts.element, "choice-selected", { choice });
            }
        }
    }

    override destroy() {
        document.querySelector("label[for='" + this.search.getAttribute('id') + "']")?.setAttribute(
            'for', this.opts.element.getAttribute("id"));

        super.destroy();

        cleanupJQueryElements.call(this,
            "searchContainer",
            "selection"
        );
    }

    private searchContainer: HTMLElement;
    private keydowns: number;

    protected override initContainer() {

        var selector = ".select2-choices";

        this.searchContainer = this.container.querySelector(".select2-search-field");
        var selection: HTMLElement;
        this.selection = selection = this.container.querySelector(selector);

        var _this = this;
        Fluent.on(this.selection, "click", ".select2-search-choice:not(.select2-locked)", (e: Event) => {
            //killEvent(e);
            _this.search.focus();
            _this.selectChoice((e.target as HTMLElement).closest('.select2-search-choice') as HTMLElement);
        });

        // rewrite labels from original element to focusser
        this.search.setAttribute("id", "s2id_autogen" + nextUid());

        this.search.previousElementSibling.textContent = document.querySelector("label[for='" + this.opts.element.getAttribute("id") + "']")?.textContent;
        this.search.previousElementSibling.setAttribute("for", this.search.getAttribute('id'));

        ["input", "paste"].forEach(ev =>  Fluent.on(this.search, ev, () => {
            if (this.search.getAttribute('placeholder') && this.search.value.length === 0) return;
            if (!this.isInterfaceEnabled()) return;
            if (!this.opened()) {
                this.open();
            }
        }));

        this.search.setAttribute("tabindex", this.elementTabIndex);

        this.keydowns = 0;
        Fluent.on(this.search, "keydown", e => {
            if (!this.isInterfaceEnabled()) return;

            ++this.keydowns;
            var selected = selection.querySelector<HTMLElement>(".select2-search-choice-focus");
            var prev = Fluent(selected).prevSibling(".select2-search-choice:not(.select2-locked)").getNode();
            var next = Fluent(selected).nextSibling(".select2-search-choice:not(.select2-locked)").getNode();
            var pos = getCursorInfo(this.search);

            if (selected &&
                (e.which == KEY.LEFT || e.which == KEY.RIGHT || e.which == KEY.BACKSPACE || e.which == KEY.DELETE || e.which == KEY.ENTER)) {
                var selectedChoice = selected;
                if (e.which == KEY.LEFT && prev) {
                    selectedChoice = prev;
                }
                else if (e.which == KEY.RIGHT) {
                    selectedChoice = next ? next : null;
                }
                else if (e.which === KEY.BACKSPACE) {
                    if (this.unselect(selected)) {
                        this.search.style.width = "10px";
                        selectedChoice = prev ? prev : next;
                    }
                } else if (e.which == KEY.DELETE) {
                    if (this.unselect(selected)) {
                        this.search.style.width = "10px";
                        selectedChoice = next ? next : null;
                    }
                } else if (e.which == KEY.ENTER) {
                    selectedChoice = null;
                }

                this.selectChoice(selectedChoice);
                killEvent(e);
                if (!selectedChoice || !selectedChoice) {
                    this.open();
                }
                return;
            } else if (((e.which === KEY.BACKSPACE && this.keydowns == 1)
                || e.which == KEY.LEFT) && (pos.offset == 0 && !pos.length)) {

                this.selectChoice(Array.from(selection.querySelectorAll<HTMLElement>(".select2-search-choice:not(.select2-locked)")).pop());
                killEvent(e);
                return;
            } else {
                this.selectChoice(null);
            }

            if (this.opened()) {
                switch (e.which) {
                    case KEY.UP:
                    case KEY.DOWN:
                        this.moveHighlight((e.which === KEY.UP) ? -1 : 1);
                        killEvent(e);
                        return;
                    case KEY.ENTER:
                        this.selectHighlighted();
                        killEvent(e);
                        return;
                    case KEY.TAB:
                        this.selectHighlighted({ noFocus: true });
                        this.close();
                        return;
                    case KEY.ESC:
                        this.cancel(e);
                        killEvent(e);
                        return;
                }
            }

            if (e.which === KEY.TAB || KEY.isControl(e) || KEY.isFunctionKey(e)
                || e.which === KEY.BACKSPACE || e.which === KEY.ESC) {
                return;
            }

            if (e.which === KEY.ENTER) {
                if (this.opts.openOnEnter === false) {
                    return;
                } else if (e.altKey || e.ctrlKey || e.shiftKey || e.metaKey) {
                    return;
                }
            }

            this.open();

            if (e.which === KEY.PAGE_UP || e.which === KEY.PAGE_DOWN) {
                // prevent the page from scrolling
                killEvent(e);
            }

            if (e.which === KEY.ENTER) {
                // prevent form from being submitted
                killEvent(e);
            }

        });

        Fluent.on(this.search, "keyup", e => {
            this.keydowns = 0;
            this.resizeSearch();
        });

        Fluent.on(this.search, "blur", e => {
            this.container.classList.remove("select2-container-active");
            this.search.classList.remove("select2-focused");
            this.selectChoice(null);
            if (!this.opened()) this.clearSearch();
            e.stopImmediatePropagation();
            Fluent.trigger(this.opts.element, "select2-blur");
        });

        Fluent.on(this.container, "click", selector, (e: Event) => {
            if (!this.isInterfaceEnabled()) return;
            if ((e.target as HTMLElement)?.closest(".select2-search-choice")) {
                // clicked inside a select2 search choice, do not open
                return;
            }
            this.selectChoice(null);
            this.clearPlaceholder();
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.open();
            this.focusSearch();
            e.preventDefault();
        });

        Fluent.on(this.container, "focus", selector, () => {
            if (!this.isInterfaceEnabled()) return;
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
            this.dropdown.classList.add("select2-drop-active");
            this.clearPlaceholder();
        });

        this.initContainerWidth();
        this.opts.element.classList.add("select2-offscreen");

        // set the placeholder if necessary
        this.clearSearch();
    }

    protected override enableInterface() {
        var result = super.enableInterface();
        if (result) {
            this.search.disabled = !this.isInterfaceEnabled();
        }
        return result;
    }

    protected initSelection() {
        if (this.opts.element.value === "" && this.opts.element.textContent === "") {
            this.updateSelection([]);
            this.close();
            // set the placeholder if necessary
            this.clearSearch();
        }
        if (this.select || this.opts.element.value !== "") {
            var self = this;
            this.opts.initSelection.call(null, this.opts.element, function (data: Select2Item[]) {
                if (!self.container)
                    return;
                if (data !== undefined && data !== null) {
                    self.updateSelection(data);
                    self.close();
                    // set the placeholder if necessary
                    self.clearSearch();
                }
            });
        }
    }

    protected clearSearch() {
        var placeholder = this.getPlaceholder(),
            maxWidth = this.getMaxSearchWidth();

        if (placeholder !== undefined && this.getVal().length === 0 && !this.search.classList.contains("select2-focused")) {
            this.search.value = placeholder ?? "";
            this.search.classList.add("select2-default");
            // stretch the search box to full width of the container so as much of the placeholder is visible as possible
            // we could call this.resizeSearch(), but we do not because that requires a sizer and we do not want to create one so early because of a firefox bug, see #944
            this.search.style.width = maxWidth > 0 ? (maxWidth + "px") : getComputedStyle(this.container).width;
        } else {
            this.search.value = "";
            this.search.style.width = "10px";
        }
    }

    protected clearPlaceholder() {
        if (this.search.classList.contains("select2-default")) {
            this.search.value = "";
            this.search.classList.remove("select2-default");
        }
    }

    protected opening() {
        this.clearPlaceholder(); // should be done before super so placeholder is not used to search
        this.resizeSearch();

        super.opening();

        this.focusSearch();

        // initializes search's value with nextSearchTerm (if defined by user)
        // ignore nextSearchTerm if the dropdown is opened by the user pressing a letter
        if (this.search.value === "") {
            if (this.nextSearchTerm != undefined) {
                this.search.value = this.nextSearchTerm ?? "";
                this.search.select();
            }
        }

        this.updateResults(true);
        if (this.opts.shouldFocusInput(this)) {
            this.search.focus();
        }
        Fluent.trigger(this.opts.element, "select2-open");
    }

    override close() {
        if (!this.opened()) return;
        super.close();
    }

    override focus() {
        this.close();
        this.search.focus();
    }

    override isFocused() {
        return this.search.classList.contains("select2-focused");
    }

    protected updateSelection(data: Select2Item[]) {
        var ids: string[] = [], filtered: Select2Item[] = [], self = this;

        // filter out duplicates
        data.forEach(function (x) {
            if (indexOf(self.id(x), ids) < 0) {
                ids.push(self.id(x));
                filtered.push(x);
            }
        });
        data = filtered;

        this.selection.querySelectorAll(".select2-search-choice").forEach(el => Fluent.remove(el));
        data.forEach(function (x) {
            self.addSelectedChoice(x);
        });
        self.postprocessResults();
    }

    protected tokenize(): string {
        var input = this.search.value;
        input = this.opts.tokenizer.call(this, input, this.data(), this.onSelect.bind(this), this.opts);
        if (input != null && input != undefined) {
            this.search.value = input ?? "";
            if (input.length > 0) {
                this.open();
            }
        }
        return undefined;
    }

    protected override onSelect(data: Select2Item, options: any) {

        if (!this.triggerSelect(data) || data.text === "") { return; }

        this.addSelectedChoice(data);

        Fluent.trigger(this.opts.element, "selected", { val: this.id(data), choice: data });

        // keep track of the search's value before it gets cleared
        this.nextSearchTerm = this.opts.nextSearchTerm(data, this.search.value);

        this.clearSearch();
        this.updateResults();

        if (this.select || !this.opts.closeOnSelect) this.postprocessResults(undefined, false, this.opts.closeOnSelect === true);

        if (this.opts.closeOnSelect) {
            this.close();
            this.search.style.width = "10px";
        } else {
            if (this.countSelectableResults() > 0) {
                this.search.style.width = "10px";
                this.resizeSearch();
                if (this.getMaximumSelectionSize() > 0 && this.val().length >= this.getMaximumSelectionSize()) {
                    // if we reached max selection size repaint the results so choices
                    // are replaced with the max selection reached message
                    this.updateResults(true);
                } else {
                    // initializes search's value with nextSearchTerm and update search result
                    if (this.nextSearchTerm != undefined) {
                        this.search.value = this.nextSearchTerm ?? "";
                        this.updateResults();
                        this.search.select();
                    }
                }
                this.positionDropdown();
            } else {
                // if nothing left to select close
                this.close();
                this.search.style.width = "10px";
            }
        }

        // since its not possible to select an element that has already been
        // added we do not need to check if this is a new element before firing change
        this.triggerChange({ added: data });

        if (!options || !options.noFocus)
            this.focusSearch();
    }

    protected cancel(e?: Event) {
        this.close();
        this.focusSearch();
    }

    protected addSelectedChoice(data: Select2Item) {
        var enableChoice = !data.locked;
        var choice = document.createElement("li"),
            id = this.id(data),
            val = this.getVal(),
            formatted,
            cssClass;

        choice.classList.add("select2-search-choice");
        choice.appendChild(document.createElement("div"));
        if (enableChoice) {
            var a = choice.appendChild(document.createElement("a"));
            a.classList.add("select2-search-choice-close");
            a.setAttribute("href", "#");
            a.setAttribute("tabindex", "-1");
        }
        else {
            choice.classList.add("select2-locked");
        }

        var div = choice.querySelector("div");
        formatted = this.opts.formatSelection(data, div, this.opts.escapeMarkup);
        if (formatted != undefined) {
            if (formatted instanceof Node)
                div.appendChild(formatted);
            else
                div.innerHTML = formatted ?? "";
        }
        cssClass = this.opts.formatSelectionCssClass(data, div);
        if (cssClass != undefined) {
            Fluent.addClass(choice, cssClass);
        }

        if (enableChoice) {
            var close = choice.querySelector(".select2-search-choice-close");
            Fluent.on(close, "mousedown", killEvent);
            ["click", "dblclick"].forEach(ev => Fluent.on(close, ev, (e) => {
                if (!this.isInterfaceEnabled()) return;

                this.unselect(e.target as HTMLElement);
                this.selection.querySelector(".select2-search-choice-focus")?.classList.remove("select2-search-choice-focus");
                killEvent(e);
                this.close();
                this.focusSearch();
            }));
            
            Fluent.on(close, "focus", () => {
                if (!this.isInterfaceEnabled()) return;
                this.container.classList.add("select2-container-active");
                this.dropdown.classList.add("select2-drop-active");
            });
        }

        (choice as any).select2data = data;
        Fluent(choice).insertBefore(this.searchContainer);

        val.push(id);
        this.setVal(val);
    }

    protected unselect(selected: HTMLElement) {
        var val = this.getVal(),
            data,
            index;
        selected = selected.closest(".select2-search-choice");

        if (!selected) {
            throw "Invalid argument: " + selected + ". Must be .select2-search-choice";
        }

        data = (selected as any).select2data;

        if (!data) {
            // prevent a race condition when the 'x' is clicked really fast repeatedly the event can be queued
            // and invoked on an element already removed
            return;
        }

        var evt = Fluent.trigger(this.opts.element, "select2-removing", { val: this.id(data), choice: data });

        if (Fluent.isDefaultPrevented(evt)) {
            return false;
        }

        while ((index = indexOf(this.id(data), val)) >= 0) {
            val.splice(index, 1);
            this.setVal(val);
            if (this.select) this.postprocessResults();
        }

        selected.remove();

        Fluent.trigger(this.opts.element, "select2-removed", { val: this.id(data), choice: data });
        this.triggerChange({ removed: data });

        return true;
    }

    protected override postprocessResults(data?: Select2Result, initial?: boolean, noHighlightUpdate?: boolean) {
        var val = this.getVal(),
            choices = this.results.querySelectorAll(".select2-result"),
            compound = this.results.querySelectorAll(".select2-result-with-children"),
            self = this;

        choices.forEach(function (choice) {
            var id = self.id((choice as any).select2data);
            if (indexOf(id, val) >= 0) {
                choice.classList.add("select2-selected");
                // mark all children of the selected parent as selected
                choice.querySelectorAll(".select2-result-selectable").forEach(x => x.classList.add("select2-selected"));
            }
        });

        compound.forEach(function (choice) {
            // hide an optgroup if it doesn't have any selectable children
            if (!choice.matches('.select2-result-selectable')
                && !choice.querySelector(".select2-result-selectable:not(.select2-selected)")) {
                choice.classList.add("select2-selected");
            }
        });

        if (this.highlight() == -1 && noHighlightUpdate !== false) {
            self.highlight(0);
        }

        //If all results are chosen render formatNoMatches
        if (!this.opts.createSearchChoice && !Array.from(choices).some(x => x.matches('.select2-result:not(.select2-selected)'))) {
            if (!data || data && !data.more && !this.results.querySelector(".select2-no-results")) {
                if (checkFormatter(self.opts.formatNoMatches, "formatNoMatches")) {
                    var noResults = evaluate(self.opts.formatNoMatches, self.opts.element, self.search.value);
                    var li = document.createElement("li");
                    li.classList.add("select2-no-results");
                    if (noResults instanceof Node)
                        li.appendChild(noResults);
                    else
                        li.innerHTML = noResults ?? "";
                }
            }
        }

    }

    protected getMaxSearchWidth() {
        return this.selection.getBoundingClientRect().width - getSideBorderPadding(this.search);
    }

    protected resizeSearch() {
        var minimumWidth, left, maxWidth, containerLeft, searchWidth,
            sideBorderPadding = getSideBorderPadding(this.search);

        minimumWidth = measureTextWidth(this.search) + 10;

        left = getOffset(this.search).left;

        maxWidth = this.selection.getBoundingClientRect().width;
        containerLeft = getOffset(this.selection).left;

        searchWidth = maxWidth - (left - containerLeft) - sideBorderPadding;

        if (searchWidth < minimumWidth) {
            searchWidth = maxWidth - sideBorderPadding;
        }

        if (searchWidth < 40) {
            searchWidth = maxWidth - sideBorderPadding;
        }

        if (searchWidth <= 0) {
            searchWidth = minimumWidth;
        }

        this.search.style.width = (Math.floor(searchWidth)) + "px";
    }

    protected getVal(): string[] {
        var val;
        if (this.select) {
            val = Array.from(this.select.selectedOptions).map(x => x.value);
            return val === null ? [] : val;
        } else {
            val = this.opts.element.value;
            return splitVal(val, this.opts.separator);
        }
    }

    protected setVal(val: string[]) {
        if (this.select) {
            val ??= [];
            var opt = this.select.options;
            for (var i = 0; i < opt.length; i++) {
                var o = opt.item(i);
                o.selected = val.includes(o.value);
            }
        } else {
            var unique = val || [];
            unique = unique.filter((x, i) => unique.indexOf(x) === i);
            this.opts.element.value = (unique.length === 0 ? "" : unique.join(this.opts.separator));
        }
    }

    protected buildChangeDetails(old: Select2Item[], current: Select2Item[]) {
        var current = current.slice(0),
            old = old.slice(0);

        // remove intersection from each array
        for (var i = 0; i < current.length; i++) {
            for (var j = 0; j < old.length; j++) {
                if (equal(this.opts.id(current[i]), this.opts.id(old[j]))) {
                    current.splice(i, 1);
                    if (i > 0) {
                        i--;
                    }
                    old.splice(j, 1);
                    j--;
                }
            }
        }

        return { added: current, removed: old };
    }

    val(val?: string[], triggerChange?: boolean) {
        var self = this;

        if (arguments.length === 0) {
            return this.getVal();
        }

        var oldData = this.data();
        if (!oldData) oldData = [];

        // val is an id. !val is true for [undefined,null,'',0] - 0 is legal
        if (!val && (val as any) !== 0) {
            this.opts.element.value = "";
            this.updateSelection([]);
            this.clearSearch();
            if (triggerChange) {
                this.triggerChange({ added: this.data(), removed: oldData });
            }
            return;
        }

        // val is a list of ids
        this.setVal(val);

        if (this.select) {
            this.opts.initSelection(this.select, this.updateSelection.bind(this));
            if (triggerChange) {
                this.triggerChange(this.buildChangeDetails(oldData, this.data()));
            }
        } else {
            if (this.opts.initSelection === undefined) {
                throw new Error("val() cannot be called if initSelection() is not defined");
            }

            this.opts.initSelection(this.opts.element, function (data) {
                var ids = data.map(self.id);
                self.setVal(ids);
                self.updateSelection(data);
                self.clearSearch();
                if (triggerChange) {
                    self.triggerChange(self.buildChangeDetails(oldData, self.data()));
                }
            });
        }
        this.clearSearch();
    }

    protected onSortStart() {
        if (this.select) {
            throw new Error("Sorting of elements is not supported when attached to <select>. Attach to <input type='hidden'/> instead.");
        }

        // collapse search field into 0 width so its container can be collapsed as well
        this.search.style.width = "0px";
        // hide the container
        this.searchContainer.style.display = "none";
    }

    protected onSortEnd() {

        var val: string[] = [], self = this;

        // show search and move it to the end of the list
        this.searchContainer.style.display = "";
        // make sure the search container is the last item in the list
        Fluent(this.searchContainer.parentElement).append(this.searchContainer);
        // since we collapsed the width in dragStarted, we resize it here
        this.resizeSearch();

        // update selection
        this.selection.querySelectorAll(".select2-search-choice").forEach(el => {
            val.push(self.opts.id((el as any).select2data));
        });
        this.setVal(val);
        this.triggerChange();
    }

    data(): Select2Item[];
    data(values: Select2Item[], triggerChange?: boolean): void;
    data(values?: Select2Item[], triggerChange?: boolean): Select2Item[] | void {
        var self = this, ids, old;
        if (arguments.length === 0) {
            var result: Select2Item[] = [];
            this.selection
                .querySelectorAll(":scope > .select2-search-choice")
                .forEach(x => result.push((x as any).select2data));
            return result;
        } else {
            old = this.data();
            if (!values) { values = []; }
            ids = values.map(self.opts.id);
            this.setVal(ids);
            this.updateSelection(values);
            this.clearSearch();
            if (triggerChange) {
                this.triggerChange(this.buildChangeDetails(old, this.data()));
            }
        }
    }
}


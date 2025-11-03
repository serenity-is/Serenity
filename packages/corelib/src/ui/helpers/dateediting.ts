import { addDisposingListener } from "@serenity-is/domwise";
import { Culture, formatDate, getjQuery, parseDate } from "../../base";

export function dateInputChangeHandler(e: Event) {
    if (Culture.dateOrder !== 'dmy' && Culture.dateOrder !== 'mdy')
        return;

    var input = e.target as HTMLInputElement;
    if (input?.getAttribute("type") === "date")
        return;

    var val = input.value ?? '';
    if (val.length >= 6 && val.length <= 8 && /^[0-9]*$/g.test(val)) {
        input.value = val.substring(0, 2) + Culture.dateSeparator + val.substring(2, 4) + Culture.dateSeparator + val.substring(4);
        input.value = val.substring(0, 2) + Culture.dateSeparator + val.substring(2, 4) + Culture.dateSeparator + val.substring(4);
    }

    val = input.value ?? '';
    if (val.length >= 5) {
        var d = parseDate(val);
        if (d && !isNaN(d.valueOf()))
            input.value = formatDate(d, null) ?? '';
    }
}

function isDigit(c: string): boolean {
    return c >= '0' && c <= '9';
}

export function dateInputKeyupHandler(e: KeyboardEvent) {

    var input = e.target as HTMLInputElement;
    if (input?.getAttribute("type") == "date") {
        // for browser date editors, format might not match culture setting
        return;
    }

    if (input.getAttribute('readonly') != null || input.getAttribute("disabled") != null)
        return;

    var val: string = input.value ?? '';
    if (!!(val.length === 0 || input.selectionEnd !== val.length))
        return;

    if (val.indexOf(Culture.dateSeparator + Culture.dateSeparator) >= 0) {
        // remove double separator
        input.value = val.split(Culture.dateSeparator + Culture.dateSeparator).join(Culture.dateSeparator);
        return;
    }

    if (e.key === 'slash' || e.key === 'NumpadDivide' || e.key === '/') {

        if (val[val.length - 1] !== Culture.dateSeparator)
            return;

        switch (val.length) {
            case 2: {
                if (isDigit(val[0])) {
                    // 4/ -> 04/
                    val = '0' + val;
                    break;
                }
                else {
                    return;
                }
            }

            case 4: {
                if (isDigit(val[0]) &&
                    isDigit(val[2]) &&
                    val[1] == Culture.dateSeparator) {
                    // 4/5/ -> 04/05/
                    val = '0' + val[0] + Culture.dateSeparator + '0' +
                        val[2] + Culture.dateSeparator;
                    break;
                }
                else {
                    return;
                }
            }

            case 5: {
                if (isDigit(val[0]) &&
                    isDigit(val[2]) &&
                    isDigit(val[3]) &&
                    val[1] === Culture.dateSeparator) {
                    // 4/12/ -> 04/12/
                    val = '0' + val;
                    break;
                }
                else if (isDigit(val[0]) &&
                    isDigit(val[1]) &&
                    isDigit(val[3]) &&
                    // 12/4/ -> 12/04/
                    val[2] === Culture.dateSeparator) {
                    val = val[0] + val[1] +
                        Culture.dateSeparator + '0' + val[3] + Culture.dateSeparator;
                    break;
                }
                else {
                    break;
                }
            }
            default: {
                return;
            }
        }
        input.value = val ?? '';
    }

    if (Culture.dateOrder !== 'dmy')
        return;    

    if (!(val.length < 6 && (e.key >= '0' && e.key <= '9' || e.key >= 'Numpad0' && e.key <= 'Numpad9') &&
        isDigit(val[val.length - 1])))
        return;

    switch (val.length) {
        case 1:
            if (val.charCodeAt(0) <= 51)
                return;
            val = '0' + val;
            break;

        case 2:
            if (!isDigit(val[0]))
                return;
            break;

        case 3:
            if (!isDigit(val[0]) ||
                val[1] !== Culture.dateSeparator ||
                val.charCodeAt(2) <= 49) {
                return;
            }
            val = '0' + val[0] + Culture.dateSeparator + '0' + val[2];
            break;

        case 4:
            if (val[1] == Culture.dateSeparator) {
                if (!isDigit(val[0]) ||
                    !isDigit(val[2])) {
                    return;
                }

                val = '0' + val;
                break;
            }
            else if (val[2] == Culture.dateSeparator) {
                if (!isDigit(val[0]) ||
                    !isDigit(val[1]) ||
                    val.charCodeAt(3) <= 49) {
                    return;
                }

                val = val[0] + val[1] + Culture.dateSeparator +
                    '0' + val[3];
                break;
            }
            else
                return;
        case 5:
            if (val[2] !== Culture.dateSeparator ||
                !isDigit(val[0]) ||
                !isDigit(val[1]) ||
                !isDigit(val[3])) {
                return;
            }
            break;
        default:
            return;
    }

    input.value = val + Culture.dateSeparator;
}

export function flatPickrTrigger(input: HTMLInputElement): HTMLElement {
    var button = document.createElement("button");
    button.type = "button";
    button.classList.add("ui-datepicker-trigger");
    const listener = () => {
        if (!input.classList.contains('readonly') && input.getAttribute('readonly') == null) {
            (input as any)._flatpickr?.open?.();
            (input as any)._flatpickr?.calendarContainer?.focus?.();
        }
    };
    button.addEventListener("click", listener);
    const removeListener = () => button.removeEventListener("click", listener);
    addDisposingListener(button, removeListener);
    addDisposingListener(input, removeListener);
    return button;
}

export function jQueryDatepickerZIndexWorkaround(input: HTMLInputElement) {
    let $ = getjQuery();
    if (!$)
        return;
    let dialog = input?.closest('.ui-dialog');
    if (!dialog)
        return;
    var dialogIndex = parseInt(getComputedStyle(dialog).zIndex, 10);
    if (dialogIndex == null || isNaN(dialogIndex))
        return;
    setTimeout(() => {
        let widget = $(input).datepicker('widget');
        if (!widget || !widget.length)
            return;
        let zIndex = parseInt(widget.css('z-index'));
        if (!isNaN(zIndex) && zIndex <= dialogIndex)
            widget.css('z-index', dialogIndex + 1);
    }, 0);
}

export function jQueryDatepickerInitialization(): boolean {
    let $ = getjQuery();
    if (!$?.datepicker?.regional?.en)
        return false;

    let order = Culture.dateOrder;
    let s = Culture.dateSeparator;
    let culture = typeof document === "undefined" ? 'en' : (document.documentElement.lang || 'en').toLowerCase();
    if (!$.datepicker.regional[culture]) {
        culture = culture.split('-')[0];
        if (!$.datepicker.regional[culture]) {
            culture = 'en';
        }
    }
    $.datepicker.setDefaults($.datepicker.regional['en']);
    $.datepicker.setDefaults($.datepicker.regional[culture]);
    $.datepicker.setDefaults({
        dateFormat: (order == 'mdy' ? 'mm' + s + 'dd' + s + 'yy' :
            (order == 'ymd' ? 'yy' + s + 'mm' + s + 'dd' :
                'dd' + s + 'mm' + s + 'yy')),
        buttonText: "",
        buttonImage: null,
        buttonImageOnly: false,
        showOn: 'both',
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true
    });

    return true;
}
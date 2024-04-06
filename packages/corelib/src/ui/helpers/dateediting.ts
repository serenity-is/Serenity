import { Culture, formatDate, getjQuery, parseDate } from "../../base";

export function dateInputChangeHandler(e: Event) {
    if (Culture.dateOrder !== 'dmy')
        return;

    var input = e.target as HTMLInputElement;
    if (input?.getAttribute("type") === "date")
        return;

    var val = input.value ?? '';
    if (val.length >= 6 && /^[0-9]*$/g.test(val))
        input.value = val.substring(0, 2) + Culture.dateSeparator + val.substring(2, 2) + Culture.dateSeparator + val.substring(4);

    val = input.value ?? '';
    if (val.length >= 5) {
        var d = parseDate(val);
        if (d && !isNaN(d.valueOf()))
            input.value = formatDate(d, null) ?? '';
    }
}

export function dateInputKeyupHandler(e: KeyboardEvent) {

    if (Culture.dateOrder !== 'dmy')
        return;

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

    if (val.indexOf(Culture.dateSeparator + Culture.dateSeparator) !== -1) {
        input.value = val.split(Culture.dateSeparator + Culture.dateSeparator).join(Culture.dateSeparator);
        return;
    }

    function isNumeric(c: string): boolean {
        return c >= '0' && c <= '9';
    }

    if (e.which === 47 || e.which === 111) {

        if (val.length >= 2 && val.charAt(val.length - 1) === Culture.dateSeparator &&
            val.charAt(val.length - 2) === Culture.dateSeparator) {
            input.value = val.substring(0, val.length - 1);
            return;
        }

        if (val.charAt(val.length - 1) !== Culture.dateSeparator)
            return;

        switch (val.length) {
            case 2: {
                if (isNumeric(val.charAt(0))) {
                    val = '0' + val;
                    break;
                }
                else {
                    return;
                }
            }

            case 4: {
                if (isNumeric(val.charAt(0)) &&
                    isNumeric(val.charAt(2)) &&
                    val.charAt(1) == Culture.dateSeparator) {
                    val = '0' + val.charAt(0) + Culture.dateSeparator + '0' +
                        val.charAt(2) + Culture.dateSeparator;
                    break;
                }
                else {
                    return;
                }
            }

            case 5: {
                if (isNumeric(val.charAt(0)) &&
                    isNumeric(val.charAt(2)) &&
                    isNumeric(val.charAt(3)) &&
                    val.charAt(1) === Culture.dateSeparator) {
                    val = '0' + val;
                    break;
                }
                else if (isNumeric(val.charAt(0)) &&
                    isNumeric(val.charAt(1)) &&
                    isNumeric(val.charAt(3)) &&
                    val.charAt(2) === Culture.dateSeparator) {
                    val = val.charAt(0) + val.charAt(1) +
                        Culture.dateSeparator + '0' + val.charAt(3) + Culture.dateSeparator;
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

    if (!(val.length < 6 && (e.which >= 48 && e.which <= 57 || e.which >= 96 && e.which <= 105) &&
        isNumeric(val.charAt(val.length - 1))))
        return;

    switch (val.length) {
        case 1:
            if (val.charCodeAt(0) <= 51)
                return;
            val = '0' + val;
            break;

        case 2:
            if (!isNumeric(val.charAt(0)))
                return;
            break;

        case 3:
            if (!isNumeric(val.charAt(0)) ||
                val.charAt(1) !== Culture.dateSeparator ||
                val.charCodeAt(2) <= 49) {
                return;
            }
            val = '0' + val.charAt(0) + Culture.dateSeparator + '0' + val.charAt(2);
            break;

        case 4:
            if (val.charAt(1) == Culture.dateSeparator) {
                if (!isNumeric(val.charAt(0)) ||
                    !isNumeric(val.charAt(2))) {
                    return;
                }

                val = '0' + val;
                break;
            }
            else if (val.charAt(2) == Culture.dateSeparator) {
                if (!isNumeric(val.charAt(0)) ||
                    !isNumeric(val.charAt(1)) ||
                    val.charCodeAt(3) <= 49) {
                    return;
                }

                val = val.charAt(0) + val.charAt(1) + Culture.dateSeparator +
                    '0' + val.charAt(3);
                break;
            }
            else
                return;
        case 5:
            if (val.charAt(2) !== Culture.dateSeparator ||
                !isNumeric(val.charAt(0)) ||
                !isNumeric(val.charAt(1)) ||
                !isNumeric(val.charAt(3))) {
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
    button.addEventListener("click", () => {
        if (!input.classList.contains('readonly') && input.getAttribute('readonly') == null) {
            (input as any)._flatpickr?.open?.();
            (input as any)._flatpickr?.calendarContainer?.focus?.();
        }
    });
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
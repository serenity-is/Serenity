
import { Culture, formatDate, parseDate } from "@serenity-is/base";

export let datePickerIconSvg = '<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 17 17"><g></g><path d="M14 2v-1h-3v1h-5v-1h-3v1h-3v15h17v-15h-3zM12 2h1v2h-1v-2zM4 2h1v2h-1v-2zM16 16h-15v-8.921h15v8.921zM1 6.079v-3.079h2v2h3v-2h5v2h3v-2h2v3.079h-15z" fill="currentColor"></path></svg>';

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
            input.value = formatDate(d, null);
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
        input.value = val;
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

export function flatPickrOptions(onChange: () => void) {
    return {
        clickOpens: true,
        allowInput: true,
        dateFormat: Culture.dateOrder.split('').join(Culture.dateSeparator).replace('y', 'Y'),
        onChange: onChange
    }
}

export function flatPickrTrigger(input: HTMLInputElement): HTMLElement {
    var i = document.createElement("i");
    i.classList.add("ui-datepicker-trigger");
    i.innerHTML = datePickerIconSvg;
    i.addEventListener("click", () => {
        if (!input.classList.contains('readonly'))
            (input as any)._flatpickr?.open?.();
    });
    return i;
}

export function jQueryDatepickerZIndexWorkaround(input: HTMLInputElement, jQuery: any) {
    let dialog = input?.closest('.ui-dialog');
    if (!dialog)
        return; 
    var dialogIndex = parseInt(getComputedStyle(dialog).zIndex, 10);
    if (dialogIndex == null || isNaN(dialogIndex))
        return;
    setTimeout(() => {
        let widget = jQuery(input).datepicker('widget');
        if (!widget || !widget.length)
            return;
        let zIndex = parseInt(widget.css('z-index'));
        if (!isNaN(zIndex) && zIndex <= dialogIndex)
            widget.css('z-index', dialogIndex + 1);
    }, 0);
}

export function jQueryDatepickerInitialization(jQuery: any): boolean {
    if (!jQuery?.datepicker?.regional?.en)
        return false;

    let order = Culture.dateOrder;
    let s = Culture.dateSeparator;
    let culture = typeof document === "undefined" ? 'en' : (document.documentElement.lang || 'en').toLowerCase();
    if (!jQuery.datepicker.regional[culture]) {
        culture = culture.split('-')[0];
        if (jQuery.datepicker.regional[culture]) {
            culture = 'en';
        }
    }
    jQuery.datepicker.setDefaults(jQuery.datepicker.regional['en']);
    jQuery.datepicker.setDefaults(jQuery.datepicker.regional[culture]);
    jQuery.datepicker.setDefaults({
        dateFormat: (order == 'mdy' ? 'mm' + s + 'dd' + s + 'yy' :
            (order == 'ymd' ? 'yy' + s + 'mm' + s + 'dd' :
                'dd' + s + 'mm' + s + 'yy')),
        buttonImage: 'data:image/svg+xml,' + encodeURIComponent(datePickerIconSvg),
        buttonImageOnly: true,
        showOn: 'both',
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true
    });

    if (jQuery.ui && jQuery.ui.version <= '1.12.1') {
        jQuery.datepicker.setDefaults({
            buttonImage: null,
            buttonImageOnly: false,
            buttonText: '<i class="fa fa-calendar"></i>'
        });
    }

    return true;
}
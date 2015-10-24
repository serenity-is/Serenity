var Q$Externals;
(function (Q$Externals) {
    function zeroPad(n, digits) {
        var s = n.toString();
        while (s.length < digits)
            s = "0" + digits;
        return s;
    }
    Q$Externals.zeroPad = zeroPad;
    function formatDayHourAndMin(n) {
        if (n === 0)
            return '0';
        else if (!n)
            return '';
        var days = Math.floor(n / 24 / 60);
        var txt = "";
        if (days > 0) {
            txt += days.toString();
        }
        var mins = zeroPad(Math.floor((n % (24 * 60)) / (60)), 2) + ':' + zeroPad(n % 60, 2);
        if (mins != '00:00') {
            if (days > 0)
                txt += ".";
            txt += mins;
        }
        return txt;
    }
    Q$Externals.formatDayHourAndMin = formatDayHourAndMin;
    ;
    function formatISODateTimeUTC(d) {
        if (d == null)
            return "";
        var zeropad = function (num) { return ((num < 10) ? '0' : '') + num; };
        var str = d.getUTCFullYear() + "-" +
            zeropad(d.getUTCMonth() + 1) + "-" +
            zeropad(d.getUTCDate()) + "T" +
            zeropad(d.getUTCHours()) + ":" +
            zeropad(d.getUTCMinutes());
        var secs = Number(d.getUTCSeconds() + "." +
            ((d.getUTCMilliseconds() < 100) ? '0' : '') +
            zeropad(d.getUTCMilliseconds()));
        str += ":" + zeropad(secs) + "Z";
        return str;
    }
    Q$Externals.formatISODateTimeUTC = formatISODateTimeUTC;
    ;
    function formatNumber(n, fmt, dec, grp) {
        var neg = '-';
        if (isNaN(n)) {
            return null;
        }
        var r = "";
        if (fmt.indexOf(".") > -1) {
            var dp = dec;
            var df = fmt.substring(fmt.lastIndexOf(".") + 1);
            n = roundNumber(n, df.length);
            var dv = n % 1;
            var ds = new String(dv.toFixed(df.length));
            ds = ds.substring(ds.lastIndexOf(".") + 1);
            for (var i = 0; i < df.length; i++) {
                if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                    dp += ds.charAt(i);
                    continue;
                }
                else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                    var notParsed = ds.substring(i);
                    if (notParsed.match('[1-9]')) {
                        dp += ds.charAt(i);
                        continue;
                    }
                    else
                        break;
                }
                else if (df.charAt(i) == "0")
                    dp += ds.charAt(i);
                else
                    dp += df.charAt(i);
            }
            r += dp;
        }
        else
            n = Math.round(n);
        var ones = Math.floor(n);
        if (n < 0)
            ones = Math.ceil(n);
        var of = "";
        if (fmt.indexOf(".") == -1)
            of = fmt;
        else
            of = fmt.substring(0, fmt.indexOf("."));
        var op = "";
        if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
            // find how many digits are in the group
            var oneText = new String(Math.abs(ones));
            var gl = 9999;
            if (of.lastIndexOf(",") != -1)
                gl = of.length - of.lastIndexOf(",") - 1;
            var gc = 0;
            for (var i = oneText.length - 1; i > -1; i--) {
                op = oneText.charAt(i) + op;
                gc++;
                if (gc == gl && i != 0) {
                    op = grp + op;
                    gc = 0;
                }
            }
            // account for any pre-data padding
            if (of.length > op.length) {
                var padStart = of.indexOf('0');
                if (padStart != -1) {
                    var padLen = of.length - padStart;
                    // pad to left with 0's or group char
                    var pos = of.length - op.length - 1;
                    while (op.length < padLen) {
                        var pc = of.charAt(pos);
                        // replace with real group char if needed
                        if (pc == ',')
                            pc = grp;
                        op = pc + op;
                        pos--;
                    }
                }
            }
        }
        if (!op && of.indexOf('0', of.length - 1) !== -1)
            op = '0';
        r = op + r;
        if (n < 0)
            r = neg + r;
        if (r.lastIndexOf(dec) == r.length - 1) {
            r = r.substring(0, r.length - 1);
        }
        return r;
    }
    Q$Externals.formatNumber = formatNumber;
    ;
    function roundNumber(n, dec) {
        var power = Math.pow(10, dec || 0);
        var value = (Math.round(n * power) / power).toString();
        // ensure the decimal places are there
        if (dec > 0) {
            var dp = value.indexOf(".");
            if (dp == -1) {
                value += '.';
                dp = 0;
            }
            else {
                dp = value.length - (dp + 1);
            }
            while (dp < dec) {
                value += '0';
                dp++;
            }
        }
        return parseFloat(value);
    }
    ;
    var isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    // -- PARSING ---
    function parseInteger(s) {
        s = Q.trim(s.toString());
        var ts = Q$Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }
    ;
    function parseDate(s, dateOrder) {
        if (!s || !s.length)
            return null;
        var dateVal;
        var dArray;
        var d, m, y;
        dArray = splitDateString(s);
        if (!dArray)
            return false;
        if (dArray.length == 3) {
            dateOrder = dateOrder || Q$Culture.dateOrder;
            switch (dateOrder) {
                case "dmy":
                    d = parseInt(dArray[0], 10);
                    m = parseInt(dArray[1], 10) - 1;
                    y = parseInt(dArray[2], 10);
                    break;
                case "ymd":
                    d = parseInt(dArray[2], 10);
                    m = parseInt(dArray[1], 10) - 1;
                    y = parseInt(dArray[0], 10);
                    break;
                case "mdy":
                default:
                    d = parseInt(dArray[1], 10);
                    m = parseInt(dArray[0], 10) - 1;
                    y = parseInt(dArray[2], 10);
                    break;
            }
            if (isNaN(d) || isNaN(m) || isNaN(y) || d < 1 || d > 31 || m < 0 || m > 11 || y > 9999 || y < 0)
                return false;
            if (y < 100) {
                var fullYear = new Date().getFullYear();
                var shortYearCutoff = (fullYear % 100) + 10;
                y += fullYear - fullYear % 100 + (y <= shortYearCutoff ? 0 : -100);
            }
            try {
                dateVal = new Date(y, m, d);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        else if (dArray.length == 1) {
            try {
                dateVal = new Date(dArray[0]);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        return dateVal;
    }
    Q$Externals.parseDate = parseDate;
    ;
    function parseDecimal(s) {
        s = Q.trim(s.toString());
        var ts = Q$Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Q$Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;
        return parseFloat(s.toString().replace(Q$Culture.decimalSeparator, '.'));
    }
    Q$Externals.parseDecimal = parseDecimal;
    ;
    function splitDateString(s) {
        s = Q.trim(s);
        if (!s.length)
            return;
        if (s.indexOf("/") >= 0)
            return s.split("/");
        else if (s.indexOf(".") >= 0)
            return s.split(".");
        else if (s.indexOf("-") >= 0)
            return s.split("-");
        else if (s.indexOf("\\") >= 0)
            return s.split("\\");
        else
            return [s];
    }
    Q$Externals.splitDateString = splitDateString;
    ;
    function parseISODateTime(s) {
        if (!s || !s.length)
            return null;
        var timestamp = Date.parse(s);
        if (!isNaN(timestamp) && typeof timestamp == "Date")
            return timestamp;
        s = s + "";
        if (typeof (s) != "string" || s.length === 0) {
            return null;
        }
        var res = s.match(isoRegexp);
        if (typeof (res) == "undefined" || res === null) {
            return null;
        }
        var year, month, day, hour, min, sec, msec;
        year = parseInt(res[1], 10);
        if (typeof (res[2]) == "undefined" || res[2] === '') {
            return new Date(year);
        }
        month = parseInt(res[2], 10) - 1;
        day = parseInt(res[3], 10);
        if (typeof (res[4]) == "undefined" || res[4] === '') {
            return new Date(year, month, day);
        }
        hour = parseInt(res[4], 10);
        min = parseInt(res[5], 10);
        sec = (typeof (res[6]) != "undefined" && res[6] !== '') ? parseInt(res[6], 10) : 0;
        if (typeof (res[7]) != "undefined" && res[7] !== '') {
            msec = Math.round(1000.0 * parseFloat("0." + res[7]));
        }
        else {
            msec = 0;
        }
        if ((typeof (res[8]) == "undefined" || res[8] === '') && (typeof (res[9]) == "undefined" || res[9] === '')) {
            return new Date(year, month, day, hour, min, sec, msec);
        }
        var ofs;
        if (typeof (res[9]) != "undefined" && res[9] !== '') {
            ofs = parseInt(res[10], 10) * 3600000;
            if (typeof (res[11]) != "undefined" && res[11] !== '') {
                ofs += parseInt(res[11], 10) * 60000;
            }
            if (res[9] == "-") {
                ofs = -ofs;
            }
        }
        else {
            ofs = 0;
        }
        return new Date(Date.UTC(year, month, day, hour, min, sec, msec) - ofs);
    }
    Q$Externals.parseISODateTime = parseISODateTime;
    ;
    function parseHourAndMin(value) {
        var v = Q.trim(value);
        if (v.length < 4 || v.length > 5)
            return NaN;
        var h, m;
        if (v.charAt(1) == ':') {
            h = parseInteger(v.substr(0, 1));
            m = parseInteger(v.substr(2, 2));
        }
        else {
            if (v.charAt(2) != ':')
                return NaN;
            h = parseInteger(v.substr(0, 2));
            m = parseInteger(v.substr(3, 2));
        }
        if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
            return NaN;
        return h * 60 + m;
    }
    Q$Externals.parseHourAndMin = parseHourAndMin;
    ;
    function parseDayHourAndMin(s) {
        var days;
        var v = Q.trim(s);
        if (!v)
            return NaN;
        var p = v.split('.');
        if (p.length == 0 || p.length > 2)
            return NaN;
        if (p.length == 1) {
            days = parseInteger(p[0]);
            if (!isNaN(days))
                return days * 24 * 60;
            return parseHourAndMin(p[0]);
        }
        else {
            days = parseInteger(p[0]);
            var hm = parseHourAndMin(p[1]);
            if (isNaN(days) || isNaN(hm))
                return NaN;
            return days * 24 * 60 + hm;
        }
    }
    Q$Externals.parseDayHourAndMin = parseDayHourAndMin;
    ;
    function parseQueryString(s) {
        var qs;
        if (arguments.length == 0)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        var result = {};
        var parts = qs.split('&');
        for (var i = 0; i < parts.length; i++) {
            var pair = parts[i].split('=');
            var name = decodeURIComponent(pair[0]);
            result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
        }
        return result;
    }
    Q$Externals.parseQueryString = parseQueryString;
    ;
    var turkishOrder;
    function turkishLocaleCompare(a, b) {
        var alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (var z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (var i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            var x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            var ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            var c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }
    Q$Externals.turkishLocaleCompare = turkishLocaleCompare;
    ;
    function turkishLocaleToUpper(a) {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }
    Q$Externals.turkishLocaleToUpper = turkishLocaleToUpper;
    ;
    // --- DIALOGS ---
    function alertDialog(message, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            okButton: Q.text('Dialogs.OkButton'),
            title: Q.text('Dialogs.AlertTitle'),
            onClose: null,
            onOpen: null,
            autoOpen: false,
            dialogClass: 's-MessageDialog s-AlertDialog',
            modal: true,
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (options.onClose)
                    options.onClose();
            }
        }, options);
        if (options.htmlEncode)
            message = Q.htmlEncode(message);
        if (!options.buttons) {
            var buttons = [];
            buttons.push({
                text: options.okButton,
                click: function () {
                    dialog.dialog('close');
                }
            });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }
    Q$Externals.alertDialog = alertDialog;
    ;
    function confirmDialog(message, onYes, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            yesButton: Q.text('Dialogs.YesButton'),
            noButton: Q.text('Dialogs.NoButton'),
            title: Q.text('Dialogs.ConfirmationTitle'),
            onNo: null,
            onCancel: null,
            onClose: null,
            autoOpen: false,
            modal: true,
            dialogClass: 's-MessageDialog s-ConfirmDialog',
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (!clicked && options.onCancel)
                    options.onCancel();
            },
            overlay: {
                opacity: 0.7,
                background: "black"
            }
        }, options);
        if (options.htmlEncode)
            message = Q.htmlEncode(message);
        var clicked = false;
        if (!options.buttons) {
            var buttons = [];
            buttons.push({
                text: options.yesButton,
                click: function () {
                    clicked = true;
                    dialog.dialog('close');
                    if (onYes)
                        onYes();
                }
            });
            if (options.noButton)
                buttons.push({
                    text: options.noButton,
                    click: function () {
                        clicked = true;
                        dialog.dialog('close');
                        if (options.onNo)
                            options.onNo();
                        else if (options.onCancel)
                            options.onCancel();
                    }
                });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }
    Q$Externals.confirmDialog = confirmDialog;
    ;
    function iframeDialog(options) {
        var doc;
        var e = $('<div><iframe></iframe></div>');
        var settings = $.extend({
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: Q.text('Dialogs.AlertTitle'),
            open: function () {
                doc = (e.find('iframe').css({
                    border: 'none',
                    width: '100%',
                    height: '100%'
                })[0]).contentDocument;
                doc.open();
                doc.write(settings.html);
                doc.close();
            },
            close: function () {
                doc.open();
                doc.write('');
                doc.close();
                e.dialog('destroy').html('');
            }
        }, options);
        e.dialog(settings);
    }
    Q$Externals.iframeDialog = iframeDialog;
    ;
    // -- AJAX HELPERS --
    function setupAjaxIndicator() {
        var loadingIndicator = null;
        var loadingTimer = 0;
        $(document).ajaxStart(function () {
            window.clearTimeout(loadingTimer);
            loadingTimer = window.setTimeout(function () {
                if (!loadingIndicator)
                    loadingIndicator = $('<div/>').addClass('s-AjaxIndicator').appendTo(document.body);
            }, 2000);
        }).ajaxStop(function () {
            if (loadingIndicator) {
                loadingIndicator.remove();
                loadingIndicator = null;
            }
            window.clearTimeout(loadingTimer);
        });
    }
    Q$Externals.setupAjaxIndicator = setupAjaxIndicator;
    ;
    function toId(id) {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15)
            return id;
        return parseInt(id, 10);
    }
    Q$Externals.toId = toId;
    ;
    var oldShowLabel;
    function validateShowLabel(element, message) {
        oldShowLabel.call(this, element, message);
        this.errorsFor(element).each(function (i, e) {
            $(e).attr('title', $(e).text());
        });
    }
    ;
    function jQueryValidationInitialization() {
        Serenity.CustomValidation.registerValidationMethods();
        var p = $.validator;
        oldShowLabel = p.showLabel;
        p.showLabel = validateShowLabel;
        jQuery.validator.addMethod("xss", function (value, element) {
            if (value.length == 0)
                return true;
            var startIndex = 0;
            while (true) {
                var pos = value.indexOf('&', startIndex);
                if (pos < 0)
                    pos = value.indexOf('<', startIndex);
                if (pos < 0)
                    return true;
                if (pos == value.length - 1)
                    return true;
                var ch = value.charAt(pos);
                var ch2 = value.charAt(pos + 1);
                if (ch != '&') {
                    if ((ch == '<') && ((((ch2 >= 'a' && ch2 <= 'z') || (ch2 >= 'A' && ch2 <= 'Z')) || (ch2 == '!')) || (ch2 == '/')))
                        return false;
                }
                else if (ch2 == '#')
                    return false;
                startIndex = pos + 1;
            }
        });
        $.validator.addMethod("dateQ", function (value, element) {
            return this.optional(element) || Q$Externals.parseDate(value) != false;
        });
        $.validator.addMethod("hourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(parseHourAndMin(value));
        });
        $.validator.addMethod("dayHourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(parseDayHourAndMin(value));
        });
        $.validator.addMethod("decimalQ", function (value, element) {
            return this.optional(element) || !isNaN(parseDecimal(value));
        });
        $.validator.addMethod("integerQ", function (value, element) {
            return this.optional(element) || !isNaN(parseInteger(value));
        });
        var oldEmail = $.validator.methods['email'];
        $.validator.addMethod("email", function (value, element) {
            if (!Q$Config.emailAllowOnlyAscii)
                return oldEmail.call(this, value, element);
            return this.optional(element) || /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(value);
        });
        $.validator.addMethod("emailMultiple", function (value, element) {
            var result = this.optional(element);
            if (result)
                return result;
            if (value.indexOf(';') >= 0)
                value = value.split(';');
            else
                value = value.split(',');
            for (var i = 0; i < value.length; i++) {
                result = $.validator.methods['email'].call(this, value[i], element);
                if (!result)
                    return result;
            }
            return result;
        });
        $.validator.addMethod("anyvalue", function (value, element) {
            return true;
        });
        var d = $.validator.defaults;
        d.ignoreTitle = true;
        d.onchange = function (element) {
            this.element(element);
        };
        p.oldinit = p.init;
        p.init = function () {
            p.oldinit.call(this);
            function changeDelegate(event) {
                if (this.form == null)
                    return;
                var validator = $.data(this.form, "validator"), eventType = "on" + event.type.replace(/^validate/, "");
                validator && validator.settings[eventType] && validator.settings[eventType].call(validator, this);
            }
            function delegate(event) {
                var el = this[0];
                if (!$.data(el, 'changebound')) {
                    $(el).change(changeDelegate);
                    $.data(el, 'changebound', true);
                }
            }
            $(this.currentForm)
                .on(":text, :password, :file, select, textarea", "focusin.validate", delegate);
        };
        p.oldfocusInvalid = p.focusInvalid;
        p.focusInvalid = function () {
            if (this.settings.abortHandler)
                this.settings.abortHandler(this);
            this.oldfocusInvalid.call(this);
        };
        p.oldstopRequest = p.focusInvalid;
        p.stopRequest = function (element, valid) {
            var formSubmitted = this.formSubmitted;
            this.oldfocusInvalid.call(this, [element, valid]);
            if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
                this.settings.abortHandler(this);
            }
        };
        p.resetAll = function () {
            this.submitted = {};
            this.prepareForm();
            this.hideErrors();
            this.elements().removeClass(this.settings.errorClass);
        };
        jQuery(function () {
            $.extend($.validator.messages, {
                email: Q.text("Validation.Email"),
                required: Q.text("Validation.Required"),
                minlength: Q.text("Validation.MinLength"),
                maxlength: Q.text("Validation.MaxLength"),
                digits: Q.text("Validation.Digits"),
                range: Q.text("Validation.Range"),
                xss: Q.text("Validation.Xss"),
                dateQ: Q.text("Validation.DateInvalid"),
                decimalQ: Q.text("Validation.Decimal"),
                integerQ: Q.text("Validation.Integer"),
                url: Q.text("Validation.Url")
            });
        });
    }
    ;
    function validatorAbortHandler(validator) {
        validator.settings.abortHandler = null;
        validator.settings.submitHandler = function () {
            return false;
        };
    }
    Q$Externals.validatorAbortHandler = validatorAbortHandler;
    ;
    function validateOptions(options) {
        return $.extend({
            ignore: ":hidden",
            meta: 'v',
            errorClass: 'error',
            errorPlacement: function (error, element) {
                var field = null;
                var vx = element.attr('data-vx-id');
                if (vx) {
                    field = $('#' + vx);
                    if (!field.length)
                        field = null;
                    else
                        field = field[0];
                }
                if (field == null) {
                    field = element.parents('div.field');
                    if (field.length) {
                        var inner = $('div.vx', field[0]);
                        if (inner.length)
                            field = inner[0];
                    }
                    else
                        field = element.parent();
                }
                error.appendTo(field);
            },
            submitHandler: function () {
                return false;
            },
            invalidHandler: function () {
                Q.notifyError(Q.text("Validation.InvalidFormMessage"));
            },
            success: function (label) {
                label.addClass('checked');
            }
        }, options);
    }
    Q$Externals.validateOptions = validateOptions;
    ;
    if (window['jQuery'] && window['jQuery']['validator'])
        jQueryValidationInitialization();
    else if (window['jQuery']) {
        jQuery(function ($) {
            if ($.validator)
                jQueryValidationInitialization();
        });
    }
    function jQueryDatepickerInitialization() {
        var order = Q$Culture.dateOrder;
        var s = Q$Culture.dateSeparator;
        var culture = ($('html').attr('lang') || 'en').toLowerCase();
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
            buttonImage: Q.resolveUrl('~/content/serenity/images/datepicker.gif'),
            buttonImageOnly: true,
            showOn: 'both',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true
        });
    }
    ;
    if (window['jQuery'] && window['jQuery']['datepicker'] && window['jQuery']['datepicker']['regional'] && window['jQuery']['datepicker']['regional']['en'])
        jQueryDatepickerInitialization();
    else
        jQuery(function ($) {
            if ($.datepicker)
                jQueryDatepickerInitialization();
        });
    function jQuerySelect2Initialization() {
        $.ui.dialog.prototype._allowInteraction = function (event) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };
    }
    ;
    if (window['jQuery'] && window['jQuery']['ui'])
        jQuerySelect2Initialization();
    else
        jQuery(function ($) {
            if (window['jQuery']['ui'])
                jQuerySelect2Initialization();
        });
    function postToService(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (Q.resolveUrl(options.url)) : Q.resolveUrl('~/services/' + options.service))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        var div = $('<div/>').appendTo(form);
        $('<input/>').attr('type', 'hidden').attr('name', 'request')
            .val($['toJSON'](options.request))
            .appendTo(div);
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }
    Q$Externals.postToService = postToService;
    ;
    function postToUrl(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', Q.resolveUrl(options.url))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        var div = $('<div/>').appendTo(form);
        if (options.params != null) {
            for (var k in options.params) {
                $('<input/>').attr('type', 'hidden').attr('name', k)
                    .val(options.params[k])
                    .appendTo(div);
            }
        }
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }
    Q$Externals.postToUrl = postToUrl;
    ;
    function ssExceptionInitialization() {
        ss.Exception.prototype.toString = function () {
            return this.get_message();
        };
    }
    ;
    if (ss && ss.Exception)
        ssExceptionInitialization();
    else
        jQuery(function ($) {
            if (ss && ss.Exception)
                ssExceptionInitialization();
        });
    function showInFrame(code) {
        var height = screen.availHeight - 60;
        var width = screen.availWidth - 10;
        var x = (screen.availWidth - width) / 2 - 5;
        if (x < 0)
            x = 0;
        var y = (screen.availHeight - height) / 2 - 25;
        if (y < 0)
            y = 0;
        var winPopup = window.open("", "", "status=0, toolbar=0, width=" + width + ", height=" + height +
            ", scrollbars=1, resizable=yes, left=" + x + ", top=" + y);
        winPopup.document.body.innerHTML = code;
    }
    Q$Externals.showInFrame = showInFrame;
    if ($.fn.button && $.fn.button.noConflict) {
        var btn = $.fn.button.noConflict();
        $.fn.btn = btn;
    }
    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    function deepClone(arg1, arg2) {
        var options, name, src, copy, copyIsArray, clone, target = arguments[0] || {}, i = 1, length = arguments.length;
        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !$.isFunction(target)) {
            target = {};
        }
        if (i === length) {
            target = {};
            i = 0;
        }
        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) !== null) {
                // Special operations for arrays
                if ($.isArray(options)) {
                    target = $.extend(true, [], options);
                }
                else {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];
                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        // Recurse if we're merging plain objects or arrays
                        if (copy && ($.isPlainObject(copy) ||
                            (copyIsArray = $.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && $.isArray(src) ? src : [];
                            }
                            else {
                                clone = src && $.isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = Q$Externals.deepClone(clone, copy);
                        }
                        else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }
        }
        // Return the modified object
        return target;
    }
    Q$Externals.deepClone = deepClone;
    ;
})(Q$Externals || (Q$Externals = {}));
//# sourceMappingURL=Serenity.Externals.js.map
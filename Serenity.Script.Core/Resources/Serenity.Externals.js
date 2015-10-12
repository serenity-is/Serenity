if (typeof(global) === "undefined")
    global = window;

var Q$Externals = {};

// -- FORMATTING ---

Q$Externals.formatDayHourAndMin = function(value) {
    if (value === 0)
        return '0';
    else if (!value)
        return '';
    var days = Math.floor(value / 24 / 60);
    var txt = "";

    if (days > 0) {
        txt += days.toString();
    }
        
    var mins = $.pi.zeroPad(Math.floor((value % (24 * 60)) / (60)), 2) + ':' + $.pi.zeroPad(value % 60, 2);
    if (mins != '00:00') {
        if (days > 0) 
            txt += ".";
        txt += mins;
    }
    return txt;
}

Q$Externals.formatISODateTimeUTC = function(date) {
    if (date == null)
        return "";

    var zeropad = function (num) { return ((num < 10) ? '0' : '') + num; }

    var str = date.getUTCFullYear() + "-" + 
        zeropad(date.getUTCMonth() + 1) + "-" + 
        zeropad(date.getUTCDate()) + "T" + 
        zeropad(date.getUTCHours()) + ":" + 
        zeropad(date.getUTCMinutes());

    var secs = Number(date.getUTCSeconds() + "." +
                ((date.getUTCMilliseconds() < 100) ? '0' : '') +
                zeropad(date.getUTCMilliseconds()));

    str += ":" + zeropad(secs) + "Z";

    return str;
}

Q$Externals.formatNumber = function (num, format, dec, group) {
    
    var neg = '-';

    if (isNaN(num)) {
        return null;
    }

    var returnString = "";
    if (format.indexOf(".") > -1) {
        var decimalPortion = dec;
        var decimalFormat = format.substring(format.lastIndexOf(".") + 1);

        num = new Number(Q$Externals.roundNumber(num, decimalFormat.length));

        var decimalValue = num % 1;
        var decimalString = new String(decimalValue.toFixed(decimalFormat.length));
        decimalString = decimalString.substring(decimalString.lastIndexOf(".") + 1);

        for (var i = 0; i < decimalFormat.length; i++) {
            if (decimalFormat.charAt(i) == '#' && decimalString.charAt(i) != '0') {
                decimalPortion += decimalString.charAt(i);
                continue;
            } else if (decimalFormat.charAt(i) == '#' && decimalString.charAt(i) == '0') {
                var notParsed = decimalString.substring(i);
                if (notParsed.match('[1-9]')) {
                    decimalPortion += decimalString.charAt(i);
                    continue;
                } else
                    break;
            } else if (decimalFormat.charAt(i) == "0")
                decimalPortion += decimalString.charAt(i);
        }
        returnString += decimalPortion
    } else
        num = Math.round(num);

    var ones = Math.floor(num);
    if (num < 0)
        ones = Math.ceil(num);

    var onesFormat = "";
    if (format.indexOf(".") == -1)
        onesFormat = format;
    else
        onesFormat = format.substring(0, format.indexOf("."));

    var onePortion = "";
    if (!(ones == 0 && onesFormat.substr(onesFormat.length - 1) == '#')) {
        // find how many digits are in the group
        var oneText = new String(Math.abs(ones));
        var groupLength = 9999;
        if (onesFormat.lastIndexOf(",") != -1)
            groupLength = onesFormat.length - onesFormat.lastIndexOf(",") - 1;
        var groupCount = 0;
        for (var i = oneText.length - 1; i > -1; i--) {
            onePortion = oneText.charAt(i) + onePortion;
            groupCount++;
            if (groupCount == groupLength && i != 0) {
                onePortion = group + onePortion;
                groupCount = 0;
            }
        }

        // account for any pre-data padding
        if (onesFormat.length > onePortion.length) {
            var padStart = onesFormat.indexOf('0');
            if (padStart != -1) {
                var padLen = onesFormat.length - padStart;

                // pad to left with 0's or group char
                var pos = onesFormat.length - onePortion.length - 1;
                while (onePortion.length < padLen) {
                    var padChar = onesFormat.charAt(pos);
                    // replace with real group char if needed
                    if (padChar == ',')
                        padChar = group;
                    onePortion = padChar + onePortion;
                    pos--;
                }
            }
        }
    }

    if (!onePortion && onesFormat.indexOf('0', onesFormat.length - 1) !== -1)
        onePortion = '0';

    returnString = onePortion + returnString;

    if (num < 0)
        returnString = neg + returnString;

    if (returnString.lastIndexOf(dec) == returnString.length - 1) {
        returnString = returnString.substring(0, returnString.length - 1);
    }
    
    return returnString;
};

Q$Externals.roundNumber = function (num, decimalPlaces) {
    var power = Math.pow(10, decimalPlaces || 0);
    var value = String(Math.round(num * power) / power);

    // ensure the decimal places are there
    if (decimalPlaces > 0) {
        var dp = value.indexOf(".");
        if (dp == -1) {
            value += '.';
            dp = 0;
        } else {
            dp = value.length - (dp + 1);
        }

        while (dp < decimalPlaces) {
            value += '0';
            dp++;
        }
    }
    return value;
};

Q$Externals.isoRegexp =  /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;


// --- LOG (ISO/UTC) DATE / USER ---

Q$Externals.logDateToString = function(date) {
    if (date) {
        return Q.formatDate(Q$Externals.parseISODateTime(date), Q$Culture.dateTimeFormat.replace(':ss', ''));
    }
    else
        return "";
}

Q$Externals.logDateUserToString = function(date, user) {
    var text = "";
    if (date) {
        text = Q.formatDate($.pi.parseISODateTime(date), Q$Culture.dateTimeFormat.replace(':ss', ''));
    }
    if (user && user.length) {
        if (text.length) 
            text += " ";
        text += '(' + user + ')'
    }
    return text;
}
       
Q$Externals.logDateUserToShortString = function(date, user) {
    var text = "";
    if (date) {
        text = Q.formatDate(Q$Externals.parseISODateTime(date), Q$Culture.dateFormat.replace('yyyy', 'yy'));
    }
    if (user && user.length) {
        if (text.length) 
            text += ' ';
        text += user;
    }
    return text;
}


// -- PARSING ---

Q$Externals.parseInteger = function(value) {
    value = Q.trim(value.toString());

    var ts = Q$Culture.get_groupSeperator();
    if (value && value.length && value.indexOf(ts) > 0) {
        value = value.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
    }

    if (!(/^[-\+]?\d+$/.test(value)))
        return NaN;

    return parseInt(value, 10);
}

Q$Externals.parseDate = function(dateString, dateOrder)
{
    if (!dateString || !dateString.length)
        return null;

    var dateVal;
    var dArray;
    var d, m, y;

    dArray = Q$Externals.splitDateString(dateString);
    if (!dArray)
        return false;
         
    if (dArray.length == 3)
    {
        dateOrder = dateOrder || Q$Culture.dateOrder;
        switch (dateOrder) {
            case "dmy" :
                d = parseInt(dArray[0], 10);
                m = parseInt(dArray[1], 10) - 1;
                y = parseInt(dArray[2], 10);
                break;
            case "ymd" :
                d = parseInt(dArray[2], 10);
                m = parseInt(dArray[1], 10) - 1;
                y = parseInt(dArray[0], 10);
                break;
            case "mdy" :
            default :
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
        } catch(e) {
            return false;
        }
    }
    else if (dArray.length == 1)
    {
        try {
            dateVal = new Date(dArray[0]);
            if (isNaN(dateVal.getFullYear()))
                return false;
        }
        catch(e) {
            return false;
        }
    }
    return dateVal;
}

Q$Externals.parseDecimal = function(value) {
    value = Q.trim(value.toString());

    var ts = Q$Culture.get_groupSeperator();

    if (value && value.length && value.indexOf(ts) > 0) {
        value = value.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
    }

    if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Q$Culture.decimalSeparator + "?(\\d*)\\s*$").test(value)))
        return NaN;

    return parseFloat(value.toString().replace(Q$Culture.decimalSeparator, '.'));
}

Q$Externals.splitDateString = function(dateString) {
    dateString = $.trim(dateString);
    if (!dateString.length)
        return;
            
    var dArray;
    if (dateString.indexOf("/") >= 0)
        dArray = dateString.split("/");
    else if (dateString.indexOf(".") >= 0)
        dArray = dateString.split(".");
    else if (dateString.indexOf("-") >= 0)
        dArray = dateString.split("-");
    else if (dateString.indexOf("\\") >= 0)
        dArray = dateString.split("\\");
    else
        dArray = [dateString];
            
    return dArray;
}

Q$Externals.parseISODateTime = function (str) 
{
    if (!str || !str.length)
        return null;

    var timestamp = Date.parse(str); //MAC SAFARI HATALI OLAB�L�R, KONTROL ET!
    if (!isNaN(timestamp) && typeof timestamp == "Date")
        return timestamp;

    str = str + "";
    if (typeof(str) != "string" || str.length === 0) {
        return null;
    }
    var res = str.match(Q$Externals.isoRegexp);
    if (typeof(res) == "undefined" || res === null) {
        return null;
    }
    var year, month, day, hour, min, sec, msec;
    year = parseInt(res[1], 10);
    if (typeof(res[2]) == "undefined" || res[2] === '') {
        return new Date(year);
    }
    month = parseInt(res[2], 10) - 1;
    day = parseInt(res[3], 10);
    if (typeof(res[4]) == "undefined" || res[4] === '') {
        return new Date(year, month, day);
    }
    hour = parseInt(res[4], 10);
    min = parseInt(res[5], 10);
    sec = (typeof(res[6]) != "undefined" && res[6] !== '') ? parseInt(res[6], 10) : 0;
    if (typeof(res[7]) != "undefined" && res[7] !== '') {
        msec = Math.round(1000.0 * parseFloat("0." + res[7]));
    } else {
        msec = 0;
    }
    if ((typeof(res[8]) == "undefined" || res[8] === '') && (typeof(res[9]) == "undefined" || res[9] === '')) {
        return new Date(year, month, day, hour, min, sec, msec);
    }
    var ofs;
    if (typeof(res[9]) != "undefined" && res[9] !== '') {
        ofs = parseInt(res[10], 10) * 3600000;
        if (typeof(res[11]) != "undefined" && res[11] !== '') {
            ofs += parseInt(res[11], 10) * 60000;
        }
        if (res[9] == "-") {
            ofs = -ofs;
        }
    } else {
        ofs = 0;
    }
    return new Date(Date.UTC(year, month, day, hour, min, sec, msec) - ofs);
}

Q$Externals.parseHourAndMin = function(value) {
    var v = Q.trim(value);
    if (v.length < 4 || v.length > 5)
        return NaN;
    var h, m;
    if (v.charAt(1) == ':') {
        h = $.pi.parseInteger(v.substr(0, 1));
        m = $.pi.parseInteger(v.substr(2, 2));
    }
    else {
        if (v.charAt(2) != ':')
            return NaN;
        h = $.pi.parseInteger(v.substr(0, 2));
        m = $.pi.parseInteger(v.substr(3, 2));
    }
    if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
        return NaN;
    return h * 60 + m;
}
    
Q$Externals.parseDayHourAndMin = function(value) {
    var days;
    var v = Q.trim(value);
    if (!v)
        return NaN;
    v = v.split('.');
    if (v.length == 0 || v.length > 2)
        return NaN;
    if (v.length == 1) {
        days = $.pi.parseInteger(v[0]);
        if (!isNaN(days))
            return days * 24 * 60;
        return $.pi.parseHourAndMin(v[0]);
    }
    else {
        days = $.pi.parseInteger(v[0]);
        var hm = $.pi.parseHourAndMin(v[1]);
        if (isNaN(days) || isNaN(hm))
            return NaN;
        return days * 24 * 60 + hm;
    }
}

Q$Externals.parseQueryString = function(queryString) {
    var qs;
    if (arguments.length == 0)
        qs = location.search.substring(1, location.search.length);
    else
        qs = queryString || '';
    var result = {};
    var parts = qs.split('&');
    for (var i = 0; i < parts.length; i++) {
        var pair = parts[i].split('=');
        var name = decodeURIComponent(pair[0]);
        result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
    }
    return result;
}

Q$Externals.turkishLocaleCompare = function(a, b) {
    var alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";

    a = a || "";
    b = b || "";

    if (a == b)
        return 0;

    if (!Q$Externals.turkishLocaleIndex) {
        Q$Externals.turkishLocaleIndex = {};
        for (var z = 0; z < alphabet.length; z++) {
            Q$Externals.turkishLocaleIndex[alphabet.charAt(z)] = z + 1;
        }
    }

    for (var i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
        var x = a.charAt(i), y = b.charAt(i);

        if (x === y)
            continue;

        var ix = Q$Externals.turkishLocaleIndex[x], iy = Q$Externals.turkishLocaleIndex[y];

        if (ix != null && iy != null)
            return ix < iy ? -1 : 1;

        var c = x.localeCompare(y);
        if (c == 0)
            continue;

        return c;
    }

    return a.localeCompare(b);
}

Q$Externals.turkishLocaleToUpper = function (a) {
    if (!a)
        return a;

    return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
}

// --- DIALOGS ---

Q$Externals.alertDialog = function(message, options) {
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
        open: function() {
            if (options.onOpen)
                options.onOpen.call(this);
        },
        close: function() {
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
            click: function() {
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

Q$Externals.confirmDialog = function(message, onYes, options) {
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
        open: function() {
            if (options.onOpen)
                options.onOpen.call(this);
        },
        close: function() {
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
            click: function() {
                clicked = true;
                dialog.dialog('close');
                if (onYes)
                    onYes();
            }
        });

        if (options.noButton)
            buttons.push({
                text: options.noButton,
                click: function() {
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

Q$Externals.iframeDialog = function(options) {
    var doc;
    var e = $('<div><iframe></iframe></div>');
    var settings = $.extend({
        autoOpen: true,
        modal: true,
        width: '60%',
        height: '400',
        title: Q.text('Dialogs.AlertTitle'),
        open: function() {
            doc = e.find('iframe').css({
                border: 'none', 
                width: '100%',
                height: '100%'})[0].contentDocument;
            doc.open();
            doc.write(settings.html);
            doc.close(); 
        },
        close: function() {
            doc.open();
            doc.write('');
            doc.close();
            e.dialog('destroy').html('');
        }
    }, options);
    e.dialog(settings);
}

// -- AJAX HELPERS --
Q$Externals.setupAjaxIndicator = function() {
    var loadingIndicator = null;
    var loadingTimer = 0;
        
    $(document).ajaxStart(function() {
        window.clearTimeout(loadingTimer);
        loadingTimer = window.setTimeout(function() {
            if (!loadingIndicator)
                loadingIndicator = $('<div/>').addClass('s-AjaxIndicator').appendTo(document.body);
        }, 2000);
    }).ajaxStop(function() {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }
        window.clearTimeout(loadingTimer);
    });
}

Q$Externals.toId = function(id) {
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

var oldShowLabel;

Q$Externals.validateShowLabel = function (element, message) {
    oldShowLabel.call(this, element, message);
    this.errorsFor(element).each(function (i, e) {
        $(e).attr('title', $(e).text());
    });
}

Q$Externals.jQueryValidationInitialization = function () {
    Serenity.CustomValidation.registerValidationMethods();

    oldShowLabel = $.validator.prototype.showLabel;
    $.validator.prototype.showLabel = Q$Externals.validateShowLabel;

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
        return this.optional(element) || !isNaN(Q.parseHourAndMin(value));
    });

    $.validator.addMethod("dayHourAndMin", function (value, element) {
        return this.optional(element) || !isNaN(Q.parseDayHourAndMin(value));
    });

    $.validator.addMethod("decimalQ", function (value, element) {
        return this.optional(element) || !isNaN(Q.parseDecimal(value));
    });

    $.validator.addMethod("integerQ", function (value, element) {
        return this.optional(element) || !isNaN(Q.parseInteger(value));
    });

    var oldEmail = $.validator.methods.email;

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
            result = $.validator.methods.email.call(this, value[i], element);
            if (!result)
                return result;
        }
        return result;
    });

    $.validator.addMethod("anyvalue", function (value, element) {
        return true;
    });

    $.validator.defaults.ignoreTitle = true;

    $.validator.defaults.onchange = function (element) {
        this.element(element);
    };

    $.validator.prototype.oldinit = $.validator.prototype.init;
    $.validator.prototype.init = function () {

        $.validator.prototype.oldinit.call(this);

        function changeDelegate(event) {
            if (this.form == null)
                return;

            var validator = $.data(this.form, "validator"),
                eventType = "on" + event.type.replace(/^validate/, "");
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
    }

    $.validator.prototype.oldfocusInvalid = $.validator.prototype.focusInvalid;
    $.validator.prototype.focusInvalid = function () {
        if (this.settings.abortHandler)
            this.settings.abortHandler(this);
        this.oldfocusInvalid.call(this);
    }

    $.validator.prototype.oldstopRequest = $.validator.prototype.focusInvalid;
    $.validator.prototype.stopRequest = function (element, valid) {
        var formSubmitted = this.formSubmitted;
        this.oldfocusInvalid.call(this, [element, valid]);
        if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
            this.settings.abortHandler(this);
        }
    }

    $.validator.prototype.resetAll = function () {
        this.submitted = {};
        this.prepareForm();
        this.hideErrors();
        this.elements().removeClass(this.settings.errorClass);
    }

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

Q$Externals.validatorAbortHandler = function (validator) {
    validator.settings.abortHandler = null;
    validator.settings.submitHandler = function() {
        return false;
    }
}

Q$Externals.validateOptions = function (options) {

    return $.extend({
        ignore: ":hidden",
        meta: 'v',
        errorClass: 'error',
        errorPlacement: function(error, element) {
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
        submitHandler: function() {
            return false;
        },
        invalidHandler: function() {
            Q.notifyError(Q.text("Validation.InvalidFormMessage"), "error");
        },          
        success: function(label) {
            label.addClass('checked');
        }
    }, options);
} 

if (window.jQuery && window.jQuery.validator)
    Q$Externals.jQueryValidationInitialization();
else if (window.jQuery) { 
    jQuery(function ($) {
        if ($.validator)
            Q$Externals.jQueryValidationInitialization();
    });
}

Q$Externals.jQueryDatepickerInitialization = function() {
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
};

if (window.jQuery && window.jQuery.datepicker && window.jQuery.datepicker.regional && window.jQuery.datepicker.regional.en)
    Q$Externals.jQueryDatepickerInitialization();
else
    jQuery(function ($) {
        if ($.datepicker)
            Q$Externals.jQueryDatepickerInitialization();
    });

Q$Externals.jQuerySelect2Initialization = function () {
    $.ui.dialog.prototype._allowInteraction = function( event ) {
        if ( $( event.target ).closest(".ui-dialog").length ) {
            return true;
        }
        return !!$( event.target ).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
    }
}

if (window.jQuery && window.jQuery.ui)
    Q$Externals.jQuerySelect2Initialization();
else
    jQuery(function ($) {
        if (window.jQuery.ui)
            Q$Externals.jQuerySelect2Initialization();
    });

Q$Externals.postToService = function (options) {
    var form = $('<form/>')
        .attr('method', 'POST')
        .attr('action', options.url ? (Q.resolveUrl(options.url)) : Q.resolveUrl('~/services/' + options.service))
        .appendTo(document.body);

    if (options.target)
        form.attr('target', options.target);

    var div = $('<div/>').appendTo(form);

    $('<input/>').attr('type', 'hidden').attr('name', 'request')
        .val($.toJSON(options.request))
        .appendTo(div);

    $('<input/>').attr('type', 'submit')
        .appendTo(div);

    form.submit();
    window.setTimeout(function () { form.remove(); }, 0);
};

Q$Externals.postToUrl = function (options) {
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
};


Q$Externals.ssExceptionInitialization = function() {
    window.ss.Exception.prototype.toString = function() {
        return this.get_message();
    };
};

if (window.ss && window.ss.Exception)
    Q$Externals.ssExceptionInitialization();
else
    jQuery(function ($) {
        if (window.ss && window.ss.Exception)
            Q$Externals.ssExceptionInitialization();
    });

global.Q$Externals = Q$Externals;

function showInFrame(code) {
    height = screen.availHeight - 60;
    width = screen.availWidth - 10;
    var x = (screen.availWidth - width) / 2 - 5; if (x < 0) x = 0;
    var y = (screen.availHeight - height) / 2 - 25; if (y < 0) y = 0;
    var winPopup = window.open("", "", "status=0, toolbar=0, width=" + width + ", height=" + height +
        ", scrollbars=1, resizable=yes, left=" + x + ", top=" + y);
    winPopup.document.body.innerHTML = code;
}

if ($.fn.button && $.fn.button.noConflict) {
    var btn = $.fn.button.noConflict(); $.fn.btn = btn;
}

// derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
Q$Externals.deepClone = function () {
    var options, name, src, copy, copyIsArray, clone,
        target = arguments[0] || {},
        i = 1,
        length = arguments.length;

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
            } else {
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

                        } else {
                            clone = src && $.isPlainObject(src) ? src : {};
                        }

                        // Never move original objects, clone them
                        target[name] = Q$Externals.deepClone(clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }
    }

    // Return the modified object
    return target;
};

var Q;
(function (Q) {
    function arrayClone(a) {
        return ss.arrayClone(a);
    }
    Q.arrayClone = arrayClone;
    function isArray(a) {
        return ss.isArray(a);
    }
    Q.isArray = isArray;
    function insert(obj, index, item) {
        ss.insert(obj, index, item);
    }
    Q.insert = insert;
    function coalesce(a, b) {
        return ss.coalesce(a, b);
    }
    Q.coalesce = coalesce;
    function isValue(a) {
        return ss.isValue(a);
    }
    Q.isValue = isValue;
    function endsWith(s, search) {
        return ss.endsWithString(s, search);
    }
    Q.endsWith = endsWith;
    function format(msg) {
        var prm = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            prm[_i - 1] = arguments[_i];
        }
        return ss.formatString.apply(ss, [msg].concat(prm));
    }
    Q.format = format;
    function padLeft(s, len, ch) {
        if (ch === void 0) { ch = ' '; }
        while (s.length < len)
            s = "0" + s;
        return s;
    }
    Q.padLeft = padLeft;
    function zeroPad(n, digits) {
        var s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }
    Q.zeroPad = zeroPad;
    function replaceAll(s, f, r) {
        return ss.replaceAllString(s, f, r);
    }
    Q.replaceAll = replaceAll;
    function startsWith(s, search) {
        return ss.startsWithString(s, search);
    }
    Q.startsWith = startsWith;
    function alert(message, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            okButton: text('Dialogs.OkButton'),
            title: text('Dialogs.AlertTitle'),
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
            message = htmlEncode(message);
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
    Q.alert = alert;
    function confirm(message, onYes, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            yesButton: text('Dialogs.YesButton'),
            noButton: text('Dialogs.NoButton'),
            title: text('Dialogs.ConfirmationTitle'),
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
                opacity: 0.77,
                background: "black"
            }
        }, options);
        if (options.htmlEncode)
            message = htmlEncode(message);
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
    Q.confirm = confirm;
    function information(message, onOk, options) {
        confirm(message, onOk, $.extend({
            title: text("Dialogs.InformationTitle"),
            dialogClass: "s-MessageDialog s-InformationDialog",
            yesButton: text("Dialogs.OkButton"),
            noButton: null,
        }, options));
    }
    Q.information = information;
    function warning(message, options) {
        alert(message, $.extend({
            title: text("Dialogs.WarningTitle"),
            dialogClass: "s-MessageDialog s-WarningDialog"
        }, options));
    }
    Q.warning = warning;
    function iframeDialog(options) {
        var doc;
        var e = $('<div><iframe></iframe></div>');
        var settings = $.extend({
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: text('Dialogs.AlertTitle'),
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
    Q.iframeDialog = iframeDialog;
    function toId(id) {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15 || !(/^\d+$/.test(id)))
            return id;
        var v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }
    Q.toId = toId;
    function log(m) {
        window.console && window.console.log(m);
    }
    function prop(type, name, getter, setter) {
        getter = getter || "get_" + name;
        setter = setter || "set_" + name;
        Object.defineProperty(type.prototype, name, {
            get: function () {
                return this[getter]();
            },
            set: function (value) {
                return this[setter](value);
            },
            configurable: true,
            enumerable: true
        });
    }
    Q.prop = prop;
    var blockUICount = 0;
    function blockUIWithCheck(opt) {
        if (blockUICount > 0) {
            blockUICount++;
            return;
        }
        $.blockUI(opt);
        blockUICount++;
    }
    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    function blockUI(options) {
        options = $.extend({
            baseZ: 2000,
            message: '',
            overlayCSS: {
                opacity: '0.0',
                zIndex: 2000,
                cursor: 'wait'
            }, fadeOut: 0
        }, options);
        if (options.useTimeout) {
            window.setTimeout(function () {
                blockUIWithCheck(options);
            }, 0);
        }
        else {
            blockUIWithCheck(options);
        }
    }
    Q.blockUI = blockUI;
    function blockUndo() {
        if (blockUICount > 1) {
            blockUICount--;
            return;
        }
        blockUICount--;
        $.unblockUI({ fadeOut: 0 });
    }
    Q.blockUndo = blockUndo;
    function toGrouping(items, getKey) {
        var lookup = {};
        for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
            var x = items_1[_i];
            var key = getKey(x) || "";
            var d = lookup[key];
            if (!d) {
                d = lookup[key] = [];
            }
            d.push(x);
        }
        return lookup;
    }
    Q.toGrouping = toGrouping;
    function first(array, predicate) {
        for (var _i = 0, array_1 = array; _i < array_1.length; _i++) {
            var x = array_1[_i];
            if (predicate(x))
                return x;
        }
        throw new Error("first:No element satisfies the condition.!");
    }
    Q.first = first;
    function tryFirst(array, predicate) {
        for (var _i = 0, array_2 = array; _i < array_2.length; _i++) {
            var x = array_2[_i];
            if (predicate(x))
                return x;
        }
    }
    Q.tryFirst = tryFirst;
    function single(array, predicate) {
        var match;
        var found = false;
        for (var _i = 0, array_3 = array; _i < array_3.length; _i++) {
            var x = array_3[_i];
            if (predicate(x)) {
                if (found)
                    throw new Error("single:sequence contains more than one element.");
                found = true;
                match = x;
            }
        }
        if (!found)
            throw new Error("single:No element satisfies the condition.");
        return match;
    }
    Q.single = single;
    function any(array, predicate) {
        for (var _i = 0, array_4 = array; _i < array_4.length; _i++) {
            var x = array_4[_i];
            if (predicate(x))
                return true;
        }
        return false;
    }
    Q.any = any;
    function count(array, predicate) {
        var count = 0;
        for (var _i = 0, array_5 = array; _i < array_5.length; _i++) {
            var x = array_5[_i];
            if (predicate(x))
                count++;
        }
        return count;
    }
    Q.count = count;
    function indexOf(array, predicate) {
        for (var i = 0; i < array.length; i++)
            if (predicate(array[i]))
                return i;
        return -1;
    }
    Q.indexOf = indexOf;
    function formatDate(date, format) {
        if (!date) {
            return '';
        }
        if (format == null) {
            format = Culture.dateFormat;
        }
        var pad = function (i) {
            return Q.zeroPad(i, 2);
        };
        return format.replace(new RegExp('dd?|MM?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/', 'g'), function (fmt) {
            switch (fmt) {
                case '/': return Culture.dateSeparator;
                case 'hh': return pad(((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12)));
                case 'h': return ((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12));
                case 'HH': return pad(date.getHours());
                case 'H': return date.getHours();
                case 'mm': return pad(date.getMinutes());
                case 'm': return date.getMinutes();
                case 'ss': return pad(date.getSeconds());
                case 's': return date.getSeconds();
                case 'yyyy': return date.getFullYear();
                case 'yy': return date.getFullYear().toString().substr(2, 4);
                case 'dd': return pad(date.getDate());
                case 'd': return date.getDate().toString();
                case 'MM': return pad(date.getMonth() + 1);
                case 'M': return date.getMonth() + 1;
                case 't': return ((date.getHours() < 12) ? 'A' : 'P');
                case 'tt': return ((date.getHours() < 12) ? 'AM' : 'PM');
                case 'fff': return Q.zeroPad(date.getMilliseconds(), 3);
                case 'zzz':
                case 'zz':
                case 'z': return '';
                default: return fmt;
            }
        });
    }
    Q.formatDate = formatDate;
    function htmlEncoder(a) {
        switch (a) {
            case '&': return '&amp;';
            case '>': return '&gt;';
            case '<': return '&lt;';
        }
        return a;
    }
    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    function htmlEncode(s) {
        var text = (s == null ? '' : s.toString());
        if ((new RegExp('[><&]', 'g')).test(text)) {
            return text.replace(new RegExp('[><&]', 'g'), htmlEncoder);
        }
        return text;
    }
    Q.htmlEncode = htmlEncode;
    function newBodyDiv() {
        return $('<div/>').appendTo(document.body);
    }
    Q.newBodyDiv = newBodyDiv;
    function addOption(select, key, text) {
        $('<option/>').val(key).text(text).appendTo(select);
    }
    Q.addOption = addOption;
    function clearOptions(select) {
        select.html('');
    }
    Q.clearOptions = clearOptions;
    function addEmptyOption(select) {
        addOption(select, '', text("Controls.SelectEditor.EmptyItemText"));
    }
    Q.addEmptyOption = addEmptyOption;
    function trim(s) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }
    Q.trim = trim;
    function isEmptyOrNull(s) {
        return s == null || s.length === 0;
    }
    Q.isEmptyOrNull = isEmptyOrNull;
    function trimToNull(s) {
        if (s == null || s.length === 0) {
            return null;
        }
        s = trim(s);
        if (s.length === 0) {
            return null;
        }
        else {
            return s;
        }
    }
    Q.trimToNull = trimToNull;
    function isTrimmedEmpty(s) {
        return trimToNull(s) == null;
    }
    Q.isTrimmedEmpty = isTrimmedEmpty;
    function trimToEmpty(s) {
        if (s == null || s.length === 0) {
            return '';
        }
        return trim(s);
    }
    Q.trimToEmpty = trimToEmpty;
    function findElementWithRelativeId(element, relativeId) {
        var elementId = element.attr('id');
        if (isEmptyOrNull(elementId)) {
            return $('#' + relativeId);
        }
        var result = $(elementId + relativeId);
        if (result.length > 0) {
            return result;
        }
        result = $(elementId + '_' + relativeId);
        if (result.length > 0) {
            return result;
        }
        while (true) {
            var idx = elementId.lastIndexOf('_');
            if (idx <= 0) {
                return $('#' + relativeId);
            }
            elementId = elementId.substr(0, idx);
            result = $('#' + elementId + '_' + relativeId);
            if (result.length > 0) {
                return result;
            }
        }
    }
    Q.findElementWithRelativeId = findElementWithRelativeId;
    function outerHtml(element) {
        return $('<i/>').append(element.eq(0).clone()).html();
    }
    Q.outerHtml = outerHtml;
    function layoutFillHeightValue(element) {
        var h = 0;
        element.parent().children().not(element).each(function (i, e) {
            var q = $(e);
            if (q.is(':visible')) {
                h += q.outerHeight(true);
            }
        });
        h = element.parent().height() - h;
        if (element.css('box-sizing') !== 'border-box') {
            h = h - (element.outerHeight(true) - element.height());
        }
        return h;
    }
    Q.layoutFillHeightValue = layoutFillHeightValue;
    function layoutFillHeight(element) {
        var h = layoutFillHeightValue(element);
        var n = h + 'px';
        if (element.css('height') != n) {
            element.css('height', n);
        }
    }
    Q.layoutFillHeight = layoutFillHeight;
    function initFullHeightGridPage(gridDiv) {
        $('body').addClass('full-height-page');
        gridDiv.addClass('responsive-height');
        var layout = function () {
            var inPageContent = gridDiv.parent().hasClass('page-content') ||
                gridDiv.parent().is('section.content');
            if (inPageContent) {
                gridDiv.css('height', '1px').css('overflow', 'hidden');
            }
            layoutFillHeight(gridDiv);
            if (inPageContent) {
                gridDiv.css('overflow', '');
            }
            gridDiv.triggerHandler('layout');
        };
        if ($('body').hasClass('has-layout-event')) {
            $('body').bind('layout', layout);
        }
        else if (window.Metronic) {
            window.Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }
        layout();
    }
    Q.initFullHeightGridPage = initFullHeightGridPage;
    function addFullHeightResizeHandler(handler) {
        $('body').addClass('full-height-page');
        var layout = function () {
            var avail;
            try {
                avail = parseInt($('.page-content').css('min-height') || '0')
                    - parseInt($('.page-content').css('padding-top') || '0')
                    - parseInt($('.page-content').css('padding-bottom') || '0');
            }
            catch ($t1) {
                avail = 100;
            }
            handler(avail);
        };
        if (window.Metronic) {
            window.Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }
        layout();
    }
    Q.addFullHeightResizeHandler = addFullHeightResizeHandler;
    function triggerLayoutOnShow(element) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }
    Q.triggerLayoutOnShow = triggerLayoutOnShow;
    function autoFullHeight(element) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }
    Q.autoFullHeight = autoFullHeight;
    function setMobileDeviceMode() {
        var isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            window.matchMedia('(max-width: 767px)').matches;
        var body = $(document.body);
        if (body.hasClass('mobile-device')) {
            if (!isMobile) {
                body.removeClass('mobile-device');
            }
        }
        else if (isMobile) {
            body.addClass('mobile-device');
        }
    }
    Q.setMobileDeviceMode = setMobileDeviceMode;
    Q.defaultNotifyOptions = {
        timeOut: 3000,
        showDuration: 250,
        hideDuration: 500,
        extendedTimeOut: 500,
        positionClass: 'toast-top-full-width'
    };
    function positionToastContainer(create) {
        if (typeof toastr === 'undefined') {
            return;
        }
        var dialog = $(window.document.body).children('.ui-dialog:visible').last();
        var container = toastr.getContainer(null, create);
        if (container.length === 0) {
            return;
        }
        if (dialog.length > 0) {
            var position = dialog.position();
            container.addClass('positioned-toast toast-top-full-width');
            container.css({ position: 'absolute', top: position.top + 28 + 'px', left: position.left + 6 + 'px', width: dialog.width() - 12 + 'px' });
        }
        else {
            container.addClass('toast-top-full-width');
            if (container.hasClass('positioned-toast')) {
                container.removeClass('positioned-toast');
                container.css({ position: '', top: '', left: '', width: '' });
            }
        }
    }
    Q.positionToastContainer = positionToastContainer;
    function getToastrOptions(options) {
        options = $.extend({}, Q.defaultNotifyOptions, options);
        positionToastContainer(true);
        return options;
    }
    function notifyWarning(message, title, options) {
        toastr.warning(message, title, getToastrOptions(options));
    }
    Q.notifyWarning = notifyWarning;
    function notifySuccess(message, title, options) {
        toastr.success(message, title, getToastrOptions(options));
    }
    Q.notifySuccess = notifySuccess;
    function notifyInfo(message, title, options) {
        toastr.info(message, title, getToastrOptions(options));
    }
    Q.notifyInfo = notifyInfo;
    function notifyError(message, title, options) {
        toastr.error(message, title, getToastrOptions(options));
    }
    Q.notifyError = notifyError;
    function getRemoteData(key) {
        return ScriptData.ensure('RemoteData.' + key);
    }
    Q.getRemoteData = getRemoteData;
    function getRemoteDataAsync(key) {
        return ScriptData.ensureAsync('RemoteData.' + key);
    }
    Q.getRemoteDataAsync = getRemoteDataAsync;
    function getLookup(key) {
        return ScriptData.ensure('Lookup.' + key);
    }
    Q.getLookup = getLookup;
    function getLookupAsync(key) {
        return ScriptData.ensureAsync('Lookup.' + key);
    }
    Q.getLookupAsync = getLookupAsync;
    function reloadLookup(key) {
        ScriptData.reload('Lookup.' + key);
    }
    Q.reloadLookup = reloadLookup;
    function reloadLookupAsync(key) {
        return ScriptData.reloadAsync('Lookup.' + key);
    }
    Q.reloadLookupAsync = reloadLookupAsync;
    function getColumns(key) {
        return ScriptData.ensure('Columns.' + key);
    }
    Q.getColumns = getColumns;
    function getColumnsAsync(key) {
        return ScriptData.ensureAsync('Columns.' + key);
    }
    Q.getColumnsAsync = getColumnsAsync;
    function getForm(key) {
        return ScriptData.ensure('Form.' + key);
    }
    Q.getForm = getForm;
    function getFormAsync(key) {
        return ScriptData.ensureAsync('Form.' + key);
    }
    Q.getFormAsync = getFormAsync;
    function getTemplate(key) {
        return ScriptData.ensure('Template.' + key);
    }
    Q.getTemplate = getTemplate;
    function getTemplateAsync(key) {
        return ScriptData.ensureAsync('Template.' + key);
    }
    Q.getTemplateAsync = getTemplateAsync;
    function canLoadScriptData(name) {
        return ScriptData.canLoad(name);
    }
    Q.canLoadScriptData = canLoadScriptData;
    function serviceCall(options) {
        var handleError = function (response) {
            if (Config.notLoggedInHandler != null &&
                response &&
                response.Error &&
                Config.notLoggedInHandler(options, response)) {
                return;
            }
            if (options.onError != null) {
                options.onError(response);
                return;
            }
            ErrorHandling.showServiceError(response.Error);
        };
        var url = options.service;
        if (url && url.length && url.charAt(0) != '~' && url.charAt(0) != '/' && url.indexOf('://') < 0)
            url = resolveUrl("~/services/") + url;
        options = $.extend({
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST',
            cache: false,
            blockUI: true,
            url: url,
            data: $.toJSON(options.request),
            success: function (data, textStatus, request) {
                try {
                    if (!data.Error && options.onSuccess) {
                        options.onSuccess(data);
                    }
                }
                finally {
                    if (options.blockUI) {
                        blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            },
            error: function (xhr, status, ev) {
                try {
                    if (xhr.status === 403) {
                        var l = null;
                        try {
                            l = xhr.getResponseHeader('Location');
                        }
                        catch ($t1) {
                            l = null;
                        }
                        if (l) {
                            window.top.location.href = l;
                            return;
                        }
                    }
                    if ((xhr.getResponseHeader('content-type') || '')
                        .toLowerCase().indexOf('application/json') >= 0) {
                        var json = $.parseJSON(xhr.responseText);
                        if (json && json.Error) {
                            handleError(json);
                            return;
                        }
                    }
                    var html = xhr.responseText;
                    iframeDialog({ html: html });
                }
                finally {
                    if (options.blockUI) {
                        blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            }
        }, options);
        if (options.blockUI) {
            blockUI(null);
        }
        return $.ajax(options);
    }
    Q.serviceCall = serviceCall;
    function serviceRequest(service, request, onSuccess, options) {
        return serviceCall($.extend({
            service: service,
            request: request,
            onSuccess: onSuccess
        }, options));
    }
    Q.serviceRequest = serviceRequest;
    function setEquality(request, field, value) {
        if (request.EqualityFilter == null) {
            request.EqualityFilter = {};
        }
        request.EqualityFilter[field] = value;
    }
    Q.setEquality = setEquality;
    function toSingleLine(str) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }
    Q.toSingleLine = toSingleLine;
    function text(key) {
        var t = LT.$table[key];
        if (t == null) {
            t = key || '';
        }
        return t;
    }
    Q.text = text;
    function tryGetText(key) {
        return LT.$table[key];
    }
    Q.tryGetText = tryGetText;
    function resolveUrl(url) {
        if (url && url.substr(0, 2) === '~/') {
            return Config.applicationPath + url.substr(2);
        }
        return url;
    }
    Q.resolveUrl = resolveUrl;
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
    Q.formatDayHourAndMin = formatDayHourAndMin;
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
    Q.formatISODateTimeUTC = formatISODateTimeUTC;
    function formatNumber(n, fmt, dec, grp) {
        var neg = '-';
        if (isNaN(n)) {
            return null;
        }
        dec = dec || Culture.decimalSeparator;
        grp = grp || Culture.get_groupSeperator();
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
    Q.formatNumber = formatNumber;
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
    var isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    function parseInteger(s) {
        s = trim(s.toString());
        var ts = Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }
    Q.parseInteger = parseInteger;
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
            dateOrder = dateOrder || Culture.dateOrder;
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
    Q.parseDate = parseDate;
    function parseDecimal(s) {
        if (s == null)
            return null;
        s = trim(s.toString());
        if (s.length == 0)
            return null;
        var ts = Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;
        return parseFloat(s.toString().replace(Culture.decimalSeparator, '.'));
    }
    Q.parseDecimal = parseDecimal;
    function splitDateString(s) {
        s = trim(s);
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
    Q.splitDateString = splitDateString;
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
    Q.parseISODateTime = parseISODateTime;
    function parseHourAndMin(value) {
        var v = trim(value);
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
    Q.parseHourAndMin = parseHourAndMin;
    function parseDayHourAndMin(s) {
        var days;
        var v = trim(s);
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
    Q.parseDayHourAndMin = parseDayHourAndMin;
    function parseQueryString(s) {
        var qs;
        if (s === undefined)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        var result = {};
        var parts = qs.split('&');
        for (var i = 0; i < parts.length; i++) {
            var pair = parts[i].split('=');
            var name_1 = decodeURIComponent(pair[0]);
            result[name_1] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name_1);
        }
        return result;
    }
    Q.parseQueryString = parseQueryString;
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
    Q.turkishLocaleCompare = turkishLocaleCompare;
    function turkishLocaleToUpper(a) {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }
    Q.turkishLocaleToUpper = turkishLocaleToUpper;
    function postToService(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/services/' + options.service))
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
    Q.postToService = postToService;
    function postToUrl(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', resolveUrl(options.url))
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
    Q.postToUrl = postToUrl;
    if ($.fn.button && $.fn.button.noConflict) {
        var btn = $.fn.button.noConflict();
        $.fn.btn = btn;
    }
    $.fn.flexHeightOnly = function (flexY) {
        if (flexY === void 0) { flexY = 1; }
        return this.flexWidthHeight(0, flexY);
    };
    $.fn.flexWidthOnly = function (flexX) {
        if (flexX === void 0) { flexX = 1; }
        return this.flexWidthHeight(flexX, 0);
    };
    $.fn.flexWidthHeight = function (flexX, flexY) {
        if (flexX === void 0) { flexX = 1; }
        if (flexY === void 0) { flexY = 1; }
        return this.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
    };
    $.fn.flexX = function (flexX) {
        return this.data('flex-x', flexX);
    };
    $.fn.flexY = function (flexY) {
        return this.data('flex-y', flexY);
    };
    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    function deepClone(arg1) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
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
                            target[name] = deepClone(clone, copy);
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
    Q.deepClone = deepClone;
    function enumerateTypes(global, namespaces, callback) {
        function scan(root, fullName, depth) {
            if (!root)
                return;
            if ($.isArray(root) ||
                root instanceof Date)
                return;
            var t = typeof (root);
            if (t == "string" ||
                t == "number")
                return;
            if ($.isFunction(root))
                callback(root, fullName);
            if (depth > 3)
                return;
            for (var _i = 0, _a = Object.getOwnPropertyNames(root); _i < _a.length; _i++) {
                var k = _a[_i];
                if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                    continue;
                if (k.indexOf('$') >= 0)
                    continue;
                if (k == "prototype")
                    continue;
                scan(root[k], fullName + '.' + k, depth + 1);
            }
        }
        for (var _i = 0, namespaces_1 = namespaces; _i < namespaces_1.length; _i++) {
            var nsRoot = namespaces_1[_i];
            if (nsRoot == null || !nsRoot.length) {
                continue;
            }
            if (nsRoot.indexOf('.') >= 0) {
                var g = global;
                var parts = nsRoot.split('.');
                for (var _a = 0, parts_1 = parts; _a < parts_1.length; _a++) {
                    var p = parts_1[_a];
                    if (!p.length)
                        continue;
                    g = g[p];
                    if (!g)
                        continue;
                }
                scan(g, nsRoot, 0);
            }
            scan(global[nsRoot], nsRoot, 0);
        }
    }
    var ErrorHandling;
    (function (ErrorHandling) {
        function showServiceError(error) {
            var msg;
            if (error == null) {
                msg = '??ERROR??';
            }
            else {
                msg = error.Message;
                if (msg == null) {
                    msg = error.Code;
                }
            }
            Q.alert(msg);
        }
        ErrorHandling.showServiceError = showServiceError;
    })(ErrorHandling = Q.ErrorHandling || (Q.ErrorHandling = {}));
    var Config;
    (function (Config) {
        Config.applicationPath = '/';
        Config.emailAllowOnlyAscii = true;
        Config.rootNamespaces = ['Serenity'];
        Config.notLoggedInHandler = null;
        var pathLink = $('link#ApplicationPath');
        if (pathLink.length > 0) {
            Config.applicationPath = pathLink.attr('href');
        }
    })(Config = Q.Config || (Q.Config = {}));
    var Culture;
    (function (Culture) {
        Culture.decimalSeparator = '.';
        Culture.dateSeparator = '/';
        Culture.dateOrder = 'dmy';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        function get_groupSeperator() {
            return ((Culture.decimalSeparator === ',') ? '.' : ',');
        }
        Culture.get_groupSeperator = get_groupSeperator;
        ;
        var s = Q.trimToNull($('script#ScriptCulture').html());
        if (s != null) {
            var sc = $.parseJSON(s);
            if (sc.DecimalSeparator != null)
                Culture.decimalSeparator = sc.DecimalSeparator;
            if (sc.DateSeparator != null)
                Culture.dateSeparator = sc.DateSeparator;
            if (sc.DateOrder != null)
                Culture.dateOrder = sc.DateOrder;
            if (sc.DateFormat != null)
                Culture.dateFormat = sc.DateFormat;
            if (sc.DateTimeFormat != null)
                Culture.dateTimeFormat = sc.DateTimeFormat;
        }
    })(Culture = Q.Culture || (Q.Culture = {}));
    var Lookup = (function () {
        function Lookup(options, items) {
            this.items = [];
            this.itemById = {};
            options = options || {};
            this.textFormatter = options.textFormatter;
            this.idField = options.idField;
            this.parentIdField = options.parentIdField;
            this.textField = options.textField;
            this.textFormatter = options.textFormatter;
            if (items != null)
                this.update(items);
        }
        Lookup.prototype.update = function (value) {
            this.items = [];
            this.itemById = {};
            if (value) {
                for (var _i = 0, value_1 = value; _i < value_1.length; _i++) {
                    var k = value_1[_i];
                    this.items.push(k);
                }
            }
            var idField = this.idField;
            if (!Q.isEmptyOrNull(idField)) {
                for (var _a = 0, _b = this.items; _a < _b.length; _a++) {
                    var r = _b[_a];
                    var v = r[idField];
                    if (v != null) {
                        this.itemById[v] = r;
                    }
                }
            }
        };
        Lookup.prototype.get_idField = function () {
            return this.idField;
        };
        Lookup.prototype.get_parentIdField = function () {
            return this.parentIdField;
        };
        Lookup.prototype.get_textField = function () {
            return this.textField;
        };
        Lookup.prototype.get_textFormatter = function () {
            return this.textFormatter;
        };
        Lookup.prototype.get_itemById = function () {
            return this.itemById;
        };
        Lookup.prototype.get_items = function () {
            return this.items;
        };
        return Lookup;
    }());
    Q.Lookup = Lookup;
    var LT = (function () {
        function LT(key) {
            this.key = key;
        }
        LT.add = function (obj, pre) {
            if (!obj) {
                return;
            }
            pre = pre || '';
            for (var _i = 0, _a = Object.keys(obj); _i < _a.length; _i++) {
                var k = _a[_i];
                var actual = pre + k;
                var o = obj[k];
                if (typeof (o) === 'object') {
                    LT.add(o, actual + '.');
                }
                else {
                    LT.$table[actual] = o;
                }
            }
        };
        LT.prototype.get = function () {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        };
        LT.prototype.toString = function () {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        };
        LT.$table = {};
        LT.empty = new LT('');
        LT.initializeTextClass = function (type, prefix) {
            var $t1 = Q.arrayClone(Object.keys(type));
            for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                var member = $t1[$t2];
                var value = type[member];
                if (value instanceof LT) {
                    var lt = value;
                    var key = prefix + member;
                    LT.$table[key] = lt.$key;
                    type[member] = new LT(key);
                }
            }
        };
        LT.getDefault = function (key, defaultText) {
            var t = LT.$table[key];
            if (t == null) {
                t = defaultText;
                if (t == null) {
                    t = key || '';
                }
            }
            return t;
        };
        return LT;
    }());
    Q.LT = LT;
    var ScriptData;
    (function (ScriptData) {
        var registered = {};
        var loadedData = {};
        function bindToChange(name, regClass, onChange) {
            $(document.body).bind('scriptdatachange.' + regClass, function (e, s) {
                if (s == name) {
                    onChange();
                }
            });
        }
        ScriptData.bindToChange = bindToChange;
        function triggerChange(name) {
            $(document.body).triggerHandler('scriptdatachange', [name]);
        }
        ScriptData.triggerChange = triggerChange;
        function unbindFromChange(regClass) {
            $(document.body).unbind('scriptdatachange.' + regClass);
        }
        ScriptData.unbindFromChange = unbindFromChange;
        function syncLoadScript(url) {
            $.ajax({ async: false, cache: true, type: 'GET', url: url, data: null, dataType: 'script' });
        }
        function loadScriptAsync(url) {
            return RSVP.resolve().then(function () {
                Q.blockUI(null);
                return RSVP.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always(function () {
                    Q.blockUndo();
                }));
            }, null);
        }
        function loadScriptData(name) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            name = name + '.js?' + registered[name];
            syncLoadScript(Q.resolveUrl('~/DynJS.axd/') + name);
        }
        function loadScriptDataAsync(name) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                name = name + '.js?' + registered[name];
                return loadScriptAsync(Q.resolveUrl('~/DynJS.axd/') + name);
            }, null);
        }
        function ensure(name) {
            var data = loadedData[name];
            if (data == null) {
                loadScriptData(name);
            }
            data = loadedData[name];
            if (data == null)
                throw new Error(Q.format("Can't load script data: {0}!", name));
            return data;
        }
        ScriptData.ensure = ensure;
        function ensureAsync(name) {
            return RSVP.resolve().then(function () {
                var data = loadedData[name];
                if (data != null) {
                    return RSVP.resolve(data);
                }
                return loadScriptDataAsync(name).then(function () {
                    data = loadedData[name];
                    if (data == null) {
                        throw new Error(Q.format("Can't load script data: {0}!", name));
                    }
                    return data;
                }, null);
            }, null);
        }
        ScriptData.ensureAsync = ensureAsync;
        function reload(name) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            loadScriptData(name);
            var data = loadedData[name];
            return data;
        }
        ScriptData.reload = reload;
        function reloadAsync(name) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                registered[name] = (new Date()).getTime().toString();
                return loadScriptDataAsync(name).then(function () {
                    return loadedData[name];
                }, null);
            }, null);
        }
        ScriptData.reloadAsync = reloadAsync;
        function canLoad(name) {
            return (loadedData[name] != null || registered[name] != null);
        }
        ScriptData.canLoad = canLoad;
        function setRegisteredScripts(scripts) {
            registered = {};
            for (var k in scripts) {
                registered[k] = scripts[k].toString();
            }
        }
        ScriptData.setRegisteredScripts = setRegisteredScripts;
        function set(name, value) {
            loadedData[name] = value;
            triggerChange(name);
        }
        ScriptData.set = set;
    })(ScriptData = Q.ScriptData || (Q.ScriptData = {}));
    (function (global) {
        if (typeof RSVP !== undefined) {
            RSVP.on && RSVP.on(function (e) {
                log(e);
                log((e.get_stack && e.get_stack()) || e.stack);
            });
        }
        // fake assembly for typescript apps
        ss.initAssembly({}, 'App', {});
        // for backward compability, avoid!
        global.Q$Externals = Q;
        global.Q$Config = Q.Config;
        global.Q$Culture = Q.Culture;
        global.Q$Lookup = Q.Lookup;
        global.Q$ScriptData = Q.ScriptData;
        global.Q$LT = Q.LT;
        function initializeTypes() {
            enumerateTypes(global, Q.Config.rootNamespaces, function (obj, fullName) {
                // probably Saltaralle class
                if (obj.hasOwnProperty("__typeName") &&
                    !obj.__register)
                    return;
                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    fullName.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter];
                }
                if (!obj.__class) {
                    var baseType = ss.getBaseType(obj);
                    if (baseType.__class)
                        obj.__class = true;
                }
                if (obj.__class) {
                    obj.__typeName = fullName;
                    if (!obj.__assembly) {
                        obj.__assembly = ss.__assemblies['App'];
                    }
                    obj.__assembly.__types[fullName] = obj;
                }
                delete obj.__register;
            });
        }
        $(function () {
            initializeTypes();
            setMobileDeviceMode();
            $(global).bind('resize', function () {
                setMobileDeviceMode();
            });
        });
    })(window || {});
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var ColumnsKeyAttribute = (function () {
        function ColumnsKeyAttribute(value) {
            this.value = value;
        }
        return ColumnsKeyAttribute;
    }());
    Serenity.ColumnsKeyAttribute = ColumnsKeyAttribute;
    var DialogTypeAttribute = (function () {
        function DialogTypeAttribute(value) {
            this.value = value;
        }
        return DialogTypeAttribute;
    }());
    Serenity.DialogTypeAttribute = DialogTypeAttribute;
    var EditorAttribute = (function () {
        function EditorAttribute() {
        }
        return EditorAttribute;
    }());
    Serenity.EditorAttribute = EditorAttribute;
    var ElementAttribute = (function () {
        function ElementAttribute(value) {
            this.value = value;
        }
        return ElementAttribute;
    }());
    Serenity.ElementAttribute = ElementAttribute;
    var EntityTypeAttribute = (function () {
        function EntityTypeAttribute(value) {
            this.value = value;
        }
        return EntityTypeAttribute;
    }());
    Serenity.EntityTypeAttribute = EntityTypeAttribute;
    var EnumKeyAttribute = (function () {
        function EnumKeyAttribute(value) {
            this.value = value;
        }
        return EnumKeyAttribute;
    }());
    Serenity.EnumKeyAttribute = EnumKeyAttribute;
    var FlexifyAttribute = (function () {
        function FlexifyAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return FlexifyAttribute;
    }());
    Serenity.FlexifyAttribute = FlexifyAttribute;
    var FormKeyAttribute = (function () {
        function FormKeyAttribute(value) {
            this.value = value;
        }
        return FormKeyAttribute;
    }());
    Serenity.FormKeyAttribute = FormKeyAttribute;
    var GeneratedCodeAttribute = (function () {
        function GeneratedCodeAttribute(origin) {
            this.origin = origin;
        }
        return GeneratedCodeAttribute;
    }());
    Serenity.GeneratedCodeAttribute = GeneratedCodeAttribute;
    var IdPropertyAttribute = (function () {
        function IdPropertyAttribute(value) {
            this.value = value;
        }
        return IdPropertyAttribute;
    }());
    Serenity.IdPropertyAttribute = IdPropertyAttribute;
    var IsActivePropertyAttribute = (function () {
        function IsActivePropertyAttribute(value) {
            this.value = value;
        }
        return IsActivePropertyAttribute;
    }());
    Serenity.IsActivePropertyAttribute = IsActivePropertyAttribute;
    var ItemNameAttribute = (function () {
        function ItemNameAttribute(value) {
            this.value = value;
        }
        return ItemNameAttribute;
    }());
    Serenity.ItemNameAttribute = ItemNameAttribute;
    var LocalTextPrefixAttribute = (function () {
        function LocalTextPrefixAttribute(value) {
            this.value = value;
        }
        return LocalTextPrefixAttribute;
    }());
    Serenity.LocalTextPrefixAttribute = LocalTextPrefixAttribute;
    var MaximizableAttribute = (function () {
        function MaximizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return MaximizableAttribute;
    }());
    Serenity.MaximizableAttribute = MaximizableAttribute;
    var NamePropertyAttribute = (function () {
        function NamePropertyAttribute(value) {
            this.value = value;
        }
        return NamePropertyAttribute;
    }());
    Serenity.NamePropertyAttribute = NamePropertyAttribute;
    var OptionAttribute = (function () {
        function OptionAttribute() {
        }
        return OptionAttribute;
    }());
    Serenity.OptionAttribute = OptionAttribute;
    var OptionsTypeAttribute = (function () {
        function OptionsTypeAttribute(value) {
            this.value = value;
        }
        return OptionsTypeAttribute;
    }());
    Serenity.OptionsTypeAttribute = OptionsTypeAttribute;
    var PanelAttribute = (function () {
        function PanelAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return PanelAttribute;
    }());
    Serenity.PanelAttribute = PanelAttribute;
    var ResizableAttribute = (function () {
        function ResizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResizableAttribute;
    }());
    Serenity.ResizableAttribute = ResizableAttribute;
    var ResponsiveAttribute = (function () {
        function ResponsiveAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResponsiveAttribute;
    }());
    Serenity.ResponsiveAttribute = ResponsiveAttribute;
    var ServiceAttribute = (function () {
        function ServiceAttribute(value) {
            this.value = value;
        }
        return ServiceAttribute;
    }());
    Serenity.ServiceAttribute = ServiceAttribute;
    function Criteria(field) {
        return [field];
    }
    Serenity.Criteria = Criteria;
    var Criteria;
    (function (Criteria) {
        var C = Criteria;
        function isEmpty(c) {
            return c == null ||
                c.length === 0 ||
                (c.length === 1 &&
                    typeof c[0] == "string" &&
                    c[0].length === 0);
        }
        Criteria.isEmpty = isEmpty;
        function join(c1, op, c2) {
            if (C.isEmpty(c1))
                return c2;
            if (C.isEmpty(c2))
                return c1;
            return [c1, op, c2];
        }
        Criteria.join = join;
        function paren(c) {
            return C.isEmpty(c) ? c : ['()', c];
        }
        Criteria.paren = paren;
        function and(c1, c2) {
            var rest = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                rest[_i - 2] = arguments[_i];
            }
            var result = join(c1, 'and', c2);
            if (rest) {
                for (var _a = 0, rest_1 = rest; _a < rest_1.length; _a++) {
                    var k = rest_1[_a];
                    result = join(result, 'and', k);
                }
            }
            return result;
        }
        Criteria.and = and;
        function or(c1, c2) {
            var rest = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                rest[_i - 2] = arguments[_i];
            }
            var result = join(c1, 'or', c2);
            if (rest) {
                for (var _a = 0, rest_2 = rest; _a < rest_2.length; _a++) {
                    var k = rest_2[_a];
                    result = join(result, 'or', k);
                }
            }
            return result;
        }
        Criteria.or = or;
    })(Criteria = Serenity.Criteria || (Serenity.Criteria = {}));
    var Decorators;
    (function (Decorators) {
        function addAttribute(type, attr) {
            var old = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
        Decorators.addAttribute = addAttribute;
        function addMemberAttr(type, memberName, attr) {
            var old = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.members = type.__metadata.members || [];
            var member = undefined;
            for (var _i = 0, _a = type.__metadata.members; _i < _a.length; _i++) {
                var m = _a[_i];
                if (m.name == memberName) {
                    member = m;
                    break;
                }
            }
            if (!member) {
                member = { name: memberName };
                type.__metadata.members.push(member);
            }
            member.attr = member.attr || [];
            member.attr.push(attr);
        }
        Decorators.addMemberAttr = addMemberAttr;
        function columnsKey(value) {
            return function (target) {
                addAttribute(target, new ColumnsKeyAttribute(value));
            };
        }
        Decorators.columnsKey = columnsKey;
        function dialogType(value) {
            return function (target) {
                addAttribute(target, new DialogTypeAttribute(value));
            };
        }
        Decorators.dialogType = dialogType;
        function editor(key) {
            return function (target) {
                var attr = new EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                addAttribute(target, attr);
            };
        }
        Decorators.editor = editor;
        function element(value) {
            return function (target) {
                addAttribute(target, new ElementAttribute(value));
            };
        }
        Decorators.element = element;
        function entityType(value) {
            return function (target) {
                addAttribute(target, new EntityTypeAttribute(value));
            };
        }
        Decorators.entityType = entityType;
        function enumKey(value) {
            return function (target) {
                addAttribute(target, new EnumKeyAttribute(value));
            };
        }
        Decorators.enumKey = enumKey;
        function flexify(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new FlexifyAttribute(value));
            };
        }
        Decorators.flexify = flexify;
        function formKey(value) {
            return function (target) {
                addAttribute(target, new FormKeyAttribute(value));
            };
        }
        Decorators.formKey = formKey;
        function generatedCode(origin) {
            return function (target) {
                addAttribute(target, new GeneratedCodeAttribute(origin));
            };
        }
        Decorators.generatedCode = generatedCode;
        function idProperty(value) {
            return function (target) {
                addAttribute(target, new IdPropertyAttribute(value));
            };
        }
        Decorators.idProperty = idProperty;
        function registerClass(intf, asm) {
            return function (target) {
                target.__register = true;
                target.__class = true;
                target.__assembly = asm || ss.__assemblies['App'];
                if (intf)
                    target.__interfaces = intf;
            };
        }
        Decorators.registerClass = registerClass;
        function registerEditor(intf, asm) {
            return registerClass(intf, asm);
        }
        Decorators.registerEditor = registerEditor;
        function registerFormatter(intf, asm) {
            if (intf === void 0) { intf = [Serenity.ISlickFormatter]; }
            return registerClass(intf, asm);
        }
        Decorators.registerFormatter = registerFormatter;
        function filterable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.FilterableAttribute(value));
            };
        }
        Decorators.filterable = filterable;
        function itemName(value) {
            return function (target) {
                addAttribute(target, new ItemNameAttribute(value));
            };
        }
        Decorators.itemName = itemName;
        function isActiveProperty(value) {
            return function (target) {
                addAttribute(target, new IsActivePropertyAttribute(value));
            };
        }
        Decorators.isActiveProperty = isActiveProperty;
        function localTextPrefix(value) {
            return function (target) {
                addAttribute(target, new LocalTextPrefixAttribute(value));
            };
        }
        Decorators.localTextPrefix = localTextPrefix;
        function maximizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new MaximizableAttribute(value));
            };
        }
        Decorators.maximizable = maximizable;
        function nameProperty(value) {
            return function (target) {
                addAttribute(target, new NamePropertyAttribute(value));
            };
        }
        Decorators.nameProperty = nameProperty;
        function option() {
            return function (target, propertyKey) {
                addMemberAttr(target.constructor, propertyKey, new OptionAttribute());
            };
        }
        Decorators.option = option;
        function optionsType(value) {
            return function (target) {
                addAttribute(target, new OptionsTypeAttribute(value));
            };
        }
        Decorators.optionsType = optionsType;
        function panel(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new PanelAttribute(value));
            };
        }
        Decorators.panel = panel;
        function resizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new ResizableAttribute(value));
            };
        }
        Decorators.resizable = resizable;
        function responsive(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new ResponsiveAttribute(value));
            };
        }
        Decorators.responsive = responsive;
        function service(value) {
            return function (target) {
                addAttribute(target, new ServiceAttribute(value));
            };
        }
        Decorators.service = service;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    var TabsExtensions;
    (function (TabsExtensions) {
        function setDisabled(tabs, tabKey, isDisabled) {
            if (!tabs)
                return;
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }
            tabs.tabs(isDisabled ? 'disable' : 'enable', index);
        }
        TabsExtensions.setDisabled = setDisabled;
        function activeTabKey(tabs) {
            var href = tabs.children('ul')
                .children('li')
                .eq(tabs.tabs('option', 'active'))
                .children('a')
                .attr('href')
                .toString();
            var prefix = '_Tab';
            var lastIndex = href.lastIndexOf(prefix);
            if (lastIndex >= 0) {
                href = href.substr(lastIndex + prefix.length);
            }
            return href;
        }
        TabsExtensions.activeTabKey = activeTabKey;
        function indexByKey(tabs) {
            var indexByKey = tabs.data('indexByKey');
            if (!indexByKey) {
                indexByKey = {};
                tabs.children('ul').children('li').children('a').each(function (index, el) {
                    var href = el.getAttribute('href').toString();
                    var prefix = '_Tab';
                    var lastIndex = href.lastIndexOf(prefix);
                    if (lastIndex >= 0) {
                        href = href.substr(lastIndex + prefix.length);
                    }
                    indexByKey[href] = index;
                });
                tabs.data('indexByKey', indexByKey);
            }
            return indexByKey;
        }
        TabsExtensions.indexByKey = indexByKey;
    })(TabsExtensions = Serenity.TabsExtensions || (Serenity.TabsExtensions = {}));
    var LazyLoadHelper;
    (function (LazyLoadHelper) {
        var autoIncrement = 0;
        function executeOnceWhenShown(element, callback) {
            autoIncrement++;
            var eventClass = 'ExecuteOnceWhenShown' + autoIncrement;
            var executed = false;
            if (element.is(':visible')) {
                callback();
            }
            else {
                var uiTabs = element.closest('.ui-tabs');
                if (uiTabs.length > 0) {
                    uiTabs.bind('tabsshow.' + eventClass, function (e) {
                        if (element.is(':visible')) {
                            uiTabs.unbind('tabsshow.' + eventClass);
                            if (!executed) {
                                executed = true;
                                element.unbind('shown.' + eventClass);
                                callback();
                            }
                        }
                    });
                }
                var dialog;
                if (element.hasClass('ui-dialog')) {
                    dialog = element.children('.ui-dialog-content');
                }
                else {
                    dialog = element.closest('.ui-dialog-content');
                }
                if (dialog.length > 0) {
                    dialog.bind('dialogopen.' + eventClass, function () {
                        dialog.unbind('dialogopen.' + eventClass);
                        if (element.is(':visible') && !executed) {
                            executed = true;
                            element.unbind('shown.' + eventClass);
                            callback();
                        }
                    });
                }
                element.bind('shown.' + eventClass, function () {
                    if (element.is(':visible')) {
                        element.unbind('shown.' + eventClass);
                        if (!executed) {
                            executed = true;
                            callback();
                        }
                    }
                });
            }
        }
        LazyLoadHelper.executeOnceWhenShown = executeOnceWhenShown;
        function executeEverytimeWhenShown(element, callback, callNowIfVisible) {
            autoIncrement++;
            var eventClass = 'ExecuteEverytimeWhenShown' + autoIncrement;
            var wasVisible = element.is(':visible');
            if (wasVisible && callNowIfVisible) {
                callback();
            }
            var check = function (e) {
                if (element.is(':visible')) {
                    if (!wasVisible) {
                        wasVisible = true;
                        callback();
                    }
                }
                else {
                    wasVisible = false;
                }
            };
            var uiTabs = element.closest('.ui-tabs');
            if (uiTabs.length > 0) {
                uiTabs.bind('tabsactivate.' + eventClass, check);
            }
            var dialog = element.closest('.ui-dialog-content');
            if (dialog.length > 0) {
                dialog.bind('dialogopen.' + eventClass, check);
            }
            element.bind('shown.' + eventClass, check);
        }
        LazyLoadHelper.executeEverytimeWhenShown = executeEverytimeWhenShown;
    })(LazyLoadHelper = Serenity.LazyLoadHelper || (Serenity.LazyLoadHelper = {}));
})(Serenity || (Serenity = {}));

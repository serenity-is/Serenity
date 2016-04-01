declare interface RSVP<TResult> {
}

declare namespace Serenity {
    interface PostToServiceOptions {
        url?: string;
        service?: string;
        target?: string;
        request: any;
    }

    interface PostToUrlOptions {
        url?: string;
        target?: string;
        params: any;
    }

    interface CommonDialogOptions extends JQueryUI.DialogOptions {
        onOpen?: () => void;
        onClose?: () => void;
        htmlEncode?: boolean;
    }

    interface AlertOptions extends CommonDialogOptions {
        okButton?: string;
    }

    interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string;
        noButton?: string;
        cancelButton?: string;
        onCancel?: () => void;
        onNo?: () => void;
    }

    interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
    }

    interface ServiceResponse {
        Error?: ServiceError;
    }

    interface ServiceRequest {
    }

    interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
    }

    interface SaveResponse extends ServiceResponse {
    }

    interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
        Localizations?: { [key: string]: TEntity };
    }

    interface DeleteRequest extends ServiceRequest {
        EntityId?: any;
    }

    interface DeleteResponse extends ServiceResponse {
    }

    interface UndeleteRequest extends ServiceRequest {
        EntityId?: any;
    }

    interface UndeleteResponse extends ServiceResponse {
    }

    interface ListRequest extends ServiceRequest {
        Skip?: number;
        Take?: number;
        Sort?: string[];
    }

    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        TotalCount: number;
        Skip: number;
        Take: number;
    }

    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
    }

    interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity: TEntity;
    }

    interface RetrieveLocalizationRequest extends RetrieveRequest {
    }

    interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: { [key: string]: TEntity };
    }

    interface CheckTreeItem {
    }

    interface PropertyItem {
    }

    class ISlickFormatter {
    }

    class ScriptContext {
    }

    class PrefixedContext extends ScriptContext {
        constructor(prefix: string);
        w(id: string, type: Function): any;
    }

    class Widget<TOptions> {
        constructor(element: JQuery, options?: TOptions);
    }

    class StringEditor extends Widget<any> {
    }

    class Select2Editor<TItem, TOptions> {
    }

    class CheckTreeEditor<TItem, TOptions> {
    }

    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain: boolean;
    }

    class EmailEditor extends Widget<EmailEditorOptions> {
    }

    class PasswordEditor extends StringEditor {
    }

    class DateEditor extends Widget<any> {
    }

    class DateTimeEditor extends Widget<any> {
    }

    interface LookupEditorOptions {
    }

    class LookupEditorBase<TOptions, TItem> extends Widget<TOptions> {
    }

    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    }

    class ImageUploadEditor extends Widget<any> {
    }

    class BooleanEditor extends Widget<any> {
    }

    class IntegerEditor extends Widget<any> {
    }

    class DecimalEditor extends Widget<any> {
    }

    interface HtmlContentEditorOptions {
    }

    class HtmlContentEditor {
    }

    class TemplatedWidget<TOptions> extends Widget<TOptions> {
    }

    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
    }

    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
    }

    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
    }

    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
    }

    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }

    class DataGrid<TItem, TOptions> {
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }

    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }

    class FilterStore {
    }

    interface CKEditorConfig {
    }

    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.Data.RemoteView;
        getFilterStore(): Serenity.FilterStore;
    }

    interface ToolButton {
    }

    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }

    namespace CustomValidation {
        function registerValidationMethods(): void;
    }

    interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }

    interface IFrameDialogOptions {
        html?: string;
    }
}

interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}

interface JQBlockUIOptions {
    useTimeout?: boolean;
}

declare module RSVP {
    function on(handler: (e: any) => void);
    function resolve(): Thenable<any>;
}

declare namespace Slick {

    interface FormatterContext {
        row: number;
        cell: number;
        value: any;
        column: any;
    }

    interface Formatter {
        format(ctx: FormatterContext): string;
    }

    class Grid {
    }

    interface GridOptions {
    }

    interface Column {
    }
}

interface Toastr {
    getContainer(options?: ToastrOptions, create?: boolean): JQuery;
}

declare namespace ss {
    interface AssemblyReg {
        name: string;
        __types: ClassReg[];
    }

    interface ClassReg {
        __class: boolean;
        __assembly: AssemblyReg;
        __interfaces: any[];
    }

    let __assemblies: { [name: string]: AssemblyReg; };

    class Exception {
        constructor(msg: string);
    }

    class NotSupportedException extends Exception {
        constructor(msg: string);
    }

    function arrayClone<T>(a: T[]): T[];
    function formatString(msg: string, ...prm: any[]): string;
    function getEnumerator(e: any): any;
    function getBaseType(e: any): any;
    function initAssembly(obj, name: string, res: { [name: string]: any });
    function padLeftString(s: string, m: number, n: number);
    function replaceAllString(s: string, f: string, r: string): string;
    function startsWithString(s: string, search: string): boolean;
}

namespace Q {
    import S = Serenity;

    export function alert(message: string, options?: S.AlertOptions) {
        let dialog;
        options = <S.AlertOptions>$.extend({
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
            let buttons = [];
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

    export function confirm(message: string, onYes: () => void, options?: S.ConfirmOptions): void {
        let dialog;
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
        let clicked = false;
        if (!options.buttons) {
            let buttons = [];
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

    export function information(message: string, onOk: () => void, options?: S.ConfirmOptions) {
        confirm(message, onOk, $.extend<S.ConfirmOptions>(
            {
                title: text("Dialogs.InformationTitle"),
                dialogClass: "s-MessageDialog s-InformationDialog",
                yesButton: text("Dialogs.OkButton"),
                noButton: null,
            }, options));
    }

    export function warning(message: string, options?: S.AlertOptions) {
        alert(message, $.extend<S.AlertOptions>(
            {
                title: text("Dialogs.WarningTitle"),
                dialogClass: "s-MessageDialog s-WarningDialog"
            }, options));
    }

    export function iframeDialog(options: S.IFrameDialogOptions) {
        let doc;
        let e = $('<div><iframe></iframe></div>');
        let settings: S.IFrameDialogOptions = $.extend(<JQueryUI.DialogOptions>{
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: text('Dialogs.AlertTitle'),
            open: function () {
                doc = (<HTMLIFrameElement>(e.find('iframe').css({
                    border: 'none',
                    width: '100%',
                    height: '100%'
                })[0])).contentDocument;
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

    export function toId(id: any): any {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15 || !(/^\d+$/.test(id)))
            return id;
        let v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }

    function log(m: any) {
        (<any>window).console && (<any>window).console.log(m);
    }

    let blockUICount: number = 0;

    function blockUIWithCheck(opt: JQBlockUIOptions) {
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
    export function blockUI(options: JQBlockUIOptions) {
        options = $.extend<JQBlockUIOptions>({
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

    export function blockUndo() {
        if (blockUICount > 1) {
            blockUICount--;
            return;
        }

        blockUICount--;
        $.unblockUI({ fadeOut: 0 });
    }

    export function formatDate(date: Date, format?: string) {
        if (!date) {
            return '';
        }

        if (format == null) {
            format = Culture.dateFormat;
        }

        let pad = function (i) {
            return ss.padLeftString(i.toString(), 2, 48);
        };

        return format.replace(new RegExp('dd?|MM?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/', 'g'),
            function (fmt) {
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
                    case 'fff': return ss.padLeftString(date.getMilliseconds().toString(), 3, 48);
                    case 'zzz':
                    case 'zz':
                    case 'z': return '';
                    default: return fmt;
                }
            }
        );
    }

    function htmlEncoder(a: string): string {
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
    export function htmlEncode(s: any): string {
        let text = (s == null ? '' : s.toString());
        if ((new RegExp('[><&]', 'g')).test(text)) {
            return text.replace(new RegExp('[><&]', 'g'), htmlEncoder);
        }
        return text;
    }

    export function newBodyDiv(): JQuery {
        return $('<div/>').appendTo(document.body);
    }

    export function addOption(select: JQuery, key: string, text: string) {
        $('<option/>').val(key).text(text).appendTo(select);
    }

    export function clearOptions(select: JQuery) {
        select.html('');
    }

    export function addEmptyOption(select: JQuery) {
        addOption(select, '', text("Controls.SelectEditor.EmptyItemText"));
    }

    export function trim(s: string) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }

    export function isEmptyOrNull(s: string) {
        return s == null || s.length === 0;
    }

    export function trimToNull(s: string) {
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

    export function isTrimmedEmpty(s: string) {
        return trimToNull(s) == null;
    }

    export function trimToEmpty(s: string) {
        if (s == null || s.length === 0) {
            return '';
        }

        return trim(s);
    }

    export function findElementWithRelativeId(element: JQuery, relativeId: string) {
        let elementId = element.attr('id');
        if (isEmptyOrNull(elementId)) {
            return $('#' + relativeId);
        }

        let result = $(elementId + relativeId);
        if (result.length > 0) {
            return result;
        }

        result = $(elementId + '_' + relativeId);

        if (result.length > 0) {
            return result;
        }

        while (true) {
            let idx = elementId.lastIndexOf('_');
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

    export function outerHtml(element: JQuery) {
        return $('<i/>').append(element.eq(0).clone()).html();
    }

    export function layoutFillHeightValue(element: JQuery) {
        let h = 0;
        element.parent().children().not(element).each(function (i, e) {
            let q = $(e);
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

    export function layoutFillHeight(element: JQuery) {
        let h = layoutFillHeightValue(element);
        let n = h + 'px';
        if (element.css('height') != n) {
            element.css('height', n);
        }
    }

    export function initFullHeightGridPage(gridDiv: JQuery) {
        $('body').addClass('full-height-page');
        gridDiv.addClass('responsive-height');

        let layout = function () {
            let inPageContent = gridDiv.parent().hasClass('page-content') ||
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
        else if ((window as any).Metronic) {
            (window as any).Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }

        layout();
    }

    export function addFullHeightResizeHandler(handler) {
        $('body').addClass('full-height-page');
        let layout = function () {
            let avail;
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

        if ((window as any).Metronic) {
            (window as any).Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }

        layout();
    }

    export function triggerLayoutOnShow(element) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }

    export function autoFullHeight(element: JQuery) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }

    export function setMobileDeviceMode() {
        let isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            window.matchMedia('(max-width: 767px)').matches;

        let body = $(document.body);
        if (body.hasClass('mobile-device')) {
            if (!isMobile) {
                body.removeClass('mobile-device');
            }
        }
        else if (isMobile) {
            body.addClass('mobile-device');
        }
    }

    export let defaultNotifyOptions: ToastrOptions = {
        timeOut: 3000,
        showDuration: 250,
        hideDuration: 500,
        extendedTimeOut: 500,
        positionClass: 'toast-top-full-width'
    }

    export function positionToastContainer(create: boolean) {
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

    function getToastrOptions(options: ToastrOptions) {
        options = $.extend<ToastrOptions>({}, defaultNotifyOptions, options);
        positionToastContainer(true);
        return options;
    }

    export function notifyWarning(message: string, title?: string, options?: ToastrOptions) {
        toastr.warning(message, title, getToastrOptions(options));
    }

    export function notifySuccess(message: string, title?: string, options?: ToastrOptions) {
        toastr.success(message, title, getToastrOptions(options));
    }

    export function notifyInfo(message: string, title?: string, options?: ToastrOptions) {
        toastr.info(message, title, getToastrOptions(options));
    }

    export function notifyError(message: string, title?: string, options?: ToastrOptions) {
        toastr.error(message, title, getToastrOptions(options));
    }

    export function getRemoteData(key) {
        return ScriptData.ensure('RemoteData.' + key);
    }

    export function getRemoteDataAsync(key) {
        return ScriptData.ensureAsync('RemoteData.' + key);
    }

    export function getLookup(key) {
        return ScriptData.ensure('Lookup.' + key);
    }

    export function getLookupAsync(key) {
        return ScriptData.ensureAsync('Lookup.' + key);
    }

    export function reloadLookup(key) {
        ScriptData.reload('Lookup.' + key);
    }

    export function reloadLookupAsync(key) {
        return ScriptData.reloadAsync('Lookup.' + key);
    }

    export function getColumns(key) {
        return ScriptData.ensure('Columns.' + key);
    }

    export function getColumnsAsync(key) {
        return ScriptData.ensureAsync('Columns.' + key);
    }

    export function getForm(key) {
        return ScriptData.ensure('Form.' + key);
    }

    export function getFormAsync(key) {
        return ScriptData.ensureAsync('Form.' + key);
    }

    export function getTemplate(key) {
        return ScriptData.ensure('Template.' + key);
    }

    export function getTemplateAsync(key) {
        return ScriptData.ensureAsync('Template.' + key);
    }

    export function canLoadScriptData(name) {
        return ScriptData.canLoad(name);
    }

    export function serviceCall<TResponse>(options: S.ServiceOptions<TResponse>) {
        let handleError = function (response: any) {
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

        options = $.extend<S.ServiceOptions<TResponse>>({
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

    export function serviceRequest<TResponse>(service: string, request?: any,
        onSuccess?: (response: TResponse) => void, options?: S.ServiceOptions<TResponse>) {
        return serviceCall($.extend<S.ServiceOptions<TResponse>>({
            service: service,
            request: request,
            onSuccess: onSuccess
        }, options));
    }

    export function setEquality(request, field, value) {
        if (request.EqualityFilter == null) {
            request.EqualityFilter = {};
        }
        request.EqualityFilter[field] = value;
    }

    export function toSingleLine(str: string) {
        return ss.replaceAllString(ss.replaceAllString(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }

    export function text(key: string): string {
        let t = LT.$table[key];
        if (t == null) {
            t = key || '';
        }
        return t;
    }

    export function tryGetText(key): string {
        return LT.$table[key];
    }

    export function resolveUrl(url) {
        if (url && url.substr(0, 2) === '~/') {
            return Config.applicationPath + url.substr(2);
        }

        return url;
    }

    export function zeroPad(n: number, digits: number): string {
        let s = n.toString();
        while (s.length < digits)
            s = "0" + digits;
        return s;
    }

    export function formatDayHourAndMin(n: number): string {
        if (n === 0)
            return '0';
        else if (!n)
            return '';
        let days = Math.floor(n / 24 / 60);
        let txt = "";
        if (days > 0) {
            txt += days.toString();
        }
        let mins = zeroPad(Math.floor((n % (24 * 60)) / (60)), 2) + ':' + zeroPad(n % 60, 2);
        if (mins != '00:00') {
            if (days > 0)
                txt += ".";
            txt += mins;
        }
        return txt;
    }

    export function formatISODateTimeUTC(d: Date): string {
        if (d == null)
            return "";
        let zeropad = function (num) { return ((num < 10) ? '0' : '') + num; };
        let str = d.getUTCFullYear() + "-" +
            zeropad(d.getUTCMonth() + 1) + "-" +
            zeropad(d.getUTCDate()) + "T" +
            zeropad(d.getUTCHours()) + ":" +
            zeropad(d.getUTCMinutes());
        let secs = Number(d.getUTCSeconds() + "." +
            ((d.getUTCMilliseconds() < 100) ? '0' : '') +
            zeropad(d.getUTCMilliseconds()));
        str += ":" + zeropad(secs) + "Z";
        return str;
    }

    export function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string {
        let neg = '-';
        if (isNaN(n)) {
            return null;
        }

        dec = dec || Culture.decimalSeparator;
        grp = grp || Culture.get_groupSeperator();

        let r = "";
        if (fmt.indexOf(".") > -1) {
            let dp = dec;
            let df = fmt.substring(fmt.lastIndexOf(".") + 1);
            n = roundNumber(n, df.length);
            let dv = n % 1;
            let ds = new String(dv.toFixed(df.length));
            ds = ds.substring(ds.lastIndexOf(".") + 1);
            for (let i = 0; i < df.length; i++) {
                if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                    dp += ds.charAt(i);
                    continue;
                }
                else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                    let notParsed = ds.substring(i);
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

        let ones = Math.floor(n);
        if (n < 0)
            ones = Math.ceil(n);
        let of = "";
        if (fmt.indexOf(".") == -1)
            of = fmt;
        else
            of = fmt.substring(0, fmt.indexOf("."));

        let op = "";
        if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
            // find how many digits are in the group
            let oneText = new String(Math.abs(ones));
            let gl = 9999;
            if (of.lastIndexOf(",") != -1)
                gl = of.length - of.lastIndexOf(",") - 1;
            let gc = 0;
            for (let i = oneText.length - 1; i > -1; i--) {
                op = oneText.charAt(i) + op;
                gc++;
                if (gc == gl && i != 0) {
                    op = grp + op;
                    gc = 0;
                }
            }

            // account for any pre-data padding
            if (of.length > op.length) {
                let padStart = of.indexOf('0');
                if (padStart != -1) {
                    let padLen = of.length - padStart;
                    // pad to left with 0's or group char
                    let pos = of.length - op.length - 1;
                    while (op.length < padLen) {
                        let pc = of.charAt(pos);
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

    function roundNumber(n: number, dec?: number): number {
        let power = Math.pow(10, dec || 0);
        let value = (Math.round(n * power) / power).toString();
        // ensure the decimal places are there
        if (dec > 0) {
            let dp = value.indexOf(".");
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

    let isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    
    export function parseInteger(s: string): number {
        s = trim(s.toString());
        let ts = Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }

    export function parseDate(s: string, dateOrder?: string): any {
        if (!s || !s.length)
            return null;
        let dateVal;
        let dArray;
        let d, m, y;
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
                let fullYear = new Date().getFullYear();
                let shortYearCutoff = (fullYear % 100) + 10;
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

    export function parseDecimal(s: string): number {
        if (s == null)
            return null;

        s = trim(s.toString());
        if (s.length == 0)
            return null;

        let ts = Culture.get_groupSeperator();

        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }

        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;

        return parseFloat(s.toString().replace(Culture.decimalSeparator, '.'));
    }

    export function splitDateString(s: string): string[] {
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

    export function parseISODateTime(s: string): Date {
        if (!s || !s.length)
            return null;

        let timestamp = Date.parse(s);
        if (!isNaN(timestamp) && typeof timestamp == "Date")
            return <Date><any>timestamp;

        s = s + "";
        if (typeof (s) != "string" || s.length === 0) {
            return null;
        }

        let res = s.match(isoRegexp);
        if (typeof (res) == "undefined" || res === null) {
            return null;
        }

        let year, month, day, hour, min, sec, msec;
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

        let ofs;
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

    export function parseHourAndMin(value: string) {
        let v = trim(value);
        if (v.length < 4 || v.length > 5)
            return NaN;
        let h, m;
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

    export function parseDayHourAndMin(s: string): number {
        let days;
        let v = trim(s);
        if (!v)
            return NaN;
        let p = v.split('.');
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
            let hm = parseHourAndMin(p[1]);
            if (isNaN(days) || isNaN(hm))
                return NaN;
            return days * 24 * 60 + hm;
        }
    }

    export function parseQueryString(s?: string): {} {
        let qs: string;
        if (s === undefined)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        let result = {};
        let parts = qs.split('&');
        for (let i = 0; i < parts.length; i++) {
            let pair = parts[i].split('=');
            let name = decodeURIComponent(pair[0]);
            result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
        }
        return result;
    }

    let turkishOrder: {};

    export function turkishLocaleCompare(a: string, b: string): number {
        let alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (let z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (let i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            let x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            let ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            let c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }

    export function turkishLocaleToUpper(a: string): string {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }

    export function postToService(options: S.PostToServiceOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/services/' + options.service))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        $('<input/>').attr('type', 'hidden').attr('name', 'request')
            .val($['toJSON'](options.request))
            .appendTo(div);
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }

    export function postToUrl(options: S.PostToUrlOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', resolveUrl(options.url))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        if (options.params != null) {
            for (let k in options.params) {
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

    if ($.fn.button && $.fn.button.noConflict) {
        let btn = $.fn.button.noConflict();
        $.fn.btn = btn;
    }

    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    export function deepClone(arg1: any, arg2: any) {
        let options, name, src, copy, copyIsArray, clone, target = arguments[0] || {}, i = 1, length = arguments.length;
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

    function enumerateTypes(global, namespaces: string[], callback: (type: any, fullName: string) => void) {
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

            for (var k of Object.getOwnPropertyNames(root)) {
                if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                    continue;

                if (k.indexOf('$') >= 0)
                    continue;

                if (k == "prototype")
                    continue;

                scan(root[k], fullName + '.' + k, depth + 1);
            }
        }

        for (var nsRoot of namespaces) {
            if (nsRoot == null || !nsRoot.length) {
                continue;
            }

            if (nsRoot.indexOf('.') >= 0) {
                let g = global;
                let parts = nsRoot.split('.');
                for (var p of parts) {
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

    export namespace ErrorHandling {

        export function showServiceError(error: any) {
            let msg: any;
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
    }

    export namespace Config {
        export let applicationPath = '/';
        export let emailAllowOnlyAscii = true;
        export let rootNamespaces = ['Serenity'];
        export let notLoggedInHandler: Function = null;

        var pathLink = $('link#ApplicationPath');
        if (pathLink.length > 0) {
            applicationPath = pathLink.attr('href');
        }
    }

    export namespace Culture {
        export let decimalSeparator = '.';
        export let dateSeparator = '/';
        export let dateOrder = 'dmy';
        export let dateFormat = 'dd/MM/yyyy';
        export let dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';

        export function get_groupSeperator(): string {
            return ((decimalSeparator === ',') ? '.' : ',');
        };

        var s = Q.trimToNull($('script#ScriptCulture').html());
        if (s != null) {
            var sc = $.parseJSON(s);
            if (sc.DecimalSeparator != null)
                decimalSeparator = sc.DecimalSeparator;
            if (sc.DateSeparator != null)
                dateSeparator = sc.DateSeparator;
            if (sc.DateOrder != null)
                dateOrder = sc.DateOrder;
            if (sc.DateFormat != null)
                dateFormat = sc.DateFormat;
            if (sc.DateTimeFormat != null)
                dateTimeFormat = sc.DateTimeFormat;
        }
    }

    export interface LookupOptions<TItem> {
        idField: string;
        parentIdField: string;
        textField: string;
        textFormatter(item: TItem): string;
    }

    export class Lookup<TItem> {
        private items: TItem[] = [];
        private itemById: { [key: string]: TItem } = {};

        constructor(private options: LookupOptions<TItem>, items?: TItem[]) {
            if (items != null)
                this.update(items);
        }
        
        update(value: TItem[]) {
            this.items = [];
            this.itemById = {};
            if (value) {
                for (var k of value)
                    this.items.push(k);
            }
            var idField = this.options.idField;
            if (!Q.isEmptyOrNull(idField)) {
                for (var r of this.items) {
                    var v = r[idField];
                    if (v != null) {
                        this.itemById[v] = r;
                    }
                }
            }
        }

        get_idField() {
            return this.options.idField;
        }

        get_parentIdField() {
            return this.options.parentIdField;
        }

        get_textField() {
            return this.options.textField;
        }

        get_textFormatter() {
            return this.options.textFormatter;
        }

        get_itemById() {
            return this.itemById;
        }

        get_items() {
            return this.items;
        }
    }

    export class LT {
        static $table: { [key: string]: string } = {};
        static empty: LT = new LT('');

        constructor(private key: string) {
        }

        static add(obj: any, pre?: string) {
            if (!obj) {
                return;
            }
            pre = pre || '';
            var e = ss.getEnumerator(Object.keys(obj));
            try {
                while (e.moveNext()) {
                    var k = e.current();
                    var actual = pre + k;
                    var o = obj[k];
                    if (typeof (o) === 'object') {
                        LT.add(o, actual + '.');
                    }
                    else {
                        LT.$table[actual] = o;
                    }
                }
            }
            finally {
                e.dispose();
            }
        }

        get() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        toString() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        static initializeTextClass = function (type, prefix) {
            var $t1 = ss.arrayClone(Object.keys(type));
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
        }

        getDefault = function (key, defaultText) {
            var t = LT.$table[key];
            if (t == null) {
                t = defaultText;
                if (t == null) {
                    t = key || '';
                }
            }
            return t;
        }
    }

    export namespace ScriptData {
        let registered: { [key: string]: any } = {};
        let loadedData: { [key: string]: any } = {};

        export function bindToChange(name, regClass, onChange) {
            ($(document.body) as any).bind('scriptdatachange.' + regClass, function (e, s) {
                if (s == name) {
                    onChange();
                }
            });
        }

        export function triggerChange(name) {
            $(document.body).triggerHandler('scriptdatachange', [name]);
        }

        export function unbindFromChange(regClass) {
            $(document.body).unbind('scriptdatachange.' + regClass);
        }

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

        function loadScriptData(name: string) {
            if (registered[name] == null) {
                throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
            }

            name = name + '.js?' + registered[name];
            syncLoadScript(Q.resolveUrl('~/DynJS.axd/') + name);
        }

        function loadScriptDataAsync(name: string): RSVP.Thenable<any> {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
                }

                name = name + '.js?' + registered[name];
                return loadScriptAsync(Q.resolveUrl('~/DynJS.axd/') + name);
            }, null);
        }

        export function ensure(name: string) {
            var data = loadedData[name];
            if (data == null) {
                loadScriptData(name);
            }

            data = loadedData[name];

            if (data == null)
                throw new ss.NotSupportedException(ss.formatString("Can't load script data: {0}!", name));

            return data;
        }

        export function ensureAsync(name: string): RSVP.Thenable<any> {
            return RSVP.resolve().then(function () {
                var data = loadedData[name];
                if (data != null) {
                    return RSVP.resolve(data);
                }

                return loadScriptDataAsync(name).then(function () {
                    data = loadedData[name];
                    if (data == null) {
                        throw new ss.NotSupportedException(ss.formatString("Can't load script data: {0}!", name));
                    }
                    return data;
                }, null);
            }, null);
        }

        export function reload(name: string) {
            if (registered[name] == null) {
                throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            loadScriptData(name);
            var data = loadedData[name];
            return data;
        }

        export function reloadAsync(name: string) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
                }
                registered[name] = (new Date()).getTime().toString();
                return loadScriptDataAsync(name).then(function () {
                    return loadedData[name];
                }, null);
            }, null);
        }

        export function canLoad(name: string) {
            return (loadedData[name] != null || registered[name] != null);
        }

        export function setRegisteredScripts(scripts) {
            registered = {};
            for (var k in scripts) {
                registered[k] = scripts[k].toString();
            }
        }

        export function set(name: string, value) {
            loadedData[name] = value;
            triggerChange(name);
        }
    }

    (function (global: any) {
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
                if (obj.__typeName)
                    return;

                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    fullName.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter]
                }

                if (!obj.class) {
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
}

namespace Serenity {

    export class ColumnsKeyAttribute {
        constructor(public value: string) { }
    }

    export class DialogTypeAttribute {
        constructor(public value: Function) { }
    }

    export class EditorAttribute {
        constructor() { }
        key: string;
    }

    export class ElementAttribute {
        constructor(public value: string) { }
    }

    export class EntityTypeAttribute {
        constructor(public value: string) { }
    }

    export class EnumKeyAttribute {
        constructor(public value: string) { }
    }

    export class FlexifyAttribute {
        constructor(public value = true) { }
    }

    export class FormKeyAttribute {
        constructor(public value: string) { }
    }

    export class IdPropertyAttribute {
        constructor(public value: string) { }
    }

    export class IsActivePropertyAttribute {
        constructor(public value: string) { }
    }

    export class ItemNameAttribute {
        constructor(public value: string) { }
    }

    export class LocalTextPrefixAttribute {
        constructor(public value: string) { }
    }

    export class MaximizableAttribute {
        constructor(public value = true) { }
    }

    export class NamePropertyAttribute {
        constructor(public value: string) { }
    }

    export class OptionAttribute {
        constructor() { }
    }

    export class OptionsTypeAttribute {
        constructor(public value: Function) { }
    }

    export class PanelAttribute {
        constructor(public value = true) { }
    }

    export class ResizableAttribute {
        constructor(public value = true) { }
    }

    export class ResponsiveAttribute {
        constructor(public value = true) { }
    }

    export class ServiceAttribute {
        constructor(public value: string) { }
    }

    export namespace Criteria {
        import C = Criteria;

        export function isEmpty(c: any[]): boolean {
            return c == null ||
                c.length === 0 ||
                (c.length === 1 &&
                    typeof c[0] == "string" &&
                    c[0].length === 0);
        }

        export function join(c1: any[], op: string, c2: any[]): any[] {
            if (C.isEmpty(c1))
                return c2;

            if (C.isEmpty(c2))
                return c1;

            return [c1, op, c2];
        }

        export function paren(c: any[]): any[] {
            return C.isEmpty(c) ? c : ['()', c];
        }
    }

    export namespace Decorators {

        export function addAttribute(type: any, attr: any) {
            let old: any = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }

        export function columnsKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new ColumnsKeyAttribute(value));
            }
        }

        export function dialogType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new DialogTypeAttribute(value));
            }
        }

        export function editor(key?: string) {
            return function (target: Function) {
                var attr = new EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                addAttribute(target, attr);
            }
        }

        export function element(value: string) {
            return function (target: Function) {
                addAttribute(target, new ElementAttribute(value));
            }
        }

        export function entityType(value: string) {
            return function (target: Function) {
                addAttribute(target, new EntityTypeAttribute(value));
            }
        }

        export function enumKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new EnumKeyAttribute(value));
            }
        }

        export function flexify(value = true) {
            return function (target: Function) {
                addAttribute(target, new FlexifyAttribute(value));
            }
        }

        export function formKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new FormKeyAttribute(value));
            }
        }

        export function idProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IdPropertyAttribute(value));
            }
        }

        export function registerClass(intf?: any[], asm?: ss.AssemblyReg) {
            return function (target: Function) {
                (target as any).__class = true;
                (target as any).__assembly = asm || ss.__assemblies['App'];
                if (intf)
                    (target as any).__interfaces = intf;
            }
        }

        export function registerFormatter(intf = [Serenity.ISlickFormatter], asm?: ss.AssemblyReg) {
            return registerClass(intf, asm);
        }

        export function itemName(value: string) {
            return function (target: Function) {
                addAttribute(target, new ItemNameAttribute(value));
            }
        }

        export function isActiveProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IsActivePropertyAttribute(value));
            }
        }

        export function localTextPrefix(value: string) {
            return function (target: Function) {
                addAttribute(target, new LocalTextPrefixAttribute(value));
            }
        }

        export function maximizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new MaximizableAttribute(value));
            }
        }

        export function nameProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new NamePropertyAttribute(value));
            }
        }

        export function option(value: Function) {
            return function (target: Function) {
                addAttribute(target, new OptionAttribute());
            }
        }

        export function optionsType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new OptionsTypeAttribute(value));
            }
        }

        export function panel(value = true) {
            return function (target: Function) {
                addAttribute(target, new PanelAttribute(value));
            }
        }

        export function resizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResizableAttribute(value));
            }
        }

        export function responsive(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResponsiveAttribute(value));
            }
        }

        export function service(value: string) {
            return function (target: Function) {
                addAttribute(target, new ServiceAttribute(value));
            }
        }
    }

    export namespace TabsExtensions {
        export function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean) {
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

        export function activeTabKey(tabs: JQuery) {
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

        export function indexByKey(tabs: JQuery): any {
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
    }

    export namespace LazyLoadHelper {
        let autoIncrement = 0;

        export function executeOnceWhenShown(element: JQuery, callback: Function) {
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

        export function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean) {
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
    }
}
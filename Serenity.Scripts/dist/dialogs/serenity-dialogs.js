var Q;
(function (Q) {
    var Router;
    (function (Router) {
        var oldURL;
        var resolving = 0;
        var autoinc = 0;
        var listenerTimeout;
        var ignoreHash = 0;
        var ignoreTime = 0;
        Router.enabled = true;
        function isEqual(url1, url2) {
            return url1 == url2 || url1 == url2 + '#' || url2 == url1 + '#';
        }
        function navigate(hash, tryBack, silent) {
            if (!Router.enabled || resolving > 0)
                return;
            hash = hash || '';
            hash = hash.replace(/^#/, '');
            hash = (!hash ? "" : '#' + hash);
            var newURL = window.location.href.replace(/#$/, '')
                .replace(/#.*$/, '') + hash;
            if (newURL != window.location.href) {
                if (tryBack && oldURL != null && isEqual(oldURL, newURL)) {
                    if (silent)
                        ignoreChange();
                    var prior = window.location.href;
                    oldURL = null;
                    window.history.back();
                    return;
                }
                if (silent)
                    ignoreChange();
                oldURL = window.location.href;
                window.location.hash = hash;
            }
        }
        Router.navigate = navigate;
        function replace(hash, tryBack) {
            navigate(hash, tryBack, true);
        }
        Router.replace = replace;
        function replaceLast(hash, tryBack) {
            if (!Router.enabled)
                return;
            var current = window.location.hash || '';
            if (current.charAt(0) == '#')
                current = current.substr(1, current.length - 1);
            var parts = current.split('/+/');
            if (parts.length > 1) {
                if (hash && hash.length) {
                    parts[parts.length - 1] = hash;
                    hash = parts.join("/+/");
                }
                else {
                    parts.splice(parts.length - 1, 1);
                    hash = parts.join("/+/");
                }
            }
            replace(hash, tryBack);
        }
        Router.replaceLast = replaceLast;
        function visibleDialogs() {
            return $('.ui-dialog-content:visible, .ui-dialog.panel-hidden>.ui-dialog-content, .s-Panel').toArray().sort(function (a, b) {
                return ($(a).data('qrouterorder') || 0) - ($(b).data('qrouterorder') || 0);
            });
        }
        function dialogOpen(owner, element, hash) {
            var route = [];
            var isDialog = owner.hasClass(".ui-dialog-content") || owner.hasClass('.s-Panel');
            var dialog = isDialog ? owner :
                owner.closest('.ui-dialog-content, .s-Panel');
            var value = hash();
            var idPrefix;
            if (dialog.length) {
                var dialogs = visibleDialogs();
                var index = dialogs.indexOf(dialog[0]);
                for (var i = 0; i <= index; i++) {
                    var q = $(dialogs[i]).data("qroute");
                    if (q && q.length)
                        route.push(q);
                }
                if (!isDialog) {
                    idPrefix = dialog.attr("id");
                    if (idPrefix) {
                        idPrefix += "_";
                        var id = owner.attr("id");
                        if (id && Q.startsWith(id, idPrefix))
                            value = id.substr(idPrefix.length) + '@' + value;
                    }
                }
            }
            else {
                var id = owner.attr("id");
                if (id && (!owner.hasClass("route-handler") ||
                    $('.route-handler').first().attr("id") != id))
                    value = id + "@" + value;
            }
            route.push(value);
            element.data("qroute", value);
            replace(route.join("/+/"));
            element.bind("dialogclose.qrouter panelclose.qrouter", function (e) {
                element.data("qroute", null);
                element.unbind(".qrouter");
                var prhash = element.data("qprhash");
                var tryBack = $(e.target).closest('.s-MessageDialog').length > 0 || (e && e.originalEvent &&
                    ((e.originalEvent.type == "keydown" && e.originalEvent.keyCode == 27) ||
                        $(e.originalEvent.target).hasClass("ui-dialog-titlebar-close") ||
                        $(e.originalEvent.target).hasClass("panel-titlebar-close")));
                if (prhash != null)
                    replace(prhash, tryBack);
                else
                    replaceLast('', tryBack);
            });
        }
        function dialog(owner, element, hash) {
            if (!Router.enabled)
                return;
            element.on("dialogopen.qrouter panelopen.qrouter", function (e) {
                dialogOpen(owner, element, hash);
            });
        }
        Router.dialog = dialog;
        function resolve(hash) {
            if (!Router.enabled)
                return;
            resolving++;
            try {
                hash = Q.coalesce(Q.coalesce(hash, window.location.hash), '');
                if (hash.charAt(0) == '#')
                    hash = hash.substr(1, hash.length - 1);
                var dialogs = visibleDialogs();
                var newParts = hash.split("/+/");
                var oldParts = dialogs.map(function (el) { return $(el).data('qroute'); });
                var same = 0;
                while (same < dialogs.length &&
                    same < newParts.length &&
                    oldParts[same] == newParts[same]) {
                    same++;
                }
                for (var i = same; i < dialogs.length; i++) {
                    var d = $(dialogs[i]);
                    if (d.hasClass('ui-dialog-content'))
                        d.dialog('close');
                    else if (d.hasClass('s-Panel'))
                        Serenity.TemplatedDialog.closePanel(d);
                }
                for (var i = same; i < newParts.length; i++) {
                    var route = newParts[i];
                    var routeParts = route.split('@');
                    var handler;
                    if (routeParts.length == 2) {
                        var dialog = i > 0 ? $(dialogs[i - 1]) : $([]);
                        if (dialog.length) {
                            var idPrefix = dialog.attr("id");
                            if (idPrefix) {
                                handler = $('#' + idPrefix + "_" + routeParts[0]);
                                if (handler.length) {
                                    route = routeParts[1];
                                }
                            }
                        }
                        if (!handler || !handler.length) {
                            handler = $('#' + routeParts[0]);
                            if (handler.length) {
                                route = routeParts[1];
                            }
                        }
                    }
                    if (!handler || !handler.length) {
                        handler = i > 0 ? $(dialogs[i - 1]) :
                            $('.route-handler').first();
                    }
                    handler.triggerHandler("handleroute", {
                        handled: false,
                        route: route,
                        parts: newParts,
                        index: i
                    });
                }
            }
            finally {
                resolving--;
            }
        }
        Router.resolve = resolve;
        function hashChange(e, o) {
            if (ignoreHash > 0) {
                if (new Date().getTime() - ignoreTime > 1000) {
                    ignoreHash = 0;
                }
                else {
                    ignoreHash--;
                    return;
                }
            }
            resolve();
        }
        function ignoreChange() {
            ignoreHash++;
            ignoreTime = new Date().getTime();
        }
        window.addEventListener("hashchange", hashChange, false);
        var routerOrder = 1;
        $(document).on("dialogopen panelopen", ".ui-dialog-content, .s-Panel", function (event, ui) {
            if (!Router.enabled)
                return;
            var dlg = $(event.target);
            dlg.data("qrouterorder", routerOrder++);
            if (dlg.data("qroute"))
                return;
            dlg.data("qprhash", window.location.hash);
            var owner = $(visibleDialogs).not(dlg).last();
            if (!owner.length)
                owner = $('html');
            dialogOpen(owner, dlg, function () {
                return "!" + (++autoinc).toString(36);
            });
        });
    })(Router = Q.Router || (Q.Router = {}));
})(Q || (Q = {}));
var Q;
(function (Q) {
    function autoFullHeight(element) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }
    Q.autoFullHeight = autoFullHeight;
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
        gridDiv.one('remove', function () {
            $(window).off('layout', layout);
            $('body').off('layout', layout);
        });
        // ugly, but to it is to make old pages work without having to add this
        Q["Router"] && Q["Router"].resolve && Q["Router"].resolve();
    }
    Q.initFullHeightGridPage = initFullHeightGridPage;
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
        var n = Math.round(h) + 'px';
        if (element.css('height') != n) {
            element.css('height', n);
        }
    }
    Q.layoutFillHeight = layoutFillHeight;
    function setMobileDeviceMode() {
        var isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            (window.matchMedia && window.matchMedia('(max-width: 767px)').matches);
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
    setMobileDeviceMode();
    $(function () {
        if (globalObj && Q.Config.rootNamespaces) {
            for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                var ns = _a[_i];
                var obj = Q.getNested(globalObj, ns);
                if (obj != null)
                    Q.initializeTypes(obj, ns + ".", 3);
            }
        }
        globalObj && $(globalObj).bind('resize', function () {
            setMobileDeviceMode();
        });
    });
    function triggerLayoutOnShow(element) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }
    Q.triggerLayoutOnShow = triggerLayoutOnShow;
    function centerDialog(el) {
        if (!el.hasClass("ui-dialog"))
            el = el.closest(".ui-dialog");
        el.position({ at: 'center center', of: window });
        var pos = el.position();
        if (pos.left < 0)
            el.css("left", "0px");
        if (pos.top < 0)
            el.css("top", "0px");
    }
    Q.centerDialog = centerDialog;
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var DialogExtensions;
    (function (DialogExtensions) {
        function dialogResizable(dialog, w, h, mw, mh) {
            var dlg = dialog.dialog();
            dlg.dialog('option', 'resizable', true);
            if (mw != null) {
                dlg.dialog('option', 'minWidth', mw);
            }
            if (w != null) {
                dlg.dialog('option', 'width', w);
            }
            if (mh != null) {
                dlg.dialog('option', 'minHeight', mh);
            }
            if (h != null) {
                dlg.dialog('option', 'height', h);
            }
            return dialog;
        }
        DialogExtensions.dialogResizable = dialogResizable;
        function dialogMaximizable(dialog) {
            dialog.dialogExtend({
                closable: true,
                maximizable: true,
                dblclick: 'maximize',
                icons: { maximize: 'ui-icon-maximize-window' }
            });
            return dialog;
        }
        DialogExtensions.dialogMaximizable = dialogMaximizable;
    })(DialogExtensions = Serenity.DialogExtensions || (Serenity.DialogExtensions = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedDialog = /** @class */ (function (_super) {
        __extends(TemplatedDialog, _super);
        function TemplatedDialog(options) {
            var _this = _super.call(this, Q.newBodyDiv().addClass('s-TemplatedDialog hidden'), options) || this;
            _this.element.attr("id", _this.uniqueName);
            _this.initValidator();
            _this.initTabs();
            _this.initToolbar();
            return _this;
        }
        TemplatedDialog_1 = TemplatedDialog;
        Object.defineProperty(TemplatedDialog.prototype, "isMarkedAsPanel", {
            get: function () {
                var panelAttr = Q.getAttributes(Q.getInstanceType(this), Serenity.PanelAttribute, true);
                return panelAttr.length > 0 && panelAttr[panelAttr.length - 1].value !== false;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(TemplatedDialog.prototype, "isResponsive", {
            get: function () {
                return Q.Config.responsiveDialogs ||
                    Q.getAttributes(Q.getInstanceType(this), Serenity.ResponsiveAttribute, true).length > 0;
            },
            enumerable: true,
            configurable: true
        });
        TemplatedDialog.getCssSize = function (element, name) {
            var cssSize = element.css(name);
            if (cssSize == null) {
                return null;
            }
            if (!Q.endsWith(cssSize, 'px')) {
                return null;
            }
            cssSize = cssSize.substr(0, cssSize.length - 2);
            var i = Q.parseInteger(cssSize);
            if (i == null || isNaN(i) || i == 0)
                return null;
            return i;
        };
        TemplatedDialog.applyCssSizes = function (opt, dialogClass) {
            var size;
            var dialog = $('<div/>').hide().addClass(dialogClass).appendTo(document.body);
            try {
                var sizeHelper = $('<div/>').addClass('size').appendTo(dialog);
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'minWidth');
                if (size != null)
                    opt.minWidth = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'width');
                if (size != null)
                    opt.width = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'height');
                if (size != null)
                    opt.height = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'minHeight');
                if (size != null)
                    opt.minHeight = size;
            }
            finally {
                dialog.remove();
            }
        };
        ;
        TemplatedDialog.prototype.destroy = function () {
            this.tabs && this.tabs.tabs('destroy');
            this.tabs = null;
            this.toolbar && this.toolbar.destroy();
            this.toolbar = null;
            this.validator && this.byId('Form').remove();
            this.validator = null;
            if (this.element != null &&
                this.element.hasClass('ui-dialog-content')) {
                this.element.dialog('destroy');
                this.element.removeClass('ui-dialog-content');
            }
            else if (this.element != null &&
                this.element.hasClass('modal-body')) {
                var modal = this.element.closest('.modal').data('bs.modal', null);
                this.element && this.element.removeClass('modal-body');
                window.setTimeout(function () { return modal.remove(); }, 0);
            }
            $(window).unbind('.' + this.uniqueName);
            _super.prototype.destroy.call(this);
        };
        TemplatedDialog.prototype.initDialog = function () {
            var _this = this;
            if (this.element.hasClass('ui-dialog-content'))
                return;
            this.element.removeClass('hidden');
            this.element.dialog(this.getDialogOptions());
            this.element.closest('.ui-dialog').on('resize', function (e) { return _this.arrange(); });
            var type = Q.getInstanceType(this);
            if (this.isResponsive) {
                Serenity.DialogExtensions.dialogResizable(this.element);
                $(window).bind('resize.' + this.uniqueName, function (e) {
                    if (_this.element && _this.element.is(':visible')) {
                        _this.handleResponsive();
                    }
                });
                this.element.closest('.ui-dialog').addClass('flex-layout');
            }
            if (Q.getAttributes(type, Serenity.MaximizableAttribute, true).length > 0) {
                Serenity.DialogExtensions.dialogMaximizable(this.element);
            }
            var self = this;
            this.element.bind('dialogopen.' + this.uniqueName, function () {
                $(document.body).addClass('modal-dialog-open');
                if (_this.isResponsive) {
                    _this.handleResponsive();
                }
                self.onDialogOpen();
            });
            this.element.bind('dialogclose.' + this.uniqueName, function () {
                $(document.body).toggleClass('modal-dialog-open', $('.ui-dialog:visible').length > 0);
                self.onDialogClose();
            });
        };
        TemplatedDialog.prototype.getModalOptions = function () {
            return {
                backdrop: false,
                keyboard: false,
                size: 'lg',
                modalClass: this.getCssClass()
            };
        };
        TemplatedDialog.prototype.initModal = function () {
            var _this = this;
            if (this.element.hasClass('modal-body'))
                return;
            var title = Q.coalesce(this.element.data('dialogtitle'), this.getDialogTitle()) || '';
            var opt = this.getModalOptions();
            opt["show"] = false;
            var modalClass = "s-Modal";
            if (opt.modalClass)
                modalClass += ' ' + opt.modalClass;
            var markup = Q.bsModalMarkup(title, '', modalClass);
            var modal = $(markup).eq(0).appendTo(document.body).addClass('flex-layout');
            modal.one('shown.bs.modal.' + this.uniqueName, function () {
                _this.element.triggerHandler('shown.bs.modal');
                _this.onDialogOpen();
            });
            modal.one('hidden.bs.modal.' + this.uniqueName, function () {
                $(document.body).toggleClass('modal-open', $('.modal.show').length + $('.modal.in').length > 0);
                _this.onDialogClose();
            });
            if (opt.size)
                modal.find('.modal-dialog').addClass('modal-' + opt.size);
            var footer = modal.find('.modal-footer');
            var buttons = this.getDialogButtons();
            if (buttons != null) {
                for (var _i = 0, buttons_1 = buttons; _i < buttons_1.length; _i++) {
                    var x = buttons_1[_i];
                    $(Q.dialogButtonToBS(x)).appendTo(footer).click(x.click);
                }
            }
            else
                footer.hide();
            modal.modal(opt);
            modal.find('.modal-body').replaceWith(this.element.removeClass('hidden').addClass('modal-body'));
            $(window).on('resize.' + this.uniqueName, this.arrange.bind(this));
        };
        TemplatedDialog.prototype.initToolbar = function () {
            var toolbarDiv = this.byId('Toolbar');
            if (toolbarDiv.length === 0) {
                return;
            }
            var hotkeyContext = this.element.closest('.ui-dialog');
            if (hotkeyContext.length === 0) {
                hotkeyContext = this.element.closest('.modal');
                if (hotkeyContext.length == 0)
                    hotkeyContext = this.element;
            }
            var opt = { buttons: this.getToolbarButtons(), hotkeyContext: hotkeyContext[0] };
            this.toolbar = new Serenity.Toolbar(toolbarDiv, opt);
        };
        TemplatedDialog.prototype.getToolbarButtons = function () {
            return [];
        };
        TemplatedDialog.prototype.getValidatorOptions = function () {
            return {};
        };
        TemplatedDialog.prototype.initValidator = function () {
            var form = this.byId('Form');
            if (form.length > 0) {
                var valOptions = this.getValidatorOptions();
                this.validator = form.validate(Q.validateOptions(valOptions));
            }
        };
        TemplatedDialog.prototype.resetValidation = function () {
            this.validator && this.validator.resetAll();
        };
        TemplatedDialog.prototype.validateForm = function () {
            return this.validator == null || !!this.validator.form();
        };
        TemplatedDialog.prototype.dialogOpen = function (asPanel) {
            var _this = this;
            asPanel = Q.coalesce(asPanel, this.isMarkedAsPanel);
            if (asPanel) {
                if (!this.element.hasClass('s-Panel')) {
                    // so that panel title is created if needed
                    this.element.on('panelopen.' + this.uniqueName, function () {
                        _this.onDialogOpen();
                    });
                    this.element.on('panelclose.' + this.uniqueName, function () {
                        _this.onDialogClose();
                    });
                }
                TemplatedDialog_1.openPanel(this.element, this.uniqueName);
                this.setupPanelTitle();
            }
            else if (this.useBSModal()) {
                this.initModal();
                this.element.closest('.modal').modal('show');
            }
            else {
                this.initDialog();
                this.element.dialog('open');
            }
        };
        TemplatedDialog.prototype.useBSModal = function () {
            return !!((!$.ui || !$.ui.dialog) || TemplatedDialog_1.bootstrapModal);
        };
        TemplatedDialog.openPanel = function (element, uniqueName) {
            var container = $('.panels-container');
            if (!container.length)
                container = $('section.content');
            element.data('paneluniquename', uniqueName);
            if (container.length) {
                container = container.last();
                container.children()
                    .not(element)
                    .not('.panel-hidden')
                    .addClass('panel-hidden panel-hidden-' + uniqueName);
                if (element[0].parentElement !== container[0])
                    element.appendTo(container);
            }
            $('.ui-dialog:visible, .ui-widget-overlay:visible, .modal.show, .modal.in')
                .not(element)
                .addClass('panel-hidden panel-hidden-' + uniqueName);
            element
                .removeClass('hidden')
                .removeClass('panel-hidden')
                .addClass('s-Panel')
                .trigger('panelopen');
        };
        TemplatedDialog.closePanel = function (element, e) {
            if (!element.hasClass('s-Panel') || element.hasClass('hidden'))
                return;
            var query = $.Event(e);
            query.type = 'panelbeforeclose';
            query.target = element[0];
            element.trigger(query);
            if (query.isDefaultPrevented())
                return;
            element.addClass('hidden');
            var uniqueName = element.data('paneluniquename') || new Date().getTime();
            var klass = 'panel-hidden-' + uniqueName;
            $('.' + klass).removeClass(klass).removeClass('panel-hidden');
            $(window).triggerHandler('resize');
            $('.require-layout:visible').triggerHandler('layout');
            var e = $.Event(e);
            e.type = 'panelclose';
            e.target = element[0];
            element.trigger(e);
        };
        TemplatedDialog.prototype.onDialogOpen = function () {
            if (!$(document.body).hasClass('mobile-device'))
                $(':input', this.element).not('button').eq(0).focus();
            this.arrange();
            this.tabs && this.tabs.tabs('option', 'active', 0);
        };
        TemplatedDialog.prototype.arrange = function () {
            this.element.find('.require-layout').filter(':visible').each(function (i, e) {
                $(e).triggerHandler('layout');
            });
        };
        TemplatedDialog.prototype.onDialogClose = function () {
            var _this = this;
            $(document).trigger('click');
            // for tooltips etc.
            if ($.qtip) {
                $(document.body).children('.qtip').each(function (index, el) {
                    $(el).qtip('hide');
                });
            }
            window.setTimeout(function () {
                var element = _this.element;
                _this.destroy();
                element.remove();
                Q.positionToastContainer(false);
            }, 0);
        };
        TemplatedDialog.prototype.addCssClass = function () {
            if (this.isMarkedAsPanel) {
                _super.prototype.addCssClass.call(this);
                if (this.isResponsive)
                    this.element.addClass("flex-layout");
            }
        };
        TemplatedDialog.prototype.getDialogButtons = function () {
            return undefined;
        };
        TemplatedDialog.prototype.getDialogOptions = function () {
            var opt = {};
            var dialogClass = 's-Dialog ' + this.getCssClass();
            opt.dialogClass = dialogClass;
            var buttons = this.getDialogButtons();
            if (buttons != null)
                opt.buttons = buttons.map(Q.dialogButtonToUI);
            opt.width = 920;
            TemplatedDialog_1.applyCssSizes(opt, dialogClass);
            opt.autoOpen = false;
            var type = Q.getInstanceType(this);
            opt.resizable = Q.getAttributes(type, Serenity.ResizableAttribute, true).length > 0;
            opt.modal = true;
            opt.position = { my: 'center', at: 'center', of: $(window.window) };
            opt.title = Q.coalesce(this.element.data('dialogtitle'), this.getDialogTitle()) || '';
            return opt;
        };
        TemplatedDialog.prototype.getDialogTitle = function () {
            return "";
        };
        TemplatedDialog.prototype.dialogClose = function () {
            if (this.element.hasClass('ui-dialog-content'))
                this.element.dialog().dialog('close');
            else if (this.element.hasClass('modal-body'))
                this.element.closest('.modal').modal('hide');
            else if (this.element.hasClass('s-Panel') && !this.element.hasClass('hidden')) {
                TemplatedDialog_1.closePanel(this.element);
            }
        };
        Object.defineProperty(TemplatedDialog.prototype, "dialogTitle", {
            get: function () {
                if (this.element.hasClass('ui-dialog-content'))
                    return this.element.dialog('option', 'title');
                else if (this.element.hasClass('modal-body'))
                    return this.element.closest('.modal').find('.modal-header').children('h5').text();
                return this.element.data('dialogtitle');
            },
            set: function (value) {
                var oldTitle = this.dialogTitle;
                this.element.data('dialogtitle', value);
                if (this.element.hasClass('ui-dialog-content'))
                    this.element.dialog('option', 'title', value);
                else if (this.element.hasClass('modal-body')) {
                    this.element.closest('.modal').find('.modal-header').children('h5').text(value !== null && value !== void 0 ? value : '');
                }
                else if (this.element.hasClass('s-Panel')) {
                    if (oldTitle != this.dialogTitle) {
                        this.setupPanelTitle();
                        this.arrange();
                    }
                }
            },
            enumerable: true,
            configurable: true
        });
        TemplatedDialog.prototype.setupPanelTitle = function () {
            var _this = this;
            var value = Q.coalesce(this.dialogTitle, this.getDialogTitle());
            var pt = this.element.children('.panel-titlebar');
            if (Q.isEmptyOrNull(value)) {
                pt.remove();
            }
            else {
                if (!this.element.children('.panel-titlebar').length) {
                    pt = $("<div class='panel-titlebar'><div class='panel-titlebar-text'></div></div>")
                        .prependTo(this.element);
                }
                pt.children('.panel-titlebar-text').text(value);
                if (this.element.hasClass('s-Panel')) {
                    if (!pt.children('.panel-titlebar-close').length) {
                        $('<button class="panel-titlebar-close">&nbsp;</button>')
                            .prependTo(pt)
                            .click(function (e) {
                            TemplatedDialog_1.closePanel(_this.element, e);
                        });
                    }
                }
            }
        };
        TemplatedDialog.prototype.set_dialogTitle = function (value) {
            this.dialogTitle = value;
        };
        TemplatedDialog.prototype.initTabs = function () {
            var _this = this;
            var tabsDiv = this.byId('Tabs');
            if (tabsDiv.length === 0) {
                return;
            }
            this.tabs = tabsDiv.tabs({});
            this.tabs.bind('tabsactivate', function () { return _this.arrange(); });
        };
        TemplatedDialog.prototype.handleResponsive = function () {
            var dlg = this.element.dialog();
            var uiDialog = this.element.closest('.ui-dialog');
            if ($(document.body).hasClass('mobile-device')) {
                var data = this.element.data('responsiveData');
                if (!data) {
                    data = {};
                    data.draggable = dlg.dialog('option', 'draggable');
                    data.resizable = dlg.dialog('option', 'resizable');
                    data.position = dlg.css('position');
                    var pos = uiDialog.position();
                    data.left = pos.left;
                    data.top = pos.top;
                    data.width = uiDialog.width();
                    data.height = uiDialog.height();
                    data.contentHeight = this.element.height();
                    this.element.data('responsiveData', data);
                    dlg.dialog('option', 'draggable', false);
                    dlg.dialog('option', 'resizable', false);
                }
                uiDialog.addClass('mobile-layout');
                uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px', position: 'fixed' });
                $(document.body).scrollTop(0);
                Q.layoutFillHeight(this.element);
            }
            else {
                var d = this.element.data('responsiveData');
                if (d) {
                    dlg.dialog('option', 'draggable', d.draggable);
                    dlg.dialog('option', 'resizable', d.resizable);
                    this.element.closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px', position: d.position });
                    this.element.height(d.contentHeight);
                    uiDialog.removeClass('mobile-layout');
                    this.element.removeData('responsiveData');
                }
            }
        };
        var TemplatedDialog_1;
        TemplatedDialog = TemplatedDialog_1 = __decorate([
            Serenity.Decorators.registerClass([Serenity.IDialog])
        ], TemplatedDialog);
        return TemplatedDialog;
    }(Serenity.TemplatedWidget));
    Serenity.TemplatedDialog = TemplatedDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var PropertyDialog = /** @class */ (function (_super) {
        __extends(PropertyDialog, _super);
        function PropertyDialog(opt) {
            var _this = _super.call(this, opt) || this;
            _this.initPropertyGrid();
            _this.loadInitialEntity();
            return _this;
        }
        PropertyDialog.prototype.destroy = function () {
            if (this.propertyGrid) {
                this.propertyGrid.destroy();
                this.propertyGrid = null;
            }
            if (this.validator) {
                this.byId('Form').remove();
                this.validator = null;
            }
            _super.prototype.destroy.call(this);
        };
        PropertyDialog.prototype.getDialogOptions = function () {
            var opt = _super.prototype.getDialogOptions.call(this);
            opt.width = 400;
            return opt;
        };
        PropertyDialog.prototype.getDialogButtons = function () {
            var _this = this;
            return [{
                    text: Q.text('Dialogs.OkButton'),
                    click: function () { return _this.okClick(); }
                }, {
                    text: Q.text('Dialogs.CancelButton'),
                    click: function () { return _this.cancelClick(); }
                }];
        };
        PropertyDialog.prototype.okClick = function () {
            if (!this.validateBeforeSave()) {
                return;
            }
            this.okClickValidated();
        };
        PropertyDialog.prototype.okClickValidated = function () {
            this.dialogClose();
        };
        PropertyDialog.prototype.cancelClick = function () {
            this.dialogClose();
        };
        PropertyDialog.prototype.initPropertyGrid = function () {
            var pgDiv = this.byId('PropertyGrid');
            if (pgDiv.length <= 0) {
                return;
            }
            var pgOptions = this.getPropertyGridOptions();
            this.propertyGrid = (new Serenity.PropertyGrid(pgDiv, pgOptions)).init(null);
            if (this.element.closest('.ui-dialog').hasClass('s-Flexify')) {
                this.propertyGrid.element.children('.categories').flexHeightOnly(1);
            }
        };
        PropertyDialog.prototype.getFormKey = function () {
            var attributes = Q.getAttributes(Q.getInstanceType(this), Serenity.FormKeyAttribute, true);
            if (attributes.length >= 1) {
                return attributes[0].value;
            }
            else {
                var name = Q.getTypeFullName(Q.getInstanceType(this));
                var px = name.indexOf('.');
                if (px >= 0) {
                    name = name.substring(px + 1);
                }
                if (Q.endsWith(name, 'Dialog')) {
                    name = name.substr(0, name.length - 6);
                }
                else if (Q.endsWith(name, 'Panel')) {
                    name = name.substr(0, name.length - 5);
                }
                return name;
            }
        };
        PropertyDialog.prototype.getPropertyGridOptions = function () {
            return {
                idPrefix: this.idPrefix,
                items: this.getPropertyItems(),
                mode: 1,
                useCategories: false,
                localTextPrefix: 'Forms.' + this.getFormKey() + '.'
            };
        };
        PropertyDialog.prototype.getPropertyItems = function () {
            var formKey = this.getFormKey();
            return Q.getForm(formKey);
        };
        PropertyDialog.prototype.getSaveEntity = function () {
            var entity = new Object();
            if (this.propertyGrid) {
                this.propertyGrid.save(entity);
            }
            return entity;
        };
        PropertyDialog.prototype.loadInitialEntity = function () {
            this.propertyGrid && this.propertyGrid.load(new Object());
        };
        PropertyDialog.prototype.get_entity = function () {
            return this._entity;
        };
        PropertyDialog.prototype.set_entity = function (value) {
            this._entity = Q.coalesce(value, new Object());
        };
        PropertyDialog.prototype.get_entityId = function () {
            return this._entityId;
        };
        PropertyDialog.prototype.set_entityId = function (value) {
            this._entityId = value;
        };
        PropertyDialog.prototype.validateBeforeSave = function () {
            return this.validator.form();
        };
        PropertyDialog.prototype.updateTitle = function () {
        };
        PropertyDialog = __decorate([
            Serenity.Decorators.registerClass('Serenity.PropertyDialog')
        ], PropertyDialog);
        return PropertyDialog;
    }(Serenity.TemplatedDialog));
    Serenity.PropertyDialog = PropertyDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var EntityDialog = /** @class */ (function (_super) {
        __extends(EntityDialog, _super);
        function EntityDialog(opt) {
            var _this = _super.call(this, opt) || this;
            _this.initPropertyGrid();
            _this.initLocalizationGrid();
            return _this;
        }
        EntityDialog.prototype.destroy = function () {
            if (this.propertyGrid) {
                this.propertyGrid.destroy();
                this.propertyGrid = null;
            }
            if (this.localizationGrid) {
                this.localizationGrid.destroy();
                this.localizationGrid = null;
            }
            this.undeleteButton = null;
            this.applyChangesButton = null;
            this.deleteButton = null;
            this.saveAndCloseButton = null;
            this.editButton = null;
            this.cloneButton = null;
            this.toolbar = null;
            _super.prototype.destroy.call(this);
        };
        EntityDialog.prototype.get_entity = function () {
            return this.entity;
        };
        EntityDialog.prototype.set_entity = function (entity) {
            this.entity = entity || new Object();
        };
        EntityDialog.prototype.get_entityId = function () {
            return this.entityId;
        };
        EntityDialog.prototype.set_entityId = function (value) {
            this.entityId = value;
        };
        EntityDialog.prototype.getEntityNameFieldValue = function () {
            return Q.coalesce(this.get_entity()[this.getNameProperty()], '').toString();
        };
        EntityDialog.prototype.getEntityTitle = function () {
            if (!this.isEditMode()) {
                return Q.format(Q.text('Controls.EntityDialog.NewRecordTitle'), this.getEntitySingular());
            }
            else {
                var titleFormat = (this.isViewMode() || this.readOnly || !this.hasSavePermission()) ?
                    Q.text('Controls.EntityDialog.ViewRecordTitle') : Q.text('Controls.EntityDialog.EditRecordTitle');
                var title = Q.coalesce(this.getEntityNameFieldValue(), '');
                return Q.format(titleFormat, this.getEntitySingular(), (Q.isEmptyOrNull(title) ? '' : (' (' + title + ')')));
            }
        };
        EntityDialog.prototype.updateTitle = function () {
            this.dialogTitle = this.getEntityTitle();
        };
        EntityDialog.prototype.isCloneMode = function () {
            return false;
        };
        EntityDialog.prototype.isEditMode = function () {
            return this.get_entityId() != null && !this.isCloneMode();
        };
        EntityDialog.prototype.isDeleted = function () {
            if (this.get_entityId() == null) {
                return false;
            }
            var isDeletedProperty = this.getIsDeletedProperty();
            if (isDeletedProperty) {
                return !!this.get_entity()[isDeletedProperty];
            }
            var value = this.get_entity()[this.getIsActiveProperty()];
            if (value == null) {
                return false;
            }
            return value < 0;
        };
        EntityDialog.prototype.isNew = function () {
            return this.get_entityId() == null;
        };
        EntityDialog.prototype.isNewOrDeleted = function () {
            return this.isNew() || this.isDeleted();
        };
        EntityDialog.prototype.getDeleteOptions = function (callback) {
            return {};
        };
        EntityDialog.prototype.deleteHandler = function (options, callback) {
            Q.serviceCall(options);
        };
        EntityDialog.prototype.doDelete = function (callback) {
            var _this = this;
            var self = this;
            var request = {
                EntityId: this.get_entityId()
            };
            var baseOptions = {
                service: this.getService() + '/Delete',
                request: request,
                onSuccess: function (response) {
                    self.onDeleteSuccess(response);
                    if (callback != null) {
                        callback(response);
                    }
                    self.element.triggerHandler('ondatachange', [{
                            entityId: request.EntityId,
                            entity: _this.entity,
                            type: 'delete'
                        }]);
                }
            };
            var thisOptions = this.getDeleteOptions(callback);
            var finalOptions = Q.extend(baseOptions, thisOptions);
            this.deleteHandler(finalOptions, callback);
        };
        EntityDialog.prototype.onDeleteSuccess = function (response) {
        };
        EntityDialog.prototype.attrs = function (attrType) {
            return Q.getAttributes(Q.getInstanceType(this), attrType, true);
        };
        EntityDialog.prototype.getEntityType = function () {
            if (this.entityType != null)
                return this.entityType;
            var typeAttributes = this.attrs(Serenity.EntityTypeAttribute);
            if (typeAttributes.length === 1)
                return (this.entityType = typeAttributes[0].value);
            // remove global namespace
            var name = Q.getTypeFullName(Q.getInstanceType(this));
            var px = name.indexOf('.');
            if (px >= 0)
                name = name.substring(px + 1);
            // don't like this kind of convention, make it obsolete soon...
            if (Q.endsWith(name, 'Dialog') || Q.endsWith(name, 'Control'))
                name = name.substr(0, name.length - 6);
            else if (Q.endsWith(name, 'Panel'))
                name = name.substr(0, name.length - 5);
            return (this.entityType = name);
        };
        EntityDialog.prototype.getFormKey = function () {
            if (this.formKey != null)
                return this.formKey;
            var attributes = this.attrs(Serenity.FormKeyAttribute);
            if (attributes.length >= 1)
                return (this.formKey = attributes[0].value);
            return (this.formKey = this.getEntityType());
        };
        EntityDialog.prototype.getLocalTextDbPrefix = function () {
            if (this.localTextDbPrefix != null)
                return this.localTextDbPrefix;
            this.localTextDbPrefix = Q.coalesce(this.getLocalTextPrefix(), '');
            if (this.localTextDbPrefix.length > 0 && !Q.endsWith(this.localTextDbPrefix, '.'))
                this.localTextDbPrefix = 'Db.' + this.localTextDbPrefix + '.';
            return this.localTextDbPrefix;
        };
        EntityDialog.prototype.getLocalTextPrefix = function () {
            var attributes = this.attrs(Serenity.LocalTextPrefixAttribute);
            if (attributes.length >= 1)
                return attributes[0].value;
            return this.getEntityType();
        };
        EntityDialog.prototype.getEntitySingular = function () {
            if (this.entitySingular != null)
                return this.entitySingular;
            var attributes = this.attrs(Serenity.ItemNameAttribute);
            if (attributes.length >= 1) {
                this.entitySingular = attributes[0].value;
                this.entitySingular = Q.LT.getDefault(this.entitySingular, this.entitySingular);
            }
            else {
                var es = Q.tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular');
                if (es == null)
                    es = this.getEntityType();
                this.entitySingular = es;
            }
            return this.entitySingular;
        };
        EntityDialog.prototype.getNameProperty = function () {
            if (this.nameProperty != null)
                return this.nameProperty;
            var attributes = this.attrs(Serenity.NamePropertyAttribute);
            if (attributes.length >= 1)
                this.nameProperty = attributes[0].value;
            else
                this.nameProperty = 'Name';
            return this.nameProperty;
        };
        EntityDialog.prototype.getIdProperty = function () {
            if (this.idProperty != null)
                return this.idProperty;
            var attributes = this.attrs(Serenity.IdPropertyAttribute);
            if (attributes.length >= 1)
                this.idProperty = attributes[0].value;
            else
                this.idProperty = 'ID';
            return this.idProperty;
        };
        EntityDialog.prototype.getIsActiveProperty = function () {
            if (this.isActiveProperty != null)
                return this.isActiveProperty;
            var attributes = this.attrs(Serenity.IsActivePropertyAttribute);
            if (attributes.length >= 1)
                this.isActiveProperty = attributes[0].value;
            else
                this.isActiveProperty = 'IsActive';
            return this.isActiveProperty;
        };
        EntityDialog.prototype.getIsDeletedProperty = function () {
            return null;
        };
        EntityDialog.prototype.getService = function () {
            if (this.service != null)
                return this.service;
            var attributes = this.attrs(Serenity.ServiceAttribute);
            if (attributes.length >= 1)
                this.service = attributes[0].value;
            else
                this.service = Q.replaceAll(this.getEntityType(), '.', '/');
            return this.service;
        };
        EntityDialog.prototype.load = function (entityOrId, done, fail) {
            var _this = this;
            var action = function () {
                if (entityOrId == null) {
                    _this.loadResponse({});
                    done && done();
                    return;
                }
                var scriptType = typeof (entityOrId);
                if (scriptType === 'string' || scriptType === 'number') {
                    var entityId = entityOrId;
                    _this.loadById(entityId, function (response) {
                        if (done)
                            window.setTimeout(done, 0);
                    }, null);
                    return;
                }
                var entity = entityOrId || new Object();
                _this.loadResponse({ Entity: entity });
                done && done();
            };
            if (fail == null) {
                action();
                return;
            }
            try {
                action();
            }
            catch (ex1) {
                var ex = Q.Exception.wrap(ex1);
                fail(ex);
            }
        };
        EntityDialog.prototype.loadNewAndOpenDialog = function (asPanel) {
            this.loadResponse({});
            this.dialogOpen(asPanel);
        };
        EntityDialog.prototype.loadEntityAndOpenDialog = function (entity, asPanel) {
            this.loadResponse({ Entity: entity });
            this.dialogOpen(asPanel);
        };
        EntityDialog.prototype.loadResponse = function (data) {
            data = data || {};
            this.onLoadingData(data);
            var entity = data.Entity || new Object();
            this.beforeLoadEntity(entity);
            this.loadEntity(entity);
            this.set_entity(entity);
            this.afterLoadEntity();
        };
        EntityDialog.prototype.loadEntity = function (entity) {
            var idField = this.getIdProperty();
            if (idField != null)
                this.set_entityId(entity[idField]);
            this.set_entity(entity);
            if (this.propertyGrid != null) {
                this.propertyGrid.set_mode((this.isEditMode() ?
                    2 /* update */ : 1 /* insert */));
                this.propertyGrid.load(entity);
            }
        };
        EntityDialog.prototype.beforeLoadEntity = function (entity) {
            this.localizationPendingValue = null;
            this.localizationLastValue = null;
        };
        EntityDialog.prototype.afterLoadEntity = function () {
            this.updateInterface();
            this.updateTitle();
        };
        EntityDialog.prototype.loadByIdAndOpenDialog = function (entityId, asPanel) {
            var _this = this;
            this.loadById(entityId, function (response) { return window.setTimeout(function () { return _this.dialogOpen(asPanel); }, 0); }, function () {
                if (!_this.element.is(':visible')) {
                    _this.element.remove();
                }
            });
        };
        EntityDialog.prototype.onLoadingData = function (data) {
        };
        EntityDialog.prototype.getLoadByIdOptions = function (id, callback) {
            return {};
        };
        EntityDialog.prototype.getLoadByIdRequest = function (id) {
            var request = {};
            request.EntityId = id;
            return request;
        };
        EntityDialog.prototype.reloadById = function () {
            this.loadById(this.get_entityId());
        };
        EntityDialog.prototype.loadById = function (id, callback, fail) {
            var _this = this;
            var baseOptions = {
                service: this.getService() + '/Retrieve',
                blockUI: true,
                request: this.getLoadByIdRequest(id),
                onSuccess: function (response) {
                    _this.loadResponse(response);
                    callback && callback(response);
                },
                onCleanup: function () {
                    if (_this.validator != null) {
                        Q.validatorAbortHandler(_this.validator);
                    }
                }
            };
            var thisOptions = this.getLoadByIdOptions(id, callback);
            var finalOptions = Q.extend(baseOptions, thisOptions);
            this.loadByIdHandler(finalOptions, callback, fail);
        };
        EntityDialog.prototype.loadByIdHandler = function (options, callback, fail) {
            var request = Q.serviceCall(options);
            fail && request.fail(fail);
        };
        EntityDialog.prototype.initLocalizationGrid = function () {
            var pgDiv = this.byId('PropertyGrid');
            if (pgDiv.length <= 0) {
                return;
            }
            var pgOptions = this.getPropertyGridOptions();
            this.initLocalizationGridCommon(pgOptions);
        };
        EntityDialog.prototype.initLocalizationGridCommon = function (pgOptions) {
            var pgDiv = this.byId('PropertyGrid');
            if (!Q.any(pgOptions.items, function (x) { return x.localizable === true; }))
                return;
            var localGridDiv = $('<div/>')
                .attr('id', this.idPrefix + 'LocalizationGrid')
                .hide().insertAfter(pgDiv);
            pgOptions.idPrefix = this.idPrefix + 'Localization_';
            var items = [];
            for (var _i = 0, _a = pgOptions.items; _i < _a.length; _i++) {
                var item1 = _a[_i];
                var langs = null;
                if (item1.localizable === true) {
                    var copy = Q.extend({}, item1);
                    copy.oneWay = true;
                    copy.readOnly = true;
                    copy.required = false;
                    copy.defaultValue = null;
                    items.push(copy);
                    if (langs == null)
                        langs = this.getLangs();
                    for (var _b = 0, langs_1 = langs; _b < langs_1.length; _b++) {
                        var lang = langs_1[_b];
                        copy = Q.extend({}, item1);
                        copy.name = lang[0] + '$' + copy.name;
                        copy.title = lang[1];
                        copy.cssClass = [copy.cssClass, 'translation'].join(' ');
                        copy.insertable = true;
                        copy.updatable = true;
                        copy.oneWay = false;
                        copy.required = false;
                        copy.localizable = false;
                        copy.defaultValue = null;
                        items.push(copy);
                    }
                }
            }
            pgOptions.items = items;
            this.localizationGrid = (new Serenity.PropertyGrid(localGridDiv, pgOptions)).init(null);
            localGridDiv.addClass('s-LocalizationGrid');
        };
        EntityDialog.prototype.isLocalizationMode = function () {
            return this.localizationButton != null && this.localizationButton.hasClass('pressed');
        };
        EntityDialog.prototype.isLocalizationModeAndChanged = function () {
            if (!this.isLocalizationMode()) {
                return false;
            }
            var newValue = this.getLocalizationGridValue();
            return $.toJSON(this.localizationLastValue) != $.toJSON(newValue);
        };
        EntityDialog.prototype.localizationButtonClick = function () {
            if (this.isLocalizationMode() && !this.validateForm()) {
                return;
            }
            if (this.isLocalizationModeAndChanged()) {
                var newValue = this.getLocalizationGridValue();
                this.localizationLastValue = newValue;
                this.localizationPendingValue = newValue;
            }
            this.localizationButton.toggleClass('pressed');
            this.updateInterface();
            if (this.isLocalizationMode()) {
                this.loadLocalization();
            }
        };
        EntityDialog.prototype.getLanguages = function () {
            if (Serenity.EntityDialog.defaultLanguageList != null)
                return Serenity.EntityDialog.defaultLanguageList() || [];
            return [];
        };
        // for compatibility with older getLanguages methods written in Saltaralle
        EntityDialog.prototype.getLangs = function () {
            var langsTuple = this.getLanguages();
            var langs = Q.safeCast(langsTuple, Array);
            if (langs == null || langs.length === 0 ||
                langs[0] == null || !Q.isArray(langs[0])) {
                langs = Array.prototype.slice.call(langsTuple.map(function (x) {
                    return [x.item1, x.item2];
                }));
            }
            return langs;
        };
        EntityDialog.prototype.loadLocalization = function () {
            var _this = this;
            if (this.localizationLastValue == null && this.isNew()) {
                this.localizationGrid.load({});
                this.setLocalizationGridCurrentValues();
                this.localizationLastValue = this.getLocalizationGridValue();
                return;
            }
            if (this.localizationLastValue != null) {
                this.localizationGrid.load(this.localizationLastValue);
                this.setLocalizationGridCurrentValues();
                return;
            }
            var opt = {
                service: this.getService() + '/Retrieve',
                blockUI: true,
                request: {
                    EntityId: this.get_entityId(),
                    ColumnSelection: 'keyOnly',
                    IncludeColumns: ['Localizations']
                },
                onSuccess: function (response) {
                    var copy = Q.extend(new Object(), _this.get_entity());
                    if (response.Localizations) {
                        for (var _i = 0, _a = Object.keys(response.Localizations); _i < _a.length; _i++) {
                            var language = _a[_i];
                            var entity = response.Localizations[language];
                            for (var _b = 0, _c = Object.keys(entity); _b < _c.length; _b++) {
                                var key = _c[_b];
                                copy[language + '$' + key] = entity[key];
                            }
                        }
                    }
                    _this.localizationGrid.load(copy);
                    _this.setLocalizationGridCurrentValues();
                    _this.localizationPendingValue = null;
                    _this.localizationLastValue = _this.getLocalizationGridValue();
                }
            };
            Q.serviceCall(opt);
        };
        EntityDialog.prototype.setLocalizationGridCurrentValues = function () {
            var _this = this;
            var valueByName = {};
            this.localizationGrid.enumerateItems(function (item, widget) {
                if (item.name.indexOf('$') < 0 && widget.element.is(':input')) {
                    valueByName[item.name] = _this.byId(item.name).val();
                    widget.element.val(valueByName[item.name]);
                }
            });
            this.localizationGrid.enumerateItems(function (item1, widget1) {
                var idx = item1.name.indexOf('$');
                if (idx >= 0 && widget1.element.is(':input')) {
                    var hint = valueByName[item1.name.substr(idx + 1)];
                    if (hint != null && hint.length > 0) {
                        widget1.element.attr('title', hint).attr('placeholder', hint);
                    }
                }
            });
        };
        EntityDialog.prototype.getLocalizationGridValue = function () {
            var value = {};
            this.localizationGrid.save(value);
            for (var _i = 0, _a = Object.keys(value); _i < _a.length; _i++) {
                var k = _a[_i];
                if (k.indexOf('$') < 0) {
                    delete value[k];
                }
            }
            return value;
        };
        EntityDialog.prototype.getPendingLocalizations = function () {
            if (this.localizationPendingValue == null) {
                return null;
            }
            var result = {};
            var idField = this.getIdProperty();
            var langs = this.getLangs();
            for (var _i = 0, langs_2 = langs; _i < langs_2.length; _i++) {
                var pair = langs_2[_i];
                var language = pair[0];
                var entity = {};
                if (idField != null) {
                    entity[idField] = this.get_entityId();
                }
                var prefix = language + '$';
                for (var _a = 0, _b = Object.keys(this.localizationPendingValue); _a < _b.length; _a++) {
                    var k = _b[_a];
                    if (Q.startsWith(k, prefix))
                        entity[k.substr(prefix.length)] = this.localizationPendingValue[k];
                }
                result[language] = entity;
            }
            return result;
        };
        EntityDialog.prototype.initPropertyGrid = function () {
            var pgDiv = this.byId('PropertyGrid');
            if (pgDiv.length <= 0) {
                return;
            }
            var pgOptions = this.getPropertyGridOptions();
            this.propertyGrid = (new Serenity.PropertyGrid(pgDiv, pgOptions)).init(null);
            if (this.element.closest('.ui-dialog').hasClass('s-Flexify')) {
                this.propertyGrid.element.children('.categories').flexHeightOnly(1);
            }
        };
        EntityDialog.prototype.getPropertyItems = function () {
            var formKey = this.getFormKey();
            return Q.getForm(formKey);
        };
        EntityDialog.prototype.getPropertyGridOptions = function () {
            return {
                idPrefix: this.idPrefix,
                items: this.getPropertyItems(),
                mode: 1 /* insert */,
                localTextPrefix: 'Forms.' + this.getFormKey() + '.',
                useCategories: true
            };
        };
        EntityDialog.prototype.validateBeforeSave = function () {
            return true;
        };
        EntityDialog.prototype.getSaveOptions = function (callback) {
            var _this = this;
            var opt = {};
            opt.service = this.getService() + '/' + (this.isEditMode() ? 'Update' : 'Create'),
                opt.onSuccess = function (response) {
                    _this.onSaveSuccess(response);
                    callback && callback(response);
                    var typ = (_this.isEditMode() ? 'update' : 'create');
                    var ent = opt.request == null ? null : opt.request.Entity;
                    var eid = _this.isEditMode() ? _this.get_entityId() :
                        (response == null ? null : response.EntityId);
                    var dci = {
                        type: typ,
                        entity: ent,
                        entityId: eid
                    };
                    _this.element.triggerHandler('ondatachange', [dci]);
                };
            opt.onCleanup = function () {
                _this.validator && Q.validatorAbortHandler(_this.validator);
            };
            opt.request = this.getSaveRequest();
            return opt;
        };
        EntityDialog.prototype.getSaveEntity = function () {
            var entity = new Object();
            if (this.propertyGrid != null) {
                this.propertyGrid.save(entity);
            }
            if (this.isEditMode()) {
                var idField = this.getIdProperty();
                if (idField != null && entity[idField] == null) {
                    entity[idField] = this.get_entityId();
                }
            }
            return entity;
        };
        EntityDialog.prototype.getSaveRequest = function () {
            var entity = this.getSaveEntity();
            var req = {};
            req.Entity = entity;
            if (this.isEditMode()) {
                var idField = this.getIdProperty();
                if (idField != null) {
                    req.EntityId = this.get_entityId();
                }
            }
            if (this.localizationPendingValue != null) {
                req.Localizations = this.getPendingLocalizations();
            }
            return req;
        };
        EntityDialog.prototype.onSaveSuccess = function (response) {
        };
        EntityDialog.prototype.save_submitHandler = function (callback) {
            var options = this.getSaveOptions(callback);
            this.saveHandler(options, callback);
        };
        EntityDialog.prototype.save = function (callback) {
            var _this = this;
            return Serenity.ValidationHelper.submit(this.byId('Form'), function () { return _this.validateBeforeSave(); }, function () { return _this.save_submitHandler(callback); });
        };
        EntityDialog.prototype.saveHandler = function (options, callback) {
            Q.serviceCall(options);
        };
        EntityDialog.prototype.initToolbar = function () {
            _super.prototype.initToolbar.call(this);
            if (!this.toolbar)
                return;
            this.saveAndCloseButton = this.toolbar.findButton('save-and-close-button');
            this.applyChangesButton = this.toolbar.findButton('apply-changes-button');
            this.deleteButton = this.toolbar.findButton('delete-button');
            this.undeleteButton = this.toolbar.findButton('undo-delete-button');
            this.editButton = this.toolbar.findButton('edit-button');
            this.cloneButton = this.toolbar.findButton('clone-button');
            this.localizationButton = this.toolbar.findButton('localization-button');
        };
        EntityDialog.prototype.showSaveSuccessMessage = function (response) {
            Q.notifySuccess(Q.text('Controls.EntityDialog.SaveSuccessMessage'), '', null);
        };
        EntityDialog.prototype.getToolbarButtons = function () {
            var _this = this;
            var list = [];
            list.push({
                title: Q.text('Controls.EntityDialog.SaveButton'),
                cssClass: 'save-and-close-button',
                hotkey: 'alt+s',
                onClick: function () {
                    _this.save(function (response) {
                        _this.dialogClose();
                    });
                },
                visible: function () { return !_this.isDeleted() && !_this.isViewMode(); },
                disabled: function () { return !_this.hasSavePermission() || _this.readOnly; }
            });
            list.push({
                title: '',
                hint: Q.text('Controls.EntityDialog.ApplyChangesButton'),
                cssClass: 'apply-changes-button',
                hotkey: 'alt+a',
                onClick: function () {
                    _this.save(function (response1) {
                        if (_this.isEditMode()) {
                            var id1 = response1.EntityId;
                            if (id1 == null) {
                                id1 = _this.get_entityId();
                            }
                            _this.loadById(id1);
                        }
                        else {
                            _this.loadById(response1.EntityId);
                        }
                        _this.showSaveSuccessMessage(response1);
                    });
                },
                visible: function () { return !_this.isDeleted() && !_this.isViewMode(); },
                disabled: function () { return !_this.hasSavePermission() || _this.readOnly; }
            });
            list.push({
                title: Q.text('Controls.EntityDialog.DeleteButton'),
                cssClass: 'delete-button',
                hotkey: 'alt+x',
                onClick: function () {
                    Q.confirm(Q.text('Controls.EntityDialog.DeleteConfirmation'), function () {
                        _this.doDelete(function () { return _this.dialogClose(); });
                    });
                },
                visible: function () { return _this.isEditMode() && !_this.isDeleted() && !_this.isViewMode(); },
                disabled: function () { return !_this.hasDeletePermission() || _this.readOnly; }
            });
            list.push({
                title: Q.text('Controls.EntityDialog.UndeleteButton'),
                cssClass: 'undo-delete-button',
                onClick: function () {
                    if (_this.isDeleted()) {
                        Q.confirm(Q.text('Controls.EntityDialog.UndeleteConfirmation'), function () {
                            _this.undelete(function () { return _this.loadById(_this.get_entityId()); });
                        });
                    }
                },
                visible: function () { return _this.isEditMode() && _this.isDeleted() && !_this.isViewMode(); },
                disabled: function () { return !_this.hasDeletePermission() || _this.readOnly; }
            });
            if (this.useViewMode()) {
                list.push({
                    title: Q.text('Controls.EntityDialog.EditButton'),
                    cssClass: 'edit-button',
                    icon: 'fa-edit',
                    onClick: function () {
                        if (!_this.isEditMode())
                            return;
                        _this.editClicked = true;
                        _this.updateInterface();
                        _this.updateTitle();
                    },
                    visible: function () { return _this.isViewMode(); },
                    disabled: function () { return !_this.hasSavePermission() || _this.readOnly; }
                });
            }
            list.push({
                title: Q.text('Controls.EntityDialog.LocalizationButton'),
                cssClass: 'localization-button',
                onClick: function () { return _this.localizationButtonClick(); }
            });
            list.push({
                title: Q.text('Controls.EntityDialog.CloneButton'),
                cssClass: 'clone-button',
                onClick: function () {
                    if (!_this.isEditMode()) {
                        return;
                    }
                    var cloneEntity = _this.getCloningEntity();
                    Serenity.Widget.create({
                        type: Q.getInstanceType(_this),
                        init: function (w) { return Serenity.SubDialogHelper.bubbleDataChange(Serenity.SubDialogHelper.cascade(w, _this.element), _this, true)
                            .loadEntityAndOpenDialog(cloneEntity, null); }
                    });
                },
                visible: function () { return false; },
                disabled: function () { return !_this.hasInsertPermission() || _this.readOnly; }
            });
            return list;
        };
        EntityDialog.prototype.getCloningEntity = function () {
            var clone = new Object();
            clone = Q.extend(clone, this.get_entity());
            var idField = this.getIdProperty();
            if (!Q.isEmptyOrNull(idField)) {
                delete clone[idField];
            }
            var isActiveField = this.getIsActiveProperty();
            if (!Q.isEmptyOrNull(isActiveField)) {
                delete clone[isActiveField];
            }
            var isDeletedField = this.getIsDeletedProperty();
            if (!Q.isEmptyOrNull(isDeletedField)) {
                delete clone[isDeletedField];
            }
            return clone;
        };
        EntityDialog.prototype.updateInterface = function () {
            Serenity.EditorUtils.setContainerReadOnly(this.byId('Form'), false);
            var isDeleted = this.isDeleted();
            var isLocalizationMode = this.isLocalizationMode();
            var hasSavePermission = this.hasSavePermission();
            var viewMode = this.isViewMode();
            var isDeleted = this.isDeleted();
            var readOnly = this.readOnly;
            this.toolbar.updateInterface();
            if (this.tabs != null) {
                Serenity.TabsExtensions.setDisabled(this.tabs, 'Log', this.isNewOrDeleted());
            }
            if (this.propertyGrid != null) {
                this.propertyGrid.element.toggle(!isLocalizationMode);
            }
            if (this.localizationGrid != null) {
                this.localizationGrid.element.toggle(isLocalizationMode);
            }
            if (this.localizationButton != null) {
                this.localizationButton.toggle(this.localizationGrid != null);
                this.localizationButton.find('.button-inner')
                    .text((this.isLocalizationMode() ?
                    Q.text('Controls.EntityDialog.LocalizationBack') :
                    Q.text('Controls.EntityDialog.LocalizationButton')));
            }
            if (isLocalizationMode) {
                if (this.toolbar != null)
                    this.toolbar.findButton('tool-button')
                        .not('.localization-hidden')
                        .addClass('.localization-hidden').hide();
                this.localizationButton && this.localizationButton.show();
                return;
            }
            this.toolbar.findButton('localization-hidden')
                .removeClass('localization-hidden').show();
            this.saveAndCloseButton && this.saveAndCloseButton
                .find('.button-inner').text(Q.text((this.isNew() ? 'Controls.EntityDialog.SaveButton' :
                'Controls.EntityDialog.UpdateButton')));
            if (!hasSavePermission || viewMode || readOnly)
                Serenity.EditorUtils.setContainerReadOnly(this.byId("Form"), true);
        };
        EntityDialog.prototype.getUndeleteOptions = function (callback) {
            return {};
        };
        EntityDialog.prototype.undeleteHandler = function (options, callback) {
            Q.serviceCall(options);
        };
        EntityDialog.prototype.undelete = function (callback) {
            var _this = this;
            var baseOptions = {};
            baseOptions.service = this.getService() + '/Undelete';
            var request = {};
            request.EntityId = this.get_entityId();
            baseOptions.request = request;
            baseOptions.onSuccess = function (response) {
                callback && callback(response);
                _this.element.triggerHandler('ondatachange', [{
                        entityId: _this.get_entityId(),
                        entity: _this.entity,
                        type: 'undelete'
                    }]);
            };
            var thisOptions = this.getUndeleteOptions(callback);
            var finalOptions = Q.extend(baseOptions, thisOptions);
            this.undeleteHandler(finalOptions, callback);
        };
        Object.defineProperty(EntityDialog.prototype, "readOnly", {
            get: function () {
                return this.get_readOnly();
            },
            set: function (value) {
                this.set_readOnly(value);
            },
            enumerable: true,
            configurable: true
        });
        EntityDialog.prototype.get_readOnly = function () {
            return !!this._readonly;
        };
        EntityDialog.prototype.set_readOnly = function (value) {
            if (!!this._readonly != !!value) {
                this._readonly = !!value;
                this.updateInterface();
                this.updateTitle();
            }
        };
        EntityDialog.prototype.getInsertPermission = function () {
            return null;
        };
        EntityDialog.prototype.getUpdatePermission = function () {
            return null;
        };
        EntityDialog.prototype.getDeletePermission = function () {
            return null;
        };
        EntityDialog.prototype.hasDeletePermission = function () {
            var deletePermission = this.getDeletePermission();
            return deletePermission == null || Q.Authorization.hasPermission(deletePermission);
        };
        EntityDialog.prototype.hasInsertPermission = function () {
            var insertPermission = this.getInsertPermission();
            return insertPermission == null || Q.Authorization.hasPermission(insertPermission);
        };
        EntityDialog.prototype.hasUpdatePermission = function () {
            var updatePermission = this.getUpdatePermission();
            return updatePermission == null || Q.Authorization.hasPermission(updatePermission);
        };
        EntityDialog.prototype.hasSavePermission = function () {
            return this.isNew() ? this.hasInsertPermission() : this.hasUpdatePermission();
        };
        EntityDialog.prototype.isViewMode = function () {
            return this.useViewMode() && this.isEditMode() && !this.editClicked;
        };
        EntityDialog.prototype.useViewMode = function () {
            return false;
        };
        EntityDialog = __decorate([
            Serenity.Decorators.registerClass('Serenity.EntityDialog', [Serenity['IEditDialog'], Serenity.IReadOnly])
        ], EntityDialog);
        return EntityDialog;
    }(Serenity.TemplatedDialog));
    Serenity.EntityDialog = EntityDialog;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-dialogs.js.map
$.widget("ui.dialogExtend", {
    options: {
        "dblclick": true,
        "load": null,
        "beforeRestore": null,
        "restore": null,
        "maximizable": false,
        "beforeMaximize": null,
        "maximize": null
    },
    _create: function () {
        this._state = "normal";
        this._initButtons();
        this._setState("normal");
        return this._trigger("load");
    },
    _setState: function (state) {
        this.element.removeClass("ui-dialog-" + this._state).addClass("ui-dialog-" + state);
        return this._state = state;
    },
    _initButtons: function () {
        var _this = this;

        this._addButton("maximize", this.options.maximizable, Q.localText('Dialogs.MaximizeHint'), "fa fa-window-maximize");
        this._addButton("restore", false, Q.localText('Dialogs.RestoreHint'), "fa fa-window-restore");

        var titlebar = this.element.closest('.ui-dialog').children('.ui-dialog-titlebar');
        titlebar.dblclick(function (evt) {
            if (_this.options.dblclick) {
                if (_this._state !== "normal") {
                    return _this.restore();
                } else {
                    return _this[_this.options.dblclick]();
                }
            }
        }).select(function () {
            return false;
        });
    },
    _addButton: function (name, show, hint, icon) {
        var _this = this;

        var titlebar = this.element.closest('.ui-dialog').children('.ui-dialog-titlebar');
        var closeButton = titlebar.find('.ui-dialog-titlebar-close').first();
        var button = $('<button class="ui-button ui-corner-all ui-button-icon-only ui-dialog-titlebar-'
            + name + '" href="javascript:;" tabindex="-1"><i class="' + icon + '"></i></a>')
            .attr('title', hint)
            .toggle(show)
            .click(function (e) {
                e.preventDefault();
                return _this[name]();
            });

        if (closeButton)
            button.insertBefore(closeButton);
        else
            button.appendTo(titlebar);

        return button;
    },
    maximize: function () {
        var newHeight, newWidth;

        newHeight = $(window).height() - 1;
        newWidth = $(window).width() - 1;
        this._trigger("beforeMaximize");
        if (this._state !== "normal") {
            this._restore();
        }
        this._saveSnapshot();
        if ($(this.element[0]).dialog("option", "draggable")) {
            $(this.element[0]).dialog("widget").draggable("option", "handle", null).find(".ui-dialog-draggable-handle").css("cursor", "text").end();
        }
        $(this.element[0]).dialog("widget").css("position", "fixed").find(".ui-dialog-content").show().dialog("widget").find(".ui-dialog-buttonpane").show().end().find(".ui-dialog-content").dialog("option", {
            "resizable": false,
            "draggable": false,
            "height": newHeight,
            "width": newWidth,
            "position": {
                of: window,
                my: "left top",
                at: "left top"
            }
        });
        this._setState("maximized");
        this._toggleButtons();

        if (this.original_config_resizable)
            this.element.closest('.ui-dialog').triggerHandler("resize");

        return this._trigger("maximize");
    },
    _restore_maximized: function () {
        var original;

        original = this._loadSnapshot();
        $(this.element[0]).dialog("widget").css("position", original.position.mode).find(".ui-dialog-titlebar").css("white-space", original.titlebar.wrap).end().find(".ui-dialog-content").dialog("option", {
            "resizable": original.config.resizable,
            "draggable": original.config.draggable,
            "height": original.size.height,
            "width": original.size.width,
            "maxHeight": original.size.maxHeight,
            "position": {
                of: window,
                my: "left top",
                at: "left+" + original.position.left + " top+" + original.position.top
            }
        });
        if ($(this.element[0]).dialog("option", "draggable")) {
            return $(this.element[0]).dialog("widget").draggable("option", "handle", $(this.element[0]).dialog("widget").find(".ui-dialog-draggable-handle").length ? $(this.element[0]).dialog("widget").find(".ui-dialog-draggable-handle") : ".ui-dialog-titlebar").find(".ui-dialog-draggable-handle").css("cursor", "move");
        }
    },
    state: function () {
        return this._state;
    },
    restore: function () {
        this._trigger("beforeRestore");
        this._restore();
        this._setState("normal");
        this._toggleButtons();

        if (this.original_config_resizable)
            this.element.closest('.ui-dialog').triggerHandler("resize");

        return this._trigger("restore");
    },
    _restore: function () {
        if (this._state !== "normal") {
            return this["_restore_" + this._state]();
        }
    },
    _saveSnapshot: function () {
        if (this._state === "normal") {
            this.original_config_resizable = $(this.element[0]).dialog("option", "resizable");
            this.original_config_draggable = $(this.element[0]).dialog("option", "draggable");
            this.original_size_height = $(this.element[0]).dialog("widget").outerHeight();
            this.original_size_width = $(this.element[0]).dialog("option", "width");
            this.original_size_maxHeight = $(this.element[0]).dialog("option", "maxHeight");
            this.original_position_mode = $(this.element[0]).dialog("widget").css("position");
            this.original_position_left = $(this.element[0]).dialog("widget").offset().left - $('body').scrollLeft();
            this.original_position_top = $(this.element[0]).dialog("widget").offset().top - $('body').scrollTop();
            return this.original_titlebar_wrap = $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar").css("white-space");
        }
    },
    _loadSnapshot: function () {
        return {
            "config": {
                "resizable": this.original_config_resizable,
                "draggable": this.original_config_draggable
            },
            "size": {
                "height": this.original_size_height,
                "width": this.original_size_width,
                "maxHeight": this.original_size_maxHeight
            },
            "position": {
                "mode": this.original_position_mode,
                "left": this.original_position_left,
                "top": this.original_position_top
            },
            "titlebar": {
                "wrap": this.original_titlebar_wrap
            }
        };
    },
    _toggleButtons: function () {
        var uiDialog = this.element.closest('.ui-dialog');
        uiDialog.find(".ui-dialog-titlebar-restore").toggle(this._state !== "normal");
        uiDialog.find(".ui-dialog-titlebar-maximize").toggle(this._state !== "maximized");
    }
});
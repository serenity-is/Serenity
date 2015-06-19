(function() {
  var $;

  $ = jQuery;

  $.widget("ui.dialogExtend", {
    version: "2.0.0",
    modes: {},
    options: {
      "closable": true,
      "dblclick": false,
      "titlebar": false,
      "icons": {
        "close": "ui-icon-closethick",
        "restore": "ui-icon-newwin"
      },
      "load": null,
      "beforeRestore": null,
      "restore": null
    },
    _create: function() {
      this._state = "normal";
      if (!$(this.element[0]).data("ui-dialog")) {
        $.error("jQuery.dialogExtend Error : Only jQuery UI Dialog element is accepted");
      }
      this._verifyOptions();
      this._initButtons();
      this._initTitleBar();
      this._setState("normal");
      this._on("load", function(e) {
        return console.log("test", e);
      });
      return this._trigger("load");
    },
    _setState: function(state) {
      $(this.element[0]).removeClass("ui-dialog-" + this._state).addClass("ui-dialog-" + state);
      return this._state = state;
    },
    _verifyOptions: function() {
      var name, _ref, _results;

      if (this.options.dblclick && !(this.options.dblclick in this.modes)) {
        $.error("jQuery.dialogExtend Error : Invalid <dblclick> value '" + this.options.dblclick + "'");
        this.options.dblclick = false;
      }
      if (this.options.titlebar && ((_ref = this.options.titlebar) !== "none" && _ref !== "transparent")) {
        $.error("jQuery.dialogExtend Error : Invalid <titlebar> value '" + this.options.titlebar + "'");
        this.options.titlebar = false;
      }
      _results = [];
      for (name in this.modes) {
        if (this["_verifyOptions_" + name]) {
          _results.push(this["_verifyOptions_" + name]());
        } else {
          _results.push(void 0);
        }
      }
      return _results;
    },
    _initButtons: function() {
      var buttonPane, mode, name, titlebar, _ref,
        _this = this;

      titlebar = $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar");
      buttonPane = $('<div class="ui-dialog-titlebar-buttonpane"></div>').appendTo(titlebar);
      buttonPane.css({
        "position": "absolute",
        "top": "50%",
        "right": "0.3em",
        "margin-top": "-10px",
        "height": "18px"
      });
      titlebar.addClass("has-maximize-restore");

        buttonPane.append('<a class="ui-dialog-titlebar-restore ui-corner-all ui-state-default" href="#"><span class="ui-icon ' + this.options.icons.restore + '">Önceki Boyutuna Dön</span></a>')
            .find('.ui-dialog-titlebar-restore').attr("role", "button").attr('title', 'Önceki Boyutuna Dön').mouseover(function () {
        return $(this).addClass("ui-state-hover");
      }).mouseout(function() {
        return $(this).removeClass("ui-state-hover");
      }).focus(function() {
        return $(this).addClass("ui-state-focus");
      }).blur(function() {
        return $(this).removeClass("ui-state-focus");
      }).end().find(".ui-dialog-titlebar-closex").toggle(this.options.closable).end().find(".ui-dialog-titlebar-restore").hide().click(function(e) {
        e.preventDefault();
        return _this.restore();
      }).end();
      _ref = this.modes;
      for (name in _ref) {
        mode = _ref[name];
        this._initModuleButton(name, mode);
      }
      titlebar.find("a").attr("tabindex", -1);
      return titlebar.dblclick(function(evt) {
        if (_this.options.dblclick) {
          if (_this._state !== "normal") {
            return _this.restore();
          } else {
            return _this[_this.options.dblclick]();
          }
        }
      }).select(function() {
        return false;
      });
    },
    _initModuleButton: function(name, mode) {
      var buttonPane,
        _this = this;

      buttonPane = $(this.element[0]).dialog("widget").find('.ui-dialog-titlebar-buttonpane');
      var result = buttonPane.append('<a class="ui-dialog-titlebar-' + name + ' ui-corner-all ui-state-default" href="#"><span class="ui-icon ' + this.options.icons[name] + '">' + name + '</span></a>').find(".ui-dialog-titlebar-" + name).attr("role", "button").mouseover(function() {
        return $(this).addClass("ui-state-hover");
      }).mouseout(function() {
        return $(this).removeClass("ui-state-hover");
      }).focus(function() {
        return $(this).addClass("ui-state-focus");
      }).blur(function() {
        return $(this).removeClass("ui-state-focus");
      }).end().find(".ui-dialog-titlebar-" + name).toggle(this.options[mode.option]).click(function(e) {
        e.preventDefault();
        return _this[name]();
      }).end();

      if (name == 'maximize')
          buttonPane.find('a.ui-dialog-titlebar-maximize').attr('title', 'Ekranı Kapla').find('span').text('Ekranı Kapla');

      return result;
    },
    _initTitleBar: function() {
      var handle;

      switch (this.options.titlebar) {
        case false:
          return 0;
        case "none":
          if ($(this.element[0]).dialog("option", "draggable")) {
            handle = $("<div />").addClass("ui-dialog-draggable-handle").css("cursor", "move").height(5);
            $(this.element[0]).dialog("widget").prepend(handle).draggable("option", "handle", handle);
          }
          return $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar").find(".ui-dialog-title").html("&nbsp;").end().css({
            "background-color": "transparent",
            "background-image": "none",
            "border": 0,
            "position": "absolute",
            "right": 0,
            "top": 0,
            "z-index": 9999
          }).end();
        case "transparent":
          return $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar").css({
            "background-color": "transparent",
            "background-image": "none",
            "border": 0
          });
        default:
          return $.error("jQuery.dialogExtend Error : Invalid <titlebar> value '" + this.options.titlebar + "'");
      }
    },
    state: function() {
      return this._state;
    },
    restore: function() {
      this._trigger("beforeRestore");
      this._restore();
      this._setState("normal");
      this._toggleButtons();

      if (this.original_config_resizable)
          this.element.closest('.ui-dialog').triggerHandler("resize");

      return this._trigger("restore");
    },
    _restore: function() {
      if (this._state !== "normal") {
        return this["_restore_" + this._state]();
      }
    },
    _saveSnapshot: function() {
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
    _loadSnapshot: function() {
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
    _toggleButtons: function() {
      var mode, name, _ref, _results;

      $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar-restore").toggle(this._state !== "normal").css({
        "right": "1.4em"
      }).end();
      _ref = this.modes;
      _results = [];
      for (name in _ref) {
        mode = _ref[name];
        _results.push($(this.element[0]).dialog("widget").find(".ui-dialog-titlebar-" + name).toggle(this._state !== mode.state && this.options[mode.option]));
      }
      return _results;
    }
  });

}).call(this);

(function() {
  var $;

  $ = jQuery;

  $.extend(true, $.ui.dialogExtend.prototype, {
    modes: {
      "collapse": {
        option: "collapsable",
        state: "collapsed"
      }
    },
    options: {
      "collapsable": false,
      "icons": {
        "collapse": "ui-icon-triangle-1-s"
      },
      "beforeCollapse": null,
      "collapse": null
    },
    collapse: function() {
      var newHeight;

      newHeight = $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar").height() + 15;
      this._trigger("beforeCollapse");
      if (this._state !== "normal") {
        this._restore();
      }
      this._saveSnapshot();
      $(this.element[0]).dialog("option", {
        "resizable": false,
        "height": newHeight,
        "maxHeight": newHeight
      }).on('dialogclose', this._collapse_restore).hide().dialog("widget").find(".ui-dialog-buttonpane:visible").hide().end().find(".ui-dialog-titlebar").css("white-space", "nowrap").end().find(".ui-dialog-content");
      this._setState("collapsed");
      this._toggleButtons();
      return this._trigger("collapse");
    },
    _restore_collapsed: function() {
      var original;

      original = this._loadSnapshot();
      return $(this.element[0]).show().dialog("widget").find(".ui-dialog-buttonpane:hidden").show().end().find(".ui-dialog-titlebar").css("white-space", original.titlebar.wrap).end().find(".ui-dialog-content").dialog("option", {
        "resizable": original.config.resizable,
        "height": original.size.height,
        "maxHeight": original.size.maxHeight
      }).off('dialogclose', this._collapse_restore);
    },
    _collapse_restore: function() {
      return $(this).dialogExtend("restore");
    }
  });

}).call(this);

(function() {
  var $;

  $ = jQuery;

  $.extend(true, $.ui.dialogExtend.prototype, {
    modes: {
      "maximize": {
        option: "maximizable",
        state: "maximized"
      }
    },
    options: {
      "maximizable": false,
      "icons": {
        "maximize": "ui-icon-extlink"
      },
      "beforeMaximize": null,
      "maximize": null
    },
    maximize: function() {
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
    _restore_maximized: function() {
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
    }
  });

}).call(this);

(function() {
  var $;

  $ = jQuery;

  $.extend(true, $.ui.dialogExtend.prototype, {
    modes: {
      "minimize": {
        option: "minimizable",
        state: "minimized"
      }
    },
    options: {
      "minimizable": false,
      "minimizeLocation": "left",
      "icons": {
        "minimize": "ui-icon-minus"
      },
      "beforeMinimize": null,
      "minimize": null
    },
    minimize: function() {
      var fixedContainer, newHeight, newWidth, overlay;

      newHeight = $(this.element[0]).dialog("widget").find(".ui-dialog-titlebar").height() + 15;
      newWidth = 200;
      if ($("#dialog-extend-fixed-container").length) {
        fixedContainer = $("#dialog-extend-fixed-container");
      } else {
        fixedContainer = $('<div id="dialog-extend-fixed-container"></div>').appendTo("body");
      }
      fixedContainer.css({
        "position": "fixed",
        "bottom": 1,
        "left": 1,
        "right": 1,
        "z-index": 9999
      });
      overlay = $('<div/>').css({
        "float": this.options.minimizeLocation,
        "margin": 1
      });
      fixedContainer.append(overlay);
      $(this.element[0]).data("dialog-extend-minimize-overlay", overlay);
      this._trigger("beforeMinimize");
      this._saveSnapshot();
      if ($(this.element[0]).dialog("option", "draggable")) {
        $(this.element[0]).dialog("widget").draggable("option", "handle", null).find(".ui-dialog-draggable-handle").css("cursor", "text").end();
      }
      $(this.element[0]).dialog("option", {
        "resizable": false,
        "draggable": false,
        "height": newHeight,
        "width": newWidth
      }).on('dialogclose', this._minimize_removeOverlay).dialog("widget").css("position", "static").appendTo(overlay).find(".ui-dialog-content").dialog("widget").find(".ui-dialog-titlebar").each(function() {
        var buttonPane, titleText, titlebar;

        titlebar = $(this);
        buttonPane = titlebar.find(".ui-dialog-titlebar-buttonpane");
        titleText = titlebar.find(".ui-dialog-title");
        return titleText.css({
          'overflow': 'hidden',
          'width': titlebar.width() - buttonPane.width() + 10
        });
      }).end().find(".ui-dialog-content").hide().dialog("widget").find(".ui-dialog-buttonpane:visible").hide().end().find(".ui-dialog-titlebar").css("white-space", "nowrap").end().find(".ui-dialog-content");
      this._setState("minimized");
      this._toggleButtons();
      return this._trigger("minimize");
    },
    _restore_minimized: function() {
      var original;

      original = this._loadSnapshot();
      $(this.element[0]).dialog("widget").appendTo("body").css({
        "float": "none",
        "margin": 0,
        "position": original.position.mode
      }).find(".ui-dialog-content").dialog("widget").find(".ui-dialog-title").css("width", "auto").end().find(".ui-dialog-content").show().dialog("widget").find(".ui-dialog-buttonpane:hidden").show().end().find(".ui-dialog-titlebar").css("white-space", original.titlebar.wrap).end().find(".ui-dialog-content").dialog("option", {
        "resizable": original.config.resizable,
        "draggable": original.config.draggable,
        "height": original.size.height,
        "width": original.size.width,
        "maxHeight": original.size.maxHeight,
        "position": {
          my: "left top",
          at: "left+" + original.position.left + " top+" + original.position.top
        }
      }).off('dialogclose', this._minimize_removeOverlay);
      if ($(this.element[0]).dialog("option", "draggable")) {
        $(this.element[0]).dialog("widget").draggable("option", "handle", $(this.element[0]).dialog("widget").find(".ui-dialog-draggable-handle").length ? $(this.element[0]).dialog("widget").find(".ui-dialog-draggable-handle") : ".ui-dialog-titlebar").find(".ui-dialog-draggable-handle").css("cursor", "move");
      }
      $(this.element[0]).data("dialog-extend-minimize-overlay").remove();
      return $(this.element[0]).removeData("dialog-extend-overlay");
    },
    _verifyOptions_minimize: function() {
      var _ref;

      if (!this.options.minimizeLocation || ((_ref = this.options.minimizeLocation) !== 'left' && _ref !== 'right')) {
        $.error("jQuery.dialogExtend Error : Invalid <minimizeLocation> value '" + this.options.minimizeLocation + "'");
        return this.options.minimizeLocation = "left";
      }
    },
    _minimize_removeOverlay: function() {
      $(this).dialogExtend("restore");
      $(this).dialog("widget").appendTo($('body'));
      $(this).data("dialog-extend-minimize-overlay").remove();
      return $(this).removeData("dialog-extend-overlay");
    }
  });

}).call(this);

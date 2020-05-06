declare var Vue: any;

namespace Q {

    function jQueryDatepickerInitialization(): void {
        let order = Q.Culture.dateOrder;
        let s = Q.Culture.dateSeparator;
        let culture = ($('html').attr('lang') || 'en').toLowerCase();
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
            buttonImage: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAAHNSURBVEhLtZU/S8NAGIf7FfpJHDoX136ADp3s4qDgUpeCDlJEEIcubk7ZFJ06OBRcdJAOdhCx6CAWodJFrP8KFexrnhcv3sVLVYyBp/fLXe75hbSkGRFxaLVaEgSBMMbXkpi0Rz8eVlbkrlhUdnM56XQ6Opq57/DtwRkVMCFMhJzn89JoNHQ0c9/h26NOU3BdKMh4eVneFhflcGpK1rNZHTn/CfE9uHA6BSyMFhZkODsrt2E7I+c/Ib4Hl1NwOT0dXcgx2N6Wh+FQx16vp/O/ARdO3FrAs2PhcWZGm3l+9sj8b8Cl34ddwMKgVEoFXE5BUK9zkurx4fwseBuPUyWxYKfZTCQIf+txtvb2HLwFm7WaTg5fX1V0cnWlxPPR2ZmC2GSkB+22QsaBC6dTwMLzaPTnAhy4vhSwcP/yEl1s2D8+ngjFNjhwJRZwt9wB2Jm79WVuws4TC3qDgUrJYGekvozUzjjITsFGtarNpsBw2u0654ghnhEbcODCmVjAewjsjMyXkdo5seDm6Uku+n2VcgHYGakvI7UzDlxOwWql4hT4QOoDqY0pwBkVLM3NRQVpgAtnVFApl3UyTXBGBWvhX9x8+JpNE5xRwf8hmXe+7B9dZrOuOwAAAABJRU5ErkJggg==',
            buttonImageOnly: true,
            showOn: 'both',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true
        });
    };

    if ($.datepicker && $.datepicker.regional && $.datepicker.regional.en)
        jQueryDatepickerInitialization();
    else $(function () {
        $.datepicker && $.datepicker.regional && $.datepicker.regional.en && jQueryDatepickerInitialization();
    });

    function jQueryUIInitialization(): void {
        $.ui.dialog.prototype._allowInteraction = function (event: any) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };

        (function (orig) {
            $.ui.dialog.prototype._focusTabbable = function () {
                if ($(document.body).hasClass('mobile-device')) {
                    this.uiDialog && this.uiDialog.focus();
                    return;
                }
                orig.call(this);
            }
        })($.ui.dialog.prototype._focusTabbable);

        (function (orig) {
            $.ui.dialog.prototype._createTitlebar = function () {
                orig.call(this);
                this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html('<i class="fa fa-times" />');
            }
        })($.ui.dialog.prototype._createTitlebar);
    }

    if ($.ui && $.ui.dialog && $.ui.dialog.prototype) {
        jQueryUIInitialization();
    }
    else {
        jQuery(function () {
            if ($.ui && $.ui.dialog && $.ui.dialog.prototype)
                jQueryUIInitialization();
        });
    }

    (jQuery as any).cleanData = (function (orig) {
        return function (elems: any[]) {
            var events, elem, i, e;
            var cloned = elems;
            for (i = 0; (elem = cloned[i]) != null; i++) {
                try {
                    events = ($ as any)._data(elem, "events");
                    if (events && events.remove) {
                        // html collection might change during remove event, so clone it!
                        if (cloned === elems)
                            cloned = Array.prototype.slice.call(elems);
                        $(elem).triggerHandler("remove");
                        delete events.remove;
                    }
                } catch (e) { }
            }
            orig(elems);
        };
    })((jQuery as any).cleanData);

    function ssExceptionInitialization() {
        Q.Exception.prototype.toString = function () {
            return this.get_message();
        };
    };

    if (Q && Q.Exception)
        ssExceptionInitialization();
    else {
        jQuery(function ($) {
            if (Q && Q.Exception)
                ssExceptionInitialization();
        });
    }

    function vueInitialization() {
        Vue.component('editor', {
            props: {
                type: {
                    type: String,
                    required: true,                    
                },
                id: {
                    type: String,
                    required: false
                },
                name: {
                    type: String,
                    required: false
                },
                placeholder: {
                    type: String,
                    required: false
                },
                value: {
                    required: false
                },
                options: {
                    required: false
                },
                maxLength: {
                    required: false
                }
            },
            render: function (createElement: any) {
                var editorType = Serenity.EditorTypeRegistry.get(this.type);
                var elementAttr = Q.getAttributes(editorType, Serenity.ElementAttribute, true);
                var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>') as string;
                var domProps: any = {};
                var element = $(elementHtml)[0];
                var attrs = element.attributes;
                for (var i = 0; i < attrs.length; i++) {
                    var attr = attrs.item(i);
                    domProps[attr.name] = attr.value;
                }

                if (this.id != null)
                    domProps.id = this.id;

                if (this.name != null)
                    domProps.name = this.name;

                if (this.placeholder != null)
                    domProps.placeholder = this.placeholder;

                var editorParams = this.options;
                var optionsType: any = null;

                var self = this
                var el = createElement(element.tagName, {
                    domProps: domProps
                });

                this.$editorType = editorType;

                return el;
            },
            watch: {
                value: function (v: any) {
                    Serenity.EditorUtils.setValue(this.$widget, v);
                }
            },
            mounted: function () {
                var self = this;

                this.$widget = new this.$editorType($(this.$el), this.options);
                this.$widget.initialize();

                if (this.maxLength) {
                    (Serenity.PropertyGrid as any).$setMaxLength(this.$widget, this.maxLength);
                }

                if (this.options)
                    Serenity.ReflectionOptionsSetter.set(this.$widget, this.options);

                if (this.value != null)
                    Serenity.EditorUtils.setValue(this.$widget, this.value);

                if ($(this.$el).data('select2'))
                    Serenity.WX.changeSelect2(this.$widget, function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
                else
                    Serenity.WX.change(this.$widget, function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
            },
            destroyed: function () {
                if (this.$widget) {
                    this.$widget.destroy();
                    this.$widget = null;
                }
            }
        });
    }

    window['Vue'] ? vueInitialization() : $(function () { window['Vue'] && vueInitialization(); });
}

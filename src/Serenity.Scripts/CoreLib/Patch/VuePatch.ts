import { EditorTypeRegistry } from "../Types/EditorTypeRegistry";
import { getAttributes } from "../Q/System";
import { ElementAttribute } from "../Decorators";
import { ReflectionOptionsSetter } from "../UI/Widgets/ReflectionOptionsSetter";
import { EditorUtils } from "../UI/Editors/EditorUtils";

export function vuePatch(Vue: any) {
    function vueIntegration() {
        // @ts-ignore
        if (typeof Vue === "undefined")
            return false; 
        // @ts-ignore
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
                var editorType = EditorTypeRegistry.get(this.type);
                var elementAttr = getAttributes(editorType, ElementAttribute, true);
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

                var el = createElement(element.tagName, {
                    domProps: domProps
                });

                this.$editorType = editorType;

                return el;
            },
            watch: {
                value: function (v: any) {
                    EditorUtils.setValue(this.$widget, v);
                }
            },
            mounted: function () {
                var self = this;

                this.$widget = new this.$editorType($(this.$el), this.options);
                this.$widget.initialize();

                if (this.maxLength) {
                    var widget = this.$widget;
                    if (widget.element.is(':input')) {
                        if (this.maxLength > 0)
                            widget.element.attr('maxlength', this.maxLength);
                        else
                            widget.element.removeAttr('maxlength');
                    }       
                }

                if (this.options)
                    ReflectionOptionsSetter.set(this.$widget, this.options);

                if (this.value != null)
                    EditorUtils.setValue(this.$widget, this.value);

                if ($(this.$el).data('select2'))
                    this.$widget.changeSelect2(function () {
                        self.$emit('input', EditorUtils.getValue(self.$widget));
                    });
                else
                    this.$widget.change(function () {
                        self.$emit('input', EditorUtils.getValue(self.$widget));
                    });
            },
            destroyed: function () {
                if (this.$widget) {
                    this.$widget.destroy();
                    this.$widget = null;
                }
            }
        });
        return true;
    }

    // @ts-ignore
    !vueIntegration() && typeof $ !== "undefined" && $.fn && $(vueIntegration);
}
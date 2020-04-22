
namespace Q {

    export function initFormType(typ: Function, nameWidgetPairs: any[]) {
        for (var i = 0; i < nameWidgetPairs.length - 1; i += 2) {
            (function (name: string, widget: any) {
                Object.defineProperty(typ.prototype, name, {
                    get: function () {
                        return this.w(name, widget);
                    },
                    enumerable: true,
                    configurable: true
                });
            })(nameWidgetPairs[i], nameWidgetPairs[i + 1]);
        }
    }

    export function prop(type: any, name: string, getter?: string, setter?: string) {
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

    export function typeByFullName(fullName: string, global?: any): any {
        if (!fullName)
            return null;

        var parts = fullName.split('.');
        var root = global || window;
        for (var i = 0; i < parts.length; i++) {
            root = root[parts[i]];
            if (root == null)
                return null;
        }

        if (typeof root != "function")
            return null;

        return root;
    }
}
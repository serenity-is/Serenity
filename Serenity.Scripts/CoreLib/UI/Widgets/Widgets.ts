namespace Serenity {
    export class PrefixedContext {
        constructor(public readonly idPrefix: string) {
        }

        byId(id: string) {
            return $('#' + this.idPrefix + id);
        }

        w<TWidget>(id: string, type: { new (...args: any[]): TWidget }): TWidget {
            return $('#' + this.idPrefix + id).getWidget<TWidget>(type);
        }
    }

    export namespace ReflectionOptionsSetter {
        export function set(target: any, options: any): void {
            if (options == null) {
                return;
            }

            var type = Q.getInstanceType(target);

            if (type === Object) {
                return;
            }

            var propByName = type.__propByName;
            var fieldByName = type.__fieldByName;
            if (propByName == null) {
                var props = Q.getMembers(type, Q.MemberType.property);
                var propList = props.filter(function (x: any) {
                    return !!x.setter && ((x.attr || []).filter(function (a: any) {
                        return Q.isInstanceOfType(a, Serenity.OptionAttribute);
                    }).length > 0 || (x.attr || []).filter(function (a: any) {
                        return Q.isInstanceOfType(a, System.ComponentModel.DisplayNameAttribute);
                    }).length > 0);
                });

                propByName = {};
                for (var k of propList) {
                    propByName[ReflectionUtils.makeCamelCase(k.name)] = k;
                }

                type.__propByName = propByName;
            }

            if (fieldByName == null) {
                var fields = Q.getMembers(type, Q.MemberType.field);
                var fieldList = fields.filter(function (x1: any) {
                    return (x1.attr || []).filter(function (a: any) {
                        return Q.isInstanceOfType(a, Serenity.OptionAttribute);
                    }).length > 0 || (x1.attr || []).filter(function (a: any) {
                        return Q.isInstanceOfType(a, System.ComponentModel.DisplayNameAttribute);
                    }).length > 0;
                });

                fieldByName = {};
                for (var $t2 = 0; $t2 < fieldList.length; $t2++) {
                    var k1 = fieldList[$t2];
                    fieldByName[ReflectionUtils.makeCamelCase(k1.name)] = k1;
                }
                type.__fieldByName = fieldByName;
            }

            var keys = Object.keys(options);
            for (var k2 of keys) {
                var v = options[k2];
                var cc = ReflectionUtils.makeCamelCase(k2);
                var p = propByName[cc] || propByName[k2];
                if (p != null) {
                    var func = (target[p.setter] as Function);
                    func && func.call(target, v);
                }
                else {
                    var f = fieldByName[cc] || fieldByName[k2];
                    f && (target[f.name] = v);
                }
            }
        }
    }

    export namespace ReflectionUtils {
        export function getPropertyValue(o: any, property: string): any {
            var d = o;
            var getter = d['get_' + property];
            if (!!!(typeof (getter) === 'undefined')) {
                return getter.apply(o);
            }
            var camelCase = makeCamelCase(property);
            getter = d['get_' + camelCase];
            if (!!!(typeof (getter) === 'undefined')) {
                return getter.apply(o);
            }
            return d[camelCase];
        }

        export function setPropertyValue(o: any, property: string, value: any): void {
            var d = o;
            var setter = d['set_' + property];
            if (!!!(typeof (setter) === 'undefined')) {
                setter.apply(o, [value]);
                return;
            }
            var camelCase = makeCamelCase(property);
            setter = d['set_' + camelCase];
            if (!!!(typeof (setter) === 'undefined')) {
                setter.apply(o, [value]);
                return;
            }
            d[camelCase] = value;
        }

        export function makeCamelCase(s: string): string {
            if (Q.isEmptyOrNull(s)) {
                return s;
            }

            if (s === 'ID') {
                return 'id';
            }

            var hasNonUppercase = false;
            var numUppercaseChars = 0;
            for (var index = 0; index < s.length; index++) {
                if (s.charCodeAt(index) >= 65 && s.charCodeAt(index) <= 90) {
                    numUppercaseChars++;
                }
                else {
                    hasNonUppercase = true;
                    break;
                }
            }

            if (!hasNonUppercase && s.length !== 1 || numUppercaseChars === 0) {
                return s;
            }
            else if (numUppercaseChars > 1) {
                return s.substr(0, numUppercaseChars - 1).toLowerCase() + s.substr(numUppercaseChars - 1);
            }
            else if (s.length === 1) {
                return s.toLowerCase();
            }
            else {
                return s.substr(0, 1).toLowerCase() + s.substr(1);
            }
        }
    }

    export interface ScriptContext {
    }

    @Decorators.registerClass('Serenity.ScriptContext')
    export class ScriptContext {
    }
}

declare namespace Serenity {

    class CheckListEditor extends Widget<CheckListEditorOptions> {
        constructor(div: JQuery, opt: CheckListEditorOptions);
        getItems(): CheckListItem[];
        updateItems(): void;
    }

    interface CheckListEditorOptions {
        items?: CheckListItem[];
        selectAllOptionText?: string;
    }

    class CheckListItem {
        id: string;
        text: string;
        parentId: string;
    }

    interface CheckTreeItem<TSource> {
        isSelected?: boolean;
        hideCheckBox?: boolean;
        isAllDescendantsSelected?: boolean;
        id?: string;
        text?: string;
        parentId?: string;
        children?: CheckTreeItem<TSource>[];
        source?: TSource;
    }

    interface HtmlContentEditorOptions {
    }

    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }

    interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }

    interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: Q.Dictionary<any>;
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }

    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }

    interface CKEditorConfig {
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

    interface DialogButton {
        text: string;
        click: () => void;
    }

    interface EditorTypeInfo {
        type?: Function;
        displayName?: string;
        optionsType?: Function;
    }
}
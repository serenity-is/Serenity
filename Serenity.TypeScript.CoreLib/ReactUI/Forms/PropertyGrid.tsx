namespace Serenity {
    export interface PropertyGridOptions {
        idPrefix?: string;
        items?: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
        renderCategories?: (tab: string, props: UI.CategoriesProps) => React.ReactNode;
        renderCategory?: (props: UI.CategoryProps) => React.ReactNode;
        renderField?: (props: PropertyItem) => React.ReactNode;
        setRef?: (name: string, editor: any) => void;
    }
}

namespace Serenity.UI {

    export class IntraPropertyGrid extends React.Component<PropertyGridOptions> {

        loadFrom(source: any, editors: UI.EditorRefs): void {
            var items = this.props.items || [];

            for (var item of items) {

                if (this.props.mode != PropertyGridMode.update &&
                    item.defaultValue != null &&
                    typeof (source[item.name]) === 'undefined') {
                    source[item.name] = item.defaultValue;
                }
            }

            editors.loadFrom(source, items.map(x => x.name));
        }

        canModifyItem(item: PropertyItem) {
            if (this.props.mode != PropertyGridMode.update) {
                if (item.insertable === false) {
                    return false;
                }

                if (item.insertPermission == null) {
                    return true;
                }

                return Q.Authorization.hasPermission(item.insertPermission);
            }

            if (item.updatable === false) {
                return false;
            }

            if (item.updatePermission == null) {
                return true;
            }

            return Q.Authorization.hasPermission(item.updatePermission);
        }

        saveTo(target: any, editors: EditorRefs): void {
            editors.saveTo(target);
        }

        getItems() {
            return (this.props.items || []).map(item => {
                var canModify = this.canModifyItem(item);
                var oneWay = !!item.oneWay || !canModify;
                var readOnly = item.readOnly === true || !canModify;
                var required = !readOnly && !!item.required && item.editorType !== 'Boolean';
                var visible = !((item.readPermission != null &&
                    !Q.Authorization.hasPermission(item.readPermission)) ||
                    item.visible === false ||
                    (this.props.mode != Serenity.PropertyGridMode.update && item.hideOnInsert === true) ||
                    (this.props.mode == Serenity.PropertyGridMode.update && item.hideOnUpdate === true));

                return Q.extend(Q.extend({}, item), {
                    readOnly: readOnly,
                    oneWay: oneWay,
                    required: required,
                    visible: visible
                });
            });
        }

        render(): React.ReactNode {

            var props: PropertyGridOptions = Q.extend({}, this.props);
            props.items = this.getItems();

            var useTabs = Q.any(props.items, function (x) {
                return !Q.isEmptyOrNull(x.tab);
            });

            if (useTabs)
                return React.createElement(UI.PropertyTabs, props);

            var useCategories = props.useCategories !== false && Q.any(props.items, function (x) {
                return !Q.isEmptyOrNull(x.category);
            });

            if (useCategories) {
                <React.Fragment>
                    {React.createElement(UI.CategoryLinks, props)}
                    {React.createElement(UI.Categories, props)}
                </React.Fragment>
            }

            return (
                <div className="categories">
                    {React.createElement(UI.Category, props)}
                </div>
            )
        }
    }

    export class PropertyGrid extends IntraPropertyGrid {

        render() {
            return (
                <div className="s-PropertyGrid">
                    {super.render()}
                </div>
            );
        }
    }
}
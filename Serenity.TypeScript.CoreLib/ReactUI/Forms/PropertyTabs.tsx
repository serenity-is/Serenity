namespace Serenity.UI {

    export interface PropertyTabProps {
        idPrefix?: string;
        items?: Serenity.PropertyItem[];
        localTextPrefix?: string;
        categoryOrder?: string;
        defaultCategory?: string;
        renderCategories?: (tab: string, props: CategoriesProps) => React.ReactNode;
        renderCategory?: (props: CategoryProps) => React.ReactNode;
        renderField?: (props: PropertyItem) => React.ReactNode;
        setRef?: (name: string, editor: any) => void;
    }

    export class PropertyTabs extends React.Component<PropertyTabProps> {

        protected text = Q.prefixedText(this.props.localTextPrefix);

        static groupByTab(items: PropertyItem[]): Q.Groups<PropertyItem> {
            return Q.groupBy(items || [], x => Q.coalesce(x.tab, ''));
        }

        renderTab(group: Q.Group<PropertyItem>) {
            return (
                <li className={group.order == 0 ? "active" : null} key={group.order}>
                    <a data-toggle='tab' role='tab' href={"#" + this.props.idPrefix + "_Tab" + group.order}>
                        {this.text(group.key, "Tabs." + group.key)}
                    </a>
                </li>
            );
        }

        renderPane(group: Q.Group<PropertyItem>) {
            return (
                <div id={this.props.idPrefix + "_Tab" + group.order} key={group.order}
                    className={"tab-pane fade" + (group.order == 0 ? " in active" : "")}
                    role="tabpanel">
                    {this.renderCategories(group)}
                </div>
            );
        }

        renderCategories(group: Q.Group<PropertyItem>) {

            var props: CategoriesProps = {
                items: group.items,
                idPrefix: this.props.idPrefix,
                localTextPrefix: this.props.localTextPrefix,
                categoryOrder: this.props.categoryOrder,
                defaultCategory: this.props.defaultCategory,
                renderCategory: this.props.renderCategory,
                renderField: this.props.renderField,
                setRef: this.props.setRef
            }

            if (this.props.renderCategories) {
                var content = this.props.renderCategories(group.key, props);
                if (content !== undefined)
                    return content;
            }

            return <Categories {...props} />
        }

        render() {
            var tabs = PropertyTabs.groupByTab(this.props.items || []);

            return (
                <React.Fragment>
                    <ul className="nav nav-tabs property-tabs" role="tablist">
                        {tabs.inOrder.map((g) => this.renderTab(g))}
                    </ul>
                    <div className="tab-content property-panes">
                        {tabs.inOrder.map((g) => this.renderPane(g))}
                    </div>
                </React.Fragment>
            );
        }
    }
}
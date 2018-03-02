namespace Serenity.UI {

    export interface CategoryProps {
        categoryId?: string;
        category?: string;
        collapsed?: boolean;
        idPrefix?: string;
        localTextPrefix?: string;
        items?: PropertyItem[];
        renderField?: (props: PropertyItem) => React.ReactNode;
        setRef?: (name: string, editor: any) => void;
    }

    export class Category extends React.Component<CategoryProps, Partial<CategoryProps>> {

        protected text = Q.prefixedText(this.props.localTextPrefix);

        constructor(props: CategoryProps, context?: any) {
            super(props, context);

            this.state = {
                collapsed: this.props.collapsed
            };
        }

        componentWillReceiveProps(nextProps: CategoryProps) {
            if (nextProps.collapsed !== this.props.collapsed) {
                this.setState({
                    collapsed: nextProps.collapsed
                });
            }
        }

        getClassName() {
            if (this.state.collapsed == null)
                return "category ";

            if (this.state.collapsed == true)
                return "category collapsible collapsed";

            return "category collapsible";
        }

        getCategoryId() {
            if (!this.props.categoryId)
                return null;

            return Q.coalesce(this.props.idPrefix, '') + this.props.categoryId;
        }

        handleTitleClick() {
            if (this.state.collapsed == null)
                return;

            this.setState({
                collapsed: !this.state.collapsed
            });
        }

        renderTitle() {
            if (this.props.category == null)
                return null;

            return (
                <CategoryTitle categoryId={this.getCategoryId()}
                    collapsed={this.state.collapsed}
                    onClick={() => this.handleTitleClick()}>
                    {this.text(this.props.category, "Categories." + this.props.category)}
                </CategoryTitle>
            );
        }

        renderField(item: PropertyItem) {
            var props = Q.extend({
                idPrefix: this.props.idPrefix,
                localTextPrefix: this.props.localTextPrefix,
                key: item.name,
                setRef: this.props.setRef
            }, item);

            if (this.props.renderField != null) {
                var content = this.props.renderField(props);
                if (content !== undefined)
                    return content;
            }

            return React.createElement(PropertyField, props);
        }

        renderWithBreak(item: PropertyItem) {
            return [<CategoryLineBreak breakClass={item.formCssClass} key={"break-" + item.name} />, this.renderField(item)];
        }

        render() {
            var props = this.props;
            return (
                <div className={this.getClassName()} >
                    {this.renderTitle()}
                    {props.items && props.items.map(item => {
                        if (item.formCssClass && item.formCssClass.indexOf('line-break-') >= 0)
                            return this.renderWithBreak(item);

                        return this.renderField(item);
                    })}
                    {props.children}
                </div>
            );
        }
    }
}
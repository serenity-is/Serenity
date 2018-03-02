namespace Serenity.UI {

    export interface CategoryTitleProps {
        categoryId?: string;
        collapsed?: boolean;
        onClick?: React.EventHandler<any>;
    }

    export class CategoryTitle extends React.Component<CategoryTitleProps> {

        static collapsedIcon = <i className="fa fa-plus"></i>;
        static expandedIcon = <i className="fa fa-minus"></i>;

        render() {
            return (
                <div className="category-title" onClick={this.props.onClick}>
                    <a className="category-anchor" id={this.props.categoryId}>
                        {this.props.children}
                    </a>
                    {this.props.collapsed != null && (this.props.collapsed ? CategoryTitle.collapsedIcon : CategoryTitle.expandedIcon)}
                </div>
            )
        }
    }
}
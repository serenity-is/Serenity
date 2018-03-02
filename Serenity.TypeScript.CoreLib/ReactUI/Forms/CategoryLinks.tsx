namespace Serenity.UI {

    export interface CategoryLinksProps {
        idPrefix?: string;
        items?: Serenity.PropertyItem[];
        defaultCategory?: string;
        categoryOrder?: string;
        localTextPrefix?: string;
    }

    export class CategoryLinks extends React.Component<CategoryLinksProps> {

        protected text = Q.prefixedText(this.props.localTextPrefix);

        renderSeparator(key: any) {
            return <span className="separator" key={key}>|</span>;
        }

        render() {
            var groups = Categories.groupByCategory(this.props.items);

            return (
                <div className="category-links">
                    {groups.inOrder.map((g, idx) => [
                        (idx > 0 && this.renderSeparator("sep-" + idx)),
                        <CategoryLink categoryId={"Category" + g.order} key={idx}>
                            {this.text(g.key, "Categories." + g.key)}
                        </CategoryLink>
                    ])}
                </div>
            )
        }
    }
}
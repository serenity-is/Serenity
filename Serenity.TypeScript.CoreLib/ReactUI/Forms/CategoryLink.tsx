namespace Serenity.UI {

    export interface CategoryLinkProps {
        categoryId?: string;
        onClick?: React.EventHandler<any>
    }

    export class CategoryLink extends React.Component<CategoryLinkProps> {

        handleClick(e: React.MouseEvent<any>) {
            e.preventDefault();

            this.props.onClick && this.props.onClick(e);

            var title = $('a[id=' + this.props.categoryId + ']');

            var category = title.closest('.category');
            if (category.hasClass('collapsed'))
                category.children('.category-title').click();

            if (!title && !title.fadeTo)
                return;

            var animate = function () {
                title.fadeTo(100, 0.5, function () {
                    title.fadeTo(100, 1, function () {
                    });
                });
            };

            if (category.closest(':scrollable(both)').length === 0)
                animate();
            else {
                var siv = (category as any).scrollintoview;
                siv && siv.scrollintoview({
                    duration: 'fast',
                    direction: 'y',
                    complete: animate
                });
            }
        }

        getLink() {
            if (Q.isEmptyOrNull(this.props.categoryId))
                return null;

            return "#" + this.props.categoryId;
        }

        render() {
            return (
                <a className="category-link" tabIndex={-1}
                    onClick={e => this.handleClick(e)} href={this.getLink()}>
                    {this.props.children}
                </a>
            )
        }
    }
}
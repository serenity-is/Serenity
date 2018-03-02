namespace Serenity.UI {
    export interface CategoryLineBreakProps {
        breakClass: string;
    }

    export class CategoryLineBreak extends React.Component<CategoryLineBreakProps> {

        getBreakClass() {
            var breakClass = "line-break";

            var splitted = this.props.breakClass.split(' ');
            if (splitted.indexOf('line-break-xs') >= 0) {
            }
            else if (splitted.indexOf('line-break-sm') >= 0) {
                breakClass += " hidden-xs";
            }
            else if (splitted.indexOf('line-break-md') >= 0) {
                breakClass += " hidden-sm";
            }
            else if (splitted.indexOf('line-break-lg') >= 0) {
                breakClass += " hidden-md";
            }

            return breakClass;
        }

        render() {
            return (<div className={this.getBreakClass()} style={{ width: "100%" }}></div>);
        }
    }
}
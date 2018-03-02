namespace Serenity.UI {
    export interface LabelProps {
        hint?: string;
        htmlFor?: string;
        width?: string | number;
        required?: boolean;
    }

    export class Label extends React.Component<LabelProps> {
        render() {
            var props = this.props;
            var width = typeof props.width == "string" || props.width == null ? null :
                (typeof (props.width) == "number" ? props.width + "px" :
                    props.width);

            var style = width == null ? null : (width == "0" ? { display: "none" } : { width: width });

            return (
                <label className="caption"
                    htmlFor={props.htmlFor}
                    title={props.hint}
                    style={style}>
                    {props.required && <sup title={Q.text('Controls.PropertyGrid.RequiredHint')}>*</sup>}
                    {props.children}
                </label>
            );
        }
    }
}
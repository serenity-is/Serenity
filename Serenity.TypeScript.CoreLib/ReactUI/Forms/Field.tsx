namespace Serenity.UI {
    export type EditorRenderProps = {
        name?: string;
        id?: string;
        required?: boolean;
    }

    export interface FieldProps {
        name?: string;
        id?: string;
        className?: string;
        caption?: string | false;
        labelWidth?: number | string;
        label?: ((p: LabelProps) => JSX.Element);
        htmlFor?: string;
        editor?: ((p: EditorRenderProps) => JSX.Element);
        setRef?: (name: string, editor: any) => void;
        hint?: string;
        required?: boolean;
        vx?: boolean;
    }

    export interface EditorProps {
        className?: string;
        name?: string;
        id?: string;
        editor?: ((p: EditorRenderProps) => JSX.Element);
        required?: boolean;
        ref?: (name: string, editor: any) => void;
    }

    export class Field extends React.Component<FieldProps> {

        private editorRef: any;

        componentWillReceiveProps(nextProps: FieldProps, nextContext?: any): void {
            if (nextProps != null && nextProps.setRef !== this.props.setRef) {
                this.editorRef = null;
            }
        }

        render() {
            var props = this.props;

            var lblElement: JSX.Element;
            if (props.label != null)
                lblElement = props.label(this.props);
            else if (props.caption !== false) {
                lblElement = (
                    <Label
                        htmlFor={props.htmlFor === undefined ? props.id : props.htmlFor}
                        hint={props.hint}
                        width={props.labelWidth}
                        required={props.required}>
                        {props.caption}
                    </Label>
                );
            }

            var className = "field";
            if (props.name) {
                className += " " + props.name;
            }

            if (props.className) {
                className += " " + props.className;
            }

            var name = props.name;
            if (this.props.setRef != null &&
                this.editorRef == null) {
                var setRef = this.props.setRef;
                this.editorRef = function (x: any) {
                    setRef(name, x);
                };
            }

            var editorProps: EditorProps = {
                className: "editor",
                name: name,
                id: this.props.id,
                required: this.props.required,
                ref: this.editorRef
            }

            return (
                <div className={className}>
                    {lblElement}
                    {props.editor != null && props.editor(editorProps)}
                    {props.children}
                    {props.vx !== false && <ValidationMark />}
                    <div className="clear"></div>
                </div>
            );
        }
    }
}
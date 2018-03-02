namespace Serenity.UI {
    export interface PropertyFieldProps extends Serenity.PropertyItem {
        idPrefix?: string
        localTextPrefix?: string;
        setRef?: (name: string, editor: any) => void;
    }

    export class PropertyField extends React.Component<PropertyFieldProps> {

        protected text = Q.prefixedText(this.props.localTextPrefix);

        getCaption(): string {
            return this.text(this.props.title, this.props.name);
        }

        getHint(): string {
            var hint = this.text(this.props.hint, this.props.name + '_Hint');

            if (hint == null)
                return this.getCaption();

            return hint || null;
        }

        getPlaceHolder() {
            return this.text(this.props.placeholder, this.props.name + '_Placeholder');
        }

        getClassName() {
            var className = this.props.cssClass || "";

            if (!Q.isEmptyOrNull(this.props.formCssClass)) {
                className += (className ? " " : "") + this.props.formCssClass;
            }

            return className;
        }

        getHtmlFor(editorType: any) {
            var htmlFor;
            if ((editorType === RadioButtonEditor || editorType === BooleanEditor) &&
                (this.props.editorParams == null || !!!this.props.editorParams['labelFor'])) {
                htmlFor = null;
            }
            return htmlFor;
        }

        getEditorType() {
            if (this.props.editorType == null)
                return Serenity.StringEditor;

            if (typeof this.props.editorType == "string")
                return Serenity.EditorTypeRegistry.get(this.props.editorType);

            return this.props.editorType;
        }

        getEditorId() {
            return (this.props.idPrefix || "") + (this.props.name || "");
        }

        getMaxLength() {
            return this.props.maxLength > 0 ? this.props.maxLength : null;
        }

        render() {
            var EditorType = this.getEditorType();

            return (
                <Field
                    className={this.getClassName()}
                    caption={this.getCaption()}
                    id={this.getEditorId()}
                    name={this.props.name}
                    labelWidth={this.props.labelWidth}
                    htmlFor={this.getHtmlFor(EditorType)}
                    hint={this.getHint()}
                    required={this.props.required}
                    setRef={this.props.setRef}
                    editor={ed =>
                        <EditorType {...ed}
                            maxlength={this.getMaxLength()}
                            {...this.props.editorParams}
                            setOptions={this.props.editorParams} />
                    }
                >
                    {this.props.children}
                </Field>
            );
        }
    }
}
namespace Serenity.UI {
    export type ValidateFormProps = {
        options?: JQueryValidation.ValidationOptions;
    } & React.HTMLAttributes<HTMLFormElement>;

    export class ValidateForm extends React.Component<ValidateFormProps> {

        private validator: JQueryValidation.Validator;
        private form: HTMLFormElement;

        render() {
            var { options, ...other } = this.props;
            return (
                <form {...other} ref={el => { this.form = el as any }}
                    onSubmit={this.props.onSubmit || ((e) => this.handleSubmit(e))}>
                    {this.props.children}
                </form>
            );
        }

        handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        }

        componentDidMount() {
            this.validator = $(this.form).validate(Q.validateOptions(this.props.options));
        }

        validateForm(): boolean {
            return this.validator == null || !!this.validator.form();
        }

        serialize(): any {
            var result: any = {};
            $(this.form).find(':input, .editor').each((i, e) => {
                var name = $(e).attr('name');
                if (!name)
                    return;

                var widget = $(e).tryGetWidget(Widget);
                if (widget)
                    result[name] = EditorUtils.getValue(widget);
                else
                    result[name] = $(e).val();
            });
            return result;
        }
    }
}
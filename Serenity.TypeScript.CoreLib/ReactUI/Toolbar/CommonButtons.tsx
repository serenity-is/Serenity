namespace Serenity.UI {
    export interface SaveButtonProps extends ToolButtonProps {
        isUpdate?: boolean;
    }

    export class SaveButton extends React.Component<SaveButtonProps> {

        render() {
            var title = Q.text("Controls.EntityDialog." + (this.props.isUpdate ? "UpdateButton" : "SaveButton"));

            return (
                <ToolButton icon="fa-save text-light-blue" title={title} {...this.props}>
                </ToolButton>
            );
        }

    }

    export class ApplyChangesButton extends React.Component<ToolButtonProps> {

        render() {
            var hint = Q.text("Controls.EntityDialog.ApplyChangesButton");

            return (
                <ToolButton icon="fa-save" cssClass="text-black" hint={hint} {...this.props}>
                </ToolButton>
            );
        }

    }

    export class DeleteButton extends React.Component<ToolButtonProps> {

        render() {
            var title = Q.text("Controls.EntityDialog.DeleteButton");

            return (
                <ToolButton icon="fa-times text-red" title={title} {...this.props}>
                </ToolButton>
            );
        }
    }
}
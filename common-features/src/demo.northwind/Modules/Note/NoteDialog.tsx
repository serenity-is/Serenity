import { Decorators, DialogButton, BaseDialog, HtmlContentEditor, HtmlNoteContentEditor, cancelDialogButton, okDialogButton } from "@serenity-is/corelib";

@Decorators.registerClass('Serenity.Demo.Northwind.NoteDialog')
export class NoteDialog<P = {}> extends BaseDialog<P> {

    declare private textEditor: HtmlContentEditor;

    protected renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <form id={id.Form} class="s-Form">
                <textarea id={id.Text} class="required" ref={el => this.textEditor = new HtmlNoteContentEditor({ element: el })} />
            </form>
        );
    }

    protected getDialogButtons(): DialogButton[] {
        return [
            okDialogButton({
                click: (e: Event) => {
                    if (!this.validateForm()) {
                        e.preventDefault();
                        return;
                    }
                    this.okClick && this.okClick();
                }
            }),
            cancelDialogButton()
        ];
    }

    get text(): string {
        return this.textEditor.value;
    }

    set text(value: string) {
        this.textEditor.value = value;
    }

    declare public okClick: () => void;
}

import { Decorators, DialogTexts, BaseDialog, WidgetProps, localText, Dialog } from "@serenity-is/corelib";

@Decorators.registerClass("Serenity.Extensions.BasicProgressDialog")
export class BasicProgressDialog<P = {}> extends BaseDialog<P> {

    constructor(props?: WidgetProps<P>) {
        super(props);

        this.dialogTitle = localText('Site.BasicProgressDialog.PleaseWait');
    }

    declare public cancelled: boolean;

    public get max(): number {
        return parseInt(this.byId('ProgressBar').attr('aria-valuemax'), 10);
    }

    public set max(value: number) {
        this.byId('ProgressBar').attr('aria-valuemax', (value || 100).toString());
    }

    public get value(): number {
        return parseInt(this.byId('ProgressBar').attr('aria-valuenow'), 10);
    }

    public set value(value: number) {
        this.byId('ProgressBar').attr('aria-valuenow', (value || 0).toString())
            .text(value + ' / ' + this.max)
            .getNode().style.width = (((value || 0) / (this.max || 100)) * 100) + '%';
    }

    public get title(): string {
        return this.dialogTitle;
    }

    public set title(value: string) {
        this.dialogTitle = value;
    }

    declare public cancelTitle: string;

    getDialogButtons() {
        return [{
            text: DialogTexts.CancelButton,
            class: 'btn btn-danger',
            click: () => {
                this.cancelled = true;
                Dialog.getInstance(this.domNode)?.getFooterNode()?.querySelectorAll('button')?.forEach((el: HTMLElement) => {
                    el.setAttribute('disabled', 'disabled');
                    el.style.opacity = '0.5';
                });

                this.dialogTitle = this.cancelTitle?.trim() || localText('Site.BasicProgressDialog.CancelTitle');
            }
        }];
    }

    getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.width = 600;
        return opt;
    }

    initDialog() {
        super.initDialog();
        var close = this.domNode.closest('.ui-dialog')?.querySelector('.ui-dialog-titlebar-close') as HTMLElement;
        close && (close.style.display = 'none');
    }

    renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-DialogContent s-BasicProgressDialogContent">
                <div id={id.StatusText} class="status-text" ></div>
                <div id={id.Progress} class="progress" style="height: 1.5rem">
                    <div id={id.ProgressBar} class="progress-bar" aria-valuenow={0} aria-valuemin={0} aria-valuemax={100}></div>
                </div>
            </div>
        );
    }
}
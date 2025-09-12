import { BaseDialog, Dialog, DialogTexts, WidgetProps, localText } from "@serenity-is/corelib";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class BasicProgressDialog<P = {}> extends BaseDialog<P> {
    static override typeInfo = this.registerClass(nsExtensions);

    declare private progressBar: HTMLElement;

    constructor(props?: WidgetProps<P>) {
        super(props);

        this.dialogTitle = localText('Site.BasicProgressDialog.PleaseWait');
    }

    declare public cancelled: boolean;

    public get max(): number {
        return parseInt(this.progressBar.ariaValueMax, 10);
    }

    public set max(value: number) {
        this.progressBar.ariaValueMax = (value || 100).toString();
    }

    public get value(): number {
        return parseInt(this.progressBar.ariaValueNow, 10);
    }

    public set value(value: number) {
        this.progressBar.ariaValueNow = (value || 0).toString();
        this.progressBar.textContent = value + ' / ' + this.max;
        this.progressBar.style.width = (((value || 0) / (this.max || 100)) * 100) + '%';
    }

    public get title(): string {
        return this.dialogTitle;
    }

    public set title(value: string) {
        this.dialogTitle = value;
    }

    declare public cancelTitle: string;

    protected override getDialogButtons() {
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

    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.width = 600;
        return opt;
    }

    protected override initDialog() {
        super.initDialog();
        var close = this.domNode.closest('.ui-dialog')?.querySelector('.ui-dialog-titlebar-close') as HTMLElement;
        close && (close.style.display = 'none');
    }

    protected override renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-DialogContent s-BasicProgressDialogContent">
                <div id={id.StatusText} class="status-text" ></div>
                <div id={id.Progress} class="progress" style="height: 1.5rem">
                    <div id={id.ProgressBar} class="progress-bar" aria-valuenow={0} aria-valuemin={0} aria-valuemax={100} ref={el => this.progressBar = el}></div>
                </div>
            </div>
        );
    }
}
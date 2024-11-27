import { Dictionary, ServiceError, confirmDialog, localText, notifyError, notifySuccess, notifyWarning, stringFormat } from "@serenity-is/corelib";
import { BasicProgressDialog } from "./BasicProgressDialog";

export class BulkServiceAction {

    declare protected keys: string[];
    declare protected queue: string[];
    declare protected queueIndex: number;
    declare protected progressDialog: BasicProgressDialog;
    declare protected pendingRequests: number;
    declare protected completedRequests: number;
    declare protected errorByKey: Dictionary<ServiceError>;
    declare private successCount: number;
    declare private errorCount: number;
    declare public done: () => void;

    protected createProgressDialog() {
        this.progressDialog = new BasicProgressDialog({});
        this.progressDialog.dialogOpen();
        this.progressDialog.max = this.keys.length;
        this.progressDialog.value = 0;
    }

    protected getConfirmationFormat() {
        return localText('Site.BulkServiceAction.ConfirmationFormat');
    }

    protected getConfirmationMessage(targetCount) {
        return stringFormat(this.getConfirmationFormat(), targetCount);
    }

    protected confirm(targetCount, action) {
        confirmDialog(this.getConfirmationMessage(targetCount), action);
    }

    protected getNothingToProcessMessage() {
        return localText('Site.BulkServiceAction.NothingToProcess');
    }

    protected nothingToProcess() {
        notifyError(this.getNothingToProcessMessage());
    }

    protected getParallelRequests() {
        return 1;
    }

    protected getBatchSize() {
        return 1;
    }

    protected startParallelExecution() {
        this.createProgressDialog();
        this.successCount = 0;
        this.errorCount = 0;
        this.pendingRequests = 0;
        this.completedRequests = 0;
        this.errorCount = 0;
        this.errorByKey = {};
        this.queue = this.keys.slice();
        this.queueIndex = 0;
        var parallelRequests = this.getParallelRequests();
        while (parallelRequests-- > 0) {
            this.executeNextBatch();
        }
    }

    protected serviceCallCleanup() {
        this.pendingRequests--;
        this.completedRequests++;

        var title = localText((this.progressDialog.cancelled ?
            'Site.BasicProgressDialog.CancelTitle' : 'Site.BasicProgressDialog.PleaseWait'));

        title += ' (';
        if (this.successCount > 0) {
            title += stringFormat(localText('Site.BulkServiceAction.SuccessCount'), this.successCount);
        }

        if (this.errorCount > 0) {
            if (this.successCount > 0) {
                title += ', ';
            }

            title += stringFormat(localText('Site.BulkServiceAction.ErrorCount'), this.errorCount);
        }

        this.progressDialog.title = title + ')';
        this.progressDialog.value = this.successCount + this.errorCount;
        if (!this.progressDialog.cancelled && this.progressDialog.value < this.keys.length) {
            this.executeNextBatch();
        }

        else if (this.pendingRequests === 0) {
            this.progressDialog.dialogClose("done");
            this.showResults();
            if (this.done) {
                this.done();
                this.done = null;
            }
        }
    }

    protected executeForBatch(batch: string[]) {
    }

    protected executeNextBatch() {
        var batchSize = this.getBatchSize();
        var batch = [];
        while (true) {
            if (batch.length >= batchSize) {
                break;
            }

            if (this.queueIndex >= this.queue.length) {
                break;
            }

            batch.push(this.queue[this.queueIndex++]);
        }

        if (batch.length > 0) {
            this.pendingRequests++;
            this.executeForBatch(batch);
        }
    }

    protected getAllHadErrorsFormat() {
        return localText('Site.BulkServiceAction.AllHadErrorsFormat');
    }

    protected showAllHadErrors() {
        notifyError(stringFormat(this.getAllHadErrorsFormat(), this.errorCount));
    }

    protected getSomeHadErrorsFormat() {
        return localText('Site.BulkServiceAction.SomeHadErrorsFormat');
    }

    protected showSomeHadErrors() {
        notifyWarning(stringFormat(this.getSomeHadErrorsFormat(), this.successCount, this.errorCount));
    }

    protected getAllSuccessFormat() {
        return localText('Site.BulkServiceAction.AllSuccessFormat');
    }

    protected showAllSuccess() {
        notifySuccess(stringFormat(this.getAllSuccessFormat(), this.successCount));
    }

    protected showResults() {
        if (this.errorCount === 0 && this.successCount === 0) {
            this.nothingToProcess();
            return;
        }

        if (this.errorCount > 0 && this.successCount === 0) {
            this.showAllHadErrors();
            return;
        }

        if (this.errorCount > 0) {
            this.showSomeHadErrors();
            return;
        }

        this.showAllSuccess();
    }

    public execute(keys: string[]) {
        this.keys = keys;
        if (this.keys.length === 0) {
            this.nothingToProcess();
            return;
        }
        this.confirm(this.keys.length, () => this.startParallelExecution());
    }

    get_successCount() {
        return this.successCount;
    }

    set_successCount(value: number) {
        this.successCount = value;
    }

    get_errorCount() {
        return this.errorCount;
    }

    set_errorCount(value: number) {
        this.errorCount = value;
    }
}
import * as corelib from "@serenity-is/corelib";
import { BulkServiceAction } from "../../Modules/BulkActions/BulkServiceAction";
import { BasicProgressDialogTexts, BulkServiceActionTexts } from "../../Modules/ServerTypes/Texts";

class TestProgressDialog {
    dialogOpen = vi.fn();
    dialogClose = vi.fn();
    cancelled = false;
    max = 0;
    value = 0;
    title = "";
}

class TestAction extends BulkServiceAction {
    batches: string[][] = [];
    confirmCount: number;
    override progressDialog: any = new TestProgressDialog();
    executed = 0;

    protected override confirm(count: number, action: () => void): void {
        this.confirmCount = count;
        action();
    }

    protected override createProgressDialog(): void {
        this.progressDialog = new TestProgressDialog();
        this.progressDialog.dialogOpen();
    }

    protected override getParallelRequests(): number {
        return (this as any).parallel ?? 1;
    }

    protected override getBatchSize(): number {
        return (this as any).batchSize ?? 1;
    }

    protected override executeForBatch(batch: string[]): void {
        this.batches.push(batch);
        this.executed++;
    }
}

describe("BulkServiceAction texts and confirmation", () => {
    afterEach(() => vi.restoreAllMocks());

    it("returns format and message values", () => {
        const action = new TestAction();
        expect(action["getConfirmationFormat"]()).toBe(BulkServiceActionTexts.ConfirmationFormat);
        expect(action["getConfirmationMessage"](3)).toBe(BulkServiceActionTexts.ConfirmationFormat);
        expect(action["getNothingToProcessMessage"]()).toBe(BulkServiceActionTexts.NothingToProcess);
        expect(action["getAllHadErrorsFormat"]()).toBe(BulkServiceActionTexts.AllHadErrorsFormat);
        expect(action["getSomeHadErrorsFormat"]()).toBe(BulkServiceActionTexts.SomeHadErrorsFormat);
        expect(action["getAllSuccessFormat"]()).toBe(BulkServiceActionTexts.AllSuccessFormat);
    });

    it("has sensible defaults", () => {
        const action = new TestAction();
        expect(action["getParallelRequests"]()).toBe(1);
        expect(action["getBatchSize"]()).toBe(1);
    });

    it("uses base defaults and calls confirmDialog", () => {
        const confirmSpy = vi.spyOn(corelib, "confirmDialog").mockReturnValue({} as any);
        const action = new BulkServiceAction();
        expect(action["getParallelRequests"]()).toBe(1);
        expect(action["getBatchSize"]()).toBe(1);
        const callback = vi.fn();
        action["confirm"](2, callback);
        expect(confirmSpy).toHaveBeenCalledWith(action["getConfirmationMessage"](2), callback);
        expect(action["executeForBatch"]([])).toBeUndefined();
    });

    it("notifies when nothing to process", () => {
        const notifySpy = vi.spyOn(corelib, "notifyError");
        const action = new TestAction();
        action["nothingToProcess"]();
        expect(notifySpy).toHaveBeenCalledWith(BulkServiceActionTexts.NothingToProcess);
    });

    it("does not start when execute is called with no keys", () => {
        const notifySpy = vi.spyOn(corelib, "notifyError");
        const action = new TestAction();
        action.execute([]);
        expect(notifySpy).toHaveBeenCalledTimes(1);
        expect(action.confirmCount).toBeUndefined();
    });

    it("confirms and starts execution for keys", () => {
        const action = new TestAction();
        action.execute(["a", "b"]);
        expect(action.confirmCount).toBe(2);
        expect(action.batches).toEqual([["a"]]);
    });
});

describe("BulkServiceAction execution", () => {
    afterEach(() => vi.restoreAllMocks());

    it("creates a real progress dialog and resets counters", () => {
        const action = new TestAction();
        (action as any).keys = ["a"];
        (action as any).createProgressDialog = BulkServiceAction.prototype["createProgressDialog"];
        action["startParallelExecution"]();
        expect((action as any).progressDialog).toBeTruthy();
        expect((action as any).successCount).toBe(0);
        expect((action as any).errorCount).toBe(0);
    });

    it("executes batches respecting batch size and parallel requests", () => {
        const action = new TestAction();
        (action as any).parallel = 2;
        (action as any).batchSize = 2;
        action.execute(["a", "b", "c", "d", "e"]);
        expect(action.batches).toEqual([["a", "b"], ["c", "d"]]);
        expect((action as any).queueIndex).toBe(4);
    });

    it("updates progress and continues while batches remain", () => {
        const action = new TestAction();
        action.execute(["a", "b"]);
        const progress = action.progressDialog as TestProgressDialog;
        (action as any).successCount = 1;
        (action as any).executeNextBatch = vi.fn();
        action["serviceCallCleanup"]();
        expect(progress.title).toContain(BulkServiceActionTexts.SuccessCount.replace("{0}", "1"));
        expect(progress.value).toBe(1);
        expect((action as any).executeNextBatch).toHaveBeenCalled();
    });

    it("shows combined title when both success and errors exist", () => {
        const action = new TestAction();
        action.execute(["a", "b", "c"]);
        const progress = action.progressDialog as TestProgressDialog;
        (action as any).successCount = 1;
        (action as any).errorCount = 2;
        (action as any).pendingRequests = 1;
        (action as any).executeNextBatch = vi.fn();
        const showResults = vi.spyOn(action as any, "showResults");
        action["serviceCallCleanup"]();
        expect(progress.title).toContain(BulkServiceActionTexts.ErrorCount.replace("{0}", "2"));
        expect(showResults).toHaveBeenCalled();
    });

    it("closes the dialog and calls done when finished", () => {
        const action = new TestAction();
        action.execute(["a"]);
        const progress = action.progressDialog as TestProgressDialog;
        (action as any).successCount = 1;
        (action as any).pendingRequests = 1;
        const done = vi.fn();
        action.done = done;
        const showResults = vi.spyOn(action as any, "showResults").mockImplementation(() => { });
        action["serviceCallCleanup"]();
        expect(progress.dialogClose).toHaveBeenCalledWith("done");
        expect(showResults).toHaveBeenCalled();
        expect(done).toHaveBeenCalledTimes(1);
        expect(action.done).toBeNull();
    });

    it("uses cancel title when progress dialog is cancelled", () => {
        const action = new TestAction();
        action.execute(["a", "b"]);
        const progress = action.progressDialog as TestProgressDialog;
        progress.cancelled = true;
        (action as any).pendingRequests = 0;
        vi.spyOn(action as any, "showResults").mockImplementation(() => { });
        action["serviceCallCleanup"]();
        expect(progress.title).toContain(BasicProgressDialogTexts.CancelTitle);
    });

    it("stops executing when queue is exhausted", () => {
        const action = new TestAction();
        (action as any).queue = [];
        (action as any).queueIndex = 0;
        (action as any).pendingRequests = 0;
        action["executeNextBatch"]();
        expect(action.batches).toEqual([]);
    });
});

describe("BulkServiceAction results", () => {
    afterEach(() => vi.restoreAllMocks());

    it("notifies nothing to process", () => {
        const errorSpy = vi.spyOn(corelib, "notifyError");
        const action = new TestAction();
        (action as any).successCount = 0;
        (action as any).errorCount = 0;
        action["showResults"]();
        expect(errorSpy).toHaveBeenCalled();
    });

    it("notifies all had errors", () => {
        const errorSpy = vi.spyOn(corelib, "notifyError");
        const action = new TestAction();
        (action as any).successCount = 0;
        (action as any).errorCount = 2;
        action["showResults"]();
        expect(errorSpy).toHaveBeenCalled();
    });

    it("notifies some had errors", () => {
        const warnSpy = vi.spyOn(corelib, "notifyWarning");
        const action = new TestAction();
        (action as any).successCount = 1;
        (action as any).errorCount = 1;
        action["showResults"]();
        expect(warnSpy).toHaveBeenCalled();
    });

    it("notifies all success", () => {
        const successSpy = vi.spyOn(corelib, "notifySuccess");
        const action = new TestAction();
        (action as any).successCount = 2;
        (action as any).errorCount = 0;
        action["showResults"]();
        expect(successSpy).toHaveBeenCalled();
    });

    it("supports successCount accessors", () => {
        const action = new TestAction();
        action.set_successCount(5);
        expect(action.get_successCount()).toBe(5);
    });

    it("supports errorCount accessors", () => {
        const action = new TestAction();
        action.set_errorCount(4);
        expect(action.get_errorCount()).toBe(4);
    });
});

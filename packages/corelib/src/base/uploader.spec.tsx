import * as dialogs from "./dialogs";
import * as notify from "./notify";
import { Uploader, UploaderBatch } from "./uploader";

jest.mock("./dialogs", () => ({
    __esModule: true,
    iframeDialog: jest.fn()
}));


jest.mock("./notify", () => ({
    __esModule: true,
    notifyError: jest.fn()
}));

afterEach(() => {
    jest.resetAllMocks();
});

describe("Uploader multiple handling", () => {
    it("reads multiple from the input if not passed via options", () => {
        const input = <input multiple={true} /> as HTMLInputElement;
        const uploader = new Uploader({
            input
        });
        expect(uploader.isMultiple()).toBe(true);
    });

    it("does not read multiple from the input if passed via options", () => {
        const input = <input multiple={true} /> as HTMLInputElement;
        const uploader = new Uploader({
            input,
            multiple: false
        });
        expect(uploader.isMultiple()).toBe(false);
    });

    it("defaults to single file upload if multiple is not specified", () => {
        const input = <input /> as HTMLInputElement;
        const uploader = new Uploader({
            input
        });
        expect(uploader.isMultiple()).toBe(false);
    });

    it("reads multiple from options if specified", () => {
        const input = <input /> as HTMLInputElement;
        const uploader = new Uploader({
            input,
            multiple: true
        });
        expect(uploader.isMultiple()).toBe(true);
    });
});

describe("Uploader.errorHandler method", () => {
    it("ignores if data is null", () => {
        Uploader.errorHandler(null);
    });

    it("logs data.exception if it is an object", () => {
        const consoleError = console.error;
        console.error = jest.fn();
        try {
            Uploader.errorHandler({ exception: "test" });
            expect(console.error).toHaveBeenCalledTimes(1);
            expect(console.error).toHaveBeenCalledWith("test");
            expect(notify.notifyError).toHaveBeenCalledTimes(1);
            expect(notify.notifyError).toHaveBeenCalledWith("test");
        }
        finally {
            console.error = consoleError;
        }
    });

    it("notifies with data.response.Error.Message if it is a string", () => {
        Uploader.errorHandler({ response: { Error: { Message: "error message" } } });
        expect(notify.notifyError).toHaveBeenCalledTimes(1);
        expect(notify.notifyError).toHaveBeenCalledWith("error message");
    });

    it("notifies with 'An error occured' if exception, message, and xhr not available", () => {
        Uploader.errorHandler({ response: {} });
        expect(notify.notifyError).toHaveBeenCalledTimes(1);
        expect(notify.notifyError).toHaveBeenCalledWith("An error occurred during file upload.");
    });

    it("displays xhr.responseText in an iframe dialog if available", () => {
        Uploader.errorHandler({ xhr: { responseText: "response text" } as any });
        expect(dialogs.iframeDialog).toHaveBeenCalledTimes(1);
        expect(dialogs.iframeDialog).toHaveBeenCalledWith({ html: "response text" });
    });

    it("displays nothing if xhr.statusText is 'abort'", () => {
        Uploader.errorHandler({ xhr: { statusText: "abort" } as any });
        expect(notify.notifyError).not.toHaveBeenCalled();
        expect(dialogs.iframeDialog).not.toHaveBeenCalled();
    });

    it("displays 'An unknown connection error occurred' if xhr.statusText is not 'abort'", () => {
        Uploader.errorHandler({ xhr: { statusText: "unknown" } as any });
        expect(notify.notifyError).toHaveBeenCalledTimes(1);
        expect(notify.notifyError).toHaveBeenCalledWith("An unknown connection error occurred! Check browser console for details.");
    });

    it("displays 'Connection refused' if xhr.status is 500", () => {
        Uploader.errorHandler({ xhr: { status: 500 } as any });
        expect(notify.notifyError).toHaveBeenCalledTimes(1);
        expect(notify.notifyError).toHaveBeenCalledWith("HTTP 500: Connection refused! Check browser console for details.");
    });

    it("displays http status code if xhr.status is not 500", () => {
        Uploader.errorHandler({ xhr: { status: 404 } as any });
        expect(notify.notifyError).toHaveBeenCalledTimes(1);
        expect(notify.notifyError).toHaveBeenCalledWith("HTTP 404 error! Check browser console for details.");
    });

});

describe("Uploader.uploadBatch", () => {
    let batch: UploaderBatch;
    let orgXHR: typeof XMLHttpRequest;
    let xhrMock: XMLHttpRequest;

    beforeEach(() => {
        batch = {
            formData: new FormData(),
            filePaths: ["test.txt"]
        };
        orgXHR = window.XMLHttpRequest = jest.fn() as any;
        xhrMock = {
            open: jest.fn(),
            setRequestHeader: jest.fn(),
            send: jest.fn().mockImplementation(function () {
                this.status = 200;
                this.responseText = JSON.stringify({ success: true });
                this.onload();
            }),
            onload: jest.fn(),
            onerror: jest.fn(),
            onprogress: jest.fn()
        } as any;        
        window.XMLHttpRequest = jest.fn(() => xhrMock as any) as any;
    });

    afterEach(() => {
        window.XMLHttpRequest = orgXHR;
    });

    it("should call batchStart and batchStop", async () => {
        const batchStart = jest.fn();
        const batchStop = jest.fn();
        const uploader = new Uploader({
            batchStart,
            batchStop
        });
        await uploader.uploadBatch(batch, {});
        expect(batchStart).toHaveBeenCalledWith({ batch });
        expect(batchStop).toHaveBeenCalledWith({ batch });
    });

    it("should call batchSuccess on successful upload", async () => {
        window.XMLHttpRequest = jest.fn(() => xhrMock as any) as any;
        const batchSuccess = jest.fn();
        const uploader = new Uploader({
            batchSuccess
        });

        await uploader.uploadBatch(batch, {});
        expect(batchSuccess).toHaveBeenCalledTimes(1);
        const args = batchSuccess.mock.calls[0][0];
        expect(args.batch).toEqual(batch);
        expect(args.request).toEqual({
            responseType: "json",
            url: "/File/TemporaryUpload"
        });
        expect(args.response).toEqual({ success: true });
    });

    it("should call errorHandler on upload error", async () => {
        const errorHandler = jest.fn();
        const uploader = new Uploader({
            errorHandler
        });        
        xhrMock.send = jest.fn().mockImplementation(function () {
            this.status = 500;
            this.onerror();
        });
        await expect(uploader.uploadBatch(batch, {})).rejects.toEqual(expect.objectContaining({
            xhr: xhrMock
        }));
        expect(errorHandler).toHaveBeenCalledWith(expect.objectContaining({
            batch,
            xhr: xhrMock
        }));
    });

    it("should call batchProgress on upload progress", async () => {
        const batchProgress = jest.fn();
        const uploader = new Uploader({
            batchProgress
        });
        xhrMock.send = jest.fn().mockImplementation(function () {
            this.status = 200;
            this.responseText = JSON.stringify({ success: true });
            this.onprogress({ lengthComputable: true, loaded: 50, total: 100 });
            this.onload();
        });

        await uploader.uploadBatch(batch, {});
        expect(batchProgress).toHaveBeenCalledWith(expect.objectContaining({
            batch,
            loaded: 50,
            total: 100
        }));
    });

    it("should handle CSRF token for same origin requests", async () => {
        document.cookie = "CSRF-TOKEN=test-token";
        xhrMock.send = jest.fn().mockImplementation(function () {
            this.status = 200;
            this.responseText = JSON.stringify({ success: true });
            this.onload();
        });
        const uploader = new Uploader({
        });        
        
        await uploader.uploadBatch(batch, {});
        expect(xhrMock.setRequestHeader).toHaveBeenCalledWith("X-CSRF-TOKEN", "test-token");
    });
});
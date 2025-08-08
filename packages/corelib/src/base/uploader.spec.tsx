import * as dialogs from "./dialogs";
import * as notify from "./notify";
import { Uploader, UploaderBatch } from "./uploader";

vi.mock("./dialogs", () => ({
    __esModule: true,
    alertDialog: vi.fn(),
    iframeDialog: vi.fn()
}));


vi.mock("./notify", () => ({
    __esModule: true,
    notifyError: vi.fn()
}));

afterEach(() => {
    vi.resetAllMocks();
    vi.useRealTimers();
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
        console.error = vi.fn();
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
            filePaths: ["test.txt"],
            isFirst: true
        };
        orgXHR = window.XMLHttpRequest = vi.fn() as any;
        xhrMock = {
            open: vi.fn(),
            setRequestHeader: vi.fn(),
            send: vi.fn().mockImplementation(function () {
                this.status = 200;
                this.responseText = JSON.stringify({ success: true });
                this.onload();
            }),
            onload: vi.fn(),
            onerror: vi.fn(),
            onprogress: vi.fn()
        } as any;
        window.XMLHttpRequest = vi.fn(() => xhrMock as any) as any;
    });

    afterEach(() => {
        window.XMLHttpRequest = orgXHR;
    });

    it("should call batchStart and batchStop", async () => {
        const batchStart = vi.fn();
        const batchStop = vi.fn();
        const uploader = new Uploader({
            batchStart,
            batchStop
        });
        await uploader.uploadBatch(batch, {});
        expect(batchStart).toHaveBeenCalledWith({ batch });
        expect(batchStop).toHaveBeenCalledWith({ batch });
    });

    it("should call batchSuccess on successful upload", async () => {
        window.XMLHttpRequest = vi.fn(() => xhrMock as any) as any;
        const batchSuccess = vi.fn();
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
        const errorHandler = vi.fn();
        const uploader = new Uploader({
            errorHandler
        });
        xhrMock.send = vi.fn().mockImplementation(function () {
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
        const batchProgress = vi.fn();
        const uploader = new Uploader({
            batchProgress
        });
        xhrMock.send = vi.fn().mockImplementation(function () {
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
        xhrMock.send = vi.fn().mockImplementation(function () {
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

describe("Uploader.uploadBatch exception handling", () => {
    let orgXHR: typeof XMLHttpRequest;
    let xhrMock: XMLHttpRequest;

    beforeEach(() => {
        orgXHR = window.XMLHttpRequest;
        xhrMock = {
            open: vi.fn(),
            setRequestHeader: vi.fn(),
            send: vi.fn().mockImplementation(function () {
                throw new Error("Simulated upload error");
            }),
            onload: vi.fn(),
            onerror: vi.fn(),
            onprogress: vi.fn()
        } as any;
        window.XMLHttpRequest = vi.fn(() => xhrMock as any) as any;
    });

    afterEach(() => {
        window.XMLHttpRequest = orgXHR;
    });

    it("calls errorHandler when an exception occurs", async () => {
        const errorHandler = vi.fn();
        const batchStop = vi.fn();
        const uploader = new Uploader({ errorHandler, batchStop });

        const batch = {
            formData: new FormData(),
            filePaths: ["file1.txt"],
            isFirst: true
        };

        try {
            await uploader.uploadBatch(batch, {});
        }
        catch (e) {
            // Exception is expected, we just want to ensure errorHandler is called
        }

        expect(errorHandler).toHaveBeenCalledTimes(1);
        expect(errorHandler).toHaveBeenCalledWith(
            expect.objectContaining({
                batch,
                exception: expect.any(Error)
            })
        );
        expect(batchStop).toHaveBeenCalledTimes(1);
    });

    it("handles exceptions gracefully and continues execution", async () => {
        const errorHandler = vi.fn();
        const allStop = vi.fn();
        const uploader = new Uploader({ errorHandler, allStop });

        const batch = {
            formData: new FormData(),
            filePaths: ["file1.txt"],
            isFirst: false
        };

        try {
        await uploader.uploadBatch(batch, {});
        }
        catch (e) {
            // Exception is expected, we just want to ensure errorHandler is called
        }
        (uploader as any).endBatch(true);

        expect(errorHandler).toHaveBeenCalledTimes(1);
        expect(allStop).toHaveBeenCalledTimes(1);
    });
});

describe("Uploader constructor", () => {
    it("sets accept and multiple attributes on input element", () => {
        const input = document.createElement("input");
        new Uploader({ input, accept: "image/*", multiple: true });
        expect(input.getAttribute("accept")).toBe("image/*");
        expect(input.getAttribute("multiple")).toBe("multiple");
    });

    it("calls watchInput and watchDropZone appropriately", () => {
        const input = document.createElement("input");
        const dropZone = document.createElement("div");

        const watchInputSpy = vi.spyOn(Uploader.prototype as any, "watchInput");
        const watchDropZoneSpy = vi.spyOn(Uploader.prototype as any, "watchDropZone");

        new Uploader({ input, dropZone });

        expect(watchInputSpy).toHaveBeenCalledWith(input);
        expect(watchDropZoneSpy).toHaveBeenCalledWith(dropZone);
    });

    it("should call watchDropZone for each dropZone element", () => {
        const dropZone1 = document.createElement("div");
        const dropZone2 = document.createElement("div");
        const dropZones = [dropZone1, dropZone2];

        const watchDropZoneSpy = vi.spyOn(Uploader.prototype as any, "watchDropZone");

        new Uploader({ dropZone: dropZones });

        expect(watchDropZoneSpy).toHaveBeenCalledTimes(2);
        expect(watchDropZoneSpy).toHaveBeenCalledWith(dropZone1);
        expect(watchDropZoneSpy).toHaveBeenCalledWith(dropZone2);
    });    
});

describe("Uploader.getTypePredicate", () => {
    it("returns alwaysTrue when ignoreType is true", () => {
        const uploader = new Uploader({ ignoreType: true });
        const predicate = (uploader as any).getTypePredicate();
        expect(predicate("any/type")).toBe(true);
    });

    it("filters files based on accept attribute", () => {
        const uploader = new Uploader({ accept: "image/png, image/jpeg" });
        const predicate = (uploader as any).getTypePredicate();
        expect(predicate("image/png")).toBe(true);
        expect(predicate("image/jpeg")).toBe(true);
        expect(predicate("application/pdf")).toBe(false);
    });
});

describe("Uploader.watchInput", () => {
    it("handles change event and clears input if autoClear is true", async () => {
        const input = document.createElement("input");
        const uploader = new Uploader({ input, autoClear: true });
        const arrayApiSpy = vi.spyOn(uploader as any, "arrayApi").mockResolvedValue(Promise.resolve());

        const file = new File(["content"], "test.txt", { type: "text/plain" });
        Object.defineProperty(input, "files", { value: [file], writable: false });

        const event = new Event("change");
        input.dispatchEvent(event);

        expect(arrayApiSpy).toHaveBeenCalledWith(event, input.files);
        expect(input.value).toBe("");
    });
});

describe("Uploader.watchDropZone", () => {
    it("handles dragover, dragenter, and drop events", async () => {
        const dropZone = document.createElement("div");
        const uploader = new Uploader({ dropZone });

        const dragOverEvent = new Event("dragover");
        const dragEnterEvent = new Event("dragenter");
        const dropEvent = new Event("drop");
        const mockFile = new File(["content"], "test.png", { type: "image/png" });

        const dataTransfer = {
            value: {
                items: [
                    {
                        kind: "file",
                        type: "image/png",
                        webkitGetAsEntry: () => ({
                            isFile: true,
                            file: (callback: (file: File) => void) => callback(mockFile)
                        })
                    }
                ]
            }
        }

        Object.defineProperty(dragEnterEvent, "dataTransfer", dataTransfer);
        Object.defineProperty(dropEvent, "dataTransfer", dataTransfer);

        const uploadBatchSpy = vi.spyOn(uploader as any, "uploadBatch").mockResolvedValue(Promise.resolve({}));

        dropZone.dispatchEvent(dragOverEvent);
        dropZone.dispatchEvent(dragEnterEvent);
        dropZone.dispatchEvent(dropEvent);

        expect(uploadBatchSpy).toHaveBeenCalled();
        expect(dropZone.classList.contains("drop-valid")).toBe(true);
    });
});

describe("Uploader.watchDropZone with dataTransfer.files", () => {
    it("handles drop event with files when webkitGetAsEntry is not available", async () => {
        const dropZone = document.createElement("div");
        const uploader = new Uploader({ dropZone });

        const mockFile = new File(["content"], "test.png", { type: "image/png" });
        const dropEvent = new Event("drop");
        Object.defineProperty(dropEvent, "dataTransfer", {
            value: {
                files: [mockFile],
                items: []
            }
        });

        const arrayApiSpy = vi.spyOn(uploader as any, "arrayApi").mockResolvedValue(Promise.resolve());

        dropZone.dispatchEvent(dropEvent);

        expect(arrayApiSpy).toHaveBeenCalledWith(dropEvent, [mockFile]);
    });
});

describe("Uploader.arrayApi", () => {
    it("filters files based on type and handles single/multiple settings", async () => {
        const uploader = new Uploader({ multiple: false, accept: "image/*" });
        const file1 = new File(["content"], "test1.png", { type: "image/png" });
        const file2 = new File(["content"], "test2.jpg", { type: "image/jpeg" });
        const file3 = new File(["content"], "test3.pdf", { type: "application/pdf" });

        const event = new Event("change");
        const fileList = { 0: file1, 1: file2, 2: file3, length: 3 } as unknown as FileList;

        const addToBatchSpy = vi.spyOn(uploader as any, "addToBatch").mockResolvedValue(Promise.resolve());

        await (uploader as any).arrayApi(event, fileList);

        expect(addToBatchSpy).toHaveBeenCalledTimes(1);
        expect(addToBatchSpy).toHaveBeenCalledWith(file1, file1.name);
    });
});

describe("Uploader.entriesApi", () => {
    it("handles directories with files and subdirectories", async () => {
        vi.useFakeTimers();
        const dropZone = document.createElement("div");
        const uploader = new Uploader({ dropZone });

        const mockFile1 = new File(["content"], "file1.txt", { type: "text/plain" });
        const mockFile2 = new File(["content"], "file2.txt", { type: "text/plain" });

        const mockFileEntry1 = {
            isFile: true,
            isDirectory: false,
            file: (callback: (file: File) => void) => callback(mockFile1)
        };

        let mockFileEntry2 = {
            isFile: true,
            isDirectory: false,
            file: (callback: (file: File) => void) => callback(mockFile2)
        };

        const mockSubDirectoryEntry = {
            isFile: false,
            isDirectory: true,
            name: "subdir",
            createReader: () => {
                let read = false;
                return {
                    readEntries: (callback: (entries: any[]) => void) => {
                        if (!read) {
                            read = true;
                            callback([mockFileEntry2])
                        } else {
                            callback([]); // No more entries to read
                        }
                    }
                };
            }
        };

        const mockDirectoryEntry = {
            isFile: false,
            isDirectory: true,
            name: "rootdir",
            createReader: () => {
                let read = false;
                return {
                    readEntries: (callback: (entries: any[]) => void) => {
                        if (!read) {
                            read = true;
                            callback([mockFileEntry1, mockSubDirectoryEntry]);
                        }
                        else {
                            callback([]); // No more entries to read
                        }
                    }
                };
            }
        };

        const dropEvent = new Event("drop");
        Object.defineProperty(dropEvent, "dataTransfer", {
            value: {
                items: [
                    {
                        kind: "file",
                        webkitGetAsEntry: () => mockDirectoryEntry
                    }
                ]
            }
        });

        const addToBatchSpy = vi.spyOn(uploader as any, "addToBatch").mockResolvedValue(Promise.resolve());

        dropZone.dispatchEvent(dropEvent);
        await vi.runAllTimersAsync();
        await Promise.resolve();

        expect(addToBatchSpy).toHaveBeenCalledTimes(2);
        expect(addToBatchSpy).toHaveBeenCalledWith(mockFile1, "rootdir/file1.txt");
        expect(addToBatchSpy).toHaveBeenCalledWith(mockFile2, "rootdir/subdir/file2.txt");
    });
});

describe("Uploader.watchDropZone dragenter and dragleave events", () => {
    it("adds drop-valid class on dragenter with valid files", () => {
        const dropZone = document.createElement("div");
        new Uploader({ dropZone });

        const dragEnterEvent = new Event("dragenter");
        Object.defineProperty(dragEnterEvent, "dataTransfer", {
            value: {
                items: [
                    {
                        kind: "file",
                        type: "image/png",
                        webkitGetAsEntry: () => ({ isFile: true })
                    }
                ]
            }
        });

        dropZone.dispatchEvent(dragEnterEvent);

        expect(dropZone.classList.contains("drop-valid")).toBe(true);
        expect(dropZone.classList.contains("drop-invalid")).toBe(false);
    });

    it("adds drop-invalid class on dragenter with invalid files", () => {
        const dropZone = document.createElement("div");
        new Uploader({ dropZone, accept: "image/png" });

        const dragEnterEvent = new Event("dragenter");
        Object.defineProperty(dragEnterEvent, "dataTransfer", {
            value: {
                items: [
                    {
                        kind: "file",
                        type: "application/pdf",
                        webkitGetAsEntry: () => ({ isFile: true })
                    }
                ]
            }
        });

        dropZone.dispatchEvent(dragEnterEvent);

        expect(dropZone.classList.contains("drop-valid")).toBe(false);
        expect(dropZone.classList.contains("drop-invalid")).toBe(true);
    });

    it("removes drop-valid and drop-invalid classes on dragleave", () => {
        const dropZone = document.createElement("div");
        new Uploader({ dropZone });

        dropZone.classList.add("drop-valid");
        dropZone.classList.add("drop-invalid");

        const dragLeaveEvent = new Event("dragleave");
        dropZone.dispatchEvent(dragLeaveEvent);

        expect(dropZone.classList.contains("drop-valid")).toBe(false);
        expect(dropZone.classList.contains("drop-invalid")).toBe(false);
    });
});

describe("Uploader allStart and allStop callbacks", () => {
    let orgXHR: typeof XMLHttpRequest;
    let xhrMock: XMLHttpRequest;

    beforeEach(() => {
        orgXHR = window.XMLHttpRequest;
        xhrMock = {
            open: vi.fn(),
            setRequestHeader: vi.fn(),
            send: vi.fn().mockImplementation(function () {
                this.status = 200;
                this.responseText = JSON.stringify({ success: true });
                this.onload();
            }),
            onload: vi.fn(),
            onerror: vi.fn(),
            onprogress: vi.fn()
        } as any;
        window.XMLHttpRequest = vi.fn(() => xhrMock as any) as any;
    });

    afterEach(() => {
        window.XMLHttpRequest = orgXHR;
    });

    it("executes allStart callback for the first batch", async () => {
        const allStart = vi.fn();
        const uploader = new Uploader({ allStart });

        const batch = {
            formData: new FormData(),
            filePaths: ["file1.txt"],
            isFirst: true
        };

        await uploader.uploadBatch(batch, {});

        expect(allStart).toHaveBeenCalledTimes(1);
    });

    it("executes allStop callback for the last batch", async () => {
        const allStop = vi.fn();
        const uploader = new Uploader({ allStop });

        const batch = {
            formData: new FormData(),
            filePaths: ["file1.txt"],
            isFirst: false
        };

        await uploader.uploadBatch(batch, {});
        (uploader as any).endBatch(true);

        expect(allStop).toHaveBeenCalledTimes(1);
    });
});

describe("Uploader.watchDropZone paste event", () => {
    it("handles paste event with webkitGetAsEntry", async () => {
        const dropZone = document.createElement("div");
        const uploader = new Uploader({ dropZone });

        const mockFile = new File(["content"], "test.png", { type: "image/png" });
        const pasteEvent = new Event("paste") as ClipboardEvent;
        Object.defineProperty(pasteEvent, "clipboardData", {
            value: {
                items: [
                    {
                        kind: "file",
                        type: "image/png",
                        webkitGetAsEntry: () => ({
                            isFile: true,
                            file: (callback: (file: File) => void) => callback(mockFile)
                        })
                    }
                ]
            }
        });

        const entriesApiSpy = vi.spyOn(uploader as any, "entriesApi").mockResolvedValue(Promise.resolve());

        dropZone.dispatchEvent(pasteEvent);

        expect(entriesApiSpy).toHaveBeenCalledWith(pasteEvent, pasteEvent.clipboardData.items);
    });

    it("handles paste event with .files when webkitGetAsEntry is not available", async () => {
        const dropZone = document.createElement("div");
        const uploader = new Uploader({ dropZone });

        const mockFile = new File(["content"], "test.png", { type: "image/png" });
        const pasteEvent = new Event("paste") as ClipboardEvent;
        Object.defineProperty(pasteEvent, "clipboardData", {
            value: {
                files: [mockFile],
                items: []
            }
        });

        const arrayApiSpy = vi.spyOn(uploader as any, "arrayApi").mockResolvedValue(Promise.resolve());

        dropZone.dispatchEvent(pasteEvent);

        expect(arrayApiSpy).toHaveBeenCalledWith(pasteEvent, pasteEvent.clipboardData.files);
    });
});

/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */

import { iframeDialog } from "./dialogs";
import { notifyError } from "./notify";
import { getCookie, isSameOrigin, resolveUrl } from "./services";
import { isArrayLike } from "./system";

export interface UploaderOptions {
    /** Accept. If not specified, read from the passed input  */
    accept?: string;
    /** Auto clear input value after selection, so when same file selected it works. Default is true */
    autoClear?: boolean;
    /** Only used for multiple, default is 1 to upload multiple files in batches of size 1 */
    batchSize?: number;
    /** An optional list of dropzones. */
    dropZone?: HTMLElement | ArrayLike<HTMLElement>;
    /** Progress event that is called when a batch is about to be uploaded */
    batchStart?: (data: { batch: UploaderBatch }) => void;
    /** Progress event that is called when a batch is ended uploading or failed */
    batchStop?: (data: { batch: UploaderBatch }) => void;
    /** Called after batch is uploaded successfully */
    batchSuccess?: (data: UploaderSuccessData) => void;
    /** Progress event that is called during upload */
    batchProgress?: (data: { batch: UploaderBatch, loaded: number; total: number }) => void;
    /** Callback to handle a batch. If not specified, a default handler is used. */
    batchHandler?: (batch: UploaderBatch, uploader: Uploader) => void | Promise<void>;
    /** Only called when a change/drop event occurs, but files can't be determined */
    changeCallback?: (e: Event) => void;
    /** Error handler, if not specified Uploader.errorHandler is used */
    errorHandler?: (data: UploaderErrorData) => void;
    /** Ignore file types, e.g. don't check accept property of input or this options */
    ignoreType?: boolean;
    /** Target input. If null, dropZone should be specified. */
    input?: HTMLInputElement;
    /** Allow multiple files. If not specified is read from the input */
    multiple?: boolean;
    /** The field name to use in FormData object. Default is files[] */
    name?: string;
}

export interface UploaderRequest {
    /** A function that will return headers to be sent with request, or static set of headers */
    headers?: Record<string, string>
    /** Response type expected from the server. Default is json */
    responseType?: "json" | "text";
    /** URL to send the request to. Default is ~/File/TemporaryUpload */
    url?: string;
}

export interface UploaderBatch {
    event?: Event;
    filePaths?: string[];
    formData: FormData;
}

export interface UploaderSuccessData {
    batch: UploaderBatch;
    request: UploaderRequest; 
    event: ProgressEvent;
    xhr: XMLHttpRequest;
    response: any;
}

export interface UploaderErrorData {
    batch?: UploaderBatch,
    event?: ProgressEvent,
    exception?: any;
    request?: UploaderRequest,
    response?: any,
    xhr?: XMLHttpRequest
}

function alwaysTrue() { return true; }

export class Uploader {

    private opt: UploaderOptions;
    private batch: UploaderBatch;

    constructor(opt: UploaderOptions) {
        this.opt = opt = Object.assign({}, Uploader.defaults, opt);

        if (this.opt.batchHandler === void 0)
            this.opt.batchHandler = (batch, uploader) => uploader.uploadBatch(batch);

        if (this.opt.errorHandler === void 0)
            this.opt.errorHandler = Uploader.errorHandler;

        if (opt.input) {
            if (opt.accept)
                opt.input.setAttribute("accept", opt.accept);
            if (opt.multiple)
                opt.input.setAttribute("multiple", "multiple");
            this.watchInput(opt.input);
        }

        if (isArrayLike(opt.dropZone)) {
            for (var i = 0; i < opt.dropZone.length; i++)
                opt.dropZone[i] && this.watchDropZone(opt.dropZone[i]);
        }
        else if (opt.dropZone) {
            this.watchDropZone(opt.dropZone);
        }
    }

    private newBatch(event: Event) {
        this.batch = {
            event,
            filePaths: [],
            formData: new FormData()
        }
    }

    private async addToBatch(file: File, filePath: string): Promise<void> {
        this.batch.filePaths.push(filePath);
        this.batch.formData.set(this.opt.name, file, filePath);
        if (!this.isMultiple() ||
            (this.opt.batchSize && this.batch.filePaths.length > this.opt.batchSize)) {
            await this.endBatch();
        }
    }

    private async endBatch() {
        if (this.batch?.filePaths?.length) {
            const batch = this.batch;
            await this.opt.batchHandler?.(batch, this);
            this.newBatch(batch.event);
        }
    }

    static defaults: Partial<UploaderOptions> = {
        autoClear: true,
        batchSize: 1,
        name: "files[]"
    }

    static requestDefaults: Partial<UploaderRequest> = {
        responseType: "json"
    }

    private isMultiple() {
        return !!(this.opt.multiple ?? this.opt?.input?.getAttribute("multiple"));
    }

    private getTypePredicate(): ((type: string) => boolean) {
        if (this.opt.ignoreType)
            return alwaysTrue;

        let acceptVal = this.opt.accept ?? this.opt?.input?.getAttribute("accept")
        if (!acceptVal)
            return alwaysTrue;

        const accepts = acceptVal.toLowerCase().split(',').map((accept) => {
            return accept.split('/').map(part => part.trim());
        }).filter(acceptParts => acceptParts.length === 2);

        return (type: string) => {
            const [typeMain, typeSub] = (type ?? "").toLowerCase().split('/').map(s => s.trim());

            for (const [acceptMain, acceptSub] of accepts) {
                if (typeMain === acceptMain && (acceptSub === '*' || typeSub === acceptSub)) {
                    return true;
                }
            }
            return false;
        };
    }

    private getMatchingItems(list: DataTransferItemList): DataTransferItem[] {
        let predicate = this.getTypePredicate();
        let results: DataTransferItem[] = Array.from(list ?? []).filter(x => x.kind === "file" && predicate(x.type));
        return this.isMultiple() ? results : [results[0]];
    }

    private watchInput(input: HTMLInputElement) {
        input.addEventListener("change", async e => {
            if ((e.target as any)?.files?.length) {
                try {
                    await this.arrayApi(e, (e.target as HTMLInputElement).files);
                }
                finally {
                    if (this.opt.autoClear)
                        (e.target as HTMLInputElement).value = null;
                }
            } else {
                this.opt.changeCallback(e);
            }
        });
    }

    private watchDropZone(node: HTMLElement) {
        const stop = (e: Event) => e.preventDefault();
        node.addEventListener("dragover", stop);
        node.addEventListener("dragenter", e => {
            (node as any).dragEnterCount = ((node as any).dragEnterCount || 0) + 1;
            if ((node as any).dragEnterCount > 1) {
                return;
            }
            if (e.dataTransfer === null) {
                node.classList.add('drop-invalid');
                return;
            }

            const matchingFiles = this.getMatchingItems(e.dataTransfer.items);
            // Safari doesn't give file information on drag enter, so the best we can do is return valid.
            const validDrop: boolean = e.dataTransfer && e.dataTransfer.items.length ? (matchingFiles[0] !== undefined) : true;

            node.classList.toggle('drop-valid', !!validDrop);
            node.classList.toggle('drop-invalid', !validDrop);
        });

        node.addEventListener("dragleave", e => {
            (node as any).dragEnterCount = Math.max(((node as any).dragEnterCount || 0) - 1, 0);
            if ((node as any).dragEnterCount === 0) {
                (node as any).dragEnterCount = 0;
                node.classList.remove('drop-valid');
                node.classList.remove('drop-invalid');
                return;
            }
            if (e.dataTransfer === null) {
                node.classList.add('drop-invalid');
                return;
            }
        });

        node.addEventListener("drop", (e) => {
            e.preventDefault();
            if (e.dataTransfer.items?.[0]?.webkitGetAsEntry()) {
                this.entriesApi(e, e.dataTransfer.items);
            } else if (e.dataTransfer.files) {
                this.arrayApi(e, e.dataTransfer.files);
            } else {
                this.opt.changeCallback(e);
            }
        });

        node.addEventListener("paste", (e) => {
            if (e.clipboardData.items?.[0]?.webkitGetAsEntry()) {
                this.entriesApi(e, e.clipboardData.items);
            } else if (e.clipboardData.files) {
                this.arrayApi(e, e.clipboardData.files);
            } else {
                this.opt.changeCallback(e);
            }
        });
    }

    private async arrayApi(e: Event, fileList: FileList): Promise<void> {
        this.newBatch(e);
        let predicate = this.getTypePredicate();
        let filteredFiles = Array.from(fileList).filter(x => predicate(x.type));
        if (!this.isMultiple() && filteredFiles.length > 0)
            filteredFiles = [filteredFiles[0]];
        for (var file of filteredFiles) {
            await this.addToBatch(file, file.webkitRelativePath || file.name);
        }
        await this.endBatch();
    }

    private async entriesApi(e: Event, items: DataTransferItemList): Promise<void> {
        this.newBatch(e);
        let predicate = this.getTypePredicate();
        let multiple = this.isMultiple();
        const skipRest = () => !multiple && this.batch?.filePaths?.length > 0;

        let readDirectory = async (entry: FileSystemDirectoryEntry, path: string) => {
            if (!path)
                path = entry.name;
            await readEntries(entry, null, null, async entries => {
                for (const entry of entries) {
                    if (skipRest())
                        break;
                    if (entry.isFile) {
                        await new Promise((resolve) => {
                            (entry as FileSystemFileEntry).file(async file => {
                                if (predicate(file.type))
                                    await this.addToBatch(file, `${path}/${file.name}`);
                                resolve(void 0);
                            }, resolve.bind(void 0));
                        });
                    } else {
                        await readDirectory(entry, `${path}/${entry.name}`);
                    }
                }
            });
        }

        let readEntries = async (entry: FileSystemDirectoryEntry, reader: FileSystemDirectoryReader,
            oldEntries: FileSystemEntry[], cb: (entries: any) => void) => {
            const dirReader = reader || entry.createReader();

            await new Promise((resolve) => {
                dirReader.readEntries(entries => {
                    if (skipRest())
                        return;

                    const newEntries = oldEntries ? oldEntries.concat(entries) : entries;
                    if (entries.length) {
                        setTimeout(readEntries.bind(null, entry, dirReader, newEntries, cb), 0);
                    } else {
                        cb(newEntries);
                    }

                    resolve(void 0);
                }, () => resolve(void 0));
            });
        }

        for (var i = 0; i < items.length; i++) {
            if (skipRest())
                return;

            let entry = items[i].webkitGetAsEntry();
            if (entry) {
                await new Promise(async (resolve) => {
                    if (entry.isFile) {
                        (entry as FileSystemFileEntry).file(async (file) => {
                            if (!skipRest() && predicate(file.type))
                                await this.addToBatch(file, file.name);
                            resolve(void 0);
                        }, resolve.bind(void 0));
                    } else if (entry.isDirectory) {
                        await readDirectory(entry as FileSystemDirectoryEntry, null);
                    }
                });
            }
        }

        this.endBatch();
    }

    async uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void> {
        if (!batch || !batch.formData)
            return;

        request = Object.assign({}, Uploader.requestDefaults);
        if (request.url === void 0)
            request.url = resolveUrl("~/File/TemporaryUpload");

        this.opt.batchStart?.({ batch });
        try {
            await new Promise((resolve, reject) => {
                try {
                    const xhr = new XMLHttpRequest();
                    xhr.open("POST", request.url);
                    let json = request.responseType !== "text";

                    if (isSameOrigin(request.url)) {
                        var token = getCookie('CSRF-TOKEN');
                        if (token)
                            xhr.setRequestHeader("X-CSRF-TOKEN", token);
                    }

                    if (request.headers) {
                        for (var name of Object.keys(request.headers)) {
                            xhr.setRequestHeader(name, request.headers[name]);
                        }
                    }

                    const onerror = (data: UploaderErrorData) => {
                        data = Object.assign({
                            batch,
                            request,
                            response: json ? tryGetJson(xhr) : xhr.responseText,
                            xhr
                        }, data);
                        try {
                            try {
                                this.opt.errorHandler?.(data);
                            }
                            finally {
                                reject(data);
                            }
                        }
                        catch (exception) {
                            console.log(exception);
                        }
                    };

                    xhr.onload = (event) => {
                        try {
                            if (xhr.status === 200) {
                                var data: UploaderSuccessData = {
                                    batch,
                                    event,
                                    request,
                                    response: json ? tryGetJson(xhr) : xhr.responseText,
                                    xhr
                                }
                                this.opt.batchSuccess?.(data);
                                resolve(data);
                            }
                            else {
                                onerror({ event });                                
                            }
                        }
                        catch (exception) {
                            onerror({ event, exception });
                        }
                    };

                    xhr.onerror = event => onerror({ event });

                    xhr.onprogress = (event) => {
                        try {
                            if (event.lengthComputable) {
                                this.opt.batchProgress?.({
                                    batch,
                                    loaded: event.loaded,
                                    total: event.total
                                });
                            }
                        }
                        catch {
                        }
                    }

                    xhr.send(batch.formData);
                }
                catch (exception) {
                    const data: UploaderErrorData = {
                        exception,
                        batch,
                        request
                    }
                    try {
                        this.opt.errorHandler?.(data);
                    }
                    finally {
                        reject(data);
                    }
                }
            });
        }
        finally {
            this.opt.batchStop?.({ batch });
        }
    }

    static errorHandler = (data: UploaderErrorData) => {
        if (data?.exception) {
            console.log(data.exception);
            notifyError(data.exception.toString?.() ?? "Exception occured!");
            return;
        }

        if (data?.response?.Error?.Message) {
            notifyError(data.response.Error.Message);
            return;
        }

        let xhr = data?.xhr;
        if (!xhr) {
            notifyError('An error occurred during file upload.');
            return;
        }

        var html = xhr.responseText;
        if (html) {
            iframeDialog({ html: html });
            return;
        }

        if (!xhr.status) {
            if (xhr.statusText != "abort")
                notifyError("An unknown connection error occurred! Check browser console for details.");
            return;
        }

        if (xhr.status == 500) {
            notifyError("HTTP 500: Connection refused! Check browser console for details.");
            return;
        }

        notifyError("HTTP " + xhr.status + ' error! Check browser console for details.');
    }
}

function tryGetJson(xhr: XMLHttpRequest) {
    try {
        return JSON.parse(xhr.responseText);
    }
    catch {
        return null;
    }
}


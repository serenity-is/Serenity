/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */

import { iframeDialog } from "./dialogs";
import { Fluent } from "./fluent";
import { notifyError } from "./notify";
import { getCookie, isSameOrigin, resolveUrl } from "./services";
import { isArrayLike } from "./system";

/**
 * Options controlling file selection, drag-and-drop, batching, and event callbacks for {@link Uploader}.
 */
export interface UploaderOptions {
    /** MIME / extension filter (e.g. `"image/*,.pdf"`). Falls back to the `accept` attribute of {@link UploaderOptions.input} when omitted. */
    accept?: string;
    /** When `true` (default) clears the input value after handling the change event so re-selecting the same file re-triggers the handler. */
    autoClear?: boolean;
    /** Number of files per batch when `multiple` is enabled. Defaults to `1`. Larger values upload files in groups. */
    batchSize?: number;
    /** One or more elements that act as drag-and-drop targets. */
    dropZone?: HTMLElement | ArrayLike<HTMLElement>;
    /** Called once before the first batch starts uploading. */
    allStart?: () => void;
    /** Called once after the last batch completes or fails. */
    allStop?: () => void;
    /** Called when an individual batch is about to be uploaded. */
    batchStart?: (data: { batch: UploaderBatch }) => void;
    /** Called when an individual batch finishes uploading or fails. */
    batchStop?: (data: { batch: UploaderBatch }) => void;
    /** Called after a batch uploads successfully. */
    batchSuccess?: (data: UploaderSuccessData) => void;
    /** Called periodically with upload progress for the current batch. */
    batchProgress?: (data: { batch: UploaderBatch, loaded: number; total: number }) => void;
    /** Custom handler for uploading a batch. When omitted {@link Uploader.uploadBatch} is used. */
    batchHandler?: (batch: UploaderBatch, uploader: Uploader) => void | Promise<void>;
    /** Called when a change / drop / paste event occurs but no files could be resolved. */
    changeCallback?: (e: Event) => void;
    /** Error handler for upload failures. Defaults to {@link Uploader.errorHandler}. */
    errorHandler?: (data: UploaderErrorData) => void;
    /** When `true` disables MIME-type filtering against `accept`. */
    ignoreType?: boolean;
    /** File input that triggers selection. When `null`, {@link UploaderOptions.dropZone} must be provided. */
    input?: HTMLInputElement;
    /** Allows multiple file selection. Falls back to the `multiple` attribute of {@link UploaderOptions.input} when omitted. */
    multiple?: boolean;
    /** Form field name used when appending files to `FormData`. Defaults to `"files[]"`. */
    name?: string;
}

/**
 * Request configuration for {@link Uploader.uploadBatch}.
 */
export interface UploaderRequest {
    /** Extra headers to send with the upload request. */
    headers?: Record<string, string>
    /** Expected response type. Defaults to `"json"`. */
    responseType?: "json" | "text";
    /** Endpoint URL for the upload. Defaults to `~/File/TemporaryUpload`. */
    url?: string;
}

/**
 * Represents a single upload batch queued by {@link Uploader}.
 */
export interface UploaderBatch {
    /** Originating DOM event (change / drop / paste). */
    event?: Event;
    /** Relative paths / names of files in this batch. */
    filePaths?: string[];
    /** `FormData` payload containing the batched files. */
    formData: FormData;
    /** `true` for the first batch in a multi-batch sequence. */
    isFirst?: boolean;
}

/**
 * Data passed to {@link UploaderOptions.batchSuccess} after a successful upload.
 */
export interface UploaderSuccessData {
    /** The batch that was uploaded. */
    batch: UploaderBatch;
    /** Request configuration used for the upload. */
    request: UploaderRequest;
    /** XHR load event. */
    event: ProgressEvent;
    /** The underlying `XMLHttpRequest`. */
    xhr: XMLHttpRequest;
    /** Parsed response body (JSON or text depending on {@link UploaderRequest.responseType}). */
    response: any;
}

/**
 * Data passed to error handlers when an upload fails.
 */
export interface UploaderErrorData {
    /** The batch that failed, if available. */
    batch?: UploaderBatch,
    /** XHR progress / error event, if available. */
    event?: ProgressEvent,
    /** Exception thrown during setup or handling, if any. */
    exception?: any;
    /** Request configuration used for the failed attempt. */
    request?: UploaderRequest,
    /** Parsed response body, if available. */
    response?: any,
    /** The underlying `XMLHttpRequest`, if available. */
    xhr?: XMLHttpRequest
}

function alwaysTrue() { return true; }

/**
 * File uploader that handles `input` change, drag-and-drop, paste, and directory
 * traversal, batching files and uploading each batch via `XMLHttpRequest`.
 * Supports MIME filtering, progress events, CSRF headers, and custom batch handling.
 */
export class Uploader {

    declare private opt: UploaderOptions;
    declare private batch: UploaderBatch;

    /**
     * Creates a new uploader and wires up the configured input and drop zones.
     * @param opt - Uploader configuration; defaults from {@link Uploader.defaults} are applied.
     */
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
            for (let i = 0; i < opt.dropZone.length; i++)
                opt.dropZone[i] && this.watchDropZone(opt.dropZone[i]);
        }
        else if (opt.dropZone) {
            this.watchDropZone(opt.dropZone);
        }
    }

    private newBatch(event: Event, isFirst: boolean) {
        this.batch = {
            event,
            filePaths: [],
            formData: new FormData(),
            isFirst
        }
    }

    private async addToBatch(file: File, filePath: string): Promise<void> {
        this.batch.filePaths.push(filePath);
        this.batch.formData.set(this.opt.name, file, filePath);
        if (!this.isMultiple() ||
            (this.opt.batchSize && this.batch.filePaths.length >= this.opt.batchSize)) {
            await this.endBatch(false);
        }
    }

    private async endBatch(final: boolean) {
        if (this.batch?.filePaths?.length) {
            const batch = this.batch;
            await this.opt.batchHandler?.(batch, this);
            this.newBatch(batch.event, false);
        }
        if (final) {
            this.opt.allStop?.();
            Fluent.trigger(this.opt.input, "allStop");
        }
    }

    /** Default {@link UploaderOptions} applied when constructing an instance. */
    static defaults: Partial<UploaderOptions> = {
        autoClear: true,
        batchSize: 1,
        name: "files[]"
    }

    /** Default {@link UploaderRequest} applied when {@link Uploader.uploadBatch} is called without explicit request options. */
    static requestDefaults: Partial<UploaderRequest> = {
        responseType: "json"
    }

    /**
     * Whether the uploader is configured for multiple file selection.
     * @returns `true` if multiple files are allowed.
     */
    isMultiple() {
        return !!(this.opt.multiple ?? (this.opt?.input as HTMLInputElement)?.multiple);
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
                this.opt.changeCallback?.(e);
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
            if (e.dataTransfer.items?.[0]?.webkitGetAsEntry() || (e.dataTransfer.items?.[0] as any)?.getAsEntry()) {
                this.entriesApi(e, e.dataTransfer.items);
            } else if (e.dataTransfer.files) {
                this.arrayApi(e, e.dataTransfer.files);
            } else {
                this.opt.changeCallback?.(e);
            }
        });

        node.addEventListener("paste", (e) => {
            if (e.clipboardData.items?.[0]?.webkitGetAsEntry() || (e.clipboardData.items?.[0] as any)?.getAsEntry()) {
                this.entriesApi(e, e.clipboardData.items);
            } else if (e.clipboardData.files) {
                this.arrayApi(e, e.clipboardData.files);
            } else {
                this.opt.changeCallback?.(e);
            }
        });
    }

    private async arrayApi(e: Event, fileList: FileList): Promise<void> {
        this.newBatch(e, true);
        let predicate = this.getTypePredicate();
        let filteredFiles = Array.from(fileList).filter(x => predicate(x.type));
        if (!this.isMultiple() && filteredFiles.length > 0)
            filteredFiles = [filteredFiles[0]];
        for (const file of filteredFiles) {
            await this.addToBatch(file, file.webkitRelativePath || file.name);
        }
        await this.endBatch(true);
    }

    private async entriesApi(e: Event, items: DataTransferItemList): Promise<void> {
        this.newBatch(e, true);
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
                    } else if (entry.isDirectory) {
                        await readDirectory(entry as FileSystemDirectoryEntry, `${path}/${entry.name}`);
                    }
                }
            });
        }

        let readEntries = async (
            entry: FileSystemDirectoryEntry,
            reader: FileSystemDirectoryReader,
            oldEntries: FileSystemEntry[],
            cb: (entries: FileSystemEntry[]) => void
        ) => {
            const dirReader = reader || entry.createReader();
            let allEntries: FileSystemEntry[] = oldEntries ? oldEntries.slice() : [];

            while (true) {
                const entries: FileSystemEntry[] = await new Promise((resolve) => {
                    dirReader.readEntries(resolve, () => resolve([]));
                });
                if (skipRest()) {
                    break;
                }
                if (entries.length) {
                    allEntries = allEntries.concat(entries);
                } else {
                    break;
                }
            }
            cb(allEntries);
        }

        const entries = Array.from(items).map(x => x.webkitGetAsEntry?.() ?? (x as any).getAsEntry?.()).filter(x => !!x);

        for (let i = 0; i < entries.length; i++) {
            if (skipRest())
                return;

            let entry = entries[i];
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

        this.endBatch(true);
    }

    /**
     * Uploads a single batch via `XMLHttpRequest`.
     * @param batch - Batch payload containing `FormData` and file paths.
     * @param request - Optional request overrides merged over {@link Uploader.requestDefaults}.
     */
    async uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void> {
        if (!batch || !batch.formData)
            return;

        request = Object.assign({}, Uploader.requestDefaults, request);
        if (request.url === void 0)
            request.url = resolveUrl("~/File/TemporaryUpload");

        if (batch.isFirst) {
            this.opt.allStart?.();
            Fluent.trigger(this.opt.input, "allStart");
        }
        this.opt.batchStart?.({ batch });
        Fluent.trigger(this.opt.input, "batchStart", { detail: batch });
        try {
            await new Promise((resolve, reject) => {
                try {
                    const xhr = new XMLHttpRequest();
                    xhr.open("POST", request.url);
                    let json = request.responseType !== "text";

                    if (isSameOrigin(request.url)) {
                        const token = getCookie('CSRF-TOKEN');
                        if (token)
                            xhr.setRequestHeader("X-CSRF-TOKEN", token);
                    }

                    if (request.headers) {
                        for (const name of Object.keys(request.headers)) {
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
                                const data: UploaderSuccessData = {
                                    batch,
                                    event,
                                    request,
                                    response: json ? tryGetJson(xhr) : xhr.responseText,
                                    xhr
                                }
                                this.opt.batchSuccess?.(data);
                                Fluent.trigger(this.opt.input, "batchSuccess", { detail: data });
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
                                const data = {
                                    batch,
                                    loaded: event.loaded,
                                    total: event.total
                                }
                                this.opt.batchProgress?.(data);
                                Fluent.trigger(this.opt.input, "batchProgress", { detail: data });
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
            Fluent.trigger(this.opt.input, "batchStop", { detail: batch });
        }
    }

    /**
     * Default error handler. Logs the exception, surfaces server error messages,
     * and falls back to generic notifications or an iframe dialog for HTML responses.
     * @param data - Error context for the failed upload.
     */
    static errorHandler(data: UploaderErrorData) {
        if (data?.exception) {
            console.error(data.exception);
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

        const html = xhr.responseText;
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


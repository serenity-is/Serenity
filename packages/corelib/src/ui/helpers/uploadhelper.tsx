import { FileUploadTexts, Fluent, ServiceResponse, Uploader, blockUI, blockUndo, getjQuery, htmlEncode, isArrayLike, notifyError, resolveUrl, round, stringFormat } from "../../base";
import { replaceAll } from "../../compat";

/**
 * Helper functions for file uploads, image constraints, and file display.
 */
export namespace UploadHelper {


    /**
     * Adds an upload input to a container and returns a Fluent wrapper around it.
     * @param options - The upload input options.
     * @returns A Fluent wrapper around the created input element.
     */
    export function addUploadInput(options: UploadInputOptions): Fluent {
        return Fluent(createUploadInput(options).input);
    }

    /**
     * Creates an upload input element and its associated Uploader.
     * @param options - The upload input options.
     * @returns An object containing the created input element and uploader.
     */
    export function createUploadInput(options: UploadInputOptions): {
        input: HTMLInputElement,
        uploader: Uploader
    } {
        let container = isArrayLike(options.container) ? options.container[0] : options.container;
        if (!container)
            throw new Error("UploadHelper.createUploadInput: container is required!");
        let progress = Fluent(isArrayLike(options.progress) ? options.progress[0] : options.progress);
        const button = container.closest(".tool-button") ?? container.closest("button") ?? container;
        button.classList.add("fileinput-button");

        let uploadUrl = options.uploadUrl || '~/File/TemporaryUpload';
        if (options.uploadIntent) {
            if (uploadUrl.indexOf('?') < 0)
                uploadUrl += "?"
            else
                uploadUrl += "&";
            uploadUrl += "uploadIntent=";
            uploadUrl += encodeURIComponent(options.uploadIntent);
        }

        const input = container.appendChild(<input type="file" name={options.inputName + '[]'} data-url={resolveUrl(uploadUrl)} multiple={!!options.allowMultiple} />) as HTMLInputElement;

        const setProgress = (percent: number) => {
            let bar = progress.children()[0];
            bar && (bar.style.width = (percent ?? 0).toString() + '%');
        }

        const uploader = new Uploader({
            batchSize: 1,
            batchSuccess: data => {
                const response: UploadResponse = data.response ?? {};
                if (response?.Error) {
                    notifyError(response.Error.Message);
                    return;
                }
                options.fileDone?.(response, data.batch?.filePaths?.[0], data);
            },
            input: input,
            dropZone: options.zone,
            batchStart: () => {
                blockUI(null);
                progress.show();
                setProgress(0);
            },
            batchStop: () => {
                blockUndo();
                setProgress(100);
                progress.hide();
            },
            batchProgress: data => {
                if (typeof data.loaded == "number" && data.total > 0) {
                    var percent = data.loaded / data.total * 100;
                    setProgress(percent);
                }
            }
        });

        return { input, uploader };
    }

    /**
     * Checks an uploaded file against the given image constraints, notifying the
     * user of any violation.
     * @param file - The uploaded file response.
     * @param opt - The constraints to check against.
     * @returns True if the file satisfies all constraints, otherwise false.
     */
    export function checkImageConstraints(file: UploadResponse,
        opt: FileUploadConstraints): boolean {

        if (!file.IsImage && !opt.allowNonImage) {
            notifyError(FileUploadTexts.NotAnImageFile);
            return false;
        }
        if (opt.minSize > 0 && file.Size < opt.minSize) {
            notifyError(stringFormat(FileUploadTexts.UploadFileTooSmall,
                fileSizeDisplay(opt.minSize)));
            return false;
        }
        if (opt.maxSize > 0 && file.Size > opt.maxSize) {
            notifyError(stringFormat(FileUploadTexts.UploadFileTooBig,
                fileSizeDisplay(opt.maxSize)));
            return false;
        }
        if (!file.IsImage) {
            return true;
        }
        if (opt.minWidth > 0 && file.Width < opt.minWidth) {
            notifyError(stringFormat(FileUploadTexts.MinWidth, opt.minWidth));
            return false;
        }
        if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
            notifyError(stringFormat(FileUploadTexts.MaxWidth, opt.maxWidth));
            return false;
        }
        if (opt.minHeight > 0 && file.Height < opt.minHeight) {
            notifyError(stringFormat(FileUploadTexts.MinHeight, opt.minHeight));
            return false;
        }
        if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
            notifyError(stringFormat(FileUploadTexts.MaxHeight, opt.maxHeight));
            return false;
        }
        return true;
    }

    /**
     * Returns a display string combining a file name and its size.
     * @param name - The file name.
     * @param bytes - The file size in bytes.
     * @returns The combined display string.
     */
    export function fileNameSizeDisplay(name: string, bytes: number): string {
        return name + ' (' + fileSizeDisplay(bytes) + ')';
    }

    /**
     * Formats a byte count into a human-readable size string (KB or MB).
     * @param bytes - The file size in bytes.
     * @returns The formatted size string.
     */
    export function fileSizeDisplay(bytes: number): string {
        var byteSize = round(bytes * 100 / 1024) * 0.01;
        var suffix = 'KB';
        if (byteSize >= 1024) {
            byteSize = round(byteSize * 100 / 1024) * 0.01;
            suffix = 'MB';
        }
        var sizeParts = byteSize.toString().split(String.fromCharCode(46));
        var value;
        if (sizeParts.length > 1) {
            value = sizeParts[0] + '.' + sizeParts[1].substring(0, 2);
        }
        else {
            value = sizeParts[0];
        }
        return value + ' ' + suffix;
    }

    /**
     * Returns whether the given filename has a common image extension.
     * @param filename - The filename to check.
     * @returns True if the filename ends with a known image extension.
     */
    export function hasImageExtension(filename: string): boolean {
        if (!filename) {
            return false;
        }
        filename = filename.toLowerCase();
        return filename.endsWith('.jpg') || filename.endsWith('.jpeg') ||
            filename.endsWith('.gif') || filename.endsWith('.png') ||
            filename.endsWith('.webp');
    }

    /**
     * Returns the thumbnail file name for the given filename.
     * @param filename - The original filename.
     * @returns The thumbnail filename.
     */
    export function thumbFileName(filename: string): string {
        filename = filename ?? '';
        var idx = filename.lastIndexOf('.');
        if (idx >= 0) {
            filename = filename.substring(0, idx);
        }
        return filename + '_t.jpg';
    }

    /**
     * Returns the resolved URL for a database-stored file.
     * @param filename - The filename.
     * @returns The resolved file URL.
     */
    export function dbFileUrl(filename: string): string {
        filename = replaceAll(filename ?? '', '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    /**
     * Creates a lightbox for a single upload thumbnail anchor element.
     * It uses one of glightbox, simplelightbox or colorbox if available.
     * Override this function to use a different lightbox library.
     */
    export function lightbox(link: HTMLElement | ArrayLike<HTMLElement>): void {
        link = isArrayLike(link) ? link[0] : link;
        if (!link)
            return;

        const glightbox = (globalThis as any).GLightbox;
        if (glightbox) {
            Fluent.on(link, "click", (e: MouseEvent) => {
                e.preventDefault();
                const lightbox = glightbox({
                    elements: [
                        {
                            href: link.getAttribute('href') || '',
                            title: link.getAttribute('title') || ''
                        }
                    ],
                });
                lightbox.on('close', () => lightbox.destroy());
                lightbox.open();
            });
            return;
        }

        const simpleLightbox = (globalThis as any).SimpleLightbox;
        if (simpleLightbox) {
            simpleLightbox([link], {
                fileExt: null,
                history: false,
                overlayOpacity: 1,
            });
            return;
        }

        let $ = getjQuery();
        if (!$)
            return;
        $(link).colorbox?.({
            current: htmlEncode(FileUploadTexts.ColorboxCurrent),
            previous: htmlEncode(FileUploadTexts.ColorboxPrior),
            next: htmlEncode(FileUploadTexts.ColorboxNext),
            close: htmlEncode(FileUploadTexts.ColorboxClose)
        });
    }

    /** @deprecated use lightbox
     * Creates a lightbox for a single upload thumbnail anchor element.
     * @param link - The anchor element (or array-like of elements) to open in a lightbox.
     */
    export const colorBox = lightbox;

    /**
     * Populates a container with file item elements for the given uploaded files.
     * @param c - The container element (or array-like of elements) to populate.
     * @param items - The uploaded files to display.
     * @param displayOriginalName - Whether to display the original file names.
     * @param urlPrefix - Optional URL prefix prepended to file names.
     */
    export function populateFileSymbols(c: HTMLElement | ArrayLike<HTMLElement>, items: UploadedFile[],
        displayOriginalName?: boolean, urlPrefix?: string): void {
        let container = isArrayLike(c) ? c[0] : c;
        if (!container)
            return;
        items = items || [];
        container.innerHTML = "";
        for (var index = 0; index < items.length; index++) {
            const item = items[index];
            const isImage = hasImageExtension(item.Filename);
            const originalName = item.OriginalName ?? '';

            let fileName = item.Filename;
            if (urlPrefix != null && fileName != null &&
                !fileName.startsWith('temporary/')) {
                fileName = urlPrefix + fileName;
            }

            const thumb = <a class="thumb" href={dbFileUrl(fileName)} target="_blank"></a> as HTMLAnchorElement;

            if (originalName) {
                thumb.title = originalName;
            }

            if (isImage) {
                thumb.style.backgroundImage = "url('" + dbFileUrl(thumbFileName(item.Filename)) + "')";
                lightbox(thumb);
            }

            container.appendChild(
                <li class={["file-item", isImage ? "file-image" : "file-binary"]} data-index={index}>
                    {thumb}
                    {displayOriginalName && <div class="filename" title={originalName}>{originalName}</div>}
                </li>
            );
        }
    }
}

/**
 * Represents an uploaded file.
 */
export interface UploadedFile {
    /**
     * The stored file name.
     */
    Filename?: string;
    /**
     * The original file name.
     */
    OriginalName?: string;
}

/**
 * Options for creating an upload input.
 */
export interface UploadInputOptions {
    /**
     * The container element to add the input to.
     */
    container?: HTMLElement | ArrayLike<HTMLElement>;
    /**
     * The drop zone element.
     */
    zone?: HTMLElement | ArrayLike<HTMLElement>;
    /**
     * The progress element.
     */
    progress?: HTMLElement | ArrayLike<HTMLElement>;
    /**
     * The name of the input element.
     */
    inputName?: string;
    /**
     * Whether multiple files may be selected.
     */
    allowMultiple?: boolean;
    /**
     * An optional upload intent appended to the upload URL.
     */
    uploadIntent?: string;
    /**
     * The upload URL. Defaults to the temporary upload endpoint.
     */
    uploadUrl?: string;
    /**
     * Callback invoked when a file upload completes.
     */
    fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}

/**
 * The response returned by a file upload service.
 */
export interface UploadResponse extends ServiceResponse {
    /**
     * The temporary file name.
     */
    TemporaryFile: string;
    /**
     * The file size in bytes.
     */
    Size: number;
    /**
     * Whether the file is an image.
     */
    IsImage: boolean;
    /**
     * The image width.
     */
    Width: number;
    /**
     * The image height.
     */
    Height: number;
}

/**
 * Constraints for validating uploaded files.
 */
export interface FileUploadConstraints {
    /**
     * The minimum image width.
     */
    minWidth?: number;
    /**
     * The maximum image width.
     */
    maxWidth?: number;
    /**
     * The minimum image height.
     */
    minHeight?: number;
    /**
     * The maximum image height.
     */
    maxHeight?: number;
    /**
     * The minimum file size in bytes.
     */
    minSize?: number;
    /**
     * The maximum file size in bytes.
     */
    maxSize?: number;
    /**
     * Whether non-image files are allowed.
     */
    allowNonImage?: boolean;
    /**
     * The name of the property holding the original file name.
     */
    originalNameProperty?: string;
}

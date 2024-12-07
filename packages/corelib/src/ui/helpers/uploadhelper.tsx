import { Fluent, ServiceResponse, Uploader, blockUI, blockUndo, getjQuery, htmlEncode, isArrayLike, localText, notifyError, resolveUrl, round, stringFormat } from "../../base";
import { replaceAll } from "../../compat";

export namespace UploadHelper {

    export function addUploadInput(options: UploadInputOptions): Fluent {
        let container = isArrayLike(options.container) ? options.container[0] : options.container;
        let progress = Fluent(isArrayLike(options.progress) ? options.progress[0] : options.progress);
        var button = container.closest(".tool-button") ?? container.closest("button") ?? container;
        button.classList.add("fileinput-button");

        var uploadUrl = options.uploadUrl || '~/File/TemporaryUpload';
        if (options.uploadIntent) {
            if (uploadUrl.indexOf('?') < 0)
                uploadUrl += "?"
            else
                uploadUrl += "&";
            uploadUrl += "uploadIntent=";
            uploadUrl += encodeURIComponent(options.uploadIntent);
        }

        var uploadInput = Fluent(<input type="file" name={options.inputName + '[]'} data-url={resolveUrl(uploadUrl)} multiple={!!options.allowMultiple} />)
            .appendTo(container);

        const setProgress = (percent: number) => {
            let bar = progress.children()[0];
            bar && (bar.style.width = (percent ?? 0).toString() + '%');
        }

        var uploader = new Uploader({
            batchSize: 1,
            batchSuccess: data => {
                const response: UploadResponse = data.response ?? {};
                if (response?.Error) {
                    notifyError(response.Error.Message);
                    return;
                }
                options.fileDone?.(response, data.batch?.filePaths?.[0], data);
            },
            input: uploadInput.getNode() as HTMLInputElement,
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

        return uploadInput;
    }

    export function checkImageConstraints(file: UploadResponse,
        opt: FileUploadConstraints): boolean {

        if (!file.IsImage && !opt.allowNonImage) {
            notifyError(localText('Controls.ImageUpload.NotAnImageFile'));
            return false;
        }
        if (opt.minSize > 0 && file.Size < opt.minSize) {
            notifyError(stringFormat(localText('Controls.ImageUpload.UploadFileTooSmall'),
                fileSizeDisplay(opt.minSize)));
            return false;
        }
        if (opt.maxSize > 0 && file.Size > opt.maxSize) {
            notifyError(stringFormat(localText('Controls.ImageUpload.UploadFileTooBig'),
                fileSizeDisplay(opt.maxSize)));
            return false;
        }
        if (!file.IsImage) {
            return true;
        }
        if (opt.minWidth > 0 && file.Width < opt.minWidth) {
            notifyError(stringFormat(localText('Controls.ImageUpload.MinWidth'), opt.minWidth));
            return false;
        }
        if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
            notifyError(stringFormat(localText('Controls.ImageUpload.MaxWidth'), opt.maxWidth));
            return false;
        }
        if (opt.minHeight > 0 && file.Height < opt.minHeight) {
            notifyError(stringFormat(localText('Controls.ImageUpload.MinHeight'), opt.minHeight));
            return false;
        }
        if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
            notifyError(stringFormat(localText('Controls.ImageUpload.MaxHeight'), opt.maxHeight));
            return false;
        }
        return true;
    }

    export function fileNameSizeDisplay(name: string, bytes: number): string {
        return name + ' (' + fileSizeDisplay(bytes) + ')';
    }

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

    export function hasImageExtension(filename: string): boolean {
        if (!filename) {
            return false;
        }
        filename = filename.toLowerCase();
        return filename.endsWith('.jpg') || filename.endsWith('.jpeg') ||
            filename.endsWith('.gif') || filename.endsWith('.png') ||
            filename.endsWith('.webp');
    }

    export function thumbFileName(filename: string): string {
        filename = filename ?? '';
        var idx = filename.lastIndexOf('.');
        if (idx >= 0) {
            filename = filename.substring(0, idx);
        }
        return filename + '_t.jpg';
    }

    export function dbFileUrl(filename: string): string {
        filename = replaceAll(filename ?? '', '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    export function colorBox(link: HTMLElement | ArrayLike<HTMLElement>, options?: any): void {
        link = isArrayLike(link) ? link[0] : link;
        if (!link)
            return;
        let $ = getjQuery();
        if (!$)
            return;
        $(link).colorbox?.({
            current: htmlEncode(localText('Controls.ImageUpload.ColorboxCurrent')),
            previous: htmlEncode(localText('Controls.ImageUpload.ColorboxPrior')),
            next: htmlEncode(localText('Controls.ImageUpload.ColorboxNext')),
            close: htmlEncode(localText('Controls.ImageUpload.ColorboxClose'))
        });
    }

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
                colorBox(thumb, new Object());
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

export interface UploadedFile {
    Filename?: string;
    OriginalName?: string;
}

export interface UploadInputOptions {
    container?: HTMLElement | ArrayLike<HTMLElement>;
    zone?: HTMLElement | ArrayLike<HTMLElement>;
    progress?: HTMLElement | ArrayLike<HTMLElement>;
    inputName?: string;
    allowMultiple?: boolean;
    uploadIntent?: string;
    uploadUrl?: string;
    fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}

export interface UploadResponse extends ServiceResponse {
    TemporaryFile: string;
    Size: number;
    IsImage: boolean;
    Width: number;
    Height: number;
}

export interface FileUploadConstraints {
    minWidth?: number;
    maxWidth?: number;
    minHeight?: number;
    maxHeight?: number;
    minSize?: number;
    maxSize?: number;
    allowNonImage?: boolean;
    originalNameProperty?: string;
}

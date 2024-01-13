import { Fluent, ServiceResponse, Uploader, alertDialog, blockUI, blockUndo, getjQuery, htmlEncode, iframeDialog, isArrayLike, localText, notifyError, resolveUrl, round, stringFormat } from "@serenity-is/base";
import { replaceAll } from "../../q";

export namespace UploadHelper {

    export function addUploadInput(options: UploadInputOptions): Fluent {
        let container = isArrayLike(options.container) ? options.container[0] : options.container;
        let progress = isArrayLike(options.progress) ? options.progress[0] : options.progress;
        //container.classList.add('fileinput-button');
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

        var uploadInput = Fluent("input").attr('type', 'file')
            .attr('name', options.inputName + '[]')
            .attr('data-url', resolveUrl(uploadUrl))
            .appendTo(container);

        if (options.allowMultiple) {
            uploadInput.attr('multiple', 'multiple');
        }

        let $ = getjQuery();

        if (!$?.fn?.fileupload) {
            new Uploader({
                callback: (e, fd, files ) => console.log(files),
                input: uploadInput.getNode() as HTMLInputElement,
                dropZone: options.zone
            });
            return uploadInput;
        }

        $ && $(uploadInput).fileupload?.({
            dataType: 'json',
            dropZone: options.zone,
            pasteZone: options.zone,
            done: function (e: Event, data: any) {
                var response = data.result;
                if (response.Error) {
                    notifyError(response.Error.Message);
                    return;
                } 

                if (options.fileDone != null) {
                    options.fileDone(response, data.files[0].name, data);
                }
            },
            fail: function(e: Event, opt: any) {
                var xhr = opt?._response?.jqXHR;
                if (!xhr) {
                    notifyError('An error occurred during file upload.');
                    return;
                }

                if ((xhr.getResponseHeader('content-type') || '')
                    .toLowerCase().indexOf('application/json') >= 0) {
                    var json = JSON.parse(xhr.responseText);
                    if (json && json.Error && json.Error.Message) {
                        notifyError(json.Error.Message);
                        return;
                    }
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
            },
            start: function () {
                blockUI(null);
                if (progress) {
                    Fluent(progress).show();
                }
            },
            stop: function () {
                blockUndo();
                if (progress) {
                    Fluent(progress).hide();
                }
            },
            progress: function (e: Event, data1: any) {
                if (progress) {
                    var percent = data1.loaded / data1.total * 100;
                    var bar = progress.firstElementChild as HTMLElement;
                    bar && (bar.style.width = percent.toString() + '%');
                }
            }
        });
        return uploadInput;
    }

    export function checkImageConstraints(file: UploadResponse,
        opt: FileUploadConstraints): boolean {

        if (!file.IsImage && !opt.allowNonImage) {
            alertDialog(localText('Controls.ImageUpload.NotAnImageFile'));
            return false;
        }
        if (opt.minSize > 0 && file.Size < opt.minSize) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.UploadFileTooSmall'),
                fileSizeDisplay(opt.minSize)));
            return false;
        }
        if (opt.maxSize > 0 && file.Size > opt.maxSize) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.UploadFileTooBig'),
                fileSizeDisplay(opt.maxSize)));
            return false;
        }
        if (!file.IsImage) {
            return true;
        }
        if (opt.minWidth > 0 && file.Width < opt.minWidth) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.MinWidth'), opt.minWidth));
            return false;
        }
        if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.MaxWidth'), opt.maxWidth));
            return false;
        }
        if (opt.minHeight > 0 && file.Height < opt.minHeight) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.MinHeight'), opt.minHeight));
            return false;
        }
        if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
            alertDialog(stringFormat(localText('Controls.ImageUpload.MaxHeight'), opt.maxHeight));
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
            value = sizeParts[0] + '.' + sizeParts[1].substr(0, 2);
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
            filename = filename.substr(0, idx);
        }
        return filename + '_t.jpg';
    }

    export function dbFileUrl(filename: string): string {
        filename = replaceAll(filename ?? '', '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    export function colorBox(link: HTMLElement | ArrayLike<HTMLElement>, options: any): void {
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
            var item = items[index];
            var li = Fluent("li").addClass('file-item').data('index', index.toString());
            var isImage = hasImageExtension(item.Filename);
            if (isImage) {
                li.addClass('file-image');
            }
            else {
                li.addClass('file-binary');
            }
            var thumb = Fluent("a").addClass('thumb').appendTo(li);
            var originalName = item.OriginalName ?? '';
            var fileName = item.Filename;
            if (urlPrefix != null && fileName != null &&
                !fileName.startsWith('temporary/')) {
                fileName = urlPrefix + fileName;
            }

            thumb.attr('href', dbFileUrl(fileName));
            thumb.attr('target', '_blank');
            if (originalName) {
                thumb.attr('title', originalName);
            }

            if (isImage) {
                thumb.getNode().style.backgroundImage = "url('" + dbFileUrl(thumbFileName(item.Filename)) + "')";
                colorBox(thumb, new Object());
            }

            if (displayOriginalName) {
                Fluent("div").addClass('filename').text(originalName)
                    .attr('title', originalName).appendTo(li);
            }

            li.appendTo(container);
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

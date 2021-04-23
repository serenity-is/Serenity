import { notifyError } from "../../Q";
import { blockUI, blockUndo } from "../../Q/BlockUI";
import { format, round } from "../../Q/Formatting";
import { text } from "../../Q/LocalText";
import { resolveUrl } from "../../Q/Services";
import { endsWith, isEmptyOrNull, replaceAll, startsWith } from "../../Q/Strings";

export namespace UploadHelper {

    export function addUploadInput(options: UploadInputOptions): JQuery {
        options.container.addClass('fileinput-button');
        var uploadInput = $('<input/>').attr('type', 'file')
            .attr('name', options.inputName + '[]')
            .attr('data-url', resolveUrl('~/File/TemporaryUpload'))
            .attr('multiple', 'multiple').appendTo(options.container);

        if (options.allowMultiple) {
            uploadInput.attr('multiple', 'multiple');
        }

        (uploadInput as any).fileupload({
            dataType: 'json',
            dropZone: options.zone,
            pasteZone: options.zone,
            done: function (e: JQueryEventObject, data: any) {
                var response = data.result;
                if (!!response.Error) {
                    notifyError(response.Error.Message);
                } else {
                    if (options.fileDone != null) {
                        options.fileDone(response, data.files[0].name, data);
                    }
                }
            },
            start: function () {
                blockUI(null);
                if (options.progress != null) {
                    options.progress.show();
                }
            },
            stop: function () {
                blockUndo();
                if (options.progress != null) {
                    options.progress.hide();
                }
            },
            progress: function (e1: JQueryEventObject, data1: any) {
                if (options.progress != null) {
                    var percent = data1.loaded / data1.total * 100;
                    options.progress.children().css('width', percent.toString() + '%');
                }
            }
        });
        return uploadInput;
    }

    export function checkImageConstraints(file: UploadResponse,
        opt: FileUploadConstraints): boolean {

        if (!file.IsImage && !opt.allowNonImage) {
            alert(text('Controls.ImageUpload.NotAnImageFile'));
            return false;
        }
        if (opt.minSize > 0 && file.Size < opt.minSize) {
            alert(format(text('Controls.ImageUpload.UploadFileTooSmall'),
                fileSizeDisplay(opt.minSize)));
            return false;
        }
        if (opt.maxSize > 0 && file.Size > opt.maxSize) {
            alert(format(text('Controls.ImageUpload.UploadFileTooBig'),
                fileSizeDisplay(opt.maxSize)));
            return false;
        }
        if (!file.IsImage) {
            return true;
        }
        if (opt.minWidth > 0 && file.Width < opt.minWidth) {
            alert(format(text('Controls.ImageUpload.MinWidth'), opt.minWidth));
            return false;
        }
        if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
            alert(format(text('Controls.ImageUpload.MaxWidth'), opt.maxWidth));
            return false;
        }
        if (opt.minHeight > 0 && file.Height < opt.minHeight) {
            alert(format(text('Controls.ImageUpload.MinHeight'), opt.minHeight));
            return false;
        }
        if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
            alert(format(text('Controls.ImageUpload.MaxHeight'), opt.maxHeight));
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
        if (isEmptyOrNull(filename)) {
            return false;
        }
        filename = filename.toLowerCase();
        return endsWith(filename, '.jpg') || endsWith(filename, '.jpeg') ||
            endsWith(filename, '.gif') || endsWith(filename, '.png');
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

    export function colorBox(link: JQuery, options: any): void {
        (link as any).colorbox({
            current: text('Controls.ImageUpload.ColorboxCurrent'),
            previous: text('Controls.ImageUpload.ColorboxPrior'),
            next: text('Controls.ImageUpload.ColorboxNext'),
            close: text('Controls.ImageUpload.ColorboxClose')
        });
    }

    export function populateFileSymbols(container: JQuery, items: UploadedFile[],
        displayOriginalName?: boolean, urlPrefix?: string): void {

        items = items || [];
        container.html('');
        for (var index = 0; index < items.length; index++) {
            var item = items[index];
            var li = $('<li/>').addClass('file-item').data('index', index);
            var isImage = hasImageExtension(item.Filename);
            if (isImage) {
                li.addClass('file-image');
            }
            else {
                li.addClass('file-binary');
            }
            var editLink = '#' + index;
            var thumb = $('<a/>').addClass('thumb').appendTo(li);
            var originalName = item.OriginalName ?? '';
            var fileName = item.Filename;
            if (urlPrefix != null && fileName != null &&
                !startsWith(fileName, 'temporary/')) {
                fileName = urlPrefix + fileName;
            }

            thumb.attr('href', dbFileUrl(fileName));
            thumb.attr('target', '_blank');
            if (!isEmptyOrNull(originalName)) {
                thumb.attr('title', originalName);
            }

            if (isImage) {
                thumb.css('backgroundImage', "url('" + dbFileUrl(
                    thumbFileName(item.Filename)) + "')");
                colorBox(thumb, new Object());
            }

            if (displayOriginalName) {
                $('<div/>').addClass('filename').text(originalName)
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
    container?: JQuery;
    zone?: JQuery;
    progress?: JQuery;
    inputName?: string;
    allowMultiple?: boolean;
    fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}

export interface UploadResponse {
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

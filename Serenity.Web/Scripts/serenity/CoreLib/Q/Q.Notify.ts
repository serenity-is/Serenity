/// <reference path="../../../typings/toastr/toastr.d.ts" />

namespace Q {
    export let defaultNotifyOptions: ToastrOptions = {
        timeOut: 3000,
        showDuration: 250,
        hideDuration: 500,
        extendedTimeOut: 500,
        positionClass: 'toast-top-full-width'
    }

    function getToastrOptions(options: ToastrOptions) {
        options = $.extend<ToastrOptions>({}, defaultNotifyOptions, options);
        positionToastContainer(true);
        return options;
    }

    export function notifyWarning(message: string, title?: string, options?: ToastrOptions): void {
        toastr.warning(message, title, getToastrOptions(options));
    }

    export function notifySuccess(message: string, title?: string, options?: ToastrOptions): void {
        toastr.success(message, title, getToastrOptions(options));
    }

    export function notifyInfo(message: string, title?: string, options?: ToastrOptions): void {
        toastr.info(message, title, getToastrOptions(options));
    }

    export function notifyError(message: string, title?: string, options?: ToastrOptions): void {
        toastr.error(message, title, getToastrOptions(options));
    }

    export function positionToastContainer(create: boolean) {
        if (typeof toastr === 'undefined') {
            return;
        }

        var dialog = $(window.document.body).children('.ui-dialog:visible').last();
        var container = toastr.getContainer(null, create);
        if (container.length === 0) {
            return;
        }
        if (dialog.length > 0) {
            var position = dialog.position();
            container.addClass('positioned-toast toast-top-full-width');
            container.css({ position: 'absolute', top: position.top + 28 + 'px', left: position.left + 6 + 'px', width: dialog.width() - 12 + 'px' });
        }
        else {
            container.addClass('toast-top-full-width');
            if (container.hasClass('positioned-toast')) {
                container.removeClass('positioned-toast');
                container.css({ position: '', top: '', left: '', width: '' });
            }
        }
    }
}

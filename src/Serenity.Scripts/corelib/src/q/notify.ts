import { extend } from "./system";

export let defaultNotifyOptions: ToastrOptions = {
    timeOut: 3000,
    showDuration: 250,
    hideDuration: 500,
    extendedTimeOut: 500,
    positionClass: 'position-toast toast-top-full-width'
}

function getToastrOptions(options: ToastrOptions) {
    options = extend(extend({}, defaultNotifyOptions), options);
    positionToastContainer(true, options);
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

export function positionToastContainer(create: boolean, options?: ToastrOptions) {
    if (typeof toastr === 'undefined') {
        return;
    }

    var container = toastr.getContainer(options, create);
    if (!container.length || !container.hasClass('position-toast'))
        return;

    var dialog = $(window.document.body).children('.ui-dialog:visible, .modal.in, .modal.show').last();
    if (dialog.length > 0) {
        var position = dialog.position();
        container.addClass('positioned-toast');
        container.css({ position: 'absolute', top: position.top + 28 + 'px', left: position.left + 6 + 'px', width: dialog.width() - 12 + 'px' });
    }
    else {
        if (container.hasClass('positioned-toast')) {
            container.removeClass('positioned-toast');
            container.css({ position: '', top: '', left: '', width: '' });
        }
    }
}
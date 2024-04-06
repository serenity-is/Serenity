import toastr, { type ToastrOptions } from "./toastr2";

export let defaultNotifyOptions: ToastrOptions = {
    timeOut: 3000,
    showDuration: 250,
    hideDuration: 500,
    escapeHtml: true,
    extendedTimeOut: 500,
    positionClass: 'position-toast toast-top-full-width'
}

export function positionToastContainer(options?: ToastrOptions, create = true) {
    let container = toastr.getContainer(options, create);
    if (!container || !container.classList.contains('position-toast') || typeof document === "undefined" || !document.body)
        return;

    let dialogs = Array.from(document.body.children);
    let dialogIndex =  dialogs.findIndex(x => x.matches('.ui-dialog, .modal.in, .modal.show') && !x.matches('[style*="display:none"], [style*="display: none"], .hidden'));
    let dialog = dialogs[dialogIndex];
    if (dialog) {
        const { top, left, right } = dialog.getBoundingClientRect();
        container.classList.add('positioned-toast');
        container.style.position = 'absolute';
        container.style.top = top + 28 + 'px';
        container.style.left = left + 6 + 'px';
        container.style.width = Math.max(((right - left) - 12), 150) + 'px';
    }
    else if (container.classList.contains('positioned-toast')) {
        container.classList.remove('positioned-toast');
        container.style.position = '';
        container.style.top = '';
        container.style.left = '';
        container.style.width = '';
    }
}

function getToastrOptions(options: ToastrOptions) {
    options = Object.assign(Object.assign({}, defaultNotifyOptions), options);
    positionToastContainer(options);
    return options;
}

function showToast(type: "error" | "info" | "success" | "warning", message: string, title?: string, options?: ToastrOptions) {
    return toastr[type](message, title, getToastrOptions(options));
}

export function notifyError(message: string, title?: string, options?: ToastrOptions): void {
    showToast('error', message, title, options);
}

export function notifyInfo(message: string, title?: string, options?: ToastrOptions): void {
    showToast('info', message, title, options);
}

export function notifySuccess(message: string, title?: string, options?: ToastrOptions): void {
    showToast('success', message, title, options);
}

export function notifyWarning(message: string, title?: string, options?: ToastrOptions): void {
    showToast('warning', message, title, options);
}
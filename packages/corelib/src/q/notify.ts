import { htmlEncode } from "./html";
import { localText } from "./localtext";
import { extend } from "./system";
import initialInstance, { ToastrOptions } from "./toastr2";

// set default for toastr itself, possible security issue
if (typeof initialInstance !== "undefined" && initialInstance.options)
    initialInstance.options.escapeHtml = true;

export let defaultNotifyOptions: ToastrOptions = {
    timeOut: 3000,
    showDuration: 250,
    hideDuration: 500,
    escapeHtml: true,
    extendedTimeOut: 500,
    positionClass: 'position-toast toast-top-full-width'
}

function getToastrOptions(options: ToastrOptions) {
    options = extend(extend({}, defaultNotifyOptions), options);
    positionToastContainer(true, options);
    return options;
}

function showToast(type: string, message: string, title?: string, options?: ToastrOptions) {
    if (typeof initialInstance !== "undefined" && (typeof (initialInstance as any)[type]) === 'function') {
        (initialInstance as any)[type](message, title, getToastrOptions(options));
        return;
    }

    if (typeof document === "undefined" || !document.body) {
        console.error(`no document found! can't show toast: ${type} - ${title}: ${message}`);
        return;
    }

    var container = document.getElementById('toast-container') as HTMLDivElement;
    if (!container) {
        container = document.createElement('div');
        container.id = "toast-container"
        container.className = "toast-container top-0 end-0 p-3";
        document.body.append(container);

        var toast = document.createElement('div');
        toast.className = "toast";
        toast.role = "alert";
        toast.ariaLive = "assertive";
        toast.ariaAtomic = "true";
        toast.innerHTML = `<div class="toast-header bg-${htmlEncode(type)} text-${htmlEncode(type)}">
          <strong class="me-auto">${htmlEncode(title)}</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="${htmlEncode(localText("Dialogs.CloseButton"))}"></button>
        </div>
        <div class="toast-body">
        ${htmlEncode(message)}
        </div>
      </div>`;
        container.appendChild(toast);

        // @ts-ignore
        if (typeof bootstrap === "undefined") {
            toast.querySelector('.btn-close').addEventListener('click', function(this: HTMLButtonElement) { this.remove(); });
            setTimeout(function() { this.remove(); }, 10000);
        }
    }
}

export function notifyWarning(message: string, title?: string, options?: ToastrOptions): void {
    showToast('warning', message, title, getToastrOptions(options));
}

export function notifySuccess(message: string, title?: string, options?: ToastrOptions): void {
    showToast('success', message, title, getToastrOptions(options));
}

export function notifyInfo(message: string, title?: string, options?: ToastrOptions): void {
    showToast('info', message, title, getToastrOptions(options));
}

export function notifyError(message: string, title?: string, options?: ToastrOptions): void {
    showToast('error', message, title, getToastrOptions(options));
}

export function positionToastContainer(create: boolean, options?: ToastrOptions) {
    if (typeof initialInstance === 'undefined') {
        return;
    }

    var container = initialInstance.getContainer(options, create);
    if (!container || !container.classList.contains('position-toast'))
        return;

    var dialog = typeof $ !== "undefined" ? $(window.document.body).children('.ui-dialog:visible, .modal.in, .modal.show').last() : [] as any;
    if (dialog.length > 0) {
        var position = dialog.position();
        container.classList.add('positioned-toast');
        container.style.position = 'absolute';
        container.style.top = position.top + 28 + 'px';
        container.style.left = position.left + 6 + 'px';
        container.style.width = dialog.width() - 12 + 'px';
    }
    else if (container.classList.contains('positioned-toast')) {
        container.classList.remove('positioned-toast');
        container.style.position = '';
        container.style.top = '';
        container.style.left = '';
        container.style.width = '';
    }
}
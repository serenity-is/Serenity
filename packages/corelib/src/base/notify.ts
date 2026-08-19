import { RenderableContent } from "./html";
import toastr, { type ToastrOptions } from "./toastr2";

/**
 * Default options applied to every toast notification.
 * Individual calls may override these via their `options` argument.
 * @remarks Mutate this object to change application-wide notification defaults (e.g. timeout or position).
 * @example
 * ```ts
 * defaultNotifyOptions.timeOut = 3000;
 * defaultNotifyOptions.positionClass = "toast-top-right";
 * ```
 */
export let defaultNotifyOptions: ToastrOptions = {
    timeOut: 5000,
    escapeHtml: true,
    positionClass: 'position-toast toast-top-full-width'
}

/**
 * Positions the toast container relative to the topmost visible dialog, if any.
 * When a `.ui-dialog` / `.modal.in` / `.modal.show` element is found, the container is absolutely positioned just below it; otherwise any previous absolute positioning is cleared.
 * @param options - Toastr options used to locate the container (forwarded to `toastr.getContainer`).
 * @param create - Whether to create the container if it does not yet exist. Defaults to `true`.
 * @remarks No-ops if the container has no `position-toast` class, or if `document`/`document.body` is unavailable (e.g. SSR).
 */
export function positionToastContainer(options?: ToastrOptions, create = true) {
    let container = toastr.getContainer(options, create);
    if (!container || !container.classList.contains('position-toast') || typeof document === "undefined" || !document.body)
        return;

    let dialogs = Array.from(document.body.children);
    let dialogIndex = dialogs.findIndex(x => x.matches('.ui-dialog, .modal.in, .modal.show') && !x.matches('[hidden], [style*="display:none"], [style*="display: none"]') && !x.classList.contains('hidden'));
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

function showToast(type: "error" | "info" | "success" | "warning", message: RenderableContent, title?: RenderableContent, options?: ToastrOptions) {
    return toastr[type](message, title, getToastrOptions(options));
}

/**
 * Shows an error toast notification.
 * @param message - Main content of the toast. Accepts a plain string or {@link RenderableContent} (DOM nodes/fragments are handled by the underlying toastr renderer).
 * @param title - Optional title/header displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}. Use to customize timeout, position, or `escapeHtml` for this toast only.
 * @example
 * ```ts
 * notifyError("Failed to save record.", "Error");
 * ```
 */
export function notifyError(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void {
    showToast('error', message, title, options);
}

/**
 * Shows an informational toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifyInfo("Your changes were saved.");
 * ```
 */
export function notifyInfo(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void {
    showToast('info', message, title, options);
}

/**
 * Shows a success toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifySuccess("Record created successfully.", "Done");
 * ```
 */
export function notifySuccess(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void {
    showToast('success', message, title, options);
}

/**
 * Shows a warning toast notification.
 * @param message - Main content of the toast.
 * @param title - Optional title displayed above the message.
 * @param options - Per-call toastr overrides merged over {@link defaultNotifyOptions}.
 * @example
 * ```ts
 * notifyWarning("Some fields are missing.", "Warning");
 * ```
 */
export function notifyWarning(message: RenderableContent, title?: RenderableContent, options?: ToastrOptions): void {
    showToast('warning', message, title, options);
}
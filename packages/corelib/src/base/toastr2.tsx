import { addClass, type RenderableContent, sanitizeHtml } from "./html";

// adapted from https://github.com/JPeer264/toastr2
/**
 * Options that control the toast container element.
 * Shared by individual toast calls and the global {@link Toastr} defaults.
 */
export type ToastContainerOptions = {
    /** DOM id for the container that holds toasts. Defaults to `"toast-container"`. */
    containerId?: string;
    /** CSS class applied to the container for positioning (e.g. `"toast-top-right"`). */
    positionClass?: string;
    /** CSS selector for the parent element the container is appended to. Defaults to `"body"`. */
    target?: string;
}

/**
 * Full option set for a toast notification. Extends {@link ToastContainerOptions}
 * with display, timing, styling, and lifecycle callbacks.
 */
export type ToastrOptions = ToastContainerOptions & {
    /** Show a close button, default is false. Pass an HTMLElement for a custom button element. */
    closeButton?: boolean | HTMLElement;
    /** CSS class for the close button. Defaults to `"toast-close-button"`. */
    closeClass?: string;
    /** If `true` (default) the toast stays open while hovered and closes after {@link ToastrOptions.extendedTimeOut} when the mouse leaves. */
    closeOnHover?: boolean;
    /** Timeout in ms after mouse-leave before the toast closes when {@link ToastrOptions.closeOnHover} is enabled. Defaults to `1000`. */
    extendedTimeOut?: number;
    /** @deprecated Escape message html, default is true. Pass an HTML element to message instead. */
    escapeHtml?: boolean;
    /** CSS class for the toast icon (e.g. `"toast-info"`, `"toast-error"`). */
    iconClass?: string;
    /** CSS class for the message element. Defaults to `"toast-message"`. */
    messageClass?: string;
    /** When `true` the newest toast is inserted at the top of the container. */
    newestOnTop?: boolean;
    /** CSS class for toast positioning (also on container). Defaults to `"toast-top-right"`. */
    positionClass?: string;
    /** When `true` suppresses consecutive toasts with identical messages. Defaults to `false`. */
    preventDuplicates?: boolean;
    /** When `true` the toast message element is styled with `white-space: pre-wrap`. */
    preWrap?: boolean;
    /** Enables right-to-left layout for the toast. */
    rtl?: boolean;
    /** CSS selector for the parent element that hosts the container. Defaults to `"body"`. */
    target?: string;
    /** Duration in ms the toast stays visible. Set to `0` for sticky or `-1` to disable auto-hide (extended timeout is then ignored). Defaults to `5000`. */
    timeOut?: number;
    /** CSS class for the toast element itself. Defaults to `"toast"`. */
    toastClass?: string;
    /** When `true` (default) clicking the toast dismisses it. */
    tapToDismiss?: boolean;
    /** CSS class for the title element. Defaults to `"toast-title"`. */
    titleClass?: string;

    /** Callback invoked when the toast element is clicked. */
    onclick?: (event: MouseEvent) => void;
    /** Callback invoked when the close button is clicked. */
    onCloseClick?: (event: Event) => void;
    /** Callback invoked after the toast is hidden and removed. */
    onHidden?: () => void;
    /** Callback invoked after the toast is shown. */
    onShown?: () => void;
}

/**
 * Internal descriptor for a toast notification passed to {@link Toastr.notify}.
 */
export type NotifyMap = {
    /** Toast type key (`"success"` | `"info"` | `"warning"` | `"error"`). */
    type: string;
    /** CSS class for the toast icon corresponding to the type. */
    iconClass: string;
    /** Optional title content for the toast. */
    title?: RenderableContent;
    /** Optional message content for the toast. */
    message?: RenderableContent;
}

const initialOptions: ToastrOptions = {
    closeButton: false,
    closeClass: 'toast-close-button',
    closeOnHover: true,
    containerId: 'toast-container',
    escapeHtml: true,
    extendedTimeOut: 1000,
    iconClass: 'toast-info',
    messageClass: 'toast-message',
    newestOnTop: true,
    onclick: () => { },
    onCloseClick: () => { },
    onHidden: () => { },
    onShown: () => { },
    positionClass: 'toast-top-right',
    preventDuplicates: false,
    rtl: false,
    tapToDismiss: true,
    target: 'body',
    timeOut: 5000, // Set timeOut to 0 to make it sticky
    titleClass: 'toast-title',
    toastClass: 'toast'
}

let initialInstance: Toastr = null;

/**
 * Toast notification manager. Provides `success` / `info` / `warning` / `error`
 * helpers, container management, and duplicate suppression. A singleton instance
 * is exported as the default export; custom instances can be constructed with
 * overriding {@link ToastrOptions}.
 */
export class Toastr {
    declare private listener: any;

    declare private toastId;

    declare private previousToast: RenderableContent | null;

    /** Effective options for this instance (merged from defaults and constructor overrides). */
    declare public options: ToastrOptions;

    /**
     * Creates a new Toastr instance.
     * @param options - Options merged over the global defaults / parent instance options.
     */
    public constructor(options?: ToastrOptions) {
        this.toastId = 0;
        this.previousToast = null;
        this.options = Object.assign(Object.assign({}, initialInstance?.options ?? initialOptions), options);
    }

    /**
     * Gets the toast container element, optionally creating it.
     * @param options - Container options that override instance defaults when resolving `containerId` / `target` / `positionClass`.
     * @param create - When `true` creates the container if it does not exist.
     * @returns The container element, or `null` if not found and `create` is `false`.
     */
    public getContainer(options?: ToastContainerOptions, create = false): HTMLElement {
        let container = document.getElementById(options?.containerId ?? this.options.containerId) as HTMLElement;
        if (container || !create)
            return container;

        container = <div id={this.options.containerId} class={options?.positionClass ?? this.options.positionClass}></div> as HTMLElement;
        document.querySelector(options?.target ?? this.options.target)?.appendChild(container);
        return container;
    }

    /**
     * Shows an error toast.
     * @param message - Message content for the toast.
     * @param title - Optional title content.
     * @param opt - Per-toast options that override instance defaults.
     * @returns The created toast element, or `null` if suppressed as a duplicate.
     */
    public error(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'error',
            iconClass: 'toast-error',
            message,
            title,
        }, opt);
    }

    /**
     * Shows a warning toast.
     * @param message - Message content for the toast.
     * @param title - Optional title content.
     * @param opt - Per-toast options that override instance defaults.
     * @returns The created toast element, or `null` if suppressed as a duplicate.
     */
    public warning(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'warning',
            iconClass: 'toast-warning',
            message,
            title,
        }, opt);
    }

    /**
     * Shows a success toast.
     * @param message - Message content for the toast.
     * @param title - Optional title content.
     * @param opt - Per-toast options that override instance defaults.
     * @returns The created toast element, or `null` if suppressed as a duplicate.
     */
    public success(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'success',
            iconClass: 'toast-success',
            message,
            title,
        }, opt);
    }

    /**
     * Shows an info toast.
     * @param message - Message content for the toast.
     * @param title - Optional title content.
     * @param opt - Per-toast options that override instance defaults.
     * @returns The created toast element, or `null` if suppressed as a duplicate.
     */
    public info(
        message?: RenderableContent,
        title?: RenderableContent,
        opt?: ToastrOptions,
    ): HTMLElement | null {
        return this.notify({
            type: 'info',
            iconClass: 'toast-info',
            message,
            title,
        }, opt);
    }

    /**
     * Subscribes to toast lifecycle events.
     * @param callback - Function invoked with toast state on show / hide.
     */
    public subscribe(callback: (response: Toastr) => void): void {
        this.listener = callback;
    }

    /**
     * Publishes a toast lifecycle event to the subscriber.
     * @param args - Toast state payload.
     */
    public publish(args: Toastr): void {
        if (!this.listener) {
            return;
        }

        this.listener(args);
    }

    private removeContainerIfEmpty(options?: ToastrOptions) {
        let container = this.getContainer(options);
        if (!container)
            return;
        if (!container.hasChildNodes?.() && container.parentNode)
            container.parentNode.removeChild(container);
    }

    /**
     * Removes a single toast element from the DOM and cleans up the container if empty.
     * @param toastElement - The toast element to remove.
     * @param options - Optional container options used to locate the container for cleanup.
     */
    public removeToast(toastElement: HTMLElement, options?: ToastContainerOptions) {
        if (!toastElement)
            return;

        if (toastElement !== document.activeElement) {
            toastElement.parentNode?.removeChild(toastElement);
            this.previousToast = null;
            this.removeContainerIfEmpty(options);
        }
    }

    /**
     * Clears all toasts from the container.
     * @param options - Optional container options to resolve which container to clear.
     */
    public clear(options?: ToastContainerOptions) {
        let container = this.getContainer(options);
        if (!container)
            return;

        const toastsToClear = Array.from(container.childNodes) as HTMLElement[];
        for (let i = toastsToClear.length - 1; i >= 0; i -= 1)
            this.removeToast(toastsToClear[i], options);

        this.removeContainerIfEmpty();
    }

    private notify(map: NotifyMap, opt: ToastrOptions): HTMLElement | null {
        opt = Object.assign(Object.assign(Object.assign({}, this.options), map), opt);

        const shouldExit = (opts: ToastrOptions, exitMap: NotifyMap): boolean => {
            if (opts.preventDuplicates) {
                if (exitMap.message === this.previousToast) {
                    return true;
                }

                this.previousToast = exitMap.message || '';
            }
            return false;
        };


        if (shouldExit(opt, map)) {
            return null;
        }

        this.toastId += 1;

        const container = this.getContainer(opt, true);

        let intervalId: number = null;
        const toastEl = <div/> as HTMLElement;
        const titleEl = <div/> as HTMLElement;
        const messageEl = <div/> as HTMLElement;
        const closeEl = !opt.closeButton ? null :
            opt.closeButton instanceof HTMLElement ? opt.closeButton : <button type="button">&times;</button> as HTMLElement;

        const response: any = {
            toastId: this.toastId,
            state: 'visible',
            startTime: new Date(),
            endTime: undefined,
            opt,
            map,
        };

        const hideToast = (override: any = null): void => {
            if (toastEl === document.activeElement && !override) {
                return;
            }

            this.removeToast(toastEl);

            if (intervalId) {
                clearTimeout(intervalId);
            }

            if (opt.onHidden && response.state !== 'hidden') {
                opt.onHidden();
            }

            response.state = 'hidden';
            response.endTime = new Date();
            this.publish(response);
        };

        const setAria = (): void => {
            let ariaValue = '';

            switch (opt.iconClass) {
                case 'toast-success':
                case 'toast-info':
                    ariaValue = 'polite';

                    break;

                default:
                    ariaValue = 'assertive';
            }

            toastEl.setAttribute('aria-live', ariaValue);
        };

        const handleEvents = (): void => {
            if (opt.closeOnHover) {

                toastEl.addEventListener('mouseover', () => {
                    if (intervalId) {
                        clearTimeout(intervalId);
                    }
                });

                toastEl.addEventListener('mouseout', () => {
                    if (opt.timeOut >= 0 && (opt.timeOut > 0 || opt.extendedTimeOut > 0)) {
                        intervalId = setTimeout(hideToast, opt.extendedTimeOut);
                    }
                });
            }

            if (!opt.onclick && opt.tapToDismiss) {
                toastEl.addEventListener('click', hideToast);
            }

            if (closeEl) {
                closeEl.addEventListener('click', (event) => {
                    event.stopPropagation();

                    if (opt.onCloseClick) {
                        opt.onCloseClick(event);
                    }

                    hideToast(true);
                });
            }

            if (opt.onclick) {
                toastEl.addEventListener('click', (event) => {
                    // ts needs another check here
                    if (opt.onclick) {
                        opt.onclick(event);
                    }

                    if (opt.tapToDismiss)
                        hideToast();
                });
            }
        };

        const setTitle = (): void => {
            if (map.title) {
                titleEl.append(map.title);
                addClass(titleEl, opt.titleClass);
                toastEl.appendChild(titleEl);
            }
        };

        const setMessage = (): void => {
            if (map.message) {
                if (typeof map.message === "string" && !((opt as any).escapeHtml ?? true)) {
                    messageEl.innerHTML = sanitizeHtml(map.message);
                }
                else {
                    messageEl.append(map.message);
                }
                if (opt.preWrap) {
                    messageEl.style.whiteSpace = "pre-wrap";
                }
                addClass(messageEl, opt.messageClass);
                toastEl.appendChild(messageEl);
            }
        };

        const setCloseButton = (): void => {
            if (closeEl) {
                addClass(closeEl, opt.closeClass);
                closeEl.setAttribute('role', 'button');
                toastEl.insertBefore(closeEl, toastEl.firstChild);
            }
        };

        const setSequence = (): void => {
            if (opt.newestOnTop) {
                container.insertBefore(toastEl, container.firstChild);
            } else {
                container.appendChild(toastEl);
            }
        };

        const displayToast = (): void => {
            if (opt.onShown) {
                opt.onShown();
            }

            if (opt.timeOut > 0) {
                intervalId = setTimeout(hideToast, opt.timeOut);

            }
        };

        const personalizeToast = (): void => {
            toastEl.classList.add('show');
            opt.rtl && toastEl.classList.add('rtl');
            opt.toastClass && addClass(toastEl, opt.toastClass);
            opt.iconClass && addClass(toastEl, opt.iconClass);
            setTitle();
            setMessage();
            setCloseButton();
            setSequence();
            setAria();
        };

        personalizeToast();
        displayToast();
        handleEvents();
        this.publish(response);
        return toastEl;
    }
}

initialInstance = new Toastr();

export default initialInstance;

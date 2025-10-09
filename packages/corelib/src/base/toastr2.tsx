import { addClass, type RenderableContent, sanitizeHtml } from "./html";

// adapted from https://github.com/JPeer264/toastr2
export type ToastContainerOptions = {
    containerId?: string;
    positionClass?: string;
    target?: string;
}

export type ToastrOptions = ToastContainerOptions & {
    /** Show a close button, default is false */
    closeButton?: boolean | HTMLElement;
    /** CSS class for close button */
    closeClass?: string;
    /** If true (default) toast keeps open when hovered, and closes after extendedTimeout when mouse leaves the toast */
    closeOnHover?: boolean;
    /** If closeOnHover is true, the toast closes in extendedTimeout duration after the mouse leaves the toast. Default is 1000 */
    extendedTimeOut?: number;
    /** @deprecated Escape message html, default is true. Pass HTML element to message instead */
    escapeHtml?: boolean;
    /** CSS class for icon */
    iconClass?: string;
    /** CSS class for message */
    messageClass?: string;
    /** Show newest on top */
    newestOnTop?: boolean;
    /** CSS class for toast positioning */
    positionClass?: string;
    /** Prevent duplicates of the same toast, default is false */
    preventDuplicates?: boolean;
    /** If true the toast message element will have a white-space: pre-wrap style */
    preWrap?: boolean;
    /** Right to left */
    rtl?: boolean;
    /** The container element id */
    target?: string;
    /** The duration for the toast to stay in the page. Set to -1 to make the toast sticky, in that case extendedTimeout is ignored. */
    timeOut?: number;
    /** CSS class for toast */
    toastClass?: string;
    /** Hides the notification when clicked, default is true */
    tapToDismiss?: boolean;
    /** CSS class for title */
    titleClass?: string;

    onclick?: (event: MouseEvent) => void;
    onCloseClick?: (event: Event) => void;
    onHidden?: () => void;
    onShown?: () => void;
}

export type NotifyMap = {
    type: string;
    iconClass: string;
    title?: RenderableContent;
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

export class Toastr {
    declare private listener: any;

    declare private toastId;

    declare private previousToast: RenderableContent | null;

    declare public options: ToastrOptions;

    public constructor(options?: ToastrOptions) {
        this.toastId = 0;
        this.previousToast = null;
        this.options = Object.assign(Object.assign({}, initialInstance?.options ?? initialOptions), options);
    }

    public getContainer(options?: ToastContainerOptions, create = false): HTMLElement {
        let container = document.getElementById(options?.containerId ?? this.options.containerId) as HTMLElement;
        if (container || !create)
            return container;

        container = <div id={this.options.containerId} class={options?.positionClass ?? this.options.positionClass}></div> as HTMLElement;
        document.querySelector(options?.target ?? this.options.target)?.appendChild(container);
        return container;
    }

    public error(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'error',
            iconClass: 'toast-error',
            message,
            title,
        }, opt);
    }

    public warning(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'warning',
            iconClass: 'toast-warning',
            message,
            title,
        }, opt);
    }

    public success(message?: RenderableContent, title?: RenderableContent, opt?: ToastrOptions): HTMLElement | null {
        return this.notify({
            type: 'success',
            iconClass: 'toast-success',
            message,
            title,
        }, opt);
    }

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

    public subscribe(callback: (response: Toastr) => void): void {
        this.listener = callback;
    }

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

    public removeToast(toastElement: HTMLElement, options?: ToastContainerOptions) {
        if (!toastElement)
            return;

        if (toastElement !== document.activeElement) {
            toastElement.parentNode?.removeChild(toastElement);
            this.previousToast = null;
            this.removeContainerIfEmpty(options);
        }
    }

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

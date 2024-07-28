import { addClass, htmlEncode } from "./html";

// adapted from https://github.com/JPeer264/toastr2
export type ToastContainerOptions = {
    containerId?: string;
    positionClass?: string;
    target?: string;
}

export type ToastrOptions = ToastContainerOptions & {
    /** Show a close button, default is false */
    closeButton?: boolean;
    /** CSS class for close button */
    closeClass?: string;
    /** Close button markup */
    closeHtml?: string;
    /** If true (default) toast keeps open when hovered, and closes after extendedTimeout when mouse leaves the toast */
    closeOnHover?: boolean;
    /** If closeOnHover is true, the toast closes in extendedTimeout duration after the mouse leaves the toast. Default is 1000 */
    extendedTimeOut?: number;
    /** Escape message html, default is true */
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
    title?: string;
    message?: string;
}

const initialOptions: ToastrOptions = {
    tapToDismiss: true,
    toastClass: 'toast',
    containerId: 'toast-container',
    onShown: () => { },
    onHidden: () => { },
    closeOnHover: true,
    extendedTimeOut: 1000,
    iconClass: 'toast-info',
    positionClass: 'toast-top-right',
    timeOut: 5000, // Set timeOut to 0 to make it sticky
    titleClass: 'toast-title',
    messageClass: 'toast-message',
    escapeHtml: true,
    target: 'body',
    closeHtml: '<button type="button">&times;</button>',
    closeClass: 'toast-close-button',
    newestOnTop: true,
    preventDuplicates: false,
    rtl: false,
    onCloseClick: () => { },
    closeButton: false,
    onclick: () => { },
}

var initialInstance: Toastr = null;

export class Toastr {
    declare private listener: any;

    declare private toastId;

    declare private previousToast: string | null;

    declare public options: ToastrOptions;

    public constructor(options?: ToastrOptions) {
        this.toastId = 0;
        this.previousToast = null;
        this.options = Object.assign(Object.assign({}, initialInstance?.options ?? initialOptions), options);
    }

    public getContainer(options?: ToastContainerOptions, create = false): HTMLElement {
        let container = document.getElementById(options?.containerId ?? this.options.containerId);
        if (container || !create)
            return container;

        container = document.createElement('div');

        container.setAttribute('id', this.options.containerId);
        let positionClass = options?.positionClass ?? this.options.positionClass;
        if (positionClass)
            addClass(container, positionClass);

        let targetSelector = options?.target ?? this.options.target;
        const target = document.querySelector(targetSelector);
        if (target)
            target.appendChild(container);
    
        return container;            
    }

    public error(
        message?: string,
        title?: string,
        opt?: ToastrOptions,
    ): HTMLElement | null {
        return this.notify({
            type: 'error',
            iconClass: 'toast-error',
            message,
            title,
        }, opt);
    }

    public warning(
        message?: string,
        title?: string,
        opt?: ToastrOptions,
    ): HTMLElement | null {
        return this.notify({
            type: 'warning',
            iconClass: 'toast-warning',
            message,
            title,
        }, opt);
    }

    public success(
        message?: string,
        title?: string,
        opt?: ToastrOptions,
    ): HTMLElement | null {
        return this.notify({
            type: 'success',
            iconClass: 'toast-success',
            message,
            title,
        }, opt);
    }

    public info(
        message?: string,
        title?: string,
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

        var container = this.getContainer(opt, true);

        let intervalId: number = null;
        const toastElement = document.createElement('div');
        const $titleElement = document.createElement('div');
        const $messageElement = document.createElement('div');
        const closeContainer = document.createElement('div');
        closeContainer.innerHTML = opt.closeHtml.trim();
        const closeElement = closeContainer.firstChild as HTMLElement | null;

        const response: any = {
            toastId: this.toastId,
            state: 'visible',
            startTime: new Date(),
            endTime: undefined,
            opt,
            map,
        };

        const hideToast = (override: any = null): void => {
            if (toastElement === document.activeElement && !override) {
                return;
            }

            this.removeToast(toastElement);

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

            toastElement.setAttribute('aria-live', ariaValue);
        };

        const handleEvents = (): void => {
            if (opt.closeOnHover) {

                toastElement.addEventListener('mouseover', () => { 
                    if (intervalId) {
                        clearTimeout(intervalId); 
                    }
                });

                toastElement.addEventListener('mouseout', () => { 
                    if (opt.timeOut >= 0 && (opt.timeOut > 0 || opt.extendedTimeOut > 0)) {
                        intervalId = setTimeout(hideToast, opt.extendedTimeOut);
                    }
                });
            }

            if (!opt.onclick && opt.tapToDismiss) {
                toastElement.addEventListener('click', hideToast);
            }

            if (opt.closeButton && closeElement) {
                closeElement.addEventListener('click', (event) => {
                    event.stopPropagation();

                    if (opt.onCloseClick) {
                        opt.onCloseClick(event);
                    }

                    hideToast(true);
                });
            }

            if (opt.onclick) {
                toastElement.addEventListener('click', (event) => {
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
                let suffix = map.title;
                if (opt.escapeHtml) {
                    suffix = htmlEncode(map.title);
                }
                $titleElement.innerHTML = suffix;
                addClass($titleElement, opt.titleClass);
                toastElement.appendChild($titleElement);
            }
        };

        const setMessage = (): void => {
            if (map.message) {
                let suffix = map.message;

                if (opt.escapeHtml) {
                    suffix = htmlEncode(map.message);
                }

                $messageElement.innerHTML = suffix;
                addClass($messageElement, opt.messageClass);
                toastElement.appendChild($messageElement);
            }
        };

        const setCloseButton = (): void => {
            if (opt.closeButton && closeElement) {
                addClass(closeElement, opt.closeClass);
                closeElement.setAttribute('role', 'button');
                toastElement.insertBefore(closeElement, toastElement.firstChild);
            }
        };

        const setSequence = (): void => {
            if (opt.newestOnTop) {
                container.insertBefore(toastElement, container.firstChild);
            } else {
                container.appendChild(toastElement);
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
            toastElement.classList.add('show');
            opt.rtl && toastElement.classList.add('rtl');
            opt.toastClass && addClass(toastElement, opt.toastClass);
            opt.iconClass && addClass(toastElement, opt.iconClass);
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
        return toastElement;
    }
}

initialInstance = new Toastr();

export default initialInstance;

// adapted from https://github.com/JPeer264/toastr2

import { htmlEncode, toggleClass } from "./html";

export type ToastContainerOptions = {
    containerId?: string;
    positionClass?: string;
    target?: string;
}

export type ToastrOptions = ToastContainerOptions & {
    tapToDismiss?: boolean;
    toastClass?: string;
    showDuration?: number;
    onShown?: () => void;
    hideDuration?: number;
    onHidden?: () => void;
    closeMethod?: boolean;
    closeDuration?: number | false;
    closeEasing?: boolean;
    closeOnHover?: boolean;
    extendedTimeOut?: number;
    iconClass?: string;
    positionClass?: string;
    timeOut?: number; // Set timeOut and extendedTimeOut to 0 to make it sticky
    titleClass?: string;
    messageClass?: string;
    escapeHtml?: boolean;
    target?: string;
    closeHtml?: string;
    closeClass?: string;
    newestOnTop?: boolean;
    preventDuplicates?: boolean;
    onclick?: (event: MouseEvent) => void;
    onCloseClick?: (event: Event) => void;
    closeButton?: boolean;
    rtl?: boolean;
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
    showDuration: 300,
    onShown: () => { },
    hideDuration: 1000,
    onHidden: () => { },
    closeMethod: false,
    closeDuration: false,
    closeEasing: false,
    closeOnHover: true,
    extendedTimeOut: 1000,
    iconClass: 'toast-info',
    positionClass: 'toast-top-right',
    timeOut: 5000, // Set timeOut and extendedTimeOut to 0 to make it sticky
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
    private listener: any;

    private toastId = 0;

    private previousToast: string | null = null;

    public options: ToastrOptions;

    public constructor(options?: ToastrOptions) {
        this.options = Object.assign(Object.assign({}, initialInstance?.options ?? initialOptions), options);
        this.createContainer(options);
    }

    private createContainer(options: ToastContainerOptions = this.options): HTMLElement {
        var container = document.createElement('div');

        container.setAttribute('id', options.containerId);
        if (options.positionClass)
            toggleClass(container, options.positionClass, true);

        const target = document.getElementsByTagName(options.target);

        if (target && target[0]) {
            target[0].appendChild(container);
        }

        return container;
    }

    public getContainer(options?: ToastContainerOptions, create = false): HTMLElement {
        const container = document.getElementById(options?.containerId ?? this.options.containerId);

        if (container)
            return container;

        if (create)
            return this.createContainer(options ?? this.options);

        return null;
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

    public clear(toastElement?: HTMLElement | null, clearOptions: { force?: boolean } = {}) {
        if (!this.clearToast(toastElement, this.options, clearOptions)) {
            this.clearContainer(this.options);
        }
    }

    public remove(toastElement?: HTMLElement | null) {
        var container = this.getContainer(this.options);
        if (!container)
            return;

        if (toastElement && toastElement !== document.activeElement) {
            this.removeToast(toastElement);

            return;
        }

        if (!container.hasChildNodes()) {
            const parentNode = container.parentElement;

            if (parentNode) {
                parentNode.removeChild(container);
            }
        }
    }

    public removeToast(toastElement: HTMLElement, options = this.options) {
        var container = this.getContainer(options);

        if (!container || !toastElement.parentNode) {
            return;
        }

        // todo set after visible state
        // as this will be a transition of css
        toastElement.parentNode.removeChild(toastElement);
        // check if visible
        if (toastElement.offsetWidth > 0 && toastElement.offsetHeight > 0) {
            return;
        }

        if (!container.hasChildNodes()) {
            if (container.parentNode) {
                container.parentNode.removeChild(container);
            }

            this.previousToast = null;
        }
    }

    private clearContainer(options: Partial<ToastrOptions> = this.options) {
        var container = this.getContainer(options);
        if (!container)
            return;

        const toastsToClear = Array.from(container.childNodes) as HTMLElement[];

        for (let i = toastsToClear.length - 1; i >= 0; i -= 1) {
            this.clearToast(toastsToClear[i]);
        }
    }

    private clearToast(
        toastElement?: HTMLElement | null,
        options?: ToastContainerOptions,
        clearOptions: { force?: boolean } = {},
    ): boolean {
        if (!toastElement) {
            return false;
        }

        const force = clearOptions.force || false;

        if (toastElement && (force || toastElement !== document.activeElement)) {
            this.removeToast(toastElement, options);
            return true;
        }

        return false;
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
        const createdElement = document.createElement('div');
        createdElement.innerHTML = opt.closeHtml.trim();
        const closeElement = createdElement.firstChild as HTMLElement | null;

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

        const delayedHideToast = (): void => {
            if (opt.timeOut > 0 || opt.extendedTimeOut > 0) {
                intervalId = setTimeout(hideToast, opt.extendedTimeOut);
            }
        };

        const stickAround = (): void => {
            if (intervalId) {
                clearTimeout(intervalId);
            }
        };

        const handleEvents = (): void => {
            if (opt.closeOnHover) {
                toastElement.addEventListener('mouseover', () => stickAround());
                toastElement.addEventListener('mouseout', () => delayedHideToast());
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
                toggleClass($titleElement, opt.titleClass, true);
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
                toggleClass($messageElement, opt.messageClass, true);
                toastElement.appendChild($messageElement);
            }
        };

        const setCloseButton = (): void => {
            if (opt.closeButton && closeElement) {
                toggleClass(closeElement, opt.closeClass, true);
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
            opt.toastClass && toggleClass(toastElement, opt.toastClass, true);
            opt.iconClass && toggleClass(toastElement, opt.iconClass, true);
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

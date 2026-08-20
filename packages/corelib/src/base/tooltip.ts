import { bindThis } from "@serenity-is/domwise";
import { getjQuery, isBS3, isBS5Plus } from "./environment";
import { isArrayLike } from "./system";

/**
 * Options for initializing a Bootstrap/jQuery tooltip.
 */
export interface TooltipOptions {
    /** Text shown inside the tooltip. */
    title?: string;
    /** Trigger events (e.g. `"hover focus"`, `"click"`). Defaults vary by implementation. */
    trigger?: string;
}

/**
 * Thin wrapper around Bootstrap / jQuery tooltip plugins with a fallback to the native
 * `title` attribute. Handles instance reuse, cleanup, and title updates.
 */
export class Tooltip {
    declare private el: HTMLElement;

    /**
     * Creates or wraps a tooltip for an element.
     * @param el - Target element or array-like collection (first element is used).
     * @param opt - Tooltip options; if omitted defaults are applied.
     * @param create - When `true` (default) creates a tooltip if none exists; when `false` only wraps an existing instance.
     * @returns A `Tooltip` wrapper instance.
     */
    public constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
    /**
     * Creates or wraps a tooltip for an element (implementation).
     * @param el - Target element or array-like collection (first element is used).
     * @param opt - Tooltip options; if omitted defaults are applied.
     * @param create - When `true` (default) creates a tooltip if none exists; when `false` only wraps an existing instance.
     */
    public constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions, create = true) {
        this.el = isArrayLike(el) ? el[0] : el;

        if (create === true && this.el) {
            let instance = Tooltip.getInstance(this.el);
            if (instance) {
                opt?.title !== void 0 && this.setTitle(opt.title); // can't change trigger
            }
            else {
                opt ??= {};
                opt.trigger ??= "hover focus";

                let $ = getjQuery();
                if ($?.fn?.tooltip)
                    $(this.el).tooltip(opt);
                else if (typeof bootstrap !== "undefined" && (bootstrap as any).Tooltip)
                    new (bootstrap as any).Tooltip(this.el, opt);
                else if (opt?.title !== void 0)
                    this.el.setAttribute("title", opt.title);
            }
        }
    }

    /** Default options applied when none are supplied. */
    static defaults: TooltipOptions = {
        trigger: "click hover"
    }

    /**
     * Disposes the underlying tooltip instance and clears internal Bootstrap state.
     */
    dispose(): void {
        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return;
        instance[isBS3() ? "destroy" : "dispose"]?.();
        // workaround for https://github.com/twbs/bootstrap/issues/37474
        instance._activeTrigger = {};
        instance._element = document.createElement('noscript'); // placeholder with no behavior
    }

    /**
     * Disposes the tooltip after a delay.
     * @param delay - Delay in milliseconds before disposing. Defaults to `5000`.
     */
    delayedDispose(delay: number = 5000) {
        setTimeout(bindThis(this).dispose, delay);
    }

    /**
     * Hides the tooltip after a delay.
     * @param delay - Delay in milliseconds before hiding. Defaults to `5000`.
     */
    delayedHide(delay: number = 5000): void {
        setTimeout(bindThis(this).hide, delay);
    }

    private static existingInstance(el: HTMLElement): any {
        let instance: any;
        if (typeof bootstrap !== "undefined")
            instance = (bootstrap as any)?.Tooltip?.getInstance?.(el);
        if (instance)
            return instance;

        const $ = getjQuery();
        if ($?.fn?.tooltip)
            return $(el).data()["bs.tooltip"];
        return null;
    }

    /**
     * Gets the existing tooltip wrapper for an element, if any.
     * @param el - Target element or array-like collection.
     * @returns A `Tooltip` wrapper around the existing instance, or `null` if none exists.
     */
    static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip {
        let instance = Tooltip.existingInstance(isArrayLike(el) ? el[0] : el);
        if (!instance)
            return null;

        return new (Tooltip as any)(el, null, false);
    }

    /**
     * Whether a tooltip implementation (Bootstrap or jQuery) is available in the current environment.
     * @returns `true` if Bootstrap Tooltip or jQuery tooltip is available, otherwise `false`.
     */
    static get isAvailable(): boolean {
        return !!((typeof bootstrap !== "undefined" && (bootstrap as any).Tooltip) ||
            getjQuery()?.fn?.tooltip);
    }

    /**
     * Updates the tooltip title text and synchronizes it with the underlying implementation.
     * @param value - New title text.
     * @returns This instance for chaining.
     */
    setTitle(value: string): Tooltip {
        if (!this.el)
            return this;

        this.el.setAttribute("title", value ?? '');

        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return this;

        this.el.setAttribute(`data-${isBS5Plus() ? 'bs-' : ''}original-title`, value ?? '');
        instance?.[isBS3() ? "fixTitle" : "_fixTitle"]?.();
        const inner = instance?.tip?.querySelector?.(".tooltip-inner");
        inner && (inner.textContent = value ?? '');
        instance?.update?.();
        return this;
    }

    /**
     * Shows or hides the tooltip.
     * @param show - `true` to show, `false` to hide.
     * @returns This instance for chaining.
     */
    toggle(show: boolean): Tooltip {
        if (!this.el)
            return this;

        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return this;

        instance?.[show ? "show" : "hide"]?.();
        return this;
    }

    /**
     * Hides the tooltip.
     * @returns This instance for chaining.
     */
    hide(): Tooltip {
        return this.toggle(false);
    }

    /**
     * Shows the tooltip.
     * @returns This instance for chaining.
     */
    show(): Tooltip {
        return this.toggle(true);
    }
}

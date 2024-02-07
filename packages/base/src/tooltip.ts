import { getjQuery, isBS3, isBS5Plus } from "./environment";
import { isArrayLike } from "./system";

export interface TooltipOptions {
    title?: string;
    trigger?: string;
}

export class Tooltip {
    private el: HTMLElement;

    public constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
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

    static defaults: TooltipOptions = {
        trigger: "click hover"
    }

    dispose(): void {
        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return;
        instance[isBS3() ? "destroy" : "dispose"]?.();
    }

    delayedDispose(delay: number = 5000) {
        setTimeout(this.dispose.bind(this), delay);
    }

    delayedHide(delay: number = 5000): void {
        setTimeout(this.hide.bind(this), delay);
    }

    private static existingInstance(el: HTMLElement): any {
        let $ = getjQuery();
        let instance: any;
        if (typeof bootstrap !== "undefined")
             instance = (bootstrap as any)?.Tooltip?.getInstance?.(el);
        if (instance)
            return instance;

        if ($?.fn?.tooltip)
            return $(el).data()["bs.tooltip"];
        return null;
    }

    static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip {
        let instance = Tooltip.existingInstance(isArrayLike(el) ? el[0] : el);
        if (!instance)
            return null;
        
        return new (Tooltip as any)(el, null, false);
    }

    static get isAvailable(): boolean {
        return !!((typeof bootstrap !== "undefined" && (bootstrap as any).Tooltip) ||
            getjQuery()?.fn?.tooltip);
    }

    setTitle(value: string): Tooltip {
        if (!this.el)
            return this;

        this.el.setAttribute("title", value);

        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return this;

        this.el.setAttribute(`data-${isBS5Plus() ? 'bs-' : ''}original-title`, value ?? '');
        instance?.[isBS3() ? "fixTitle" : "_fixTitle"]?.();
        var inner = instance?.tip?.querySelector?.(".tooltip-inner");
        inner && (inner.textContent = value ?? '');
        instance?.update?.();
        return this;
    }

    toggle(show: boolean): Tooltip {
        if (!this.el)
            return;

        let instance = Tooltip.existingInstance(this.el);
        if (!instance)
            return this;

        instance?.[show ? "show" : "hide"]?.();
        return this;
    }

    hide(): Tooltip {
        return this.toggle(false);
    }

    show(): Tooltip {
        return this.toggle(true);
    }
}

import { Fluent, IconClassName, iconClassName, isArrayLike, nsSerenity } from "../../base";
import { Widget } from "./widget";

/**
 * Props describing a single toolbar button.
 */
export interface ToolButtonProps {
    /** Optional action name stored on the button's `data-action` attribute. */
    action?: string;
    /** The button's title (text or element). */
    title?: string | HTMLElement | SVGElement | MathMLElement | DocumentFragment;
    /** Optional tooltip hint shown on hover. */
    hint?: string;
    /** Optional CSS class(es) applied to the button. */
    cssClass?: string;
    /** Optional icon class name to display before the title. */
    icon?: IconClassName;
    /** Handler invoked when the button is clicked. */
    onClick?: (e: MouseEvent & { currentTarget: EventTarget & HTMLElement }) => void;
    /** Callback invoked with the created button element. */
    ref?: (el: HTMLElement) => void;
    /** Whether the button is visible; may be a function evaluated on update. */
    visible?: boolean | (() => boolean);
    /** Whether the button is disabled; may be a function evaluated on update. */
    disabled?: boolean | (() => boolean);
}

/**
 * A toolbar button definition, extending {@link ToolButtonProps} with hotkey
 * and separator support.
 */
export interface ToolButton extends ToolButtonProps {
    /** Optional hotkey binding (e.g. "ctrl+s"). */
    hotkey?: string;
    /** Whether the browser's default hotkey behavior should be allowed. */
    hotkeyAllowDefault?: boolean;
    /** Optional context element to which the hotkey is bound. */
    hotkeyContext?: any;
    /** Whether (and where) a separator should be rendered before the button. */
    separator?: (false | true | 'left' | 'right' | 'both');
}

/**
 * Creates a toolbar button element from the given props.
 * @param tb - The button props.
 * @returns The created button element.
 */
export function ToolbarButton(tb: ToolButtonProps): HTMLElement {

    const btn = Fluent(
        <div class={["tool-button", tb.cssClass, tb.icon && "icon-tool-button", !tb.title && "no-text"]}
            title={!!tb.hint && tb.hint} onClick={e => {
                if (tb.onClick && !e.currentTarget?.classList.contains("disabled")) {
                    tb.onClick(e);
                }
            }}>
            <span class="button-inner">
                {tb.icon && <><i class={iconClassName(tb.icon)} />{" "}</>}{tb.title}
            </span>
        </div>
    );

    if (tb.action != null)
        btn.data("action", tb.action);

    if (tb.visible === false)
        btn.hide();

    if (tb.disabled != null && typeof tb.disabled !== "function")
        btn.toggleClass("disabled", !!tb.disabled);

    if (typeof tb.visible === "function" || typeof tb.disabled == "function") {
        btn.on('updateInterface', () => {
            if (typeof tb.visible === "function")
                btn.toggle(tb.visible());

            if (typeof tb.disabled === "function")
                btn.toggleClass("disabled", !!tb.disabled());
        });
    }

    const node = btn.getNode();
    if (tb.ref) {
        tb.ref(node);
        delete tb.ref;
    }
    return node;
}

/**
 * Options for configuring a {@link Toolbar}.
 */
export interface ToolbarOptions {
    /** The buttons to render in the toolbar. */
    buttons?: ToolButton[];
    /** Optional default context element for hotkey bindings. */
    hotkeyContext?: any;
}

/**
 * A widget that renders a horizontal toolbar of buttons, supporting separators,
 * hotkeys and dynamic visibility/disabled state.
 * @typeParam P - Widget props type, constrained to {@link ToolbarOptions}.
 */
export class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Renders the toolbar contents, creating button groups and buttons.
     * @returns The rendered tool group element.
     */
    protected override renderContents(): any {

        let group = <div class="tool-group" />;

        this.element
            .addClass("s-Toolbar clearfix")
            .append(group);

        var buttons = this.options.buttons || [];
        var currentCount = 0;
        for (var i = 0; i < buttons.length; i++) {
            var button = buttons[i];
            if (button.separator && currentCount > 0) {
                group = group.parentElement.appendChild(<div class="tool-group" />);
                currentCount = 0;
            }
            this.createButton(group, button);
            currentCount++;
        }

        return group;
    }

    /**
     * Destroys the toolbar, removing click handlers and hotkey bindings.
     */
    override destroy() {
        this.domNode.querySelectorAll('div.tool-button').forEach(el => Fluent.off(el, 'click'));
        if (this.mouseTrap) {
            if (this.mouseTrap.__listeners) {
                this.mouseTrap.__listeners.forEach((li: { node: Element, type: string; fn: EventListenerOrEventListenerObject; }) => {
                    li.node.removeEventListener(li.type, li.fn);
                });
                this.mouseTrap.__listeners = null;
            }
            if (!!this.mouseTrap.destroy) {
                this.mouseTrap.destroy();
            }
            else {
                this.mouseTrap.reset();
            }
            this.mouseTrap = null;
        }

        super.destroy();
    }

    /** The Mousetrap instance used for hotkey bindings, if any. */
    declare protected mouseTrap: any;

    /**
     * Creates a button in the given container, handling separators and hotkeys.
     * @param container - The container (or array-like of containers) to append to.
     * @param tb - The button definition.
     * @returns The created button element.
     */
    createButton(container: ParentNode | ArrayLike<ParentNode>, tb: ToolButton): HTMLElement {

        if (isArrayLike(container)) {
            container = container[0];
        }

        if (tb.separator === 'right' || tb.separator === 'both') {
            container.appendChild(<div class="separator" />);
        }

        let button = ToolbarButton(tb);
        container.append(button);

        if (tb.hotkey && window['Mousetrap' as any] != null) {
            if (!this.mouseTrap) {
                const mouseTrap = (window['Mousetrap' as any] as any);
                const el = (tb.hotkeyContext || this.options.hotkeyContext || window.document.documentElement) as Element;
                const prevAddEventListener = el.addEventListener;
                const listeners: Array<{ node: Element, type: string, fn: EventListenerOrEventListenerObject }> = [];
                el.addEventListener = function (type: string, fn: EventListenerOrEventListenerObject, flag: boolean) {
                    listeners.push({ node: this, type, fn });
                    prevAddEventListener.call(this, type, fn, flag);
                }
                this.mouseTrap = mouseTrap(el);
                this.mouseTrap.__listeners = listeners;
                delete el.addEventListener;
                if (el.addEventListener !== prevAddEventListener) {
                    el.addEventListener = prevAddEventListener;
                }
            }
            this.mouseTrap.bind(tb.hotkey, function () {
                if (getComputedStyle(button).display !== "none") {
                    Fluent.trigger(button, "click");
                }
                return tb.hotkeyAllowDefault;
            });
        }
        return button;
    }

    /**
     * Finds a button by its CSS class name.
     * @param className - The button class name, optionally prefixed with `.`.
     * @returns A {@link Fluent} wrapper for the matching button element.
     */
    findButton(className: string) {
        if (className != null && className.startsWith('.')) {
            className = className.substring(1);
        }

        return Fluent(this.domNode.querySelector<HTMLElement>('div.tool-button.' + className));
    }

    /**
     * Triggers an `updateInterface` event on all buttons so dynamic
     * visibility/disabled functions are re-evaluated.
     */
    updateInterface() {
        this.domNode.querySelectorAll('.tool-button').forEach(function (el: Element) {
            Fluent.trigger(el, 'updateInterface', { bubbles: false });
        });
    }
}
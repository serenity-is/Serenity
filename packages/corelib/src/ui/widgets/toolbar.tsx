import { Fluent, IconClassName, iconClassName, isArrayLike } from "../../base";
import { Decorators } from "../../types/decorators";
import { Widget } from "./widget";

export interface ToolButtonProps {
    action?: string;
    title?: string | HTMLElement | SVGElement | DocumentFragment;
    hint?: string;
    cssClass?: string;
    icon?: IconClassName;
    onClick?: (e: MouseEvent & { currentTarget: EventTarget & HTMLElement }) => void;
    ref?: (el: HTMLElement) => void;
    visible?: boolean | (() => boolean);
    disabled?: boolean | (() => boolean);
}

export interface ToolButton extends ToolButtonProps {
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
}

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

export interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}

@Decorators.registerClass('Serenity.Toolbar')
export class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {

    protected renderContents(): any {

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

    destroy() {
        this.domNode.querySelectorAll('div.tool-button').forEach(el => Fluent.off(el, 'click'));
        if (this.mouseTrap) {
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

    declare protected mouseTrap: any;

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
            this.mouseTrap = this.mouseTrap || (window['Mousetrap' as any] as any)(
                tb.hotkeyContext || this.options.hotkeyContext || window.document.documentElement);

            this.mouseTrap.bind(tb.hotkey, function () {
                if (button.style.display != "none") {
                    Fluent.trigger(button, "click");
                }
                return tb.hotkeyAllowDefault;
            });
        }
        return button;
    }

    findButton(className: string) {
        if (className != null && className.startsWith('.')) {
            className = className.substring(1);
        }

        return Fluent(this.domNode.querySelector<HTMLElement>('div.tool-button.' + className));
    }

    updateInterface() {
        this.domNode.querySelectorAll('.tool-button').forEach(function (el: Element) {
            Fluent.trigger(el, 'updateInterface', { bubbles: false });
        });
    }
}
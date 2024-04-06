import { Fluent, getjQuery } from "../../base";
import { Decorators } from "../../types/decorators";
import { Widget } from "./widget";

@Decorators.registerClass("Serenity.TemplatedWidget")
export class TemplatedWidget<P> extends Widget<P> {

    protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement> {
        return this.element.findFirst<TElement>('#' + this.idPrefix + id);
    }

    protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement {
        return this.domNode?.querySelector<TElement>('#' + this.idPrefix + id);
    }

    protected getTemplate(): string {
        return null;
    }

    protected renderContents(): void {
        var template = this.getTemplate();
        if (template != null) {
            template = template.replace(new RegExp('~_', 'g'), this.idPrefix);
            let $ = getjQuery();
            if ($)
                $(this.domNode).html(template);
            else
                this.domNode.innerHTML = template;
        }
    }
}

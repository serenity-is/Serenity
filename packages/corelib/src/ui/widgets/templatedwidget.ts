import { Fluent, getjQuery } from "@serenity-is/base";
import { Decorators } from "../../types/decorators";
import { Widget } from "./widget";

@Decorators.registerClass("Serenity.TemplatedWidget")
export class TemplatedWidget<P> extends Widget<P> {

    protected byId(id: string): Fluent {
        return Fluent(this.domNode.querySelector('#' + this.idPrefix + id));
    }

    protected findById(id: string): HTMLElement {
        return this.domNode.querySelector('#' + this.idPrefix + id) as HTMLElement;
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

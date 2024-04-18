import { getjQuery } from "../../base";
import { Decorators } from "../../types/decorators";
import { Widget } from "./widget";

@Decorators.registerClass("Serenity.TemplatedWidget")
export class TemplatedWidget<P> extends Widget<P> {

    /** @deprecated Please use renderContents() and .tsx (jsx-dom) to return HTML markup */
    protected getTemplate(): string {
        return null;
    }

    protected renderContents(): any {
        var template = this.getTemplate();
        if (template != null) {
            template = template.replace(new RegExp('~_', 'g'), this.idPrefix);
            let $ = getjQuery();
            if ($)
                $(this.domNode).html(template);
            else
                this.domNode.innerHTML = template;
        }

        return super.renderContents();
    }
}

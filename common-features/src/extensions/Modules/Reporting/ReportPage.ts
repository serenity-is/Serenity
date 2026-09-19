import { Fluent, QuickSearchInput, Widget, WidgetProps, stripDiacritics } from "@serenity-is/corelib";
import { bindThis } from "@serenity-is/domwise";
import { nsExtensions } from "../ServerTypes/Namespaces";
import { ReportDialog } from "./ReportDialog";

export class ReportPage<P = {}> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsExtensions);

    constructor(props: WidgetProps<P>) {
        super(props);

        Fluent.on(this.domNode, "click", ".report-link", bindThis(this).reportLinkClick);

        new QuickSearchInput({
            element: this.domNode.querySelector('.s-QuickSearchBar input') as HTMLElement,
            onSearch: (field, text, done) => {
                this.updateMatchFlags(text);
                done(true);
            }
        });
    }

    protected updateMatchFlags(text: string) {
        const liList = this.domNode.querySelectorAll('.report-list li');
        liList.forEach(x => x.classList.remove('non-match'));
        text = text?.trim();
        if (!text)
            return;

        text = stripDiacritics(text).toUpperCase();

        const reportItems = Array.from(liList).filter(x => x.classList.contains('report-item'));
        reportItems.forEach(function (el) {
            const title = stripDiacritics((el.textContent ?? '').toUpperCase());
            if (title.indexOf(text) < 0) {
                el.classList.add('non-match');
            }
        });

        function parents(el: HTMLElement, selector: string) {
            const parents = [];
            while ((el = el.parentNode as HTMLElement) && (el as any) !== document) {
                if (!selector || el.matches(selector))
                    parents.push(el);
            }
            return parents;
        }

        const matchingItems = reportItems.filter(x => !x.classList.contains('non-match'));
        let visibles = [...matchingItems];
        matchingItems.forEach(x => visibles.push(...parents(x as HTMLElement, 'li')));
        visibles = visibles.filter((x, i) => visibles.indexOf(x) === i);
        visibles.forEach(v => {
            v.querySelectorAll(':scope [data-bs-toggle]:not([aria-expanded=true])').forEach(c => {
                c.setAttribute('aria-expanded', "true");
                c.classList.remove('collapsed');
            });
            if (v.parentElement && v.parentElement.classList.contains("collapse") &&
                !v.parentElement.classList.contains("show")) {
                v.parentElement.classList.add("show");
            }
        });

        const nonVisibles = Array.from(liList).filter(x => visibles.indexOf(x) < 0);
        nonVisibles.forEach(x => x.classList.add('non-match'));
    }

    protected reportLinkClick(e: Event) {
        e.preventDefault();
        new ReportDialog({
            reportKey: (e.target as HTMLElement).getAttribute('data-key')
        }).dialogOpen();
    }
}

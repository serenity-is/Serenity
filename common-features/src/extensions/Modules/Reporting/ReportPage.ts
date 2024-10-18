import { Decorators, Fluent, QuickSearchInput, Widget, WidgetProps, stripDiacritics } from "@serenity-is/corelib";
import { ReportDialog } from "./ReportDialog";

@Decorators.registerClass("Serenity.Extensions.ReportPage")
export class ReportPage<P = {}> extends Widget<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        Fluent.on(this.domNode, "click", ".report-link", this.reportLinkClick.bind(this));

        new QuickSearchInput({
            element: this.domNode.querySelector('.s-QuickSearchBar input') as HTMLElement,
            onSearch: (field, text, done) => {
                this.updateMatchFlags(text);
                done(true);
            }
        });
    }

    protected updateMatchFlags(text: string) {
        var liList = this.domNode.querySelectorAll('.report-list li');
        liList.forEach(x => x.classList.remove('non-match'));
        text = text?.trim();
        if (!text)
            return;

        text = stripDiacritics(text).toUpperCase();

        var reportItems = Array.from(liList).filter(x => x.classList.contains('report-item'));
        reportItems.forEach(function (el) {
            var title = stripDiacritics((el.textContent ?? '').toUpperCase());
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

        var matchingItems = reportItems.filter(x => !x.classList.contains('non-match'));
        var visibles = [...matchingItems];
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

        var nonVisibles = Array.from(liList).filter(x => visibles.indexOf(x) < 0);
        nonVisibles.forEach(x => x.classList.add('non-match'));
    }

    protected reportLinkClick(e: Event) {
        e.preventDefault();
        new ReportDialog({
            reportKey: (e.target as HTMLElement).getAttribute('data-key')
        }).dialogOpen();
    }
}

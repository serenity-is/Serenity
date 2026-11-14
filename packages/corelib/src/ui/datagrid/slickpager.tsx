import { Fluent, nsSerenity, PagerTexts } from "../../base";
import { PagerOptions } from "../../slick";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Pager widget for SlickGrid / SleekGrid views that provides page navigation,
 * page size selection, and status information.
 * @typeParam P - Options type for the widget.
 */
export class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    declare private currentPage: HTMLInputElement;
    declare private totalPages: HTMLSpanElement;
    declare private pageSize: HTMLSelectElement;
    declare private stat: HTMLSpanElement;
    declare private nextButton: HTMLDivElement;
    private indeterminate: boolean;

    /**
     * Creates a pager for the view specified in the options.
     * @param props - Widget props including the view to page.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        const opt = this.options;
        opt.showRowsPerPage ??= true;
        opt.rowsPerPageOptions ??= [20, 100, 500, 2000];
        const v = opt.view; if (!v) throw new Error("SlickPager requires view option to be set!");
        // the total of the rows is not known, so there is no last page, or total of the pages
        this.indeterminate = opt.indeterminatePaging ?? v.indeterminatePaging ?? false;

        const p = "slick-pg-";
        const Group = ({ id, children }: { id: string, children: any }) => <div class={`${p}grp ${p}grp-${id}`}>{children}</div> as HTMLDivElement;
        const Button = ({ id, onClick }: { id: string, onClick?: (e: MouseEvent) => void }) => <div class={`${p}${id} ${p}btn`} onClick={onClick}><span class={`${p}btn-span`}></span></div> as HTMLDivElement;
        const NavButton = ({ id }: { id: string }) => { const b = Button({ id }); Fluent.on(b, "click", () => this._changePage(id)); if (id === "next") this.nextButton = b; return b; }

        this.element.addClass("s-SlickPager slick-pg").append(
            <div class={p + "in"}>
                {opt.showRowsPerPage && <Group id="size">
                    {this.pageSize = <select class={`${p}size`} name="rp" onChange={() => {
                        if (opt.onRowsPerPageChange)
                            opt.onRowsPerPageChange(+this.pageSize.value);
                        else {
                            v["newp"] = 1;
                            v.setPagingOptions({
                                page: 1,
                                rowsPerPage: +this.pageSize.value
                            });
                        }
                    }}>
                        {opt.rowsPerPageOptions.map(rowsPerPage => <option value={rowsPerPage} selected={v.rowsPerPage == rowsPerPage}>{rowsPerPage}</option>)}
                    </select> as HTMLSelectElement}
                </Group>}
                <Group id="firstprev">
                    <NavButton id="first" />
                    <NavButton id="prev" />
                </Group>
                <Group id="control">
                    <span class={`${p}control`}>
                        <span class={`${p}pagetext`}>{PagerTexts.Page}</span>
                        {this.currentPage = <input class={`${p}current mx-1`} type="text" size={4} value="1" onKeyDown={e => { if (e.key === "Enter") this._changePage("input"); }} /> as HTMLInputElement}
                        {!this.indeterminate && <span class={`${p}pagesep px-1`}>/</span>}
                        {this.totalPages = !this.indeterminate ? <span class={`${p}total`}>1</span> as HTMLSpanElement : null}
                    </span>
                </Group>
                <Group id="nextlast">
                    <NavButton id="next" />
                    {!this.indeterminate && <NavButton id="last" />}
                </Group>
                <Group id="reload">
                    <Button id="reload" onClick={() => v.populate()} />
                </Group>
                <Group id="stat">
                    {this.stat = <span class={`${p}stat`} /> as HTMLSpanElement}
                </Group>
            </div>);

        v.onPagingInfoChanged.subscribe(() => this._updatePager());
    }

    /**
     * Changes the current page based on the requested navigation action.
     * @param ctype - Navigation action: "first", "prev", "next", "last", or "input".
     * @returns True if the page change was handled, false otherwise.
     */
    _changePage(ctype: string) { //change page

        const view = this.options.view;

        if (!view || view.loading)
            return true;

        const info = view.getPagingInfo();
        const rowsPerPage = info.actualRowsPerPage || info.rowsPerPage;
        const pages = (!rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / rowsPerPage);
        // the pages after the current one are only known to exist when there are more rows
        const hasNext = this.indeterminate ? !!info.hasMore : info.page < pages;

        let newp: number;

        switch (ctype) {
            case 'first': newp = 1; break;
            case 'prev': if (info.page > 1) newp = parseInt(info.page as any) - 1; break;
            case 'next': if (hasNext) newp = parseInt(info.page as any) + 1; break;
            case 'last': newp = pages; break;
            case 'input':
                let nv = parseInt(this.currentPage.value);
                if (isNaN(nv))
                    nv = 1;
                else if (nv < 1)
                    nv = 1;
                else if (!this.indeterminate && nv > pages)
                    nv = pages;

                this.currentPage.value = "" + nv;

                newp = nv;
                break;
        }

        if (newp == null || newp == info.page)
            return false;

        if (this.options.onChangePage)
            this.options.onChangePage(newp);
        else {
            view.setPagingOptions({ page: newp });
        }
    }

    /**
     * Refreshes the pager UI from the current view paging info.
     */
    _updatePager() {

        const view = this.options.view;
        const info = view.getPagingInfo();
        const rowsPerPage = info.actualRowsPerPage || info.rowsPerPage;
        const pages = (!rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / rowsPerPage);

        this.currentPage.value = info.page?.toString();
        if (this.totalPages)
            this.totalPages.textContent = "" + pages;

        if (this.nextButton) {
            const disabled = this.indeterminate && !info.hasMore;
            this.nextButton.classList.toggle("slick-pg-disabled", disabled);
            this.nextButton.setAttribute("aria-disabled", disabled ? "true" : "false");
        }

        const r1 = (info.page - 1) * rowsPerPage + 1;
        let r2 = r1 + rowsPerPage - 1;

        if (info.totalCount < r2)
            r2 = info.totalCount;

        let stat: string;

        if (info.loading) {
            stat = PagerTexts.LoadingStatus;
        }
        else if (info.error) {
            stat = info.error;
        }
        else if (this.indeterminate && info.totalCount > 0) {
            // the rows of the page, which are not as many as a page on the last one
            const rows = Math.min(rowsPerPage || info.totalCount, info.totalCount - (r1 - 1));
            stat = PagerTexts.RangeStatus
                .replace(/{from}/, r1 as any)
                .replace(/{to}/, (r1 + rows - 1) as any);
        }
        else if (info.totalCount > 0) {
            stat = PagerTexts.PageStatus;
            stat = stat.replace(/{from}/, r1 as any);
            stat = stat.replace(/{to}/, r2 as any);
            stat = stat.replace(/{total}/, info.totalCount as any);
        }
        else
            stat = PagerTexts.NoRowStatus;

        this.stat.textContent = stat;
        this.pageSize && (this.pageSize.value = (info.rowsPerPage || 0).toString());
    }
}
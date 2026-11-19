import { appendToNode, Fluent, nsSerenity, PagerTexts, RenderableContent } from "../../base";
import { IGridBoundView, IRemoteView, PagerOptions, PagingInfo, PagingRowRange } from "../../slick";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Pager widget for SlickGrid / SleekGrid views that provides page navigation, page size selection, and
 * status information. A view that shows a range of the rows instead of a page of them, e.g. the rows it
 * has loaded as the grid is scrolled, gets no page navigation, and a status that tells of its range.
 * @typeParam P - Options type for the widget.
 */
export class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * The defaults of the options of every pager, which a pager or an application can change, e.g. to
     * bring back the first and last page buttons that most pagers leave out, as the page box takes a
     * number anyway.
     */
    static readonly defaults: PagerOptions = {
        showFirstLast: false
    };

    declare private currentPage: HTMLInputElement;
    declare private totalPages: HTMLSpanElement;
    declare private pageSize: HTMLSelectElement;
    declare private stat: HTMLSpanElement;
    declare private extra: HTMLSpanElement;
    declare private firstButton: HTMLDivElement;
    declare private prevButton: HTMLDivElement;
    declare private nextButton: HTMLDivElement;
    declare private lastButton: HTMLDivElement;
    private indeterminate: boolean;
    /** Whether the view shows a range of the rows instead of a page of them, so there is no page navigation. */
    private rowsMode: boolean;
    /** What the extra content of the last update was, so it is not rendered again when it did not change. */
    private lastExtra: RenderableContent;
    /** The subscription to the paging info of the view, which is removed when the pager is destroyed. */
    private pagingInfoChanged: () => void;

    /**
     * Creates a pager for the view specified in the options.
     * @param props - Widget props including the view to page.
     */
    constructor(props: WidgetProps<P>) {
        super(props);

        const opt = this.options;
        for (const key of Object.keys(SlickPager.defaults) as Array<keyof PagerOptions>)
            (opt as any)[key] ??= SlickPager.defaults[key];

        opt.showRowsPerPage ??= true;
        opt.rowsPerPageOptions ??= [20, 100, 500, 2000];
        const v = opt.view; if (!v) throw new Error("SlickPager requires view option to be set!");
        // the total of the rows is not known, so there is no last page, or total of the pages
        this.indeterminate = opt.indeterminatePaging ?? v.indeterminatePaging ?? false;
        // a view that loads its rows as the grid is scrolled shows a range of them, and needs the grid to do it
        this.rowsMode = v.getPagingInfo?.()?.pagingMode === "rows";
        (v as IGridBoundView<any>).bindToGrid?.(opt.grid);

        const p = "slick-pg-";
        // the buttons only have an icon, so they tell what they do to both a tooltip and a screen reader,
        // and take the focus, so they can be used from the keyboard like a button
        const Button = ({ id, label, onClick }: { id: string, label: string, onClick?: (e: MouseEvent) => void }) =>
            <div class={`${p}${id} ${p}btn`} role="button" tabindex="0" title={label} aria-label={label}
                onClick={onClick}
                onKeyDown={e => {
                    if (e.key === "Enter" || e.key === " ") {
                        e.preventDefault();
                        (e.currentTarget as HTMLElement).click();
                    }
                }}>
                <span class={`${p}btn-span`}></span>
            </div> as HTMLDivElement;

        const buttonLabels = {
            first: PagerTexts.FirstPage,
            prev: PagerTexts.PreviousPage,
            next: PagerTexts.NextPage,
            last: PagerTexts.LastPage
        };

        const NavButton = ({ id }: { id: string }) => {
            const b = Button({ id, label: buttonLabels[id as keyof typeof buttonLabels] });
            Fluent.on(b, "click", (e: MouseEvent) => {
                // shift+click goes to the end of the pages when there are no first and last buttons to do it. the last
                // page is not known when the total of the rows is not, so the next button goes one page then
                if (e?.shiftKey && !opt.showFirstLast) {
                    if (id === "prev") {
                        this._changePage("first");
                        return;
                    }
                    if (id === "next" && !this.indeterminate) {
                        this._changePage("last");
                        return;
                    }
                }

                this._changePage(id);
            });
            switch (id) {
                case "first": this.firstButton = b; break;
                case "prev": this.prevButton = b; break;
                case "next": this.nextButton = b; break;
                case "last": this.lastButton = b; break;
            }
            return b;
        }

        // the page navigation comes first in the row, and the status with the controls that belong to it is
        // pushed to the end by its style, so a control that is missing does not move the others
        this.element.addClass("s-SlickPager slick-pg")
            .toggleClass(p + "range", this.rowsMode)
            .append(
            <div class={p + "in"}>
                <div class={p + "info"}>
                    {this.stat = <span class={`${p}stat`} /> as HTMLSpanElement}
                </div>
                <div class={p + "nav"}>
                    {this.extra = <span class={`${p}extra`} hidden /> as HTMLSpanElement}
                    {opt.showRowsPerPage && (this.pageSize = <select class={`${p}size`} name="rp" title={PagerTexts.RowsPerPage} aria-label={PagerTexts.RowsPerPage} onChange={() => {
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
                    </select> as HTMLSelectElement)}
                    {opt.showReload !== false && <Button id="reload" label={PagerTexts.Refresh} onClick={() => v.populate()} />}
                    {!this.rowsMode && opt.showFirstLast && <NavButton id="first" />}
                    {!this.rowsMode && <NavButton id="prev" />}
                    {!this.rowsMode && <div class={`${p}control`}>
                        {this.currentPage = <input class={`${p}current mx-1`} type="text" size={4} value="1" title={PagerTexts.Page} aria-label={PagerTexts.Page} onKeyDown={e => { if (e.key === "Enter") this._changePage("input"); }} /> as HTMLInputElement}
                        {!this.indeterminate && <span class={`${p}pagesep px-1`}>/</span>}
                        {this.totalPages = !this.indeterminate ? <span class={`${p}total`}>1</span> as HTMLSpanElement : null}
                    </div>}
                    {!this.rowsMode && <NavButton id="next" />}
                    {!this.rowsMode && opt.showFirstLast && !this.indeterminate && <NavButton id="last" />}
                </div>
            </div>);

        this.pagingInfoChanged = () => this._updatePager();
        v.onPagingInfoChanged.subscribe(this.pagingInfoChanged);
        this._updatePager();
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
     * Dims a navigation button that can't go anywhere, and tells it to assistive technology.
     * @param button - The button to set the state of.
     * @param disabled - Whether the button is disabled.
     */
    protected setDisabled(button: HTMLDivElement, disabled: boolean): void {
        if (!button)
            return;

        button.classList.toggle("slick-pg-disabled", disabled);
        button.setAttribute("aria-disabled", disabled ? "true" : "false");
    }

    /**
     * Shows the extra content the grid or the view gives before the controls, e.g. the totals of the page
     * and of all the rows. A string is text, an element or a fragment is appended as it is, so the pager
     * never parses markup. It is only rendered again when the content changes.
     * @param info - The paging info of the view.
     */
    protected updateExtra(info: PagingInfo): void {
        if (!this.extra)
            return;

        const content = this.options.getInfoText?.(info) ??
            (this.options.view as IRemoteView<any>)?.getPagingInfoText?.();

        if (content === this.lastExtra)
            return;

        this.lastExtra = content;
        this.extra.replaceChildren();
        // nothing to show, so the gap before the controls is not there either
        this.extra.hidden = content == null || content === "";
        if (!this.extra.hidden)
            appendToNode(this.extra, content);
    }

    /**
     * Removes the subscription to the view, so a view that outlives its pager does not keep updating it.
     * The grid binding of a scroll view is left alone, as the view belongs to the grid, which may set
     * another pager for it.
     */
    override destroy(): void {
        if (this.pagingInfoChanged) {
            this.options.view?.onPagingInfoChanged?.unsubscribe?.(this.pagingInfoChanged);
            this.pagingInfoChanged = null;
        }

        super.destroy();
    }

    /**
     * Gets the rows the pager tells of, which the view gives for a range view, and are the rows of its
     * current page for a paged one.
     * @param info - The paging info of the view.
     * @returns The rows, or null when there are none.
     */
    protected getRowRange(info: PagingInfo): PagingRowRange {
        if (this.rowsMode)
            return info.rowRange;

        if (!(info.totalCount > 0))
            return null;

        const rowsPerPage = info.actualRowsPerPage || info.rowsPerPage;
        const from = (info.page - 1) * rowsPerPage + 1;
        // the rows of the page, which are not as many as a page on the last one
        const rows = Math.min(rowsPerPage || info.totalCount, info.totalCount - (from - 1));
        return {
            from,
            to: from + rows - 1,
            // the total is not known when the view pages without it, and there may be more rows
            total: this.indeterminate ? undefined : info.totalCount
        };
    }

    /**
     * Refreshes the pager UI from the current view paging info.
     */
    _updatePager() {

        const view = this.options.view;
        const info = view.getPagingInfo();

        if (this.currentPage) {
            const rowsPerPage = info.actualRowsPerPage || info.rowsPerPage;
            const pages = (!rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / rowsPerPage);
            this.currentPage.value = info.page?.toString();
            if (this.totalPages)
                this.totalPages.textContent = "" + pages;

            // a button that can't go anywhere is dimmed, so it tells which end of the pages is shown
            const atStart = !(info.page > 1);
            const atEnd = this.indeterminate ? !info.hasMore : info.page >= pages;
            this.setDisabled(this.firstButton, atStart);
            this.setDisabled(this.prevButton, atStart);
            this.setDisabled(this.nextButton, atEnd);
            this.setDisabled(this.lastButton, atEnd);
        }

        let stat: string;

        if (info.loading) {
            stat = PagerTexts.LoadingStatus;
        }
        else if (info.error) {
            stat = info.error;
        }
        else {
            const range = this.getRowRange(info);
            if (!range)
                stat = PagerTexts.NoRowStatus;
            else {
                // the total is not told when it is not known, e.g. while the rows that are not loaded yet may be more
                stat = (range.total == null ? PagerTexts.RangeStatus : PagerTexts.PageStatus)
                    .replace(/{from}/, range.from as any)
                    .replace(/{to}/, range.to as any)
                    .replace(/{total}/, range.total as any);
            }
        }

        this.stat.textContent = stat;
        this.pageSize && (this.pageSize.value = (info.rowsPerPage || 0).toString());
        this.updateExtra(info);
    }
}
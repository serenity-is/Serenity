import { Fluent, localText } from "../../base";
import { PagerOptions } from "../../slick";
import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "../widgets/widget";

@Decorators.registerClass("Serenity.SlickPager")
export class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {

    declare private currentPage: HTMLInputElement;
    declare private totalPages: HTMLSpanElement;
    declare private pageSize: HTMLSelectElement;
    declare private stat: HTMLSpanElement;

    constructor(props: WidgetProps<P>) {
        super(props);

        let opt = this.options;
        opt.showRowsPerPage ??= true;
        opt.rowsPerPageOptions ??= [20, 100, 500, 2000];
        var v = opt.view; if (!v) throw "SlickPager requires view option to be set!";

        const p = "slick-pg-";
        const Group = ({ id, children }: { id: string, children: any }) => <div class={`${p}grp ${p}grp-${id}`}>{children}</div> as HTMLDivElement;
        const Button = ({ id, onClick }: { id: string, onClick?: (e: MouseEvent) => void }) => <div class={`${p}${id} ${p}btn`} onClick={onClick}><span class={`${p}btn-span`}></span></div> as HTMLDivElement;
        const NavButton = ({ id, onClick }: { id: string, onClick?: (e: MouseEvent) => void }) => { const b = Button({ id, onClick }); Fluent.on(b, "click", () => this._changePage(id)); return b; }

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
                        <span class={`${p}pagetext`}>{localText("Controls.Pager.Page")}</span>
                        {this.currentPage = <input class={`${p}current mx-1`} type="text" size={4} value="1" onKeyDown={e => { if (e.key === "Enter") this._changePage("input"); }} /> as HTMLInputElement}
                        <span class={`${p}pagesep px-1`}>/</span>
                        {this.totalPages = <span class={`${p}total`}>1</span> as HTMLSpanElement}
                    </span>
                </Group>
                <Group id="nextlast">
                    <NavButton id="next" />
                    <NavButton id="last" />
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

    _changePage(ctype: string) { //change page

        var view = this.options.view;

        if (!view || view.loading)
            return true;

        var info = view.getPagingInfo();
        var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);

        var newp: number;

        switch (ctype) {
            case 'first': newp = 1; break;
            case 'prev': if (info.page > 1) newp = parseInt(info.page as any) - 1; break;
            case 'next': if (info.page < pages) newp = parseInt(info.page as any) + 1; break;
            case 'last': newp = pages; break;
            case 'input':
                var nv = parseInt(this.currentPage.value);
                if (isNaN(nv))
                    nv = 1;
                else if (nv < 1)
                    nv = 1;
                else if (nv > pages)
                    nv = pages;

                this.currentPage.value = "" + nv;

                newp = nv;
                break;
        }

        if (newp == info.page)
            return false;

        if (this.options.onChangePage)
            this.options.onChangePage(newp);
        else {
            view.setPagingOptions({ page: newp });
        }
    }

    _updatePager() {

        var view = this.options.view;
        var info = view.getPagingInfo();
        var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);

        this.currentPage.value = info.page?.toString();
        this.totalPages.textContent = "" + pages;

        var r1 = (info.page - 1) * info.rowsPerPage + 1;
        var r2 = r1 + info.rowsPerPage - 1;

        if (info.totalCount < r2)
            r2 = info.totalCount;

        var stat: string;

        if (info.loading) {
            stat = localText("Controls.Pager.LoadingStatus");
        }
        else if (info.error) {
            stat = info.error;
        }
        else if (info.totalCount > 0) {
            stat = localText("Controls.Pager.PageStatus");
            stat = stat.replace(/{from}/, r1 as any);
            stat = stat.replace(/{to}/, r2 as any);
            stat = stat.replace(/{total}/, info.totalCount as any);
        }
        else
            stat = localText("Controls.Pager.NoRowStatus");

        this.stat.textContent = stat;
        this.pageSize && (this.pageSize.value = (info.rowsPerPage || 0).toString());
    }
}
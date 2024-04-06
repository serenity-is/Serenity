import { Fluent, localText } from "../../base";
import { PagerOptions } from "../../slick";
import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "../widgets/widget";

@Decorators.registerClass("Serenity.SlickPager")
export class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {

    private currentPage: Fluent<HTMLInputElement>;
    private totalPages: Fluent<HTMLSpanElement>;
    private pageSize: Fluent<HTMLSelectElement>;
    private stat: Fluent<HTMLSpanElement>;

    constructor(props: WidgetProps<P>) {
        super(props);

        let opt = this.options;
        opt.showRowsPerPage ??= true;
        opt.rowsPerPageOptions ??= [20, 100, 500, 2000];
        var v = opt.view; if (!v) throw "SlickPager requires view option to be set!";

        let p = "slick-pg-";
        let grp = (t: string) => Fluent("div").class(`${p}grp ${p}grp-${t}`);
        let btn = (key: string) => Fluent("div").class(`${p}${key} ${p}btn`).append(Fluent("span").class(`${p}btn-span`));
        let nav = (key: string) => btn(key).on("click", () => this._changePage(key));

        let el = this.element.addClass("s-SlickPager slick-pg");

        this.currentPage = Fluent("input")
            .class(`${p}current mx-1`)
            .attr("type", "text")
            .attr("size", 4).attr("value", 1)
            .on("keydown", e => { if (e.key === "Enter") this._changePage("input"); });

        this.totalPages = Fluent("span").class(`${p}total`).text("1");

        let control = grp("control").append(Fluent("span")
            .class(`${p}control`)
            .append(Fluent("span").class(`${p}pagetext`).text(localText("Controls.Pager.Page")))
            .append(this.currentPage)
            .append(Fluent("span").class(`${p}pagesep px-1`).text("/"))
            .append(this.totalPages)
        );

        this.stat = Fluent("span").class(`${p}stat`);

        let inner = Fluent("div").class(p + "in").appendTo(el)
            .append(grp("firstprev")
                .append(nav("first"))
                .append(nav("prev")))
            .append(control)
            .append(grp("nextlast")
                .append(nav("next"))
                .append(nav("last")))
            .append(grp("reload")
                .append(btn("reload")
                    .on("click", () => v.populate())))
            .append(grp("stat")
                .append(this.stat));

        if (this.options.showRowsPerPage) {

            this.pageSize = Fluent("select")
                .class(`${p}size`)
                .attr("name", "rp")
                .appendTo(grp("size").prependTo(inner))
                .on("change", () => {
                    if (opt.onRowsPerPageChange)
                        opt.onRowsPerPageChange(+this.pageSize.val());
                    else {
                        v["newp"] = 1;
                        v.setPagingOptions({
                            page: 1,
                            rowsPerPage: +this.pageSize.val()
                        });
                    }
                });

            for (var rowsPerPage of opt.rowsPerPageOptions) {
                Fluent("option")
                    .attr("value", rowsPerPage)
                    .attr("selected", v.rowsPerPage == rowsPerPage && "selected")
                    .text("" + rowsPerPage)
                    .appendTo(this.pageSize);
            }
        }

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
                var nv = parseInt(this.currentPage.val());
                if (isNaN(nv))
                    nv = 1;
                else if (nv < 1)
                    nv = 1;
                else if (nv > pages)
                    nv = pages;

                this.currentPage.val("" + nv);

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

        this.currentPage.val(info.page);
        this.totalPages.text("" + pages);

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
            stat = stat.replace(/{from}/, <any>r1);
            stat = stat.replace(/{to}/, <any>r2);
            stat = stat.replace(/{total}/, <any>info.totalCount);
        }
        else
            stat = localText("Controls.Pager.NoRowStatus");

        this.stat.text(stat);
        this.pageSize?.val((info.rowsPerPage || 0).toString());
    }
}
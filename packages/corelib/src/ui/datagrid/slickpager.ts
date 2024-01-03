import { htmlEncode, localText } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { PagerOptions } from "../../slick";
import { Widget, WidgetProps } from "../widgets/widget";

@Decorators.registerClass("Serenity.SlickPager")
export class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        let o = this.options;
        o.showRowsPerPage ??= true;
        o.rowsPerPageOptions ??= [20, 100, 500, 2000];
        var v = o.view; if (!v) throw "SlickPager requires view option to be set!";

        let p = "slick-pg-";
        let grp = (t: string) => ` class="${p}grp ${p}grp-${t}"`;
        let btn = (t: string) => `<div class="${p}${t} ${p}btn"><span class="${p}btn-span"></span></div>`;

        $(this.domNode).addClass('s-SlickPager slick-pg')
            .html(`<div class="${p}in"><div${grp("firstprev")}>${btn("first")}${btn("prev")}</div>
<div${grp("control")}><span class="${p}control"><span class="${p}pagetext">${htmlEncode(localText("Controls.Pager.Page"))}</span>
<input id="${this.idPrefix}CurrentPage" class="${p}current" type="text" size="4" value="1" />
<span class="${p}pagesep">/</span><span class="${p}total">1</span></span></div>
<div${grp("nextlast")}>${btn("next")}${btn("last")}</div><div${grp("reload")}>${btn("reload")}</div><div${grp("stat")}><span class="${p}stat"></span></div></div>`);

        ['first', 'prev', 'next', 'last'].forEach(s => $(`.${p}${s}`, this.domNode).on('click', () => this._changePage(s)));
        $(`.${p}reload`, this.domNode).on('click', () => v.populate());
        $(`.${p}current`, this.domNode).on('keydown', e => { if (e.key === 'Enter') this._changePage('input') });

        if (this.options.showRowsPerPage) {
            var opt: string = "", sel = "";
            for (var nx = 0; nx < o.rowsPerPageOptions.length; nx++) {
                if (v.rowsPerPage == o.rowsPerPageOptions[nx])
                    sel = 'selected="selected"'; else sel = '';
                opt += "<option value='" + o.rowsPerPageOptions[nx] + "' " + sel + " >" + o.rowsPerPageOptions[nx] + "&nbsp;&nbsp;</option>";
            };
            $(`.${p}in`, this.domNode).prepend(`<div class="${p}grp"><select class="${p}size" name="rp">${opt}</select></div>`);
            $(`select.${p}size`, this.domNode).on('change', function (this: HTMLSelectElement) {
                if (o.onRowsPerPageChange)
                    o.onRowsPerPageChange(+this.value);
                else {
                    v["newp"] = 1;
                    v.setPagingOptions({
                        page: 1,
                        rowsPerPage: +this.value
                    });
                }
            });
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
                var nv = parseInt($('input.slick-pg-current', this.element).val() as string);
                if (isNaN(nv))
                    nv = 1;
                else if (nv < 1)
                    nv = 1;
                else if (nv > pages)
                    nv = pages;

                $('.slick-pg-current', this.element).val(nv);

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

        $('.slick-pg-current', this.element).val(info.page);
        $('.slick-pg-total', this.element).html(pages as any);

        var r1 = (info.page - 1) * info.rowsPerPage + 1;
        var r2 = r1 + info.rowsPerPage - 1;

        if (info.totalCount < r2)
            r2 = info.totalCount;

        var stat: string;

        if (info.loading) {
            stat = htmlEncode(localText("Controls.Pager.LoadingStatus"));
        }
        else if (info.error) {
            stat = info.error;
        }
        else if (info.totalCount > 0) {
            stat = htmlEncode(localText("Controls.Pager.PageStatus"));
            stat = stat.replace(/{from}/, <any>r1);
            stat = stat.replace(/{to}/, <any>r2);
            stat = stat.replace(/{total}/, <any>info.totalCount);
        }
        else
            stat = htmlEncode(localText("Controls.Pager.NoRowStatus"));

        $('.slick-pg-stat', this.element).html(stat);
        $('.slick-pg-size', this.element).val((info.rowsPerPage || 0).toString());
    }
}
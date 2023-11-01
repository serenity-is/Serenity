import { htmlEncode, extend, localText } from "../../q";
import { PagerOptions } from "../../slick";
import { Decorators } from "../../decorators";
import { Widget } from "../widgets/widget";

@Decorators.registerClass("Serenity.SlickPager")
export class SlickPager extends Widget<PagerOptions> {

    constructor(div: JQuery, o: PagerOptions) {
        super(div, extend({
            showRowsPerPage: true,
            rowsPerPageOptions: [20, 100, 500, 2000]
        } as any, o));

        o = this.options;
        var v = o.view;

        if (!v)
            throw "SlickPager requires view option to be set!";

        this.element.addClass('s-SlickPager slick-pg')
            .html(`<div class="slick-pg-in">
    <div class="slick-pg-grp slick-pg-grp-firstprev">
        <div class="slick-pg-first slick-pg-btn"><span class="slick-pg-btn-span"></span></div>
        <div class="slick-pg-prev slick-pg-btn"><span class="slick-pg-btn-span"></span></div>
    </div>
    <div class="slick-pg-grp slick-pg-grp-control">
        <span class="slick-pg-control">
            <span class="slick-pg-pagetext">${htmlEncode(localText("Controls.Pager.Page"))}</span>
            <input id="${this.idPrefix}CurrentPage" class="slick-pg-current" type="text" size="4" value="1" />
            <span class="slick-pg-pagesep">/</span>
            <span class="slick-pg-total">1</span>
        </span>
    </div>
    <div class="slick-pg-grp slick-pg-grp-nextlast">
        <div class="slick-pg-next slick-pg-btn"><span class="slick-pg-btn-span"></span></div>
        <div class="slick-pg-last slick-pg-btn"><span class="slick-pg-btn-span"></span></div>
    </div>
    <div class="slick-pg-grp slick-pg-grp-reload">
        <div class="slick-pg-reload slick-pg-btn"><span class="slick-pg-btn-span"></span></div>
    </div>
    <div class="slick-pg-grp slick-pg-grp-stat">
        <span class="slick-pg-stat"></span>
    </div>
</div>`);

        var self = this;
        $('.slick-pg-reload', this.element).click(function () { v.populate() });
        $('.slick-pg-first', this.element).click(function () { self._changePage('first') });
        $('.slick-pg-prev', this.element).click(function () { self._changePage('prev') });
        $('.slick-pg-next', this.element).click(function () { self._changePage('next') });
        $('.slick-pg-last', this.element).click(function () { self._changePage('last') });
        $('.slick-pg-current', this.element).keydown(function (e) { if (e.keyCode == 13) self._changePage('input') });

        if (self.options.showRowsPerPage) {
            var opt: string = "", sel = "";
            for (var nx = 0; nx < o.rowsPerPageOptions.length; nx++) {
                if (v.rowsPerPage == o.rowsPerPageOptions[nx])
                    sel = 'selected="selected"'; else sel = '';
                opt += "<option value='" + o.rowsPerPageOptions[nx] + "' " + sel + " >" + o.rowsPerPageOptions[nx] + "&nbsp;&nbsp;</option>";
            };
            $('.slick-pg-in', this.element).prepend('<div class="slick-pg-grp"><select class="slick-pg-size" name="rp">' + opt + '</select></div>');
            $('select.slick-pg-size', this.element).change(
                function (this: HTMLSelectElement) {
                    if (o.onRowsPerPageChange)
                        o.onRowsPerPageChange(+this.value);
                    else {
                        v["newp"] = 1;
                        v.setPagingOptions({
                            page: 1,
                            rowsPerPage: +this.value
                        });
                    }
                }
            );
        }

        v.onPagingInfoChanged.subscribe(function () {
            self._updatePager();
        });
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
                var nv = parseInt($('input.slick-pg-current', this.element).val());
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
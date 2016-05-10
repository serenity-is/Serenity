interface JQuery {
    flexHeightOnly(flexY?: number): JQuery;
    flexWidthOnly(flexX?: number): JQuery;
    flexWidthHeight(flexX: number, flexY: number): JQuery;
    flexX(flexX: number): JQuery;
    flexY(flexY: number): JQuery;
}

if ($.fn.button && $.fn.button.noConflict) {
    let btn = $.fn.button.noConflict();
    $.fn.btn = btn;
}

$.fn.flexHeightOnly = function (flexY = 1) {
    return this.flexWidthHeight(0, flexY);
}

$.fn.flexWidthOnly = function (flexX = 1) {
    return this.flexWidthHeight(flexX, 0);
}

$.fn.flexWidthHeight = function (flexX = 1, flexY = 1) {
    return this.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
}

$.fn.flexX = function (flexX: number): JQuery {
    return this.data('flex-x', flexX);
}

$.fn.flexY = function (flexY): JQuery {
    return this.data('flex-y', flexY);
}
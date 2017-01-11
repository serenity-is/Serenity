if ($.fn.button && $.fn.button.noConflict) {
    let btn = $.fn.button.noConflict();
    $.fn.btn = btn;
}
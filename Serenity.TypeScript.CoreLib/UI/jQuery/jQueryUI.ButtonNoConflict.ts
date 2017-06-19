if ($.fn.button && ($.fn.button as any).noConflict) {
    let btn = ($.fn.button as any).noConflict();
    ($.fn as any).btn = btn;
}
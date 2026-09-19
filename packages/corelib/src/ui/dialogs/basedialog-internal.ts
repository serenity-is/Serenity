import { addClass, getjQuery } from "../../base";
import { isMobileView, layoutFillHeight } from "../../compat";

function getCssSize(element: HTMLElement, name: string): number {
    let cssSize = getComputedStyle(element).getPropertyValue(name);
    if (cssSize == null || !cssSize.endsWith('px'))
        return null;

    cssSize = cssSize.substring(0, cssSize.length - 2);
    const i = parseInt(cssSize, 10);
    if (i == null || isNaN(i) || i == 0)
        return null;

    return i;
}

export function applyCssSizes(opt: any, dialogClass: string) {
    let size: number;
    const dialog = document.createElement("div");
    try {
        dialog.hidden = true;
        addClass(dialog, dialogClass);
        document.body.append(dialog);

        const sizeHelper = document.createElement("div");
        sizeHelper.classList.add("size");
        dialog.append(sizeHelper);
        size = getCssSize(sizeHelper, 'minWidth');
        if (size != null)
            opt.minWidth = size;

        size = getCssSize(sizeHelper, 'width');
        if (size != null)
            opt.width = size;

        size = getCssSize(sizeHelper, 'height');
        if (size != null)
            opt.height = size;

        size = getCssSize(sizeHelper, 'minHeight');
        if (size != null)
            opt.minHeight = size;
    }
    finally {
        dialog.remove();
    }
};

export function handleUIDialogResponsive(domNode: HTMLElement) {
    const $ = getjQuery();
    if (!$)
        return;

    const uiDialog = $(domNode).closest('.ui-dialog');
    if (!uiDialog.length)
        return;

    const dlg = ($(domNode) as any)?.dialog();
    if (!dlg)
        return;

    if (isMobileView()) {
        let data = $(domNode).data('responsiveData');
        if (!data) {
            data = {};
            data.draggable = dlg.dialog('option', 'draggable');
            data.resizable = dlg.dialog('option', 'resizable');
            data.position = dlg.css('position');
            const pos = uiDialog.position();
            data.left = pos.left;
            data.top = pos.top;
            data.width = uiDialog.width();
            data.height = uiDialog.height();
            data.contentHeight = $(domNode).height();
            $(domNode).data('responsiveData', data);
            dlg.dialog('option', 'draggable', false);
            dlg.dialog('option', 'resizable', false);
        }
        uiDialog.addClass('mobile-layout');
        uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px', position: 'fixed' });
        $(document.body).scrollTop(0);
        layoutFillHeight(domNode);
    }
    else {
        const d = $(domNode).data('responsiveData');
        if (d) {
            dlg.dialog('option', 'draggable', d.draggable);
            dlg.dialog('option', 'resizable', d.resizable);
            $(domNode).closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px', position: d.position });
            $(domNode).height(d.contentHeight);
            uiDialog.removeClass('mobile-layout');
            $(domNode).removeData('responsiveData');
        }
    }
}

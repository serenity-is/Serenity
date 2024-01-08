import { getjQuery, isArrayLike } from "@serenity-is/base";

export {}

export namespace TabsExtensions {
    export function setDisabled(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, isDisabled: boolean) {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs)
            return;

        var ibk = indexByKey(tabs);
        if (!ibk)
            return;

        var index = ibk[tabKey];
        if (index == null) {
            return;
        }

        let $ = getjQuery();
        if (!$)
            return;

        if (isDisabled && index === $(tabs).tabs?.('option', 'active')) {
            $(tabs).tabs?.('option', 'active', 0);
        }

        $(tabs).tabs?.(isDisabled ? 'disable' : 'enable', index);
    }

    export function toggle(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, visible: boolean) {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs)
            return;

        var ibk = indexByKey(tabs);
        if (!ibk)
            return;

        var index = ibk[tabKey];
        if (index == null) {
            return;
        }

        let $ = getjQuery();
        if (!$)
            return;

        if (!visible && index === $(tabs).tabs?.('option', 'active')) {
            $(tabs).tabs?.('option', 'active', 0);
        }

        $(tabs).children('ul').children('li').eq(index).toggle(visible);
    }

    export function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement) {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs)
            return;

        let $ = getjQuery();
        if (!$)
            return;
    
        var href = $(tabs).children('ul')
            .children('li')
            .eq($(tabs).tabs?.('option', 'active'))
            .children('a')
            .attr('href')
            .toString();

        var prefix = '_Tab';
        var lastIndex = href.lastIndexOf(prefix);
        if (lastIndex >= 0) {
            href = href.substr(lastIndex + prefix.length);
        }
        return href;
    }

    export function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number> {
        var indexByKey: Record<string, number> = {};
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs)
            return indexByKey;

        tabs.querySelectorAll<HTMLElement>(':scope > ul > li > a').forEach(function (el, index) {
            var href = el.getAttribute('href').toString();
            var prefix = '_Tab';
            var lastIndex = href.lastIndexOf(prefix);
            if (lastIndex >= 0) {
                href = href.substring(lastIndex + prefix.length);
            }
            indexByKey[href] = index;
        });

        return indexByKey;
    }
    
    export function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string) {
        var ibk = indexByKey(tabs);
        if (!ibk)
            return;
        var index = ibk[tabKey];
        if (index == null) {
            return;
        }
        var $ = getjQuery();
        if (!$)
            return;
        if (index !== $(tabs).tabs?.('option', 'active')) {
            $(tabs).tabs?.('option', 'active', index);
        }
    }
}

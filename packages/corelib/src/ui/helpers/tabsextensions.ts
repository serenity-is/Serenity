export {}

export namespace TabsExtensions {
    export function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean) {
        if (!tabs)
            return;

        var ibk = indexByKey(tabs);
        if (!ibk)
            return;

        var index = ibk[tabKey];
        if (index == null) {
            return;
        }

        if (isDisabled && index === (tabs as any).tabs('option', 'active')) {
            (tabs as any).tabs?.('option', 'active', 0);
        }

        (tabs as any).tabs?.(isDisabled ? 'disable' : 'enable', index);
    }

    export function toggle(tabs: JQuery, tabKey: string, visible: boolean) {
        if (!tabs)
            return;

        var ibk = indexByKey(tabs);
        if (!ibk)
            return;

        var index = ibk[tabKey];
        if (index == null) {
            return;
        }

        if (!visible && index === (tabs as any).tabs?.('option', 'active')) {
            (tabs as any).tabs?.('option', 'active', 0);
        }

        tabs.children('ul').children('li').eq(index).toggle(visible);
    }

    export function activeTabKey(tabs: JQuery) {
        var href = tabs.children('ul')
            .children('li')
            .eq((tabs as any).tabs?.('option', 'active'))
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

    export function indexByKey(tabs: JQuery): any {
        var indexByKey = tabs.data('indexByKey');
        if (!indexByKey) {
            indexByKey = {};
            tabs.children('ul').children('li').children('a').each(function (index, el) {
                var href = el.getAttribute('href').toString();
                var prefix = '_Tab';
                var lastIndex = href.lastIndexOf(prefix);
                if (lastIndex >= 0) {
                    href = href.substr(lastIndex + prefix.length);
                }
                indexByKey[href] = index;
            });
            tabs.data('indexByKey', indexByKey);
        }

        return indexByKey;
    }
    
    export function selectTab(tabs: JQuery, tabKey: string) {
        var ibk = indexByKey(tabs);
        if (!ibk)
            return;
        var index = ibk[tabKey];
        if (index == null) {
            return;
        }
        if (index !== (tabs as any).tabs?.('option', 'active')) {
            (tabs as any).tabs?.('option', 'active', index);
        }
    }
}

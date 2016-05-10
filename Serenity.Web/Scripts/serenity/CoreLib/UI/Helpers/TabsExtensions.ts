namespace Serenity {
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

            if (index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }

            tabs.tabs(isDisabled ? 'disable' : 'enable', index);
        }

        export function activeTabKey(tabs: JQuery) {
            var href = tabs.children('ul')
                .children('li')
                .eq(tabs.tabs('option', 'active'))
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
    }
}
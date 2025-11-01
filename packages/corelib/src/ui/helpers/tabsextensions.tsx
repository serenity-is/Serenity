import { Fluent, getjQuery, isArrayLike } from "../../base";

export { };

export namespace TabsExtensions {
    var navLinkSelector = ":scope > ul > li > a.nav-link, :scope > li > a.nav-link, :scope > a.nav-link, :scope > ul > li > a.ui-tabs-anchor, :scope > li > a.ui-tabs-anchor";
    var navLinkSelectorActive = ":scope > ul > li > a.nav-link.active, :scope > li > a.nav-link.active, :scope > a.nav-link.active, :scope > ul > li.ui-tabs-active > a, :scope > li.ui-tabs-active > a";

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

        if (typeof tabs === "string")
            return;
        let $ = getjQuery();
        if (!$ || !$(tabs)?.data?.().uiTabs) {
            var anchors = Array.from(tabs.querySelectorAll<HTMLElement>(navLinkSelector));
            if (index < anchors.length) {
                if (isDisabled && anchors[index].classList.contains("active")) {
                    var newIndex = anchors.findIndex((x, i) => i !== index && !x.classList.contains("disabled"));
                    if (newIndex >= 0)
                        anchors[newIndex].click();
                }
                anchors[index].classList.toggle("disabled", !!isDisabled);

            }
            return;
        }

        if (isDisabled && index === $(tabs)?.tabs?.('option', 'active')) {
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

        if (typeof tabs === "string")
            return;
        let $ = getjQuery();
        if (!$ || !$(tabs).data?.().uiTabs) {
            var anchors = Array.from(tabs.querySelectorAll<HTMLAnchorElement>(navLinkSelector));
            if (index < anchors.length) {
                if (!visible && anchors[index].classList.contains("active")) {
                    var newIndex = anchors.findIndex((x, i) => i !== index && !x.classList.contains("disabled") && getComputedStyle(x).display !== "none");
                    if (newIndex >= 0)
                        anchors[newIndex].click();
                }
                anchors[index].hidden = !visible;
            }
            return;
        }

        if (!visible && index === $(tabs).tabs?.('option', 'active')) {
            $(tabs).tabs?.('option', 'active', 0);
        }

        $(tabs).children('ul').children('li').eq(index).toggle(visible);
    }

    export function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement) {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs || typeof tabs === "string")
            return;

        let $ = getjQuery();
        if (!$ || !$(tabs).data?.().uiTabs) {
            return extractTabKey(tabs.querySelector<HTMLAnchorElement>(navLinkSelectorActive));
        }

        return extractTabKey($(tabs).children('ul')
            .children('li')
            .eq($(tabs).tabs?.('option', 'active'))
            .children('a')[0]);
    }

    function extractTabKey(el: HTMLAnchorElement) {
        if (!el)
            return "";
        var tabKey = el.dataset.tabkey;
        if (tabKey)
            return tabKey;
        var href = el.getAttribute('href').toString();
        var prefix = '_Tab';
        var lastIndex = href.lastIndexOf(prefix);
        if (lastIndex >= 0) {
            return href.substring(lastIndex + prefix.length);
        }
        return href;
    }

    export function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number> {
        var indexByKey: Record<string, number> = {};
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs)
            return indexByKey;

        tabs.querySelectorAll<HTMLAnchorElement>(navLinkSelector).forEach(function (el, index) {
            indexByKey[extractTabKey(el)] = index;
        });

        return indexByKey;
    }

    export function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string | number) {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs || typeof tabs === "string")
            return;

        var index: number;
        if (typeof tabKey === "number")
            index = tabKey;
        else {
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
        }
        var $ = getjQuery();
        if (!$ || !$(tabs)?.data?.().uiTabs) {
            var anchors = Array.from(tabs.querySelectorAll<HTMLAnchorElement>(navLinkSelector));
            if (index < anchors.length) {
                anchors[index].click();
            }
            return;
        }
        if (index !== $(tabs).tabs?.('option', 'active')) {
            $(tabs).tabs?.('option', 'active', index);
        }
    }

    export function initialize(tabs: HTMLElement | ArrayLike<HTMLElement>, activeChange: () => void): Fluent<HTMLElement> {
        tabs = isArrayLike(tabs) ? tabs[0] : tabs;
        if (!tabs || typeof tabs === "string")
            return null;

        let $ = getjQuery();
        if ($?.fn?.tabs) {
            var t = $(tabs).tabs?.({});
            if (activeChange)
                t?.on('tabsactivate', activeChange);
            return Fluent(tabs);
        }
        else {
            // emulate UI tabs with bootstrap
            let ul = tabs.matches("ul") ? tabs : tabs.querySelector(":scope > ul");
            if (ul && !ul.classList.contains("nav-tabs") && !ul.classList.contains("nav-underline")) {
                ul.classList.add("nav", "nav-tabs");

                let activeLink: HTMLLinkElement;

                ul.querySelectorAll(":scope > li").forEach(li => {
                    li.classList.add("nav-item");

                    let a = li.querySelector(":scope > a") as HTMLLinkElement;
                    if (a) {
                        a.classList.add("nav-link");
                        a.dataset.bsToggle = "tab";
                        a.setAttribute("role", "tab");
                        if (a.classList.contains("ui-tabs-active")) {
                            a.classList.add("active");
                            a.classList.remove("ui-tabs-active");
                            activeLink = a;
                        }
                    }
                });

                if (!activeLink) {
                    activeLink = ul.querySelector(":scope > li > a");
                    if (activeLink) {
                        activeLink.classList.add("active");
                    }
                }

                const container = tabs.appendChild(<div class="tab-content" />);
                tabs.querySelectorAll(":scope>.tab-pane").forEach(pane => {
                    pane.classList.add("pt-3");
                    container.appendChild(pane);
                    if (activeLink && activeLink.getAttribute("href") === "#" + pane.id) {
                        pane.classList.add("show", "active");
                    }
                });
            }

            if (activeChange) {
                Fluent.on(tabs, "shown.bs.tab", activeChange);
            }

            return Fluent(tabs);
        }
    }

    export function destroy(tabs: HTMLElement | ArrayLike<HTMLElement>): void {
        if (!tabs || typeof tabs === "string")
            return;

        let $ = getjQuery();
        if ($?.(tabs).data?.().uiTabs) {
            $(tabs)?.tabs?.("destroy");
            return;
        }

        if (isArrayLike(tabs))
            tabs = tabs[0];

        if (tabs && typeof bootstrap !== "undefined" && bootstrap?.Tab?.getInstance) {
            for (const anchor of Array.from(tabs.querySelectorAll<HTMLElement>(navLinkSelector))) {
                const inst = bootstrap.Tab.getInstance(anchor);
                if (inst && typeof inst.dispose === "function") {
                    inst.dispose();
                    break;
                }
            }
        }
    }
}

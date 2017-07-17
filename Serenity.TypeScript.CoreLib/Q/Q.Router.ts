
namespace Q.Router {
    let oldURL: string;
    let resolving: number = 0;
    let autoinc: number = 0;
    let listenerTimeout: number;
    export let enabled: boolean = true;

    export function navigate(hash: string, tryBack?: boolean, silent?: boolean) {
        if (!enabled || resolving > 0)
            return;

        hash = hash || '';
        hash = hash.replace(/^#/, '');
        hash = (!hash ? "" : '#' + hash);
        var newURL = window.location.href.replace(/#$/, '')
            .replace(/#.*$/, '') + hash;
        if (newURL != window.location.href) {
            if (tryBack && (oldURL == newURL ||
                oldURL == newURL + '#' ||
                oldURL + '#' == newURL)) {
                if (silent)
                    ignoreChange();
                window.history.back();
                if (window.location.href == oldURL)
                    return;
            }

            if (silent)
                ignoreChange();
            window.location.hash = hash;
        }
    }

    export function replace(hash: string, tryBack?: boolean) {
        navigate(hash, tryBack, true);
    }

    export function replaceLast(hash: string, tryBack?: boolean) {
        if (!enabled)
            return;

        var current = window.location.hash || '';
        if (current.charAt(0) == '#')
            current = current.substr(1, current.length - 1);

        var parts = current.split('/+/');

        if (parts.length > 1) {
            if (hash && hash.length) {
                parts[parts.length - 1] = hash;
                hash = parts.join("/+/");
            }
            else {
                parts.splice(parts.length - 1, 1);
                hash = parts.join("/+/");
            }
        }
        replace(hash, tryBack);
    }

    function dialogOpen(owner: JQuery, element: JQuery, hash: () => string) {
        var route = [];
        var isDialog = owner.hasClass(".ui-dialog-content");
        var dialog = isDialog ? owner :
            owner.closest('.ui-dialog-content');
        var value = hash();

        var idPrefix: string;
        if (dialog.length) {
            var dialogs = $('.ui-dialog-content:visible');
            var index = dialogs.index(dialog[0]);

            for (var i = 0; i <= index; i++) {
                var q = dialogs.eq(i).data("qroute") as string;
                if (q && q.length)
                    route.push(q);
            }

            if (!isDialog) {
                idPrefix = dialog.attr("id");
                if (idPrefix) {
                    idPrefix += "_";
                    var id = owner.attr("id");
                    if (id && Q.startsWith(id, idPrefix))
                        value = id.substr(idPrefix.length) + '@' + value;
                }
            }
        }
        else {
            var id = owner.attr("id");
            if (id && (!owner.hasClass("route-handler") ||
                $('.route-handler').first().attr("id") != id))
                value = id + "@" + value;
        }

        route.push(value);
        element.data("qroute", value);
        replace(route.join("/+/"));

        element.bind("dialogclose.qrouter", e => {
            element.data("qroute", null);
            element.unbind(".qrouter");
            replaceLast("", true);
        });
    }

    export function dialog(owner: JQuery, element: JQuery, hash: () => string) {
        if (!enabled)
            return;
        
        element.bind("dialogopen.qrouter", e => {
            dialogOpen(owner, element, hash);
        });
    }

    export function resolve(hash?: string) {
        if (!enabled)
            return;

        resolving++;
        try {
            hash = Q.coalesce(Q.coalesce(hash, window.location.hash), '');
            if (hash.charAt(0) == '#')
                hash = hash.substr(1, hash.length - 1);

            var newParts = hash.split("/+/");
            var dialogs = $('.ui-dialog-content:visible');
            var oldParts = dialogs.toArray()
                .map(el => $(el).data('qroute') as string);

            var same = 0;
            while (same < dialogs.length &&
                same < newParts.length &&
                oldParts[same] == newParts[same]) {
                same++;
            }

            for (var i = same; i < dialogs.length; i++)
                dialogs.eq(i).dialog('close');

            for (var i = same; i < newParts.length; i++) {
                var route = newParts[i];
                var routeParts = route.split('@');
                var handler: JQuery;
                if (routeParts.length == 2) {
                    var dialog = i > 0 ? $('.ui-dialog-content:visible').eq(i - 1) : $([]);
                    if (dialog.length) {
                        var idPrefix = dialog.attr("id");
                        if (idPrefix) {
                            handler = $('#' + idPrefix + "_" + routeParts[0]);
                            if (handler.length) {
                                route = routeParts[1];
                            }
                        }
                    }

                    if (!handler || !handler.length) {
                        handler = $('#' + routeParts[0]);
                        if (handler.length) {
                            route = routeParts[1];
                        }
                    }
                }

                if (!handler || !handler.length) {
                    handler = i > 0 ? $('.ui-dialog-content:visible').eq(i - 1) :
                        $('.route-handler').first();
                }

                handler.triggerHandler("handleroute", {
                    handled: false,
                    route: route,
                    parts: newParts,
                    index: i
                });
            }
        }
        finally {
            resolving--;
        }
    }

    function hashChange(e: any, o: string) {
        oldURL = (e && e.oldURL) || o;
        resolve();
    }

    function ignoreChange() {
        window.clearTimeout(listenerTimeout);
        window.removeEventListener("hashchange", hashChange as any);
        setTimeout(function () {
            window.addEventListener("hashchange", hashChange as any, false);
        }, 1);
    }

    $(document).on("dialogopen", ".ui-dialog-content", function (event, ui) {
        if (!enabled)
            return;

        var dlg = $(event.target);
        if (dlg.data("qroute"))
            return;

        var owner = $('.ui-dialog-content:visible').not(dlg).last();
        if (!owner.length)
            owner = $('html');

        dialogOpen(owner, dlg, () => {
            return "!" + (++autoinc).toString(36);
        });
    });
}
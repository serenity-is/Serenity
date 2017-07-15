
namespace Q.Router {
    let oldURL: string;
    let setURL: string;
    let replacing: number = 0;
    let resolving: number = 0;

    export function navigate(hash: string, tryBack?: boolean) {
        if (resolving > 0)
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
                setURL = replacing <= 0 ? null : oldURL;
                window.history.back();
                if (window.location.href == oldURL)
                    return;
            }

            setURL = replacing <= 0 ? null : newURL;
            window.location.hash = hash;
        }
    }

    export function replace(hash: string, tryBack?: boolean) {
        replacing++;
        try {
            navigate(hash, tryBack);
        }
        finally {
            replacing--;
        }
    }

    export function replaceLast(hash: string, tryBack?: boolean) {
        var current = window.location.hash || '';
        if (current.charAt(0) == '#')
            current = current.substr(1, current.length - 1);

        var parts = current.split('/+/');

        if (parts.length > 1) {
            if (hash && hash.length) {
                parts[parts.length - 1] = hash;
                hash = parts.join("/+/");
            }
            else
                hash = parts.splice(parts.length - 1, 1).join("/+/");
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
                        value = id.substr(0, idPrefix.length) + '@' + value;
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
        element.bind("dialogopen.qrouter", e => {
            dialogOpen(owner, element, hash);
        });
    }

    export function resolve(hash?: string) {
        if (replacing > 0)
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

        if (setURL && setURL == window.location.href) {
            setURL = null;
            return;
        }

        setURL = null;
        resolve();
    }

    window.addEventListener("hashchange", hashChange as any);
    $(document).on("dialogopen", ".ui-dialog-content", function (event, ui) {
        var dlg = $(event.target);
        if (dlg.data("qroute"))
            return;

        var owner = $('.ui-dialog-content:visible').not(dlg).last();
        if (!owner.length)
            owner = $('body');

        dialogOpen(owner, dlg, () => {
            return dlg.attr("id") ||
                new Date().getTime().toString();
        });
    });
}
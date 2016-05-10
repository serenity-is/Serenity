namespace Q {
    export interface PostToServiceOptions {
        url?: string;
        service?: string;
        target?: string;
        request: any;
    }

    export interface PostToUrlOptions {
        url?: string;
        target?: string;
        params: any;
    }

    export function parseQueryString(s?: string): {} {
        let qs: string;
        if (s === undefined)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        let result = {};
        let parts = qs.split('&');
        for (let i = 0; i < parts.length; i++) {
            let pair = parts[i].split('=');
            let name = decodeURIComponent(pair[0]);
            result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
        }
        return result;
    }

    export function postToService(options: Q.PostToServiceOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/services/' + options.service))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        $('<input/>').attr('type', 'hidden').attr('name', 'request')
            .val($['toJSON'](options.request))
            .appendTo(div);
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }

    export function postToUrl(options: Q.PostToUrlOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', resolveUrl(options.url))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        if (options.params != null) {
            for (let k in options.params) {
                $('<input/>').attr('type', 'hidden').attr('name', k)
                    .val(options.params[k])
                    .appendTo(div);
            }
        }
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }

    export function resolveUrl(url) {
        if (url && url.substr(0, 2) === '~/') {
            return Config.applicationPath + url.substr(2);
        }

        return url;
    }
}
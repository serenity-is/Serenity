import { Fluent, ListRequest, getCookie, isSameOrigin, resolveServiceUrl, resolveUrl } from "../base";

export function setEquality(request: ListRequest, field: string, value: any) {
    if (request.EqualityFilter == null) {
        request.EqualityFilter = {};
    }
    request.EqualityFilter[field] = value;
}

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
    let result: Record<string, string> = {};
    let parts = qs.split('&');
    for (let i = 0; i < parts.length; i++) {
        let pair = parts[i].split('=');
        let name = decodeURIComponent(pair[0]);
        result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
    }
    return result;
}

function postToCommon(url: string, div: HTMLElement) {
    
    if (isSameOrigin(resolveUrl(url))) {
        var csrfToken = getCookie('CSRF-TOKEN');
        if (csrfToken) {
            Fluent("input")
                .attr('type', 'hidden')
                .attr('name', '__RequestVerificationToken')
                .appendTo(div).val(csrfToken);
        }
    }

    Fluent("input")
        .attr('type', 'submit')
        .appendTo(div);

    div.closest("form").submit();
    window.setTimeout(function () {
        div.closest("form").remove();
    }, 0);
}

export function postToService(options: PostToServiceOptions) {
    let url = options.url ? (resolveUrl(options.url)) : resolveServiceUrl(options.service);
    let form = Fluent("form")
        .attr('method', 'POST')
        .attr('action', url)
        .appendTo(document.body);

    if (options.target)
        form.attr('target', options.target);

    postToCommon(url, Fluent("div").appendTo(form).append(
        Fluent("input")
            .attr('type', 'hidden')
            .attr('name', 'request')
            .val(JSON.stringify(options.request))).getNode());
}

export function postToUrl(options: PostToUrlOptions) {
    let url = resolveUrl(options.url);
    let form = Fluent("form")
        .attr('method', 'POST')
        .attr('action', url)
        .appendTo(document.body);

    if (options.target)
        form.attr('target', options.target);

    let div = Fluent("div").appendTo(form);
    if (options.params != null) {
        for (let k in options.params) {
            Fluent("input")
                .attr('type', 'hidden')
                .attr('name', k)
                .val(options.params[k])
                .appendTo(div);
        }
    }

    postToCommon(url, div.getNode());
}

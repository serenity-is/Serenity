import { extend } from "./system";
import { Config } from "./config"
import { ErrorHandling } from "./errorhandling";
import { alertDialog, iframeDialog } from "./dialogs";
import { blockUI, blockUndo } from "./blockui";
import { ListRequest, ServiceOptions, ServiceResponse } from "./servicetypes";

export function getCookie(name: string) {
    if (($ as any).cookie)
        return ($ as any).cookie(name);

    name += '=';
    for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
        if (!ca[i].indexOf(name))
            return ca[i].replace(name, '');
}

typeof $ != 'undefined' && $.ajaxSetup && $.ajaxSetup({
    beforeSend: function (xhr, opt) {
        if (!opt || !opt.crossDomain) {
            var token = getCookie('CSRF-TOKEN');
            if (token)
                xhr.setRequestHeader('X-CSRF-TOKEN', token);
        }
    }
});

export function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>) {
    let handleError = function (response: any) {
        if (Config.notLoggedInHandler != null &&
            response &&
            response.Error &&
            response.Error.Code == 'NotLoggedIn' &&
            Config.notLoggedInHandler(options, response)) {
            return;
        }

        if (options.onError != null) {
            options.onError(response);
            return;
        }

        ErrorHandling.showServiceError(response.Error);
    };

    var url = options.service;
    if (url && url.length && url.charAt(0) != '~' && url.charAt(0) != '/' && url.indexOf('://') < 0)
        url = resolveUrl("~/Services/") + url;

    options = extend<ServiceOptions<TResponse>>({
        dataType: 'json',
        contentType: 'application/json',
        type: 'POST',
        cache: false,
        blockUI: true,
        url: url,
        data: JSON.stringify(options.request),
        success: function (data: any, textStatus: string, request: any) {
            try {
                if (!data.Error && options.onSuccess) {
                    options.onSuccess(data);
                }
            }
            finally {
                if (options.blockUI) {
                    blockUndo();
                }
                options.onCleanup && options.onCleanup();
            }
        },
        error: function (xhr: any, status: any, ev: any) {
            try {
                if (xhr.status === 403) {
                    var l: any = null;
                    try {
                        l = xhr.getResponseHeader('Location');
                    }
                    catch ($t1) {
                        l = null;
                    }
                    if (l) {
                        window.top.location.href = l;
                        return;
                    }
                }
                if ((xhr.getResponseHeader('content-type') || '')
                    .toLowerCase().indexOf('application/json') >= 0) {
                    var json = $.parseJSON(xhr.responseText);
                    if (json && json.Error) {
                        handleError(json);
                        return;
                    }
                }
                var html = xhr.responseText;
                if (!html) {
                    if (!xhr.status) {
                        if (xhr.statusText != "abort")
                            alertDialog("An unknown AJAX connection error occurred! Check browser console for details.");
                    }
                    else if (xhr.status == 500)
                        alertDialog("HTTP 500: Connection refused! Check browser console for details.");
                    else
                        alertDialog("HTTP " + xhr.status + ' error! Check browser console for details.');
                }
                else
                    iframeDialog({ html: html });
            }
            finally {
                if (options.blockUI) {
                    blockUndo();
                }
                options.onCleanup && options.onCleanup();
            }
        }
    }, options);

    if (options.blockUI) {
        blockUI(null);
    }

    return $.ajax(options);
}

export function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any,
    onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>) {
    return serviceCall(extend<ServiceOptions<TResponse>>({
        service: service,
        request: request,
        onSuccess: onSuccess
    }, options));
}

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

export function postToService(options: PostToServiceOptions) {
    let form = $('<form/>')
        .attr('method', 'POST')
        .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/Services/' + options.service))
        .appendTo(document.body);
    if (options.target)
        form.attr('target', options.target);
    let div = $('<div/>').appendTo(form);
    $('<input/>').attr('type', 'hidden').attr('name', 'request')
        .val($['toJSON'](options.request))
        .appendTo(div);
    var csrfToken = getCookie('CSRF-TOKEN');
    if (csrfToken) {
        $('<input/>').attr('type', 'hidden').attr('name', '__RequestVerificationToken')
            .appendTo(div).val(csrfToken);
    }
    $('<input/>').attr('type', 'submit')
        .appendTo(div);
    form.submit();
    window.setTimeout(function () { form.remove(); }, 0);
}

export function postToUrl(options: PostToUrlOptions) {
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
    var csrfToken = getCookie('CSRF-TOKEN');
    if (csrfToken) {
        $('<input/>').attr('type', 'hidden').attr('name', '__RequestVerificationToken')
            .appendTo(div).val(csrfToken);
    }
    $('<input/>').attr('type', 'submit')
        .appendTo(div);
    form.submit();
    window.setTimeout(function () { form.remove(); }, 0);
}

export function resolveUrl(url: string) {
    if (url && url.substr(0, 2) === '~/') {
        return Config.applicationPath + url.substr(2);
    }

    return url;
}
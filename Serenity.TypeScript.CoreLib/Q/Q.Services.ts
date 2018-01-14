
namespace Q {

    export function getCookie(name: string) {
        if ($.cookie)
            return $.cookie(name);

        name += '=';
        for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
            if (!ca[i].indexOf(name))
                return ca[i].replace(name, '');
    }

    $.ajaxSetup({
        beforeSend: function (xhr) {
            var token = Q.getCookie('CSRF-TOKEN');
            if (token)
                xhr.setRequestHeader('X-CSRF-TOKEN', token);
        }
    });

    export interface ServiceOptions<TResponse extends Serenity.ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }

    export function serviceCall<TResponse>(options: Q.ServiceOptions<TResponse>) {
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
            url = resolveUrl("~/services/") + url;

        options = $.extend<Q.ServiceOptions<TResponse>>({
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST',
            cache: false,
            blockUI: true,
            url: url,
            data: $.toJSON(options.request),
            success: function (data, textStatus, request) {
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
            error: function (xhr, status, ev) {
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
                        if (!xhr.status)
                            Q.alert("An unknown AJAX connection error occured! Check browser console for details.");
                        else if (xhr.status == 500)
                            Q.alert("HTTP 500: Connection refused! Check browser console for details.");
                        else
                            Q.alert("HTTP " + xhr.status + ' error! Check browser console for details.');
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

    export function serviceRequest<TResponse>(service: string, request?: any,
        onSuccess?: (response: TResponse) => void, options?: Q.ServiceOptions<TResponse>) {
        return serviceCall($.extend<Q.ServiceOptions<TResponse>>({
            service: service,
            request: request,
            onSuccess: onSuccess
        }, options));
    }

    export function setEquality(request: Serenity.ListRequest, field: string, value: any) {
        if (request.EqualityFilter == null) {
            request.EqualityFilter = {};
        }
        request.EqualityFilter[field] = value;
    }
}

namespace Serenity {
    export type ServiceOptions<TResponse extends ServiceResponse> = Q.ServiceOptions<TResponse>;
}
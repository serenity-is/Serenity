using System;
using System.Html;
using System.Net;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Collections.Generic;

namespace Serenity
{
    public static partial class Q
    {
        [InlineCode("Q.serviceCall({options})")]
        public static extern XmlHttpRequest ServiceCall(ServiceCallOptions options);

        [ScriptName("serviceCall"), IncludeGenericArguments(false)]
        public static jQueryXmlHttpRequest ServiceCall<TResponse>(ServiceCallOptions<TResponse> options)
            where TResponse: ServiceResponse
        {
            Action<ServiceResponse> handleError = delegate(ServiceResponse response) 
            {
                if (Q.Config.NotLoggedInHandler != null &&
                    response != null &&
                    response.Error != null &&
                    response.Error.Code == "NotLoggedIn" &&
                    Q.Config.NotLoggedInHandler(options.As<ServiceCallOptions>(), response))
                                return;

                    if (options.OnError != null)
                        options.OnError(response);
                    else
                        Q.ErrorHandling.ShowServiceError(response.Error);
            };

            options = jQuery.ExtendObject(new ServiceCallOptions<TResponse>
            {
                DataType = "json",
                ContentType = "application/json",
                Type = "POST",
                Cache = false,
                BlockUI = true,
                Url = options.Service != null && !options.Service.StartsWith('~') && !options.Service.StartsWith('/') ?
                    Q.ResolveUrl("~/services/" + options.Service) : Q.ResolveUrl(options.Service),
                Data = Q.ToJSON(options.Request),
                Success = (data, textStatus, request) =>
                {
                    var response = data.As<TResponse>();
                    try
                    {
                        if (response.Error == null)
                        {
                            if (options.OnSuccess != null)
                                options.OnSuccess(response);
                        }
                        else
                        {
                        }
                    }
                    finally
                    {
                        if (options.BlockUI)
                            Q.BlockUndo();

                        if (options.OnCleanup != null)
                            options.OnCleanup();
                    }
                },
                Error = (xhr, status, ev) =>
                {
                    try
                    {
                        if (xhr.Status == 403)
                        {
                            string l = null;
                            try { l = xhr.GetResponseHeader("Location"); }
                            catch { l = null; }
                            if (l != null)
                            {
                                Window.Top.Location.Href = l;
                                return;
                            }
                        }

                        if ((xhr.GetResponseHeader("content-type") ?? "").ToLower().IndexOf("application/json") >= 0)
                        {
                            var json = jQuery.ParseJsonData<ServiceResponse>(xhr.ResponseText);
                            if (json != null && json.Error != null)
                            {
                                handleError(json);
                                return;
                            }
                        }

                        var html = xhr.ResponseText;

                        Q.Externals.IFrameDialog(new { html = html });
                    }
                    finally
                    {
                        if (options.BlockUI)
                            Q.BlockUndo();

                        if (options.OnCleanup != null)
                            options.OnCleanup();
                    }
                }
            }, options);

            if (options.BlockUI)
                Q.BlockUI();

            return jQuery.Ajax(options.As<jQueryAjaxOptions>());
        }

        [IncludeGenericArguments(false)]
        public static void ServiceRequest<TResponse>(string service, ServiceRequest request, Action<TResponse> onSuccess, ServiceCallOptions options = null)
            where TResponse: ServiceResponse
        {
            ServiceCall(jQuery.ExtendObject(new ServiceCallOptions<TResponse>
            {
                Service = service,
                Request = request,
                OnSuccess = onSuccess
            }, options.As<ServiceCallOptions<TResponse>>()));
        }
    }
}
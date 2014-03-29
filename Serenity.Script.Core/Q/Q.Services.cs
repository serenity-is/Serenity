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
        [AlternateSignature]
        public static extern XmlHttpRequest ServiceCall(ServiceCallOptions options);

        [ScriptName("serviceCall"), IncludeGenericArguments(false)]
        public static jQueryXmlHttpRequest ServiceCall<TResponse>(ServiceCallOptions<TResponse> options)
            where TResponse: ServiceResponse
        {
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
                    //try
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
                            }
                        }
                        finally
                        {
                            if (options.BlockUI)
                                Q.BlockUndo();

                            if (options.OnCleanup != null)
                                options.OnCleanup();
                        }
                    }
                    //catch (Exception e)
                    //{
                    //    StackTrace.Log(e);
                    //}
                },
                Error = (xhr, status, ev) =>
                {
                    //try
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
                    //catch (Exception e)
                    //{
                    //    StackTrace.Log(e);
                    //}
                }
            }, options);

            if (options.BlockUI)
                Q.BlockUI();

            return jQuery.Ajax(options.As<jQueryAjaxOptions>());
        }
    }
}
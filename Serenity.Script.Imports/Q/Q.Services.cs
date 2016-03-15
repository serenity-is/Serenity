using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        [InlineCode("Q.serviceCall({options})")]
        public static extern jQueryXmlHttpRequest ServiceCall(ServiceCallOptions options);

        [InlineCode("Q.serviceCall({options})")]
        public static jQueryXmlHttpRequest ServiceCall<TResponse>(ServiceCallOptions<TResponse> options)
            where TResponse: ServiceResponse
        {
            return null;
        }

        [InlineCode("Q.serviceRequest({service}, {request}, {onSuccess}, {options})")]
        public static jQueryXmlHttpRequest ServiceRequest<TResponse>(string service, ServiceRequest request, Action<TResponse> onSuccess, ServiceCallOptions options = null)
            where TResponse: ServiceResponse
        {
            return null;
        }

        [InlineCode("Q.setEquality({request}, {field},  {value})")]
        public static void SetEquality(this ListRequest request, string field, object value)
        {
        }

        [InlineCode("Q.postToService({options})")]
        public static void PostToService(PostToServiceOptions options)
        {
        }

        [InlineCode("Q.postToUrl({options})")]
        public static void PostToUrl(PostToUrlOptions options)
        {
        }

    }
}
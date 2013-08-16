using System;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Collections.Generic;

namespace Serenity
{
    /// <summary>
    /// Options for the Q.ServiceCall function
    /// </summary>
    [Imported, Serializable]
    public class ServiceCallOptions<TResponse>
        where TResponse: ServiceResponse
    {
        /// <summary>
        /// Gets or sets service URL
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets a request object to pass to service
        /// </summary>
        public ServiceRequest Request { get; set; }

        /// <summary>
        /// Gets or sets a function to call on success with response
        /// </summary>
        public Action<TResponse> OnSuccess { get; set; }

        /// <summary>
        /// Gets or sets a function to call on error
        /// </summary>
        public Action<ServiceResponse> OnError { get; set; }

        /// <summary>
        /// Gets or sets a function to call to clean resources, after success or error
        /// </summary>
        public Action OnCleanup { get; set; }

        /// <summary>
        /// Gets or sets whether to block UI during request (default true)
        /// </summary>
        public bool BlockUI { get; set; }

        /// <summary>
        /// Gets or sets whether the request is async.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the callback to invoke before the request is sent.
        /// </summary>
        public AjaxSendingCallback BeforeSend { get; set; }

        /// <summary>
        /// Gets or sets whether the request can be cached.
        /// </summary>
        public bool Cache { get; set; }

        /// <summary>
        /// Gets or sets the callback invoked after the request is completed
        /// and success or error callbacks have been invoked.
        /// </summary>
        public AjaxCompletedCallback Complete { get; set; }

        /// <summary>
        /// Gets or sets the content type of the data sent to the server.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the element that will be the context for the request.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// A map of dataType-to-dataType converters. Each converter's value is a function that returns the transformed value of the response.
        /// </summary>
        public JsDictionary<string, Func<string, object>> Converters { get; set; }

        /// <summary>
        /// Gets or sets the data to be sent to the server.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the data type expected in response from the server.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets the callback to be invoked if the request fails.
        /// </summary>
        public AjaxErrorCallback Error { get; set; }

        /// <summary>
        /// Gets or sets the function to be used to handle the raw response data of XMLHttpRequest.
        /// </summary>
        public AjaxDataFilterCallback DataFilter { get; set; }

        /// <summary>
        /// Gets or sets whether to trigger global event handlers for this Ajax request.
        /// </summary>
        public bool Global { get; set; }

        /// <summary>
        /// Gets or sets whether the request is successful only if its been modified since
        /// the last request.
        /// </summary>
        public bool IfModified { get; set; }

        /// <summary>
        /// Gets or sets whether the current environment should be treated as a local
        /// environment (eg. when the page is loaded using file://).
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Gets or sets the callback parameter name to use for JSONP requests.
        /// </summary>
        public string Jsonp { get; set; }

        /// <summary>
        /// Gets or sets the callback name to use for JSONP requests.
        /// </summary>
        public string JsonpCallback { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the request.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the password to be used for an HTTP authentication request.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets whether the data passed in will be processed.
        /// </summary>
        public bool ProcessData { get; set; }

        /// <summary>
        /// Gets or sets how to handle character sets for script and JSONP requests.
        /// </summary>
        public string ScriptCharset { get; set; }

        /// <summary>
        /// Gets or sets the function to invoke upon successful completion of the request.
        /// </summary>
        public AjaxRequestCallback Success { get; set; }

        /// <summary>
        /// Gets or sets the timeout in milliseconds for the request.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets if you want to use traditional parameter serialization.
        /// </summary>
        public bool Traditional { get; set; }

        /// <summary>
        /// Gets or sets the type or HTTP verb associated with the request.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the URL to be requested.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of the user to use in a HTTP authentication request.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the function creating the XmlHttpRequest instance.
        /// </summary>
        [ScriptName("xhr")]
        public XmlHttpRequestCreator XmlHttpRequestCreator { get; set; }

        /// <summary>
        /// Gets or sets a set of additional name/value pairs set of the XmlHttpRequest
        /// object.
        /// </summary>
        public JsDictionary<string, object> XhrFields { get; set; }

    }

    [Imported, Serializable]
    public class ServiceCallOptions : ServiceCallOptions<ServiceResponse>
    {
    }
}

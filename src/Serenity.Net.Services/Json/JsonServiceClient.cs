using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Serenity.Services;

/// <summary>
/// A JSON service client implementation
/// </summary>
public class JsonServiceClient
{
    /// <summary>
    /// Cookie container
    /// </summary>
    protected CookieContainer cookies = new();

    /// <summary>
    /// Creates an instance of JsonServiceClient for the passed baseUrl
    /// </summary>
    /// <param name="baseUrl">The base url</param>
    public JsonServiceClient(string baseUrl)
    {
        BaseUrl = baseUrl;
        cookies = new CookieContainer();
    }

    /// <summary>
    /// Base url for the client
    /// </summary>
    protected string BaseUrl { get; set; }

    /// <summary>
    /// Post to JSON service
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected</typeparam>
    /// <param name="relativeUrl">Relative url</param>
    /// <param name="request">Request object</param>
    /// <returns></returns>
    public virtual TResponse Post<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        return InternalPost<TResponse>(relativeUrl, request);
    }

    /// <summary>
    /// Posts to a JSON service, internal version
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="relativeUrl">Relative url</param>
    /// <param name="request">The request object</param>
    /// <returns>The response</returns>
    /// <exception cref="ValidationError">Throws a validation error exception
    /// if the returned response contains a service error.</exception>
    protected TResponse InternalPost<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(UriHelper.Combine(BaseUrl, relativeUrl));
        wr.Method = "POST";
        var r = JsonConvert.SerializeObject(request, Formatting.None, JsonSettings.Strict);
        wr.ContentType = "application/json";
        var rb = System.Text.Encoding.UTF8.GetBytes(r);

        wr.CookieContainer = cookies;
        wr.ContinueTimeout = 10 * 60 * 1000;
        using (var requestStream = Task.Run(() => wr.GetRequestStreamAsync()).Result)
            requestStream.Write(rb, 0, rb.Length);

        using var response = Task.Run(() => wr.GetResponseAsync()).Result;
        using var rs = response.GetResponseStream();
        using var sr = new StreamReader(rs);
        var rt = sr.ReadToEnd();
        var resp = JsonConvert.DeserializeObject<TResponse>(rt, JsonSettings.Tolerant);

        if (resp is ServiceResponse serviceResponse &&
            serviceResponse.Error != null)
        {
            throw new ValidationError(
                serviceResponse.Error.Code,
                serviceResponse.Error.Arguments,
                serviceResponse.Error.Message);
        }

        return resp;
    }
}
using System.Net;
using System.Net.Sockets;

namespace Serenity.Services;

public class JsonServiceClientTests
{
    private class TestRequest
    {
        public int A { get; set; }
    }

    private class TestResponse
    {
        public int Value { get; set; }
    }

    private class ErrorResponse : ServiceResponse
    {
    }

    private sealed record CapturedRequest(string Method, string Path, string ContentType, string Body);

    private static CapturedRequest ServeOnce(TcpListener listener, string responseJson)
    {
        using var client = listener.AcceptTcpClient();
        using var stream = client.GetStream();

        var requestText = new StringBuilder();
        int b;
        while ((b = stream.ReadByte()) >= 0)
        {
            requestText.Append((char)b);
            if (requestText.Length >= 4 &&
                requestText[^4] == '\r' && requestText[^3] == '\n' &&
                requestText[^2] == '\r' && requestText[^1] == '\n')
                break;
        }

        var lines = requestText.ToString().Split("\r\n");
        var requestLine = lines[0].Split(' ');
        var method = requestLine[0];
        var path = requestLine[1];

        int contentLength = 0;
        var contentType = "";
        var expectContinue = false;
        foreach (var line in lines)
        {
            var idx = line.IndexOf(':');
            if (idx <= 0)
                continue;

            var name = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim();
            if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                contentLength = int.Parse(value);
            else if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                contentType = value;
            else if (name.Equals("Expect", StringComparison.OrdinalIgnoreCase) &&
                value.Contains("100-continue", StringComparison.OrdinalIgnoreCase))
                expectContinue = true;
        }

        if (expectContinue)
        {
            var cont = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
            stream.Write(cont, 0, cont.Length);
            stream.Flush();
        }

        var bodyBytes = new byte[contentLength];
        var read = 0;
        while (read < contentLength)
        {
            var n = stream.Read(bodyBytes, read, contentLength - read);
            if (n <= 0)
                break;
            read += n;
        }
        var body = Encoding.UTF8.GetString(bodyBytes, 0, read);

        var respBytes = Encoding.UTF8.GetBytes(responseJson);
        var header = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: " +
            respBytes.Length + "\r\nConnection: close\r\n\r\n";
        var headerBytes = Encoding.ASCII.GetBytes(header);
        stream.Write(headerBytes, 0, headerBytes.Length);
        stream.Write(respBytes, 0, respBytes.Length);
        stream.Flush();

        return new CapturedRequest(method, path, contentType, body);
    }

    private static (JsonServiceClient client, Task<CapturedRequest> server) CreateClient(string responseJson)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var server = Task.Run(() => ServeOnce(listener, responseJson));
        return (new JsonServiceClient($"http://127.0.0.1:{port}/"), server);
    }

    [Fact]
    public async Task Post_Sends_Request_And_Parses_Response()
    {
        var (client, server) = CreateClient("{\"Value\":42}");

        var response = client.Post<TestResponse>("svc/test", new TestRequest { A = 7 });

        var captured = await server;
        Assert.Equal(42, response.Value);
        Assert.Equal("POST", captured.Method);
        Assert.Equal("/svc/test", captured.Path);
        Assert.Equal("application/json", captured.ContentType);
        Assert.Contains("\"A\":7", captured.Body);
    }

    [Fact]
    public async Task Post_Throws_ValidationError_For_ServiceError()
    {
        var (client, server) = CreateClient(
            "{\"Error\":{\"Code\":\"TestCode\",\"Message\":\"TestMessage\",\"Arguments\":\"TestArgs\"}}");

        var ex = Assert.Throws<ValidationError>(() =>
            client.Post<ErrorResponse>("svc/test", new TestRequest()));

        Assert.Equal("TestCode", ex.ErrorCode);
        Assert.Equal("TestMessage", ex.Message);
        Assert.Equal("TestArgs", ex.Arguments);
        await server;
    }

    [Fact]
    public async Task InternalPost_Combines_BaseUrl_With_RelativeUrl()
    {
        var (client, server) = CreateClient("{\"Value\":1}");

        client.Post<TestResponse>("a/b", new TestRequest());

        var captured = await server;
        Assert.Equal("/a/b", captured.Path);
    }
}

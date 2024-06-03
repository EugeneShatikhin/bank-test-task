using RestSharp;

namespace TestTask.Client;

public class ClientServiceClient
{
    public readonly RestClient Native;

    public const string PostEndpoint = "/client";

    protected readonly KeyValuePair<string, string> ContentTypeHeader = new ("Content-Type", "application/json");

    public ClientServiceClient(RestClient client)
    {
        Native = client;
    }

    public RestResponse PostClient<T>(T requestBody, ICollection<KeyValuePair<string, string>> headers = null!) where T : class
    {
        headers ??= new List<KeyValuePair<string, string>>()
        {
            ContentTypeHeader
        };

        if (!headers.Any(h => h.Key == ContentTypeHeader.Key)) headers.Add(ContentTypeHeader);

        var request = new RestRequest(PostEndpoint, Method.Post)
            .AddJsonBody(requestBody)
            .AddHeaders(headers);

        return Native.Execute(request);
    }
}
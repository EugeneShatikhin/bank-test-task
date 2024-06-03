using RestSharp;
using TestTask.Client;
using TestTask.Extension;
using TestTask.Model;

namespace TestTask.Service;

public class ClientService
{
    public readonly ClientServiceClient ServiceClient;

    public ClientService(RestClient client)
    {
        ServiceClient = new ClientServiceClient(client);
    }

    public PostClientResponseDto PostClient(PostClientRequestDto requestBody) => ServiceClient.PostClient(requestBody).Deserialize<PostClientResponseDto>()!;
}
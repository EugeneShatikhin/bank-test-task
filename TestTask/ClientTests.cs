using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;
using TestTask.Client;
using TestTask.Configuration;
using TestTask.Extension;
using TestTask.Model;
using TestTask.Service;

namespace TestTask;

public class ClientTests
{
    private readonly string BaseUrl;

    private readonly ClientService _clientService;

    public ClientTests()
    {
        var config = ConfigurationService.Get<ClientConfiguration>();

        BaseUrl = config.BaseUrl;

        _clientService = new ClientService(new RestClient(BaseUrl));
    }

    [Test]
    public void ValidRequestBody_Returns200()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<PostClientResponseDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData.Name.Should().Be("test");
        responseData.Age.Should().Be(1);
        responseData.AdditionalInfo.Should().Be("addition_info");
    }

    [Test]
    public void MissingId_Returns400()
    {
        var requestDto = new { Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Errors.Should().ContainSingle(e => e.Message == "Missing required field 'id'");
    }

    [Test]
    public void MissingName_Returns400()
    {
        var requestDto = new { Id = 1 };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Errors.Should().ContainSingle(e => e.Message == "Missing required field 'name'");
    }

    [Test]
    public void InvalidId_Returns400()
    {
        var requestDto = new { Id = "invalid", Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Errors.Should().ContainSingle(e => e.Message == "'id' must be an integer");
    }

    [Test]
    public void EmptyName_Returns400()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Errors.Should().ContainSingle(e => e.Message == "'name' cannot be empty");
    }

    [Test]
    public void InvalidContentType_Returns415()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var headers = new List<KeyValuePair<string, string>>()
            {
                new ("Content-Type", "text/plain")
            };
        var response = _clientService.ServiceClient.PostClient(requestDto, headers);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        responseData.Errors.Should().ContainSingle(e => e.Message == "Unsupported Media Type. Expected 'application/json'");
    }

    [Test]
    public void AdditionalFields_Returns200()
    {
        var requestDto = new { Id = 1, Name = "test", Extra = "field" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<PostClientResponseDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData.Name.Should().Be("test");
        responseData.Age.Should().Be(1);
        responseData.AdditionalInfo.Should().Be("addition_info");
    }

    [Test]
    public void InvalidMethod_Returns405()
    {
        var request = new RestRequest(ClientServiceClient.PostEndpoint, Method.Get);
        var client = new RestClient(BaseUrl);
        var response = client.Execute(request);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        responseData.Errors.Should().ContainSingle(e => e.Message == "Method Not Allowed");
    }

    [Test]
    public void ResponseTime_IsAcceptable()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var response = _clientService.ServiceClient.PostClient(requestDto);
        watch.Stop();

        watch.ElapsedMilliseconds.Should().BeLessOrEqualTo(500);
    }

    [Test]
    public void ResponseHeaders_AreCorrect()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Any(h => h.Name == "Content-Type" && h.Value.ToString() == "application/json").Should().BeTrue();
    }

    [Test]
    public void SQLInjection_ReturnsError()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "'; DROP TABLE clients; --" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<ErrorDto>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseData.Errors.Should().ContainSingle(e => e.Message == "Invalid input");
    }

    [Test]
    public void BoundaryValues_ReturnsCorrectResponse()
    {
        var requestDto = new PostClientRequestDto { Id = int.MaxValue, Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        requestDto = new PostClientRequestDto { Id = 0, Name = "test" };
        response = _clientService.ServiceClient.PostClient(requestDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public void Idempotency_ReturnsSameResponse()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var response1 = _clientService.ServiceClient.PostClient(requestDto);
        var response2 = _clientService.ServiceClient.PostClient(requestDto);

        response1.Content.Should().Be(response2.Content);
    }

    [Test]
    public void ResponseBodySchema_IsValid()
    {
        var requestDto = new PostClientRequestDto { Id = 1, Name = "test" };
        var response = _clientService.ServiceClient.PostClient(requestDto);
        var responseData = response.Deserialize<PostClientResponseDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.Should().NotBeNull();
        responseData.Name.Should().BeOfType<string>();
        responseData.Age.Should().BeOfType(typeof(int));
        responseData.AdditionalInfo.Should().BeOfType<string>();
    }
}
using RestSharp;
using System.Text.Json;

namespace TestTask.Extension;

public static class RestResponseExtensions
{
    public static T Deserialize<T>(this RestResponse response) where T : class
    {
        return JsonSerializer.Deserialize<T>(response.Content!)!;
    }
}
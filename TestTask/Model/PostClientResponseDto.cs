using System.Text.Json.Serialization;

namespace TestTask.Model;

public class PostClientResponseDto : ErrorDto
{
    public string? Name { get; set; }

    public int? Age { get; set; }

    [JsonPropertyName("adi")]
    public string? AdditionalInfo { get; set; }
}
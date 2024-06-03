namespace TestTask.Model;

public class ErrorDto
{
    public Error[]? Errors { get; set; }
}

public class Error
{
    public string Code { get; set; }

    public string Message { get; set; }
}
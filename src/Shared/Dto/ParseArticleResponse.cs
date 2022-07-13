namespace Dto;

public class ParseArticleResponse
{
    public ParseArticleResponse(string? title, string? contentHtml, string? contentText, bool isSuccess, string? message = null)
    {
        Title = title;
        ContentHtml = contentHtml;
        ContentText = contentText;
        IsSuccess = isSuccess;
        Message = message;
    }

    public string? Title { get; }
    public string? ContentHtml { get; }
    public string? ContentText { get; }
    public bool IsSuccess { get; }
    public string? Message { get; }
}
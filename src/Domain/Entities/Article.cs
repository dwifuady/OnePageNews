using Domain.Common;

namespace Domain.Entities;
public class Article : BaseEntity
{
    public string StringUrl { get; }
    public Uri Url => new(StringUrl);
    public string? Title { get; private set; }
    public ArticleContent? Content { get; private set; }
    public ProviderEnum Provider { get; private set; }

    /// <summary>
    /// list of url with page
    /// </summary>
    public IEnumerable<string>? AllPageUrls { get; private set; }
    
    /// <summary>
    /// single url that contains all content in a single url
    /// </summary>
    public string? AllPageUrl { get; private set; }

    public ParseResult? ParseResult { get; private set; }

    public Article(string url)
    {
        StringUrl = url;
    }

    public void SetAllPageUrls(IEnumerable<string> urls) => AllPageUrls = urls;

    public void SetAllPageUrl(string url) => AllPageUrl = url;

    public void SetTitle(string title) => Title = title;
    
    public void SetParseSuccess(string title, ArticleContent content)
    {
        ParseResult = new ParseResult(true);
        Title = title;
        Content = content;
    }

    public void SetParseFailed(string reason)
    {
        ParseResult = new ParseResult(false, reason);
    }

    public void SetProvider(ProviderEnum provider)
    {
        Provider = provider;
    }
}

public class ParseResult
{
    public bool IsSuccess { get; }

    public string? Reason { get; }

    public ParseResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public ParseResult(bool isSuccess, string reason)
    {
        IsSuccess = isSuccess;
        Reason = reason;
    }
}

public struct ArticleContent
{
    public ArticleContent(string contentHtml, string contentText)
    {
        ContentHtml = contentHtml;
        ContentText = contentText;
    }

    public string ContentHtml { get; }
    public string ContentText { get; }
}
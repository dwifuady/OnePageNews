using System.Text;
using Domain.Common;
using HtmlAgilityPack;

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
    public string? SinglePageUrl { get; private set; }

    public ParseResult? ParseResult { get; private set; }

    public bool HasSinglePageUrl => !string.IsNullOrWhiteSpace(SinglePageUrl);

    private IEnumerable<HtmlDocument>? HtmlDocuments { get; set; }

    private bool IsDocumentsLoaded => HtmlDocuments != null && HtmlDocuments.Any();

    public Article(string url)
    {
        StringUrl = url;
    }

    public void SetAllPageUrls(string xPath)
    {
        if (!IsDocumentsLoaded)
        {
            throw new Exception("Article was not loaded");
        }
        
        var urls = new List<string>();
        var nodes = HtmlDocuments?.FirstOrDefault()?.DocumentNode.SelectNodes(xPath);
        if (nodes is not null)
        {
            urls.AddRange(nodes.Select(node => node.GetAttributeValue("href", string.Empty)));
        }

        AllPageUrls = urls;
    }

    public void SetSinglePageUrl(string xPath)
    {
        if (!IsDocumentsLoaded)
        {
            throw new Exception("Article was not loaded");
        }
        
        var node = HtmlDocuments?.FirstOrDefault()?.DocumentNode.SelectSingleNode(xPath);
        if (node is not null)
        {
            SinglePageUrl = node.GetAttributeValue("href", string.Empty);
        }
    }
    
    public void SetParseSuccess(string title, ArticleContent content)
    {
        ParseResult = new ParseResult(true);
        Title = title;
        Content = content;
    }

    public void SetParseSuccess()
    {
        ParseResult = new ParseResult(true);
    }
    
    public void SetParseFailed(string reason)
    {
        ParseResult = new ParseResult(false, reason);
    }

    public void SetHtmlDocuments(IEnumerable<HtmlDocument> htmlDocuments)
    {
        HtmlDocuments = htmlDocuments;
    }

    public void ParseTitle(string xPath)
    {
        if (!IsDocumentsLoaded)
        {
            throw new Exception("Article was not loaded");
        }
        
        Title = HtmlDocuments?.FirstOrDefault()?.DocumentNode.SelectSingleNode(xPath)?.InnerText?.Trim() ?? "";
    }

    public void ParseContent(string xPath, List<ElementRule>? rules)
    {
        if (!IsDocumentsLoaded)
        {
            throw new Exception("Article was not loaded");
        }
        
        var contentHtml = new StringBuilder();
        var contentText = new StringBuilder();
        
        foreach (var htmlDoc in HtmlDocuments!)
        {
            var content = GetContent(htmlDoc, xPath, rules);
            contentHtml.Append(content.Html);
            contentText.Append(content.Text);
        }
        
        Content = new ArticleContent(contentHtml.ToString(), contentText.ToString());
    }

    private static (string Html, string Text) GetContent(HtmlDocument htmlDoc, string xPath, List<ElementRule>? rules)
    {
        var contentHtml = new StringBuilder();
        var contentText = new StringBuilder();
        var contentNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);
        var allParagraphs = contentNode.SelectNodes("//p");

        foreach (var paragraph in allParagraphs)
        {
            var innerHtml = paragraph.InnerHtml.Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace("\"", "&quot;").Trim();
            var skippableElement = string.IsNullOrWhiteSpace(innerHtml);
            var endOfArticle = false;

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    switch (rule.RuleEnum)
                    {
                        case ElementRuleEnum.Contain:
                            if (innerHtml.Contains(rule.RuleValue))
                            {
                                if (rule.ElementType.Equals(ElementType.End))
                                {
                                    endOfArticle = true;
                                }
                                else
                                {
                                    skippableElement = true;
                                }
                            }
                            break;
                        case ElementRuleEnum.StartsWith:
                            if (innerHtml.StartsWith(rule.RuleValue))
                            {
                                if (rule.ElementType.Equals(ElementType.End))
                                {
                                    endOfArticle = true;
                                }
                                else
                                {
                                    skippableElement = true;
                                }
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }

            if (endOfArticle) break;
            if (skippableElement) continue;
            
            contentHtml.Append($"<p>{innerHtml}</p>");
            contentText.AppendLine(paragraph.InnerText);
        }
        
        return (contentHtml.ToString(), contentText.ToString());
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
using System.Text;
using Domain.Common;
using Domain.Entities;
using HtmlAgilityPack;

namespace Application.Scrapper;

public class DetikScrapper : BaseScrapper, IScrapper
{
    private const string ConstTitleDetik = "//h1[@class='detail__title']";
    private const string ConstContentDetik = "//*[@class='detail__body-text itp_bodycontent']";
    private const string ConstNextPageDetik = "//*[@class='detail__long-nav']";
    private const string ConstAllPageDetik = "//*[@class='detail__anchor-numb ']";

    public DetikScrapper(HttpClient httpClient) : base(httpClient)
    {
    }
    
    public ProviderEnum Provider => ProviderEnum.Detik;

    public async Task<Article> Parse(Article article)
    {
        try
        {
            var htmlDocs = new List<HtmlDocument>
            {
                await GetHtmlDocument(article.StringUrl)
            };

            SetAllPageUrl(article, htmlDocs.FirstOrDefault());

            if (article.AllPageUrls is not null)
            {
                var docs = await GetHtmlDocuments(article.AllPageUrls);
                htmlDocs.AddRange(docs);
            }

            var title = ParseTitle(htmlDocs.FirstOrDefault()!);
            var content = ParseContent(htmlDocs);
            
            article.SetParseSuccess(title, new ArticleContent(content.Html, content.Text));
        }
        catch (Exception e)
        {
            article.SetParseFailed($"{e.Message} {e.StackTrace}");
        }

        return article;
    }
    public string ParseTitle(HtmlDocument htmlDocument)
    {
        if (htmlDocument is null)
        {
            throw new ArgumentNullException($"Can't parse Title {nameof(htmlDocument)} is null");
        }

        return htmlDocument.DocumentNode.SelectSingleNode(ConstTitleDetik)?.InnerText?.Trim() ?? "";
    }

    public (string Html, string Text) ParseContent(IEnumerable<HtmlDocument> htmlDocuments)
    {
        var contentHtml = new StringBuilder();
        var contentText = new StringBuilder();
        foreach (var htmlDoc in htmlDocuments)
        {
            var content = GetContent(htmlDoc);
            contentHtml.Append(content.Html);
            contentText.Append(content.Text);
        }

        return (contentHtml.ToString(), contentText.ToString());
    }

    private void SetAllPageUrl(Article article, HtmlDocument? htmlDocument)
    {
        var urls = new List<string>();
        var nodes = htmlDocument?.DocumentNode.SelectNodes(ConstAllPageDetik);
        if (nodes is not null)
        {
            urls.AddRange(nodes.Select(node => node.GetAttributeValue("href", string.Empty)));
        }

        article.SetAllPageUrls(urls);
    }

    private (string Html, string Text) GetContent(HtmlDocument htmlDoc)
    {
        var contentHtml = new StringBuilder();
        var contentText = new StringBuilder();
        var contentNode = htmlDoc.DocumentNode.SelectSingleNode(ConstContentDetik);

        var allParagraphs = contentNode.SelectNodes("//p");
        foreach (var paragraph in allParagraphs)
        {
            var innerHtml = paragraph.InnerHtml.Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace("\"", "&quot;").Trim();
            if (string.IsNullOrWhiteSpace(innerHtml) || innerHtml.StartsWith("<strong>") || innerHtml.StartsWith("<em>") || (innerHtml.StartsWith("<a href") && innerHtml.Contains("20detik")))
            {
                continue;
            }

            contentHtml.Append($"<p>{innerHtml}</p>");
            contentText.Append(paragraph.InnerText);
        }

        return (contentHtml.ToString(), contentText.ToString());
    }
}
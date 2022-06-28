using System.Text;
using Domain.Common;
using Domain.Entities;
using HtmlAgilityPack;

namespace Application.Scrapper;

public class KompasScrapper : BaseScrapper, IScrapper
{
    private const string ConstAllPage = "//*[@class='paging__link paging__link--show']";
    private const string ConstTitle = "//h1[@class='read__title']";
    private const string ConstContent = "//*[@class='read__content']";
    private const string ConstContentSub = "//*[@class='clearfix']";
    public KompasScrapper(HttpClient httpClient) : base(httpClient)
    {
    }

    public ProviderEnum Provider => ProviderEnum.Kompas;
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
            else if (article.AllPageUrl is not null)
            {
                htmlDocs.Clear();
                htmlDocs.Add(await GetHtmlDocument(article.AllPageUrl));
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

    private void SetAllPageUrl(Article article, HtmlDocument? htmlDocument)
    {
        var node = htmlDocument?.DocumentNode.SelectSingleNode(ConstAllPage);
        if (node is not null)
        {
            article.SetAllPageUrl(node.GetAttributeValue("href", string.Empty));
        }
    }

    public string ParseTitle(HtmlDocument htmlDocument)
    {
        if (htmlDocument is null)
        {
            throw new ArgumentNullException($"Can't parse Title {nameof(htmlDocument)} is null");
        }

        return htmlDocument.DocumentNode.SelectSingleNode(ConstTitle)?.InnerText?.Trim() ?? "";
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

    private (string Html, string Text) GetContent(HtmlDocument htmlDoc)
    {
        var contentHtml = new StringBuilder();
        var contentText = new StringBuilder();
        var contentNode = htmlDoc.DocumentNode.SelectSingleNode(ConstContent);

        var allParagraphs = contentNode.SelectSingleNode(ConstContentSub).SelectNodes("//p");
        foreach (var paragraph in allParagraphs)
        {
            var innerHtml = paragraph.InnerHtml.Replace("\r\n", "").Replace("\n", "").Replace("\t", "").Replace("\"", "&quot;").Trim();
            if (string.IsNullOrWhiteSpace(innerHtml) || innerHtml.StartsWith("<strong>") || innerHtml.StartsWith("<em>"))
            {
                continue;
            }
            if (paragraph.InnerText.Contains("Tulis komentarmu dengan tagar")) break;

            contentHtml.Append($"<p>{innerHtml}</p>");
            contentText.AppendLine(paragraph.InnerText);
        }

        return (contentHtml.ToString(), contentText.ToString());
    }
}
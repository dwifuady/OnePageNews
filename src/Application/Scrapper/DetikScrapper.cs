using Domain.Common;
using Domain.Entities;
using HtmlAgilityPack;

namespace Application.Scrapper;

public class DetikScrapper : BaseScrapper, IScrapper
{
    private const string ConstTitleDetik = "//h1[@class='detail__title']";
    private const string ConstContentDetik = "//*[@class='detail__body-text itp_bodycontent']";
    private const string ConstAllPageDetik = "//*[@class='detail__anchor-numb ']";
    private List<ElementSkipRule> _elementSkipRules;

    public DetikScrapper(HttpClient httpClient) : base(httpClient)
    {
        _elementSkipRules = new List<ElementSkipRule>
        {
            new(ElementSkipRuleEnum.StartsWith, "<strong>"),
            new(ElementSkipRuleEnum.StartsWith, "<em>"),
            new(ElementSkipRuleEnum.Contain, "20detik"),
            new(ElementSkipRuleEnum.Contain, "ADVERTISEMENT"),
            new(ElementSkipRuleEnum.Contain, "SCROLL TO RESUME CONTENT")
        };
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

            article.SetHtmlDocuments(htmlDocs);
            article.ParseTitle(ConstTitleDetik);
            article.ParseContent(ConstContentDetik, _elementSkipRules);
            
            article.SetParseSuccess();
        }
        catch (Exception e)
        {
            article.SetParseFailed($"{e.Message} {e.StackTrace}");
        }

        return article;
    }
    
    private static void SetAllPageUrl(Article article, HtmlDocument? htmlDocument)
    {
        var urls = new List<string>();
        var nodes = htmlDocument?.DocumentNode.SelectNodes(ConstAllPageDetik);
        if (nodes is not null)
        {
            urls.AddRange(nodes.Select(node => node.GetAttributeValue("href", string.Empty)));
        }

        article.SetAllPageUrls(urls);
    }
}
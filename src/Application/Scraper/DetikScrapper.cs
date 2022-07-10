using Domain.Common;
using Domain.Entities;
using HtmlAgilityPack;

namespace Application.Scraper;

public class DetikScraper : BaseScraper, IScraper
{
    private const string ConstTitleXPath = "//h1[@class='detail__title']";
    private const string ConstContentXPath = "//*[@class='detail__body-text itp_bodycontent']";
    private const string ConstAllPageXPath = "//*[@class='detail__anchor-numb ']";
    private readonly List<ElementRule> _elementRules;

    public DetikScraper(HttpClient httpClient) : base(httpClient)
    {
        _elementRules = new List<ElementRule>
        {
            new(ElementRuleEnum.StartsWith, "<strong>", ElementType.Skip),
            new(ElementRuleEnum.StartsWith, "<em>", ElementType.Skip),
            new(ElementRuleEnum.Contain, "20detik", ElementType.Skip),
            new(ElementRuleEnum.Contain, "ADVERTISEMENT", ElementType.Skip),
            new(ElementRuleEnum.Contain, "SCROLL TO RESUME CONTENT", ElementType.Skip)
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

            article.SetHtmlDocuments(htmlDocs);
            article.SetAllPageUrls(ConstAllPageXPath);

            if (article.HasSinglePageUrl)
            {
                htmlDocs.Clear();
                htmlDocs.Add(await GetHtmlDocument(article.SinglePageUrl!));
            }
            else
            {
                if (article.AllPageUrls is not null)
                {
                    var docs = await GetHtmlDocuments(article.AllPageUrls!); 
                    htmlDocs.AddRange(docs);
                }
            }

            article.SetHtmlDocuments(htmlDocs);
            article.ParseTitle(ConstTitleXPath);
            article.ParseContent(ConstContentXPath, _elementRules);
            
            article.SetParseSuccess();
        }
        catch (Exception e)
        {
            article.SetParseFailed($"{e.Message} {e.StackTrace}");
        }

        return article;
    }
    
    public Task<Article> Parse(Article article, ParseConfig config)
    {
        throw new Exception("Only implemented by GeneralScraper");
    }
}
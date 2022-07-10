using Domain.Common;
using Domain.Entities;
using HtmlAgilityPack;

namespace Application.Scraper;

public class GeneralScraper : BaseScraper, IScraper
{
    public GeneralScraper(HttpClient httpClient) : base(httpClient)
    {
    }

    public ProviderEnum Provider => ProviderEnum.General;
    public Task<Article> Parse(Article article)
    {
        throw new Exception("Can't parse without config");
    }

    public async Task<Article> Parse(Article article, ParseConfig config)
    {
        try
        {
            var htmlDocs = new List<HtmlDocument>
            {
                await GetHtmlDocument(article.StringUrl)
            };
            
            article.SetHtmlDocuments(htmlDocs);
            if (config.HasSinglePageUrl)
            {
                article.SetSinglePageUrl(config.SinglePagePath!);
            }
            else if (config.HasAllPageUrl)
            {
                article.SetAllPageUrls(config.AllPagePath!);
            }

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
            article.ParseTitle(config.TitlePath);
            article.ParseContent(config.ContentPath, config.ElementRules);
            
            article.SetParseSuccess();
        }
        catch (Exception e)
        {
            article.SetParseFailed($"{e.Message}");
        }

        return article;
    }
}
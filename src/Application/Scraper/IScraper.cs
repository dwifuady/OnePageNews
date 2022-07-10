using Domain.Common;
using Domain.Entities;

namespace Application.Scraper;

public interface IScraper
{
    ProviderEnum Provider { get; }
    Task<Article> Parse(Article article);
    Task<Article> Parse(Article article, ParseConfig config);
}
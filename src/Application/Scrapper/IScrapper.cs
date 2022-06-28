using Domain.Common;
using Domain.Entities;

namespace Application.Scrapper;

public interface IScrapper
{
    ProviderEnum Provider { get; }
    Task<Article> Parse(Article article);
}
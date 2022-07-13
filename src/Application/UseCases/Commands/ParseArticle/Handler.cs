using Application.Extensions;
using Application.Scraper;
using Domain.Common;
using Domain.Entities;
using Dto;
using MediatR;

namespace Application.UseCases.Commands.ParseArticle
{
    public class Handler : IRequestHandler<ParseArticleRequest, ParseArticleResponse>
    {
        private readonly IEnumerable<IScraper> _scrappers;
        public Handler(IEnumerable<IScraper> scrappers)
        {
            _scrappers = scrappers;
        }
        public async Task<ParseArticleResponse> Handle(ParseArticleRequest request, CancellationToken cancellationToken)
        {
            var article = new Article(request.Url);

            article.SetProvider(article.Url.GetProviderEnum());
            
            var config = ConfigLoader.LoadConfig()?.SingleOrDefault(c => c.ProviderEnum.Equals(article.Provider));
            
            var scrapper = _scrappers.SingleOrDefault(s => s.Provider.Equals(article.Provider)) ?? _scrappers.SingleOrDefault(s => s.Provider.Equals(ProviderEnum.General));
            
            switch (scrapper)
            {
                case null:
                    article.SetParseFailed("Can't load scrapper");
                    break;
                case GeneralScraper when config is null:
                    article.SetParseFailed($"Can't load config for {article.Provider}");
                    break;
                case GeneralScraper:
                    await scrapper.Parse(article, config);
                    break;
                default:
                    await scrapper.Parse(article);
                    break;
            }
            
            return article.ParseResult is { IsSuccess: false } ? 
                new ParseArticleResponse("", "", "", false, article.ParseResult.Reason) : 
                new ParseArticleResponse(article.Title, article?.Content?.ContentHtml, article?.Content?.ContentText, true);
        }

    }
}

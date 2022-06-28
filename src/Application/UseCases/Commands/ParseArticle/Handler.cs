using Application.Extensions;
using Application.Scrapper;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ParseArticle
{
    public record ParseArticleRequest(string Url) : IRequest<ParseArticleResponse>;
    public record ParseArticleResponse(string? Title, string? ContentHtml, string? ContentText, bool IsSuccess);
    public class Handler : IRequestHandler<ParseArticleRequest, ParseArticleResponse>
    {
        private readonly IEnumerable<IScrapper> _scrappers;
        public Handler(IEnumerable<IScrapper> scrappers)
        {
            _scrappers = scrappers;
        }
        public async Task<ParseArticleResponse> Handle(ParseArticleRequest request, CancellationToken cancellationToken)
        {
            var article = new Article(request.Url);

            article.SetProvider(article.Url.GetProviderEnum());

            var scrapper = _scrappers.SingleOrDefault(s => s.Provider.Equals(article.Provider));
            
            if (scrapper == null)
            {
                article.SetParseFailed("Can't load scrapper");
            }
            else
            {
                await scrapper.Parse(article);
            }
            
            if (article.ParseResult is { IsSuccess: false })
            {
                throw new Exception(article.ParseResult.Reason);
            }

            return new ParseArticleResponse(article.Title, article?.Content?.ContentHtml, article?.Content?.ContentText, true);
        }

    }
}

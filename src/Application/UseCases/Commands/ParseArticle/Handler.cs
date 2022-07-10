﻿using Application.Extensions;
using Application.Scrapper;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Commands.ParseArticle
{
    public record ParseArticleRequest(string Url) : IRequest<ParseArticleResponse>;
    public record ParseArticleResponse(string? Title, string? ContentHtml, string? ContentText, bool IsSuccess, string? message = null);
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
            
            var config = ConfigLoader.LoadConfig()?.SingleOrDefault(c => c.ProviderEnum.Equals(article.Provider));
            
            var scrapper = _scrappers.SingleOrDefault(s => s.Provider.Equals(article.Provider)) ?? _scrappers.SingleOrDefault(s => s.Provider.Equals(ProviderEnum.General));
            
            switch (scrapper)
            {
                case null:
                    article.SetParseFailed("Can't load scrapper");
                    break;
                case GeneralScrapper when config is null:
                    article.SetParseFailed($"Can't load config for {article.Provider}");
                    break;
                case GeneralScrapper:
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

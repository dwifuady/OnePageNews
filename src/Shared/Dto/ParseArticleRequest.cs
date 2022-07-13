using MediatR;

namespace Dto;

public class ParseArticleRequest : IRequest<ParseArticleResponse>
{
    public ParseArticleRequest(string url)
    {
        Url = url;
    }

    public string Url { get; }
}
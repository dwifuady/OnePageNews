using Application.UseCases.Commands.ParseArticle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ParseController : ControllerBase
{
    private readonly IMediator _mediator;
    public ParseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Parse(ParseArticleRequest request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
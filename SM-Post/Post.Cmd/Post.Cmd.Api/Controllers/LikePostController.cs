using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController(
    ILogger<LikePostController> logger, 
    ICommandDispatcher commandDispatcher) : ControllerBase
{
    private readonly ILogger<LikePostController> _logger = logger;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

    [HttpPut("{id}")]
    public async Task<IActionResult> LikePostAsync(Guid id)
    {
        try
        {
            var command = new LikePostCommand { Id = id };

            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Post liked successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request.");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (AggregateNotFoundException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Aggregate not found.");
            return NotFound(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "An error occurred while liking the post.";
            _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    private readonly ILogger<NewPostController> _logger = logger;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

    [HttpPost]
    public async Task<IActionResult> NewPostAsync(NewPostCommand command)
    {
        var id = Guid.NewGuid();
        
        try 
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse 
            { 
                Message = "Post created successfully."
            });
        }
        catch(InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request.");
            return BadRequest(new BaseResponse 
            { 
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "An error occurred while creating a new post.";
            _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
            return StatusCode(500, new NewPostResponse
            {
                Id = id,
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}
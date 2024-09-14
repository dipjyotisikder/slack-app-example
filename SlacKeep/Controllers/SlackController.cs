using Microsoft.AspNetCore.Mvc;
using SlackNet.AspNetCore;
using SlackNet;
using SlackNet.WebApi;
using SlackNet.Events;

namespace SlacKeep.Controllers;

[ApiController]
[Route("slack")]
public class SlackController : ControllerBase
{
    private readonly ILogger<SlackController> _logger;
    private readonly ISlackRequestHandler _requestHandler;
    private readonly SlackEndpointConfiguration _endpointConfig;
    private readonly ISlackApiClient _slack;
    public SlackController(
        ILogger<SlackController> logger,
        ISlackRequestHandler requestHandler,
        SlackEndpointConfiguration endpointConfig,
        ISlackApiClient slack
        )
    {
        _logger = logger;
        _requestHandler = requestHandler;
        _endpointConfig = endpointConfig;
        _slack = slack;
    }

    [HttpPost("command")]
    public async Task<SlackResult> Command()
    {
        _logger.LogInformation("Command");
        return await _requestHandler.HandleSlashCommandRequest(HttpContext.Request);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SlackRequest request)
    {
        _logger.LogInformation("Send Request");
        await _slack.Chat.PostMessage(new Message
        {
            Text = request.Message,
            Channel = request.SlackChannel
        }, null);

        return Ok("Command");
    }

    [HttpPost("event")]
    public async Task<IActionResult> Event([FromBody] UrlVerification? urlVerification)
    {
        _logger.LogInformation("Received event");
        if (urlVerification is not null && !string.IsNullOrEmpty(urlVerification?.Challenge))
        {
            _logger.LogInformation("Handling URL Verification");
            return Ok(new { challenge = urlVerification.Challenge });
        }

        return Ok(await _requestHandler.HandleEventRequest(HttpContext.Request));
    }
}

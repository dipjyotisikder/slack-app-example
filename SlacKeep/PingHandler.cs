using SlackNet.Events;
using SlackNet.WebApi;
using SlackNet;

namespace SlacKeep;

/// <summary>
/// Represents a slack callback handler for slack bot.
/// </summary>
public class PingHandler(ISlackApiClient slack) : IEventHandler<MessageEvent>
{
    private static string PING = "ping";
    private static string PONG = "pong";

    private readonly ISlackApiClient _slack = slack;

    /// <summary>
    /// Represents a method that handles a slack message event.
    /// </summary>
    /// <param name="slackEvent">Event object from slack.</param>
    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text.Contains(PING))
        {
            await _slack.Chat
                .PostMessage(new Message
                {
                    Text = PONG,
                    Channel = slackEvent.Channel
                })
                .ConfigureAwait(false);
        }
    }
}

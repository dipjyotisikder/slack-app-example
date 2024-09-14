using SlackNet.AspNetCore;
using SlackNet.Events;
using SlacKeep;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DotNetEnv.Env.Load();
var accessToken = Environment.GetEnvironmentVariable("SLACK_ACCESS_TOKEN") ?? string.Empty;
var signingSecret = Environment.GetEnvironmentVariable("SLACK_SIGNING_SECRET") ?? string.Empty;

builder.Services
    .AddSlackNet(c => c
        .UseApiToken(accessToken)
        .UseSigningSecret(signingSecret)
        .RegisterEventHandler<MessageEvent, PingHandler>());

var app = builder.Build();

app.UseSlackNet();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
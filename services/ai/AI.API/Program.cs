using AI.Application.Abstractions;
using AI.Application.GetRecommendations;
using AI.Infrastructure;
using AI.Infrastructure.Clients;
using AI.Infrastructure.Options;
using AI.Infrastructure.Parsing;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("Gemini"));

builder.Services.AddHttpClient<IRecommendationProvider, GeminiRecommendationProvider>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});
builder.Services.AddScoped<IRecommendationPromptBuilder, RecommendationPromptBuilder>();
builder.Services.AddScoped<IRecommendationResponseParser, RecommendationResponseParser>();

builder.Services.AddScoped<GetRecommendationsHandler>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
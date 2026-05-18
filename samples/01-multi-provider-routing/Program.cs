// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Sassy Solutions">
//     Copyright (c) 2026 Sassy Solutions. Licensed under the MIT License.
//     See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------------

// Demonstrates calling two different upstream providers (OpenAI + Anthropic) through the same
// LiteLLM proxy endpoint, by changing only the model id on each request.
//
// Prereqs:
//   1. Run a LiteLLM proxy locally (default http://localhost:4000):
//        pip install "litellm[proxy]"
//        litellm --model gpt-4o-mini --model claude-haiku-4.5
//      (or point LITELLM_BASE_URL at a cloud / k8s deployment of your own).
//
//   2. Optionally set LITELLM_VIRTUAL_KEY if your proxy enforces virtual-key auth.

using Compendium.Abstractions.AI;
using Compendium.Abstractions.AI.Models;
using Compendium.Adapters.LiteLLM.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var baseUrl = Environment.GetEnvironmentVariable("LITELLM_BASE_URL") ?? "http://localhost:4000";
var virtualKey = Environment.GetEnvironmentVariable("LITELLM_VIRTUAL_KEY");

var services = new ServiceCollection();
services.AddLogging(b => b.SetMinimumLevel(LogLevel.Information));
services.AddCompendiumLiteLLM(opt =>
{
    opt.BaseUrl = baseUrl;
    opt.VirtualKey = virtualKey;
    opt.DefaultModel = "openai/gpt-4o-mini";

    // Forward LiteLLM-specific routing primitives on every outbound request.
    opt.PassThroughHeaders["x-litellm-tags"] = "sample=multi-provider-routing";
});

await using var sp = services.BuildServiceProvider();
var provider = sp.GetRequiredService<IAIProvider>();

string[] models =
{
    "openai/gpt-4o-mini",
    "anthropic/claude-haiku-4.5",
};

foreach (var model in models)
{
    Console.WriteLine($"--- {model} ---");

    var request = new CompletionRequest
    {
        Model = model,
        SystemPrompt = "You are a concise assistant. Answer in one sentence.",
        Messages = new List<Message> { Message.User("In one sentence, what is event sourcing?") },
        MaxTokens = 200
    };

    var result = await provider.CompleteAsync(request);
    if (result.IsFailure)
    {
        Console.Error.WriteLine($"  {model} failed: {result.Error.Code} — {result.Error.Message}");
        continue;
    }

    Console.WriteLine($"  Finish reason: {result.Value.FinishReason}");
    Console.WriteLine($"  Tokens (prompt+completion): {result.Value.Usage.PromptTokens}+{result.Value.Usage.CompletionTokens}");
    Console.WriteLine($"  Answer: {result.Value.Content}");
    Console.WriteLine();
}

return 0;

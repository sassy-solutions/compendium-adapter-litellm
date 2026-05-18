# Changelog

All notable changes to `Compendium.Adapters.LiteLLM` are documented here.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project
adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial implementation of `IAIProvider` against a [LiteLLM](https://github.com/BerriAI/litellm)
  proxy over its OpenAI-compatible REST surface.
- Chat completions (sync + streaming SSE) via `/v1/chat/completions`.
- Embeddings via `/v1/embeddings` with automatic batching by `MaxEmbeddingsBatchSize`.
- Tool / function-calling round-trip with `WithTools(...)` + `GetToolCalls()` helpers.
- Structured outputs: `WithStructuredOutput(schema, name, strict)` and `WithJsonMode()`.
- Virtual-key auth — `VirtualKey` option overrides `ApiKey` on the `Authorization: Bearer` header.
- Pass-through headers — `PassThroughHeaders` bag forwards arbitrary headers on every outbound
  request (e.g. `x-litellm-cache-key`, `x-litellm-tags`).
- Model listing + health check via `/v1/models`. Provider extracted from provider-prefixed model
  ids (e.g. `openai/gpt-4o`, `anthropic/claude-sonnet-4`, `bedrock/anthropic.claude-3-haiku`).
- DI extensions `AddCompendiumLiteLLM(IConfiguration)` and `AddCompendiumLiteLLM(Action<...>)`.
- Standard resilience handler (Microsoft.Extensions.Http.Resilience).
- Unit-test suite (≥ 98 % line coverage) using xUnit + FluentAssertions + RichardSzalay.MockHttp.
- Sample `samples/01-multi-provider-routing` showing one `IAIProvider` instance calling OpenAI
  and Anthropic through the same LiteLLM endpoint.

[Unreleased]: https://github.com/sassy-solutions/compendium-adapter-litellm/commits/main

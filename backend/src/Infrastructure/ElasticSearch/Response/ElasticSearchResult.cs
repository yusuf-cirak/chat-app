﻿namespace Infrastructure.ElasticSearch.Response;

public sealed class ElasticSearchResult : IElasticSearchResult //todo: refactor
{
    public bool Success { get; }
    public string? Message { get; }

    public ElasticSearchResult()
    {
        Message = string.Empty;
    }

    public ElasticSearchResult(bool success, string? message = null)
    {
        Success = success;
        Message = message;
    }
}
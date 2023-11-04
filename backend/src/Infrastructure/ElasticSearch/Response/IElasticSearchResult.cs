namespace Infrastructure.ElasticSearch.Response;

public interface IElasticSearchResult
{
    public bool Success { get; }
    public string? Message { get; }
}
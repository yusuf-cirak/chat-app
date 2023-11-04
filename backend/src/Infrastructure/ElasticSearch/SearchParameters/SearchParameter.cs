namespace Infrastructure.ElasticSearch.SearchParameters;

public class SearchParameter
{
    public string IndexName { get; set; }
    public int From { get; set; } = 0;
    public int Size { get; set; } = 10;

    public SearchParameter()
    {
        IndexName = string.Empty;
    }

    public SearchParameter(string indexName, int from, int size)
    {
        IndexName = indexName;
        From = from;
        Size = size;
    }
}
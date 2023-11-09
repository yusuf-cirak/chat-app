
namespace ElasticSearch.SearchParameters;

public sealed class SearchByFieldParameters : SearchParameter
{
    public string FieldName { get; set; }
    public string Value { get; set; }

    public SearchByFieldParameters()
    {
        FieldName = string.Empty;
        Value = string.Empty;
    }

    public SearchByFieldParameters(string fieldName, string value)
    {
        FieldName = fieldName;
        Value = value;
    }
}
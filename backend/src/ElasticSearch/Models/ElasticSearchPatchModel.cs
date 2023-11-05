using Nest;

namespace ElasticSearch.Models;

public sealed class ElasticSearchPatchModel : ElasticSearchModel
{
    public Dictionary<string, object> KeyValues { get; set; }

    public ElasticSearchPatchModel()
    {
        KeyValues = new();
    }
    
    public ElasticSearchPatchModel(Dictionary<string,dynamic> keyValues)
    {
        KeyValues = keyValues;
    }
    
    public ElasticSearchPatchModel(Id elasticId, string indexName, Dictionary<string,object> keyValues)
        : base(elasticId, indexName)
    {
        KeyValues = keyValues;
    }
    
    
}
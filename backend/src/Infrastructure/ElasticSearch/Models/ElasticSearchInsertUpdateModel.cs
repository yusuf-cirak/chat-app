using Nest;

namespace Infrastructure.ElasticSearch.Models;

public sealed class ElasticSearchInsertUpdateModel : ElasticSearchModel
{
    public object Item { get; set; }

    public ElasticSearchInsertUpdateModel(object item)
    {
        Item = item;
    }

    public ElasticSearchInsertUpdateModel(Id elasticId, string indexName, object item)
        : base(elasticId, indexName)
    {
        Item = item;
    }
}
﻿namespace Infrastructure.ElasticSearch.Models;

public sealed class ElasticSearchGetModel<T>
{
    public string ElasticId { get; set; }
    public T Item { get; set; }

    public ElasticSearchGetModel()
    {
        ElasticId = string.Empty;
        Item = default!;
    }

    public ElasticSearchGetModel(string elasticId, T item)
    {
        ElasticId = elasticId;
        Item = item;
    }
}
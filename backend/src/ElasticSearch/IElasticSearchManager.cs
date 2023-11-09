using ElasticSearch.Index;
using ElasticSearch.Models;
using ElasticSearch.Response;
using ElasticSearch.SearchParameters;
using Nest;

namespace ElasticSearch
{
    public interface IElasticSearchManager
    {
        Task<IElasticSearchResult> CreateNewIndexAsync(Action<IndexModel> indexModelAction);
        Task<IElasticSearchResult> DeleteByElasticIdAsync(Action<ElasticSearchModel> searchModelAction);
        Task<IEnumerable<ElasticSearchGetModel<T>>> GetAllSearch<T>(Action<SearchParameter> searchParametersAction) where T : class;
        IReadOnlyDictionary<IndexName, IndexState> GetIndexList();
        Task<IEnumerable<ElasticSearchGetModel<T>>> GetSearchByField<T>(Action<SearchByFieldParameters> fieldParametersAction) where T : class;
        Task<IEnumerable<ElasticSearchGetModel<T>>> GetSearchBySimpleQueryString<T>(Action<SearchByQueryParameters> queryParamsAction) where T : class;
        Task<IElasticSearchResult> InsertAsync(Action<ElasticSearchInsertUpdateModel> modelAction);
        Task<IElasticSearchResult> InsertManyAsync(string indexName, object[] items);
        Task<IElasticSearchResult> UpdateByElasticIdAsync(Action<ElasticSearchInsertUpdateModel> modelAction);
        Task<IElasticSearchResult> PatchDocumentAsync(Action<ElasticSearchPatchModel> modelAction);
    }
}
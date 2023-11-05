using System.Text;
using ElasticSearch.Config;
using ElasticSearch.Index;
using ElasticSearch.Models;
using Elasticsearch.Net;
using ElasticSearch.Response;
using ElasticSearch.SearchParameters;
using Microsoft.Extensions.Configuration;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;

namespace ElasticSearch;

public sealed class ElasticSearchManager : IElasticSearchManager
{
    private readonly ConnectionSettings _connectionSettings;

    public ElasticSearchManager(IConfiguration configuration)
    {
        const string configurationSection = "ElasticSearchConfig";
        ElasticSearchConfig settings =
            configuration.GetSection(configurationSection).Get<ElasticSearchConfig>()
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");

        SingleNodeConnectionPool pool = new(new Uri(settings.ConnectionString));
        _connectionSettings = new ConnectionSettings(
            pool,
            sourceSerializer: (builtInSerializer, connectionSettings) =>
                new JsonNetSerializer(
                    builtInSerializer,
                    connectionSettings,
                    jsonSerializerSettingsFactory: () => new JsonSerializerSettings
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                )
        ).BasicAuthentication(settings.UserName, settings.Password);
    }

    public IReadOnlyDictionary<IndexName, IndexState> GetIndexList()
    {
        ElasticClient elasticClient = new(_connectionSettings);
        return elasticClient.Indices.Get(new GetIndexRequest(Indices.All)).Indices;
    }

    public async Task<IElasticSearchResult> InsertManyAsync(string indexName, object[] items)
    {
        ElasticClient elasticClient = GetElasticClient(indexName);
        BulkResponse? response = await elasticClient.BulkAsync(a => a.Index(indexName).IndexMany(items));

        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? $"'{indexName}' created successfully." : response.ServerError.Error.Reason);
    }

    public async Task<IElasticSearchResult> CreateNewIndexAsync(Action<IndexModel> indexModelAction)
    {
        var indexModel = new IndexModel();
        indexModelAction(indexModel);

        ElasticClient elasticClient = GetElasticClient(indexModel.IndexName);
        if (elasticClient.Indices.Exists(indexModel.IndexName).Exists)
            return new ElasticSearchResult(success: false, message: "Index already exists");

        CreateIndexResponse? response = await elasticClient.Indices.CreateAsync(
            indexModel.IndexName,
            selector: se =>
                se.Settings(a =>
                        a.NumberOfReplicas(indexModel.NumberOfReplicas).NumberOfShards(indexModel.NumberOfShards))
                    .Aliases(x => x.Alias(indexModel.AliasName))
        );

        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? "Success" : response.ServerError.Error.Reason);
    }

    public async Task<IElasticSearchResult> DeleteByElasticIdAsync(Action<ElasticSearchModel> searchModelAction)
    {
        var model = new ElasticSearchModel();
        searchModelAction(model);

        ElasticClient elasticClient = GetElasticClient(model.IndexName);
        DeleteResponse? response =
            await elasticClient.DeleteAsync<object>(model.ElasticId, selector: x => x.Index(model.IndexName));
        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? "Success" : response.ServerError.Error.Reason);
    }

    public async Task<IEnumerable<ElasticSearchGetModel<T>>> GetAllSearch<T>(
        Action<SearchParameter> searchParametersAction)
        where T : class
    {
        var parameters = new SearchParameter();
        searchParametersAction(parameters);

        Type type = typeof(T);

        ElasticClient elasticClient = GetElasticClient(parameters.IndexName);
        ISearchResponse<T>? searchResponse = await elasticClient.SearchAsync<T>(
            s => s.Index(Indices.Index(parameters.IndexName)).From(parameters.From).Size(parameters.Size)
        );

        var enumerableResult = searchResponse.Hits.Select(x => new ElasticSearchGetModel<T>
            { ElasticId = x.Id, Item = x.Source });

        return enumerableResult;
    }

    public async Task<IEnumerable<ElasticSearchGetModel<T>>> GetSearchByField<T>(
        Action<SearchByFieldParameters> fieldParametersAction)
        where T : class
    {
        var fieldParameters = new SearchByFieldParameters();
        fieldParametersAction(fieldParameters);

        ElasticClient elasticClient = GetElasticClient(fieldParameters.IndexName);
        ISearchResponse<T>? searchResponse = await elasticClient.SearchAsync<T>(
            s => s.Index(fieldParameters.IndexName).From(fieldParameters.From).Size(fieldParameters.Size)
        );

        var enumerableResult = searchResponse.Hits.Select(x => new ElasticSearchGetModel<T>
            { ElasticId = x.Id, Item = x.Source });

        return enumerableResult;
    }

    public async Task<IEnumerable<ElasticSearchGetModel<T>>> GetSearchBySimpleQueryString<T>(
        Action<SearchByQueryParameters> queryParamsAction)
        where T : class
    {
        var queryParameters = new SearchByQueryParameters();
        queryParamsAction(queryParameters);

        ElasticClient elasticClient = GetElasticClient(queryParameters.IndexName);
        ISearchResponse<T>? searchResponse = await elasticClient.SearchAsync<T>(
            s =>
                s.Index(queryParameters.IndexName)
                    .From(queryParameters.From)
                    .Size(queryParameters.Size)
                    .MatchAll()
                    .Query(
                        a =>
                            a.SimpleQueryString(
                                c =>
                                    c.Name(queryParameters.QueryName)
                                        .Boost(1.1)
                                        .Fields(queryParameters.Fields)
                                        .Query(queryParameters.Query)
                                        .Analyzer("standard")
                                        .DefaultOperator(Operator.Or)
                                        .Flags(SimpleQueryStringFlags.And | SimpleQueryStringFlags.Near)
                                        .Lenient()
                                        .AnalyzeWildcard(false)
                                        .MinimumShouldMatch("30%")
                                        .FuzzyPrefixLength(0)
                                        .FuzzyMaxExpansions(50)
                                        .FuzzyTranspositions()
                                        .AutoGenerateSynonymsPhraseQuery(false)
                            )
                    )
        );

        var enumerableResult = searchResponse.Hits.Select(x => new ElasticSearchGetModel<T>
            { ElasticId = x.Id, Item = x.Source });

        return enumerableResult;
    }

    public async Task<IElasticSearchResult> InsertAsync(Action<ElasticSearchInsertUpdateModel> modelAction)
    {
        var model = new ElasticSearchInsertUpdateModel();
        modelAction(model);

        ElasticClient elasticClient = GetElasticClient(model.IndexName);

        IndexResponse? response = await elasticClient.IndexAsync(
            model.Item,
            selector: i => i.Index(model.IndexName).Id(model.ElasticId).Refresh(Refresh.True)
        );

        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? "Success" : response.ServerError.Error.Reason);
    }

    public async Task<IElasticSearchResult> UpdateByElasticIdAsync(Action<ElasticSearchInsertUpdateModel> modelAction)
    {
        var model = new ElasticSearchInsertUpdateModel();
        modelAction(model);

        ElasticClient elasticClient = GetElasticClient(model.IndexName);
        UpdateResponse<object>? response = await elasticClient.UpdateAsync<object>(
            model.ElasticId,
            selector: u => u.Index(model.IndexName).Doc(model.Item)
        );
        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? "Success" : response.ServerError.Error.Reason);
    }

    public async Task<IElasticSearchResult> PatchDocumentAsync(Action<ElasticSearchPatchModel> modelAction)
    {
        var model = new ElasticSearchPatchModel();
        modelAction(model);

        var elasticClient = GetElasticClient(model.IndexName);

        var scriptSb = new StringBuilder();
        var paramsDict = new Dictionary<string, object>();

        foreach (var (key, value) in model.KeyValues)
        {
            scriptSb.AppendLine($"ctx._source.{key} = params.{key};");
            paramsDict.Add(key, value);
        }

        UpdateResponse<object>? response = await elasticClient.UpdateAsync<object>(
            model.ElasticId,
            u => u.Index(model.IndexName)
                .Script(descriptor => descriptor.Source(scriptSb.ToString()).Params(paramsDict)));

        return new ElasticSearchResult(response.IsValid,
            message: response.IsValid ? "Success" : response.ServerError.Error.Reason);
    }

    private ElasticClient GetElasticClient(string indexName)
    {
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentNullException(indexName, message: "Index name cannot be null or empty ");

        return new ElasticClient(_connectionSettings);
    }
}
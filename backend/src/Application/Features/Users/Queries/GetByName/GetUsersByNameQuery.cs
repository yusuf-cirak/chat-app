using Application.Abstractions.Security;
using Application.Common.Extensions;
using Application.Features.Users.Dtos;
using ElasticSearch;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Features.Users.Queries.GetByName;

public sealed class GetUsersByNameQueryRequest : IRequest<List<GetUserDto>>
{
    public string UserName { get; set; }

    public GetUsersByNameQueryRequest()
    {
    }

    public GetUsersByNameQueryRequest(string userName)
    {
        UserName = userName;
    }
}

public sealed class GetUsersByNameQueryHandler : IRequestHandler<GetUsersByNameQueryRequest, List<GetUserDto>>
{
    private readonly IMongoService _mongoService;
    private readonly IElasticSearchManager _elasticSearchManager;
    private readonly ILogger<GetUsersByNameQueryRequest> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUsersByNameQueryHandler(IMongoService mongoService, IElasticSearchManager elasticSearchManager,
        ILogger<GetUsersByNameQueryRequest> logger, IHttpContextAccessor httpContextAccessor)
    {
        _mongoService = mongoService;
        _elasticSearchManager = elasticSearchManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<GetUserDto>> Handle(GetUsersByNameQueryRequest request, CancellationToken cancellationToken)
    {
        // var userProjection = Builders<User>.Projection
        //     .Include(e => e.Id)
        //     .Include(e => e.UserName)
        //     .Include(u => u.ProfileImageUrl);
        //
        // var users = await _mongoService.GetCollection<User>().Find(u => u.UserName.StartsWith(request.UserName))
        //     .Project<GetUserDto>(userProjection)
        //     .Limit(5).ToListAsync(cancellationToken);

        var users = (await _elasticSearchManager.GetSearchByField<GetUserDto>(searchModel =>
        {
            searchModel.Value = request.UserName;
            searchModel.IndexName = "users";
            searchModel.Size = 5;
        })).Select(hit => hit.Item).ToList();

        _logger.LogInformation("{RequestName} - {UserCount} of users retrieved for {UserId} {Username}",
            nameof(GetUsersByNameQueryRequest),
            users.Count,
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername()
        );

        return users;
    }
}
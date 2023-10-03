using Application.Abstractions.Security;
using Application.Features.Users.Dtos;
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

    public GetUsersByNameQueryHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public async Task<List<GetUserDto>> Handle(GetUsersByNameQueryRequest request, CancellationToken cancellationToken)
    {
        var userProjection = Builders<User>.Projection
            .Include(e => e.Id)
            .Include(e => e.UserName)
            .Include(u => u.ProfileImageUrl);

        var users = await _mongoService.GetCollection<User>().Find(u => u.UserName.StartsWith(request.UserName))
            .Project<GetUserDto>(userProjection)
            .Limit(5).ToListAsync(cancellationToken);

        return users;
    }
}
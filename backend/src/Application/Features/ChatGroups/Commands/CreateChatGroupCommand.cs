using Application.Abstractions.Services;
using Domain.Entities;
using MediatR;
using MongoDB.Bson;

namespace Application.Features.ChatGroups.Commands;

public readonly record struct CreateChatRoomCommandRequest
    (string? Name, bool IsPrivate, List<ObjectId> UserIds) : IRequest<string>;

public sealed class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommandRequest, string>
{
    private readonly IMongoService _mongoService;

    public CreateChatRoomCommandHandler(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public Task<string> Handle(CreateChatRoomCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Application.Features.Messages.Commands.Create;

public readonly record struct CreateMessageCommandRequest(string Content, string ChatGroupId) : IRequest<string>,
    ISecuredRequest;

public sealed class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest, string>
{
    private readonly IMongoService _mongoService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateMessageCommandRequest> _logger;

    public CreateMessageCommandHandler(IMongoService mongoService, IHttpContextAccessor httpContextAccessor,
        ILogger<CreateMessageCommandRequest> logger)
    {
        _mongoService = mongoService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string> Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier)
            .Value;

        var message = new Message(new ObjectId(userId).ToString()!, request.ChatGroupId, request.Content,
            sentAt: DateTime.Now);

        await _mongoService.GetCollection<Message>().InsertOneAsync(message, cancellationToken: cancellationToken);

        _logger.LogInformation("{RequestName} - Message created for {UserId} - {Username}",
            nameof(CreateMessageCommandRequest),
            _httpContextAccessor.HttpContext.User.GetUserId(),
            _httpContextAccessor.HttpContext.User.GetUsername()
        );


        return message.Id;
    }
}
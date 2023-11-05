using Application.Abstractions.Security;
using Application.Abstractions.Services.Image;
using Application.Features.Users.Rules;
using ElasticSearch;
using MongoDB.Driver;
using Nest;

namespace Application.Features.Users.Commands.UploadImage;

public sealed class UploadImageCommandRequest : MediatR.IRequest<string>, ISecuredRequest
{
    public string UserId { get; set; }
    public IFormFile File { get; set; }

    public UploadImageCommandRequest()
    {
    }

    public UploadImageCommandRequest(string userId, IFormFile file)
    {
        UserId = userId;
        File = file;
    }
}

public sealed class UploadImageCommandHandler : IRequestHandler<UploadImageCommandRequest, string>
    {
        private readonly IImageService _imageService;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IMongoService _mongoService;
        private readonly IElasticSearchManager _elasticSearchManager;

        public UploadImageCommandHandler(IImageService imageService, UserBusinessRules userBusinessRules, IMongoService mongoService, IElasticSearchManager elasticSearchManager)
        {
            _imageService = imageService;
            _userBusinessRules = userBusinessRules;
            _mongoService = mongoService;
            _elasticSearchManager = elasticSearchManager;
        }

        public async Task<string> Handle(UploadImageCommandRequest request, CancellationToken cancellationToken)
        {
            _userBusinessRules.ValidateUserBeforeUploadingOrRemovingImage(request.UserId);

            var imagePublicId = await _imageService.UploadImageAsync(request.UserId, request.File);
            
            var keyValues = new Dictionary<string, object>
            {
                {"profileImageUrl", imagePublicId}
            };

            var tasks = new List<Task>(2);

            var updateUserTask = _mongoService.GetCollection<User>().UpdateOneAsync(user => user.Id == request.UserId,
                Builders<User>.Update.Set(user => user.ProfileImageUrl, imagePublicId),
                cancellationToken: cancellationToken);

            var elasticPatchTask = _elasticSearchManager.PatchDocumentAsync(model =>
            {
                model.IndexName = "users";
                model.ElasticId = request.UserId;
                model.KeyValues = keyValues;
            });

            tasks.Add(updateUserTask);
            tasks.Add(elasticPatchTask);

            await Task.WhenAll(tasks);

            if (!updateUserTask.Result.IsModifiedCountAvailable)
            {
                await _imageService.DeleteImageAsync(imagePublicId);
                return string.Empty;
            }
           
           return imagePublicId;
        }
    }
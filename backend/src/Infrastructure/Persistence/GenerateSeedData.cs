using Application.Abstractions.Services;
using Bogus;
using Domain.Entities;
using Infrastructure.Helpers.Hashing;
using MongoDB.Bson;

namespace Infrastructure.Extensions;

public class DummyData
{
    private List<User> GenerateUsers(int count = 1000)
    {
        new HashingHelper().CreatePasswordHash("deneme",out byte[] passwordHash, out byte[] passwordSalt);
        return new Faker<User>()
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u=>u.PasswordHash,f=>passwordHash)
            .RuleFor(u=>u.PasswordSalt,f=>passwordSalt)
            .Generate(count);
    } 

    private List<ChatGroup> GenerateChatGroups(List<ObjectId> userIds, int count = 1000)
        =>
            new Faker<ChatGroup>()
                .RuleFor(cg => cg.Name, f => f.Random.Word())
                .RuleFor(cg => cg.IsPrivate, f => f.Random.Bool())
                .RuleFor(cg => cg.UserIds, f => f.PickRandom(userIds, f.Random.Int(1, 30)))
                .Generate(count);

    private List<Message> GenerateMessages(List<ObjectId> chatGroupIds, List<ObjectId> userIds, int count = 1000)
        =>
            new Faker<Message>()
                .RuleFor(m => m.ChatGroupId, f => f.PickRandom(chatGroupIds))
                .RuleFor(m => m.UserId, f => f.PickRandom(userIds))
                .RuleFor(m => m.Body, f => f.Random.Words(f.Random.Int(1, 30)))
                .Generate(count);
    
    public void Seed(IMongoService mongoService)
    {
        var users = GenerateUsers();
        var chatGroups = GenerateChatGroups(users.Select(u => u.Id).ToList());
        var messages = GenerateMessages(chatGroups.Select(cg => cg.Id).ToList(), users.Select(u => u.Id).ToList());
        
        mongoService.GetCollection<User>().InsertMany(users);
        mongoService.GetCollection<ChatGroup>().InsertMany(chatGroups);
        mongoService.GetCollection<Message>().InsertMany(messages);
    }
}
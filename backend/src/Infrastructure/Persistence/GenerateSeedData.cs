using Application.Abstractions.Services;
using Bogus;
using Domain.Entities;
using Infrastructure.Helpers.Hashing;
using MongoDB.Bson;

namespace Infrastructure.Persistence;

public class SeedDataGenerator
{
    private List<User> GenerateUsers(int count = 1000)
    {
        new HashingHelper().CreatePasswordHash("deneme",out byte[] passwordHash, out byte[] passwordSalt);
        return new Faker<User>()
            .RuleFor(u => u.UserName, f => f.Person.UserName.ToLower())
            .RuleFor(u=>u.PasswordHash,f=>passwordHash)
            .RuleFor(u=>u.PasswordSalt,f=>passwordSalt)
            .RuleFor(u=>u.ProfileImageUrl,"pgkxunrceumvwehu7kwr")
            .Generate(count);
    } 
    
    
    private List<ChatGroup> GeneratePrivateChatGroups(List<string> userIds, int count = 1000)
        =>
            new Faker<ChatGroup>()
                .RuleFor(cg => cg.IsPrivate, f => true)
                .RuleFor(cg => cg.UserIds, f => f.PickRandom(userIds, 2).ToList())
                .Generate(count);

    private List<ChatGroup> GenerateChatGroups(List<string> userIds, int count = 1000)
        =>
            new Faker<ChatGroup>()
                .RuleFor(cg => cg.Name, f => f.Random.Word())
                .RuleFor(cg => cg.IsPrivate, f => false)
                .RuleFor(cg => cg.UserIds, f => f.PickRandom(userIds, f.Random.Int(1, 50)).ToList())
                .Generate(count);

    private List<Message> GenerateMessages(List<ChatGroup> chatGroups, int count = 200)
    {
        var generatedMessages = new List<Message>();
        chatGroups.ForEach(chatGroup =>
        {
            var chatMessages = new Faker<Message>().RuleFor(m=>m.ChatGroupId,f=>chatGroup.Id)
                .RuleFor(m => m.UserId, f => f.PickRandom(chatGroup.UserIds))
                .RuleFor(m => m.Body, f => f.Random.Words(f.Random.Int(1, 50)))
                .RuleFor(m => m.SentAt, f => f.Date.Past())
                .Generate(count);
            generatedMessages.AddRange(chatMessages);
        });
        return generatedMessages.OrderBy(m=>m.SentAt).ToList();
    }
            
    
    public void GenerateAndPersist(IMongoService mongoService)
    {
        var users = GenerateUsers();
        var privateChatUserIds = users.Take(500).Select(u => u.Id).ToList();
        var publicChatUserIds = users.Skip(500).Take(500).Select(u=>u.Id).ToList();
        
        var publicChatGroups = GenerateChatGroups(publicChatUserIds);
        
        var privateChatGroups = GeneratePrivateChatGroups(privateChatUserIds);
        
        
        var privateChatMessages = GenerateMessages(privateChatGroups);
        var publicChatMessages = GenerateMessages(publicChatGroups);
        
        mongoService.GetCollection<User>().InsertMany(users);
        
        var allChatGroups = publicChatGroups.Concat(privateChatGroups).ToList();
        mongoService.GetCollection<ChatGroup>().InsertMany(allChatGroups);
        
        var allChatMessages = privateChatMessages.Concat(publicChatMessages).ToList();
        mongoService.GetCollection<Message>().InsertMany(allChatMessages);
    }
}
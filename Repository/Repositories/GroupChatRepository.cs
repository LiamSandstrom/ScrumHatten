using System.Reflection.Emit;
using Models;
using MongoDB.Driver;

namespace Repository
{
    public class GroupChatRepository : IGroupChatRepository
{
    private readonly IMongoCollection<GroupChat> _groups;
    private readonly IMongoCollection<Message> _messages;

    public GroupChatRepository(MongoConnector connector) // Ändrat från IMongoDatabase
    {
            var database = connector._database; // Få tillgång till databasen
            _groups = database.GetCollection<GroupChat>("Groups");
            _messages = database.GetCollection<Message>("Messages");    
    }

    public async Task CreateGroupAsync(GroupChat group) => 
        await _groups.InsertOneAsync(group);

    public async Task<List<GroupChat>> GetGroupsForUserAsync(string userId) =>
        await _groups.Find(g => g.MemberIds.Contains(userId)).ToListAsync();

    public async Task<GroupChat> GetGroupByIdAsync(string groupId) =>
        await _groups.Find(g => g.Id == groupId).FirstOrDefaultAsync();

    public async Task<List<Message>> GetGroupMessagesAsync(string groupId) =>
        await _messages.Find(m => m.ReceiverId == groupId).ToListAsync();
        
    public async Task UpdateGroupMembersAsync(string groupId, List<string> memberIds)
{
    var filter = Builders<GroupChat>.Filter.Eq(g => g.Id, groupId);
    var update = Builders<GroupChat>.Update.Set(g => g.MemberIds, memberIds);
    await _groups.UpdateOneAsync(filter, update);
}

public async Task DeleteGroupAsync(string groupId)
{
    await _groups.DeleteOneAsync(g => g.Id == groupId);
 
    await _messages.DeleteManyAsync(m => m.ReceiverId == groupId);
}

public async Task UpdateGroupAsync(GroupChat group)
{
    var filter = Builders<GroupChat>.Filter.Eq(g => g.Id, group.Id);
    var update = Builders<GroupChat>.Update.Set(g => g.Name, group.Name)
        .Set(g => g.MemberIds, group.MemberIds)
        .Set(g => g.CreatedAt, group.CreatedAt);
    await _groups.UpdateOneAsync(filter, update);
}
}
}
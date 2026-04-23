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
}
}
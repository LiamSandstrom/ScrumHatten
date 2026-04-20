using Models;
using MongoDB.Driver;

namespace Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageRepository(MongoConnector connector)
        {
            _messages = connector._database.GetCollection<Message>("Messages");
        }

        public async Task CreateMessageAsync(Message message) => 
            await _messages.InsertOneAsync(message);

        public async Task<List<Message>> GetConversationAsync(string userId, string contactId) =>
            await _messages.Find(m => (m.SenderId == userId && m.ReceiverId == contactId) 
                                   || (m.SenderId == contactId && m.ReceiverId == userId))
                           .SortBy(m => m.Timestamp)
                           .ToListAsync();

        public async Task<List<Message>> GetGuestMessagesAsync(string userId) =>
            await _messages.Find(m => m.ReceiverId == userId && m.SenderId == null)
                           .SortByDescending(m => m.Timestamp)
                           .ToListAsync();

        public async Task<List<Message>> GetUnreadMessagesAsync(string userId) =>
            await _messages.Find(m => m.ReceiverId == userId && !m.IsRead).ToListAsync();

        public async Task MarkAsReadAsync(List<string> messageIds)
        {
            var filter = Builders<Message>.Filter.In(m => m.Id, messageIds);
            var update = Builders<Message>.Update.Set(m => m.IsRead, true);
            await _messages.UpdateManyAsync(filter, update);
        }

        public async Task DeleteMessageAsync(string id) =>
            await _messages.DeleteOneAsync(m => m.Id == id);

        public async Task<Message?> GetMessageByIdAsync(string id) =>
            await _messages.Find(m => m.Id == id).FirstOrDefaultAsync();
    }
}
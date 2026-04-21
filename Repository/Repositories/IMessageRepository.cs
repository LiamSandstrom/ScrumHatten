using Models;

namespace Repository
{
    public interface IMessageRepository
    {
        Task CreateMessageAsync(Message message);
        Task<List<Message>> GetConversationAsync(string userId, string contactId);
        Task<List<Message>> GetGuestMessagesAsync(string userId);
        Task<List<Message>> GetUnreadMessagesAsync(string userId);
        Task MarkAsReadAsync(List<string> messageIds);
        Task DeleteMessageAsync(string id);
        Task<Message?> GetMessageByIdAsync(string id);
    }
}
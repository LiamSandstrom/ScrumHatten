using Models;

namespace Repository
{
    public interface IGroupChatRepository
    {
        Task CreateGroupAsync(GroupChat group);
        Task<List<GroupChat>> GetGroupsForUserAsync(string userId);
        Task<GroupChat> GetGroupByIdAsync(string groupId);
        Task<List<Message>> GetGroupMessagesAsync(string groupId);
    }
}
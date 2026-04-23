using Models;

namespace Repository
{
    public interface GroupChatRepository
    {
        Task CreateGroupChatAsync(GroupChat groupChat);
        Task GetGroupChatByIdAsync(string id);
        Task DeleteGroupChatAsync(string id);
        Task AddmemberToGroupChatAsync(string groupId, string userId);
        Task RemoveMemberFromGroupChatAsync(string groupId, string userId);
    }
}
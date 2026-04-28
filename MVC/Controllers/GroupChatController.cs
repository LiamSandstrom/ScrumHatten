using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using System.Security.Claims;
using MongoDB.Driver;
using DAL.Repositories.Interfaces;
namespace MVC.Controllers
{
    public class GroupChatController : Controller
    {
        private readonly IGroupChatRepository _groupRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;

        public GroupChatController(IGroupChatRepository groupRepo, IMessageRepository messageRepo, IUserRepository userRepo)
        {
            _groupRepo = groupRepo;
            _messageRepo = messageRepo;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index(string? id, string? groupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var allUsers = await _userRepo.GetAllUsersAsync();
            ViewBag.Users = allUsers.Where(u => u.Id.ToString() != currentUserId).ToList();
            ViewBag.Groups = await _groupRepo.GetGroupsForUserAsync(currentUserId);

            List<Message> messages = new List<Message>();

            if (!string.IsNullOrEmpty(groupId))
            {
                var group = await _groupRepo.GetGroupByIdAsync(groupId);
                if (group != null)
                {
                    ViewBag.SelectedUserId = groupId;
                    ViewBag.SelectedUserName = group.Name;
                    ViewBag.IsGroupChat = true;
                    messages = await _groupRepo.GetGroupMessagesAsync(groupId);
                }
            }
            else if (!string.IsNullOrEmpty(id))
            {
                ViewBag.SelectedUserId = id;
                ViewBag.IsGroupChat = false;

                var selectedUser = allUsers.FirstOrDefault(u => u.Id.ToString() == id);
                ViewBag.SelectedUserName = selectedUser?.Name ?? "Kollega";

                messages = await _messageRepo.GetConversationAsync(currentUserId, id);

                var unreadIds = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                                        .Select(m => m.Id)
                                        .ToList();

                if (unreadIds.Any())
                {
                    await _messageRepo.MarkAsReadAsync(unreadIds);
                }
            }

            var sortedMessages = messages.OrderBy(m => m.Timestamp).ToList();

            return View(sortedMessages);
        }

        [HttpPost]
        public async Task<IActionResult> SendGroupMessage(string receiverId, string content)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderName = User.Identity?.Name;
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderName))
            {
                return RedirectToAction("Login", "Account");
            }
            var newMessage = new Message
            {
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now
            };

            await _messageRepo.CreateMessageAsync(newMessage);
            return RedirectToAction("Index", new { groupId = receiverId });
        }
    }
}

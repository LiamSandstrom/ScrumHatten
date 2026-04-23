using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using System.Security.Claims;
using DAL.Repositories.Interfaces;

namespace MVC.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGroupChatRepository _groupRepo;
        private readonly UserManager<User> _userManager;

        public MessageController(IMessageRepository messageRepo, IUserRepository userRepo, UserManager<User> userManager, IGroupChatRepository groupRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _userManager = userManager;
            _groupRepo = groupRepo;
        }

        [HttpGet]
        [Route("Message/Index/{id?}")] // Använder id som standard för både person och grupp
        public async Task<IActionResult> Index(string? id, string? groupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Challenge();

            // 1. Hämta data för sidomenyn
            var allUsers = await _userRepo.GetAllUsersAsync();
            ViewBag.Users = allUsers.Where(u => u.Id.ToString() != currentUserId).ToList();
            ViewBag.Groups = await _groupRepo.GetGroupsForUserAsync(currentUserId);

            List<Message> messages = new List<Message>();

            // 2. Hantera GRUPP-chatt
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
            // 3. Hantera PRIVAT-chatt (id kan komma från antingen 'id' eller 'contactId' beroende på route)
            else if (!string.IsNullOrEmpty(id))
            {
                ViewBag.SelectedUserId = id;
                ViewBag.IsGroupChat = false;
                var selectedUser = allUsers.FirstOrDefault(u => u.Id.ToString() == id);
                ViewBag.SelectedUserName = selectedUser?.Name ?? "Kollega";

                messages = await _messageRepo.GetConversationAsync(currentUserId, id);

                var unreadIds = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                                        .Select(m => m.Id).ToList();
                if (unreadIds.Any()) await _messageRepo.MarkAsReadAsync(unreadIds);
            }

            var sortedMessages = messages.OrderBy(m => m.Timestamp).ToList();
            return View(sortedMessages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            return await ProcessMessageSend(receiverId, content, false);
        }

        [HttpPost]
        public async Task<IActionResult> SendGroupMessage(string receiverId, string content)
        {
            return await ProcessMessageSend(receiverId, content, true);
        }

        // Privat hjälpmetod för att slippa dubbelkod vid sändning
        private async Task<IActionResult> ProcessMessageSend(string receiverId, string content, bool isGroup)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Index");

            var newMessage = new Message
            {
                SenderId = currentUserId,
                SenderName = User.Identity?.Name ?? "Okänd",
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now,
                IsRead = false
            };

            await _messageRepo.CreateMessageAsync(newMessage);

            if (isGroup)
                return RedirectToAction("Index", new { groupId = receiverId });
            
            return RedirectToAction("Index", new { id = receiverId });
        }

        [HttpPost]
public async Task<IActionResult> CreateGroup(string groupName, List<string> selectedUsers)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(currentUserId)) return Challenge();

    // Lägg till skaparen själv i gruppen
    if (!selectedUsers.Contains(currentUserId))
    {
        selectedUsers.Add(currentUserId);
    }

    var newGroup = new GroupChat
    {
        Name = groupName,
        MemberIds = selectedUsers,
        CreatedAt = DateTime.Now
    };

    await _groupRepo.CreateGroupAsync(newGroup);

    return RedirectToAction("Index", new { groupId = newGroup.Id });
}
    }
}
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
[Route("Message/Index/{id?}")]
public async Task<IActionResult> Index(string? id, string? groupId)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(currentUserId)) return Challenge();

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
            ViewBag.CurrentGroup = group; // Viktigt för modalen

            // --- HÄR ÄR FIXEN: Fyll listorna för modalen i RÄTT controller ---
            var memberIds = group.MemberIds ?? new List<string>();
            
            ViewBag.GroupMembers = allUsers
                .Where(u => memberIds.Contains(u.Id.ToString()) && u.Id.ToString() != currentUserId)
                .ToList();

            ViewBag.NonMembers = allUsers
                .Where(u => !memberIds.Contains(u.Id.ToString()))
                .ToList();

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
        // ... olästa-logik ...
    }

    return View(messages.OrderBy(m => m.Timestamp).ToList());
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

[HttpPost]
public async Task<IActionResult> LeaveGroup(string groupId)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    // 1. Hämta gruppen
    var group = await _groupRepo.GetGroupByIdAsync(groupId);
    if (group == null) return RedirectToAction("Index");

    // 2. Ta bort den aktuella användaren ur listan
    group.MemberIds.Remove(currentUserId);

    // 3. KOLLEN: Är gruppen tom nu?
    if (group.MemberIds.Count == 0)
    {
        // Om ingen är kvar -> RADERA gruppen helt
        await _groupRepo.DeleteGroupAsync(groupId);
    }
    else
    {
        // Om folk är kvar -> UPPDATERA gruppen (ta bort dig själv)
        await _groupRepo.UpdateGroupAsync(group);
    }

    // Skicka användaren tillbaka till en tom chattvy
    return RedirectToAction("Index");
}

[HttpPost]
public async Task<IActionResult> UpdateMembers(string groupId, List<string> selectedUsers)
{
    // selectedUsers bör innehålla ALLA som ska vara kvar/lägga till
    await _groupRepo.UpdateGroupMembersAsync(groupId, selectedUsers);
    return RedirectToAction("Index", new { groupId = groupId });
}

 [HttpPost]
public async Task<IActionResult> RemoveMember(string groupId, string userId)
{
    var group = await _groupRepo.GetGroupByIdAsync(groupId);
    if (group != null)
    {
        group.MemberIds.Remove(userId);
        await _groupRepo.UpdateGroupMembersAsync(groupId, group.MemberIds);
    }
    return RedirectToAction("Index", new { groupId = groupId });
}

[HttpPost]
public async Task<IActionResult> AddMembers(string groupId, List<string> newUserIds)
{
    var group = await _groupRepo.GetGroupByIdAsync(groupId);
    if (group != null && newUserIds != null && newUserIds.Any())
    {
        group.MemberIds.AddRange(newUserIds);
        group.MemberIds = group.MemberIds.Distinct().ToList(); // Säkerhetsåtgärd mot dubbletter
        await _groupRepo.UpdateGroupMembersAsync(groupId, group.MemberIds);
    }
    return RedirectToAction("Index", new { groupId = groupId });
}
    }
}
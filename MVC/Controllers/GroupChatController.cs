using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using System.Security.Claims;
using MongoDB.Driver; // Använd denna istället för EF
using DAL.Repositories.Interfaces; // För att nå ditt IUserRepository
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

    // 1. Hämta data för sidomenyn (alltid samma)
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
            ViewBag.IsGroupChat = true; // Flagga för vyn
            messages = await _groupRepo.GetGroupMessagesAsync(groupId);
        }
    }
    // 3. Hantera PRIVAT-chatt
    else if (!string.IsNullOrEmpty(id))
    {
        ViewBag.SelectedUserId = id;
        ViewBag.IsGroupChat = false; // Flagga för vyn

        // Hitta den valda kollegan för att visa namnet i chat-headern
        var selectedUser = allUsers.FirstOrDefault(u => u.Id.ToString() == id);
        ViewBag.SelectedUserName = selectedUser?.Name ?? "Kollega";

        // Hämta meddelanden mellan mig och den valda kontakten
        messages = await _messageRepo.GetConversationAsync(currentUserId, id);

        // Markera olästa meddelanden som jag tagit emot som lästa
        var unreadIds = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                                .Select(m => m.Id)
                                .ToList();
        
        if (unreadIds.Any()) 
        {
            await _messageRepo.MarkAsReadAsync(unreadIds);
        }
    }

    // 4. Sortera så de nyaste hamnar längst ner (viktigt för scroll-JS)
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
            ReceiverId = receiverId, // Här är receiverId = GroupId
            Content = content,
            Timestamp = DateTime.Now
        };

        await _messageRepo.CreateMessageAsync(newMessage);
        return RedirectToAction("Index", new { groupId = receiverId });
    }
}
}
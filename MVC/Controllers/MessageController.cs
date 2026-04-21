using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using System.Security.Claims;
using MongoDB.Driver; // Använd denna istället för EF
using DAL.Repositories.Interfaces; // För att nå ditt IUserRepository

namespace MVC.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo; // Använd ditt existerande Repo
        private readonly UserManager<User> _userManager;

        public MessageController(IMessageRepository messageRepo, IUserRepository userRepo, UserManager<User> userManager)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _userManager = userManager;
        }

       [HttpGet]
[Route("Message/Index/{contactId?}")]
public async Task<IActionResult> Index(string? id) // Måste matcha asp-route-contactId
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(currentUserId)) return Challenge();

    // 1. Hämta alla användare för listan till vänster
    var allUsers = await _userRepo.GetAllUsersAsync();
    ViewBag.Users = allUsers.Where(u => u.Id.ToString() != currentUserId).ToList();

    List<Message> messages = new List<Message>();

    // 2. Om contactId INTE är null, betyder det att användaren klickat på en kontakt
    if (!string.IsNullOrEmpty(id))
    {
        // Hämta meddelanden mellan mig och den valda kontakten
        messages = await _messageRepo.GetConversationAsync(currentUserId, id);
        
        // Hitta den valda användaren för att visa namnet i chat-headern
        var selectedUser = allUsers.FirstOrDefault(u => u.Id.ToString() == id);
        
        // Sätt ViewBag-värden som din HTML letar efter (@if (ViewBag.SelectedUserId != null))
        ViewBag.SelectedUserId = id; 
        ViewBag.SelectedUserName = selectedUser?.Name ?? "Kollega";

        // (Valfritt) Markera meddelanden som lästa när man öppnar chatten
        var unreadIds = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                                .Select(m => m.Id).ToList();
        if (unreadIds.Any()) await _messageRepo.MarkAsReadAsync(unreadIds);
    }

    // 3. Skicka meddelandelistan till vyn
    // I din Index-metod i controllern:
messages = await _messageRepo.GetConversationAsync(currentUserId, id);

// Sortera så att de äldsta är först (överst) och de nyaste sist (nederst)
var sortedMessages = messages.OrderBy(m => m.Timestamp).ToList();

return View(sortedMessages);
}
    


        [HttpPost]
public async Task<IActionResult> SendMessage(string receiverId, string content)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrWhiteSpace(content)) 
        return RedirectToAction("Index");

    var newMessage = new Message
    {
        SenderId = currentUserId,
        ReceiverId = receiverId,
        Content = content,
        Timestamp = DateTime.Now,
        IsRead = false
        // SenderName kan du hämta från User.Identity.Name om det behövs
    };

    await _messageRepo.CreateMessageAsync(newMessage);

    // Skicka användaren tillbaka till samma chatt
    return RedirectToAction("Index", new { id = receiverId });
}
    }
}



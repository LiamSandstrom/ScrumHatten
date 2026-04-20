namespace Models
{
    public class Message
   {
    //Every message has a unique id that the database creates automatically
    public int Id { get; set; } 
    // Here we use senderId as nullable because a guest user might send a message (Id = null)
    public string? SenderId { get; set; } 
    //The name of the sender is fetched from their profile if logged in, else from the form input which guests provide manually
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    // This is the actual text content of each message
    public string Content { get; set; } = string.Empty;
    // The timestamp when the message was sent
    // We set a standard value to "DateTime.Now" so the time saves automatically when the object is created
    public DateTime Timestamp { get; set; } = DateTime.Now;
    // Checks if the message has been read by the receiver
    // This is used to show unread messages in the UI
    public bool IsRead { get; set; } = false;
}
}
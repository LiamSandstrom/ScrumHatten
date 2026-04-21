using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Själva meddelandet kan fortfarande ha ObjectId som sin egen nyckel
        public string Id { get; set; } = string.Empty;

        // Dessa matchar UUID-strängarna från dina User-dokument
        public string? SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
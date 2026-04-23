using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
public class GroupChat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    // Lista på UserIds som är med i gruppen
    public List<string> MemberIds { get; set; } = new List<string>();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
}
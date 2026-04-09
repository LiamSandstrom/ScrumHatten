using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Phonenumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Adress { get; set; }
}

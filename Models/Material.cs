using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Material
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public string Unit { get; set; }
}

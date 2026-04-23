using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Adress { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string City { get; set; }

    public double Discount { get; set; }

}

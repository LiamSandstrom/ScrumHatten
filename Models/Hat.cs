using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Models
{
    public class Hat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
<<<<<<< HEAD
        public double Price { get; set; }
=======
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }
>>>>>>> C_E
        public List<HatMaterial> Materials { get; set; } = new();
        public int Quantity { get; set; }
    }
}
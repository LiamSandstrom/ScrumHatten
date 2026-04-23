using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Hat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string? ImageBase64 { get; set; }
        public bool CustomHat { get; set; }

        public List<HatMaterial> Materials { get; set; } = new();
        public int Quantity { get; set; }

        public List<HatSize> Sizes { get; set; } = new();
    }
}


using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class Hat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool CustomHat { get; set; }
        public bool IsReturned { get; set; }
        public bool IsReclaimed { get; set; }
        public List<HatMaterial> Materials { get; set; } = new();

        public List<HatSize> Sizes { get; set; } = new();
    }
}


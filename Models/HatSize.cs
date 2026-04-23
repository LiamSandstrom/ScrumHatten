using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class HatSize
    {
        public string Label { get; set; }      // t.ex. XS(50-52cm), S, M
        public int Quantity { get; set; } 
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class HatMaterial
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string MaterialId { get; set; }
        public int Amount { get; set; }
    }
}

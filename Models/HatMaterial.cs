using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    //klassen viisar kopplingen mellan en hatt och material. "Hur mycket ull går det åt till just den här hatten"
    public class HatMaterial
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string MaterialId { get; set; }
        public int Amount { get; set; }
    }
}

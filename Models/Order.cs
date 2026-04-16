using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<HatMaterial> Materials { get; set; } = new();

        public List<Hat> Hats { get; set; } = new();

        public decimal TimeToMake { get; set; } 

        public DateTime DateToFinish { get; set; }

        public int HatAmount { get; set; }  

        public decimal TransportPrice { get; set; } 

        public decimal Moms {  get; set; }

        public Boolean FastOrder { get; set; }

        public Guid MakerId { get; set; }

        public string Status { get; set; }
        



    }
}

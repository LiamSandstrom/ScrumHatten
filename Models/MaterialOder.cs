using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class MaterialOder
    {


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        [BsonElement("Amound")]
        public string Amount {  get; set; }


        [BsonElement("Price")]
        public decimal Price { get; set; }
      
     
        // Lägg till Material

       // Lägg in anställd


        public MaterialOder(string amound,  decimal price,  HatMaterial hatMaterial)
        {
            Amount = amound;
            Price = price;
            HatMaterial = hatMaterial;
        }

       
    }
}

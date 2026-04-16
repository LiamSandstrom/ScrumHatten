using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class MaterialOrder
    {


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        [BsonElement("Amount")]
        public double Amount { get; set; }


        [BsonElement("Price")]
        public decimal Price { get; set; }


        // Lägg till Material

        // Lägg in anställd



        public MaterialOrder(double amount, decimal price, HatMaterial hatMaterial)
        {
            Amount = amount;
            Price = price;
            hatMaterial = hatMaterial;
        }


    }
}

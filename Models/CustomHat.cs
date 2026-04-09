namespace Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

public class CustomHat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

}

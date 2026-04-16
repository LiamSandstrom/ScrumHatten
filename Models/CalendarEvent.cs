using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CalendarEvent
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Color { get; set; } = "#007bff";
        public string TargetType { get; set; } = "private"; // "public" or "private"
        public string TargetUserName { get; set; }
    }
}

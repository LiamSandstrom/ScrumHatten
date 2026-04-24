using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models

{
    public enum Priority { Low, Medium, High }
    public enum Status { Pending, InProgress, Completed, Delivered }
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<Hat> Hats { get; set; } = new();

        public decimal TimeToMake { get; set; }

        public DateTime DateToFinish { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TransportPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public Boolean FastOrder { get; set; }

        public Guid MakerId { get; set; }
        public string CustomerId { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public bool IsDelivered { get; set; }

        public string? MakerName { get; set; }

        public decimal CustomsFee { get; set; }

        public decimal Discount { get; set; }

    }
}

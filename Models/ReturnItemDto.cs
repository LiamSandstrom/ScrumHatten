using System;

namespace Models
{
    public class ReturnItemDto
    {
        public string OrderId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public Hat Hat { get; set; } = null!;
        public string? Reason { get; set; }
    }
}
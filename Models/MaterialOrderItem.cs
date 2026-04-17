namespace Models
{
    public class MaterialOrderItem
    {
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public double Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
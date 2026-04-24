namespace Models
{
    public class HatWithMaterial
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool CustomHat { get; set; }
        public int Quantity { get; set; }
        public List<HatMaterialDetail> Materials { get; set; } = new();
    }

    public class HatMaterialDetail
    {
        public string MaterialId { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double PricePerUnit { get; set; }
        public string Unit { get; set; }
    }
}


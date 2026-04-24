namespace MVC.ViewModels
{
    public class CreateMaterialOrderViewModel
    {
        public string Supplier { get; set; }
        public List<MaterialOrderItemInputViewModel> Items { get; set; } = new();
    }
}
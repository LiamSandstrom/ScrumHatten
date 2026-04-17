namespace MVC.ViewModels
{
    public class CreateMaterialOrderViewModel
    {
        public List<MaterialOrderItemInputViewModel> Items { get; set; } = new();
    }
}
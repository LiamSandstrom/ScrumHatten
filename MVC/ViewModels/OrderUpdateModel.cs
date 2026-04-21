namespace MVC.ViewModels
{
    public class OrderUpdateModel
    {
        public decimal TransportPrice { get; set; }
        public decimal TimeToMake { get; set; }
        public DateTime DateToFinish { get; set; }
        public string SelectedUserId { get; set; }
        public string SelectedCustomerId { get; set; }
        public bool FastOrder { get; set; }
    }
}
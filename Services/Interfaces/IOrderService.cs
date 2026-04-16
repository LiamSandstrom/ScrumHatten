public interface IOrderService
{
    decimal CalculateFinalPrice(decimal basePrice, decimal momsRate, bool isPriority);
}
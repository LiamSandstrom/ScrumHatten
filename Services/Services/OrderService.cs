using Repository;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository; 
    
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<decimal> CalculateOrderTotalAsync(string orderId)
    {
        var order = await _repository.GetOrderByIdAsync(orderId);
        if (order == null) return 0;

        decimal totalBasePrice = 0;

        foreach (var hat in order.Hats)
        {
            decimal hatPrice = hat.BasePrice;

            if (hat.IsCustom)
            {
                hatPrice += order.TimeToMake * HourlyRate;
            }

            totalBasePrice += hatPrice;
        }

        decimal totalWithTransport = totalBasePrice + order.TransportPrice;
        decimal totalWithMoms = totalWithTransport * (1 + order.Moms);
        decimal finalPrice = totalWithMoms - order.Discount; 

        return finalPrice;
    }

}
using Models;
using BL.Interfaces;
using Repository;
public class OrderService : IOrderService
{
    private const decimal PriorityMultiplier = 1.20m; // 20% moms
    public decimal CalculateFinalPrice(decimal basePrice, decimal momsRate, bool isPriority)
    {
        decimal currentPrice = basePrice;
        if (isPriority)
        {
            currentPrice *= PriorityMultiplier;
        }
        decimal finalPrice = currentPrice * (1 * momsRate);
        return finalPrice; 

    }
}
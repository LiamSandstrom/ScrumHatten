using Models;
using BL.Interfaces;
using Repository;

namespace BL.Services
{
public interface ICustomsService 
{
    decimal GetCustomsRate(string country);
}

public class CustomsService : ICustomsService 
{
    public decimal GetCustomsRate(string country) 
    {
        if (string.IsNullOrEmpty(country)) return 0.15m; // Standard om land saknas

        return country.ToLower() switch 
        {
            "sverige" or "danmark" or "finland" or "tyskland" => 0m, // EU
            "norge" => 0.107m,
            "usa" => 0.12m,
            "storbritannien" => 0.08m,
            _ => 0.15m // Resten av världen
        };
    }
}
}

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
        private readonly HashSet<string> _euCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            "Belgien", "Bulgarien", "Cypern", "Danmark", "Estland", "Finland", "Frankrike",
            "Grekland", "Irland", "Italien", "Kroatien", "Lettland", "Litauen", "Luxemburg",
            "Malta", "Nederländerna", "Polen", "Portugal", "Rumänien", "Slovakien",
            "Slovenien", "Spanien", "Sverige", "Tjeckien", "Tyskland", "Ungern", "Österrike"
        };

        public decimal GetCustomsRate(string country)
        {
            if (string.IsNullOrEmpty(country)) return 0.15m;

            if (_euCountries.Contains(country.Trim()))
            {
                return 0m;
            }

            return country.Trim().ToLower() switch
            {
                "norge" => 0.107m,
                "usa" => 0.12m,
                "storbritannien" => 0.08m,
                _ => 0.15m
            };
        }
    }
}

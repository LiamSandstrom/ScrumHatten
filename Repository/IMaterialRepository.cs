using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    internal interface IHatInterface
    {
        Task<List<String>> GetAllHats();
        Task<List<String>> GetStockedHats();
        Task<List<String>> GetCustomHats();
        Task<List<int>> GetQuantityById(string id);
        Task<List<double>> GetPriceById(string id);
        
    }
}

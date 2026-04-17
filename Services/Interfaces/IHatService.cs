using Models;

namespace BL.Interfaces
{
    public interface IHatService
    {
        Task<List<Hat>> GetAllHats();
        Task AddHat(Hat hat);
        Task<Hat?> GetHatById(string id);
        Task DeleteHat(string id);
        Task UpdateHat(Hat hat);
    }
}


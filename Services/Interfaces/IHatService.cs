using Models;

namespace BL.Interfaces
{
    public interface IHatService
    {
        List<Hat> GetAllHats();
        void AddHat(Hat hat);
        Hat? GetHatById(string id);
        void DeleteHat(string id);
        void UpdateHat(Hat hat);
    }
}
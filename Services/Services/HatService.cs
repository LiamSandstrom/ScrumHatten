using Models;
using Repository.Repositories;
using BL.Interfaces;

namespace BL.Services
{
    public class HatService : IHatService
    {
        private readonly HatRepository _hatRepository;

        public HatService(HatRepository hatRepository)
        {
            _hatRepository = hatRepository;
        }

        public List<Hat> GetAllHats()
        {
            return _hatRepository.GetAllHats();
        }

        public void AddHat(Hat hat)
        {
            _hatRepository.AddHat(hat);
        }

        public Hat? GetHatById(string id)
        {
            return _hatRepository.GetHatById(id);
        }

        public void DeleteHat(string id)
        {
            _hatRepository.DeleteHat(id);
        }

        public void UpdateHat(Hat hat)
        {
            _hatRepository.UpdateHat(hat);
        }
    }
}
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
    }
}
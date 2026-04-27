using BL.Interfaces;
using Models;
using Repository.Repositories;

namespace BL.Services
{
    public class HatService : IHatService
    {
        private readonly HatRepository _hatRepository;

        public HatService(HatRepository hatRepository)
        {
            _hatRepository = hatRepository;
        }

        public async Task<List<Hat>> GetAllHats()
        {
            return await _hatRepository.GetAllHats();
        }

        public async Task AddHat(Hat hat)
        {
            await _hatRepository.AddHat(hat);
        }

        public async Task<Hat?> GetHatById(string id)
        {
            return await _hatRepository.GetHatById(id);
        }

        public async Task DeleteHat(string id)
        {
            await _hatRepository.DeleteHat(id);
        }

        public async Task UpdateHat(Hat hat)
        {
            await _hatRepository.UpdateHat(hat);
        }

        public async Task UpdateReclaimed(string id, bool isReclaimed)
        {
            await _hatRepository.UpdateReclaimed(id, isReclaimed);
        }
    }
}

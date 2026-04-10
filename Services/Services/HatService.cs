
using BL.Interfaces;
using DAL.Repositories.Interfaces;
using Models;

namespace BL.Services;
public class HatService : IHatService
{
    private readonly IHatRepository _hatRepository;

    public HatService(IHatRepository hatRepository)
    {
        _hatRepository = hatRepository;
    }

    public List<Hat> GetAllHats()
    {
        return _hatRepository.GetAllHats();
    }

    public Hat GetHatById(string id)
    {
        return _hatRepository.GetHatById(id);
    }
}
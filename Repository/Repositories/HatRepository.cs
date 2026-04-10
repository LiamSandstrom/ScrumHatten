using DAL.Repositories.Interfaces;
using Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public class HatRepository : IHatRepository
    {
        private readonly IMongoCollection<Hat> _hats;

        public HatRepository(IMongoDatabase database)
        {
            _hats = database.GetCollection<Hat>("Hats");
        }

        public List<Hat> GetAllHats()
        {
            return _hats.Find(h => true).ToList();
        }

        public Hat GetHatById(string id)
        {
            return _hats.Find(h => h.Id == id.ToString()).FirstOrDefault();
        }
    }
}
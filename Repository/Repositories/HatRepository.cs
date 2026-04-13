using Models;
using MongoDB.Driver;

namespace Repository.Repositories
{
    public class HatRepository
    {
        private readonly IMongoCollection<Hat> _hatCollection;

        public HatRepository(MongoConnector mongoConnector)
        {
            _hatCollection = mongoConnector._database.GetCollection<Hat>("Hats");
        }

        public List<Hat> GetAllHats()
        {
            return _hatCollection.Find(_ => true).ToList();
        }
    }
}
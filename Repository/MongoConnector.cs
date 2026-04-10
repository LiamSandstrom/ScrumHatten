using MongoDB.Driver;
using MongoDB.Bson;
namespace Repository
{
    public class MongoConnector
    {
        public IMongoDatabase _database;

        public MongoConnector(string connectionString)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("ScrumHatten");
            try
            {
                var result = _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Ansluten till MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Anslutning misslyckades: {ex.Message}");
            }
        }

    }
}

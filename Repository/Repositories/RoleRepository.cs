using DAL.Repositories.Interfaces;
using Models;
using MongoDB.Driver;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{

    public class RoleRepository : IRoleRepository
    {

        private readonly IMongoCollection<ApplicationRole> _collection;

        public RoleRepository(MongoConnector mc) {
            _collection = mc._database.GetCollection<ApplicationRole>("Roles");
        }
        public async Task<ApplicationRole> GetRoleByIdAsync(Guid roleId) {

            var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, roleId);
            
            return await _collection.Find(filter).FirstOrDefaultAsync();

        }

        public async Task<List<ApplicationRole>> GetAllRolesAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();

        }

    }
}

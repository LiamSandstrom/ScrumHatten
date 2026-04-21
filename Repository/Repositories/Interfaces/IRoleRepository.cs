using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<ApplicationRole> GetRoleByIdAsync(Guid roleId);

        Task<List<ApplicationRole>> GetAllRolesAsync();

       

    }
}

using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface IHatRepository
    {
        List<Hat> GetAllHats();
        Hat GetHatById(string id);
    }
}

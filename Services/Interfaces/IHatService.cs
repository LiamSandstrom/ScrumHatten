using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Interfaces
{
    public interface IHatService
    {
        List<Hat> GetAllHats();
        Hat GetHatById(string id);
    }
}

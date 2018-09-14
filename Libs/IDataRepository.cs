using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ParsingServices.Models;

namespace ParsingServices.Libs
{
    public interface IDataRepository
    {        
        InfoData Get(Guid id);
        IEnumerable<InfoData> All();

        InfoData Save(InputData value);
        void Delete(Guid id);
    }
}

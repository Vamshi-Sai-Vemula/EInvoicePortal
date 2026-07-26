using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Interfaces
{
    public interface IConfiguration
    {
        IQueryable<Configuration> GetAll();

        Configuration GetByID(object id);

        Configuration Insert(Configuration entity);

        int Update(Configuration entity);

        void Delete(object id);
    }
}

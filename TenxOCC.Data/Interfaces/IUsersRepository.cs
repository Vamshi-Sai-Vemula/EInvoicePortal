using TenxOCC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Interfaces
{
    public interface IUsersRepository
    {
        IQueryable<Users> GetAll();

        Users GetByID(object id);

        Users Insert(Users entity);

        int Update(Users entity);

        void Delete(object id);


    }
}

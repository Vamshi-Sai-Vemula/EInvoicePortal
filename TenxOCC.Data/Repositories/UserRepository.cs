using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Repositories
{
    public class UsersRepository : BaseRepository<Users>, IUsersRepository
    {
        public UsersRepository() : this(new GeneralDBContext()) { }

        public UsersRepository(GeneralDBContext context) : base(context)
        {

        }
    }
}

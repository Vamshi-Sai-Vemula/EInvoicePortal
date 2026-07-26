using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;

namespace TenxOCC.Data.Repositories
{
    public class ConfigurationRepository : BaseRepository<Configuration>, IConfiguration
    {
        public ConfigurationRepository() : this(new GeneralDBContext()) { }

        public ConfigurationRepository(GeneralDBContext context) : base(context)
        {

        }
    }
}

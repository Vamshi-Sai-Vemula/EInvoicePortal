using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Mapping
{
    public class ConfigurationMap : EntityTypeConfiguration<Configuration>
    {
        public ConfigurationMap()
        {
            this.ToTable("EInvoiceConfigurations");
            HasKey(a => a.Id);
        }
    }
}

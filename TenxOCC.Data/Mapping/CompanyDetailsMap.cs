//using System;
//using System.Collections.Generic;
//using System.Data.Entity.ModelConfiguration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TenxOCC.Data.Entities;

//namespace TenxOCC.Data.Mapping
//{
//    public class CompanyDetailsMap : EntityTypeConfiguration<CompanyDetailsEntity>
//    {
//        public CompanyDetailsMap()
//        {
//            this.ToTable("company_details");
//            HasKey(a => a.Id);
//        }
//    }
//}


using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Mapping
{
    public class CompanydetailsMap
        : EntityTypeConfiguration<CompanyDetailsEntity>
    {
        public CompanydetailsMap()
        {
            ToTable("company_details");


            HasKey(x => x.Id);


            Property(x => x.Id)
            .HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);


            Property(x => x.companyTaxRate)
            .HasPrecision(18, 2);
        }
    }
}
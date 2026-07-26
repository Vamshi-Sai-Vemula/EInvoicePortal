using TenxOCC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data
{
    public class GeneralDBContext : DbContext
    {
        public GeneralDBContext()
          : base("name=DBEntities")
        {
            Database.SetInitializer<GeneralDBContext>(null);
        }

        //public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }

        public virtual DbSet<CompanyDetailsEntity> Companydetails { get; set; }

        public virtual DbSet<InvoiceHeader> InvoiceHeaders { get; set; }

        public virtual DbSet<InvoiceLine> InvoiceLines { get; set; }

      


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new RolesMap());
            modelBuilder.Configurations.AddFromAssembly(typeof(GeneralDBContext).Assembly);

            base.OnModelCreating(modelBuilder);

        }
    }

}

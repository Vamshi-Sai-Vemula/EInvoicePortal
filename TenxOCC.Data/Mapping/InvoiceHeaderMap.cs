using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Mapping
{
    public class InvoiceHeaderMap : EntityTypeConfiguration<InvoiceHeader>
    {
        public InvoiceHeaderMap()
        {
            ToTable("TNX_INVOICE_HEADER");

            HasKey(x => x.DocEntry);

            Property(x => x.DocEntry)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasMany(x => x.InvoiceLines)
                .WithRequired(x => x.InvoiceHeader)
                .HasForeignKey(x => x.DocEntry)
                .WillCascadeOnDelete(true);
        }
    }
}
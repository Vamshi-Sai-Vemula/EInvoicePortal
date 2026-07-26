using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Mapping
{
    public class InvoiceLineMap : EntityTypeConfiguration<InvoiceLine>
    {
        public InvoiceLineMap()
        {
            ToTable("TNX_INVOICE_LINE");

            HasKey(x => new
            {
                x.DocEntry,
                x.LineNum
            });

            Property(x => x.DocEntry)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(x => x.InvoiceHeader)
                .WithMany(x => x.InvoiceLines)
                .HasForeignKey(x => x.DocEntry);
        }
    }
}
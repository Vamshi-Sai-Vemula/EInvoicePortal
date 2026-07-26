using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;

namespace TenxOCC.Data.Repositories
{
    public class InvoiceLineRepository
        : BaseRepository<InvoiceLine>, IInvoiceLine
    {

        public InvoiceLineRepository()
            : this(new GeneralDBContext())
        {

        }


        public InvoiceLineRepository(
            GeneralDBContext context)
            : base(context)
        {

        }

    }
}
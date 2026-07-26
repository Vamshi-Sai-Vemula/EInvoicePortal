using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;

namespace TenxOCC.Data.Repositories
{
    public class InvoiceHeaderRepository
        : BaseRepository<InvoiceHeader>, IInvoiceHeader
    {

        public InvoiceHeaderRepository()
            : this(new GeneralDBContext())
        {

        }


        public InvoiceHeaderRepository(
            GeneralDBContext context)
            : base(context)
        {

        }

    }
}
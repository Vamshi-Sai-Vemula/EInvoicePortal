using System.Linq;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Interfaces
{
    public interface IInvoiceHeader
    {

        IQueryable<InvoiceHeader> GetAll();

        InvoiceHeader GetByID(object id);

        InvoiceHeader Insert(InvoiceHeader entity);

        int Update(InvoiceHeader entity);

        void Delete(object id);

    }
}
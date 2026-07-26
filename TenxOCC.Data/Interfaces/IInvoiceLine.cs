using System.Linq;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Interfaces
{
    public interface IInvoiceLine
    {

        IQueryable<InvoiceLine> GetAll();

        InvoiceLine GetByID(object id);

        InvoiceLine Insert(InvoiceLine entity);

        int Update(InvoiceLine entity);

        void Delete(object id);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenxOCC.Data.Entities;

namespace TenxOCC.Data.Interfaces
{
    public interface ICompanyDetails
    {
       
            IQueryable<CompanyDetailsEntity> GetAll();

        CompanyDetailsEntity GetByID(object id);

        CompanyDetailsEntity Insert(CompanyDetailsEntity entity);

            int Update(CompanyDetailsEntity entity);

        void Delete(object id);
    }
}

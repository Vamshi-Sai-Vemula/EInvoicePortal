using TenxOCC.Data;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;
using TenxOCC.Data.Repositories;

public class CompanyDetailsRepository
: BaseRepository<CompanyDetailsEntity>, ICompanyDetails
{

    public CompanyDetailsRepository()
        : this(new GeneralDBContext())
    {

    }


    public CompanyDetailsRepository(
        GeneralDBContext context)
        : base(context)
    {

    }


}
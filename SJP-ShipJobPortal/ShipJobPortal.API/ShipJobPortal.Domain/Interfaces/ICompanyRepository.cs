using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<ReturnResult> CreateCompanyAsync(CompanyCreate company);
    Task<ReturnResult<List<CompanyDropDetails>>> GetAllCompaniesAsync();
    Task<ReturnResult<List<CompanyDetails>>> GetCompanyAsync(int CompanyId);
}
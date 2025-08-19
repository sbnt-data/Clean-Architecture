using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.Services;

namespace ShipJobPortal.Application.IServices;

public interface ICompanyService
{

    Task<ApiResponse<string>> CreateCompanyAsync(CompanyCreateDto dto, string username);
    Task<ApiResponse<IEnumerable<CompanyDto>>> GetCompanyAsync(int CompanyId);
    Task<ApiResponse<IEnumerable<CompanyDropDto>>> GetAllCompaniesAsync();
}
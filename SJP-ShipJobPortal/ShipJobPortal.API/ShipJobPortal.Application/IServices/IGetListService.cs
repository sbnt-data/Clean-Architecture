using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IGetListService
{
    Task<ApiResponse<IEnumerable<GetCountryListDto>>> GetAllCountryAsync();
    Task<ApiResponse<IEnumerable<GetStateListDto>>> GetAllStateAsync(int? countryId);
    Task<ApiResponse<IEnumerable<GetCityListDto>>> GetAllCityAsync(int? stateId);
    Task<ApiResponse<IEnumerable<VesselTypeDto>>> GetAllVesselTypeAsync();
    Task<ApiResponse<IEnumerable<ContractDurationDto>>> GetAllContractDurationAsync();
    Task<ApiResponse<IEnumerable<PositionDto>>> GetAllPositionAsync(int? companyId);
    Task<ApiResponse<IEnumerable<MobileCountryCodeDto>>> GetAllMobileCodesAsync();

}

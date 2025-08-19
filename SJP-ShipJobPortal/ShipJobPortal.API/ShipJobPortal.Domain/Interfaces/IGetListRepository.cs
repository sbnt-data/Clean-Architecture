using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IGetListRepository
{
    Task<ReturnResult<List<GetCountryList>>> GetAllCountriesAsync();
    Task<ReturnResult<List<GetStateList>>> GetAllStatesAsync(int? countryId);
    Task<ReturnResult<List<GetCityList>>> GetAllCitiesAsync(int? stateId);
    Task<ReturnResult<List<VesselTypeDrop>>> GetAllVesselTypeAsync();
    Task<ReturnResult<List<ContractDurationDrop>>> GetAllContractDurationAsync();
    Task<ReturnResult<List<PositionDrop>>> GetAllPositionAsync(int? companyId);
    Task<ReturnResult<List<MobileCountryCodeModel>>> GetAllMobileCountriesAsync();

}

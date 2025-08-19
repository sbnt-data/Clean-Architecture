using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class GetCountryListDto
    {
        public int? ID { get; set; }
        public string? Code { get; set; }
        public string? CountryName { get; set; }
    }

    public class GetStateListDto
    {
        public int? ID { get; set; }
        public string? StateName { get; set; }
    }

    public class GetCityListDto
    {
        public int? ID { get; set; }
        public string? CityName { get; set; }
    }

    public class VesselTypeDto
    {
        public int? TypeID { get; set; }
        public string? VesselCode { get; set; }
        public string? Vesseltype { get; set; }
    }

    public class ContractDurationDto 
    {
        public int? DurationId { get; set; }
        public string? Value { get; set; }
        public string? ContractMonths { get; set; }
    }

    public class PositionDto
    {
        public int? ID { get; set; }
        public string? Name { get; set; }

    }

    public class MobileCountryCodeDto 
    {
        public int Id { get; set; }
        public string? CountryCode { get; set; }
        public string? DialCode { get; set; }
        public string? CountryName { get; set; }
        public string? FlagEmoji { get; set; }
        public string? DisplayText { get; set; } 
    }

}

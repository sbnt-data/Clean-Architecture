using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class GetCountryList
{
    public int? ID { get; set; }
    public string? Code { get; set; }
    public string? CountryName { get; set; }
}

public class GetStateList
{
    public int? ID { get; set; }
    public string? StateName { get; set; }
}

public class GetCityList
{
    public int? ID { get; set; }
    public string? CityName { get; set; }
}

public class VesselTypeDrop
{
    public int? TypeID { get; set; }
    public string? VesselCode { get; set; }
    public string? Vesseltype { get; set; }
}
public class ContractDurationDrop
{
    public int? DurationId { get; set; }
    public string? Value { get; set; }
    public string? ContractMonths { get; set; }
}

public class PositionDrop
{
    public int? ID { get; set; }
    public string? Name { get; set; }
}

public class MobileCountryCodeModel 
{
    public int Id { get; set; }
    public string? CountryCode { get; set; }
    public string? DialCode { get; set; }
    public string? CountryName { get; set; }
    public string? FlagEmoji { get; set; }
    public string? DisplayText { get; set; }
}

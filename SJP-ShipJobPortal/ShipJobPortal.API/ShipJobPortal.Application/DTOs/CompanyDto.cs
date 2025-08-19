namespace ShipJobPortal.Application.DTOs
{
    public class CompanyCreateDto
    {
        public int CompanyID { get; set; }
        public int? ParentCompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LocalCompanyName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Website { get; set; }
        public string? Remarks { get; set; }
        public string? Address { get; set; }
        public int CityID { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CompanyDto
    {
        public int CompanyID { get; set; }
        public int? ParentCompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LocalCompanyName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Website { get; set; }
        public string? Remarks { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CompanyDropDto
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        
    }
}

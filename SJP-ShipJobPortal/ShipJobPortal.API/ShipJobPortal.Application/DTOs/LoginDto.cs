using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class UserCredentialsDto
    {
        public string Email_Id { get; set; }
        public string? Password { get; set; }
    }

    public class UserApplicantDto
    {
        public string? Role { get; set; }
        public string? loginType { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
        public string? Title { get; set; }
        public string? Designation { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyDetails { get; set; }
        //public string Website { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public int? CityID { get; set; }
        public int? StateID { get; set; }
        public int? CountryID { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Password { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Username { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }  // "seafarer" | "recruiter"

        [Required]
        public string LoginType { get; set; } // "portal" | "google" | "linkedin"

        // Required only for portal login
        public string? Password { get; set; }

        // Should be provided only if loginType is google/linkedin (already validated on frontend)
        public string OauthToken { get; set; }
    }

    public class AuthenticateResult
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string LoginType { get; set; }
        public string? ResumeStatus { get; set; }

        public List<ModulePrevilleages> RoleModulePrevilleages { get; set; } = new();

        // Tokens to be sent to controller for cookie storage
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

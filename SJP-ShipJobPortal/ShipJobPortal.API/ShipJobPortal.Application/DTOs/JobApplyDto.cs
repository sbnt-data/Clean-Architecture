using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Application.DTOs
{
    public class AppliedCandidatesListDto
    {
        public List<JobApplyDto>CandidatesList { get; set; }
        public PaginationDto pagination { get; set; }
    }
    public class JobApplyDto
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? Designation { get; set; }
        public string? SalaryExpected { get; set; }
        public string? Email { get; set; }
        public string? DateOfAvailability { get; set; }
        public int JobId { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? TotalYearsOfExperiance { get; set; }
    }

    public class JobApplyPostDto
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public byte[]? CoverLetter { get; set; }
        public byte[]? ResumeFile { get; set; }
        public string? Notes { get; set; }
    }

    public class AppliedJobsListDto
    {
        public List<AppliedJobDto> JobsList { get; set; }
        public PaginationDto pagination { get; set; }
    }

    public class AppliedJobDto
    {
        public int ApplicationId { get; set; }
        public long JobId { get; set; }//
        public string? JobTitle { get; set; }
        public long NationalityId { get; set; }//
        public string? Nationality { get; set; }
        public string? JobStatus { get; set; }
        public long CompanyId { get; set; }//
        public string? CompanyName { get; set; }
        public string? AppliedDate { get; set; }
    }
    public class JobWishListDto
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public string? Mode { get; set; }
    }

    public class SavedJobsListDto
    {
        public List<SavedJobsDto> JobsList { get; set; }
        public PaginationDto pagination { get; set; }
    }

    public class SavedJobsDto
    {
        public decimal? JobId { get; set; }                 // If returned as 10034.0
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? NoVacancy { get; set; }
        public DateTime? OpenDate { get; set; }            // Or string, depending on backend format
        public DateTime? CloseDate { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
    }
    public class CandidateListtoShiphireDto
    {
        public int userid { get; set; }
        public List<int>?candidates { get; set; }
    } 

    public class MatchingCandidateViewDto
    {
        public PaginationDto? pagination { get; set; }
        public List< MatchingCandidateDto>? candidates {  get; set; }
    }
    public class MatchingCandidateDto
    {
        public int? candidateid { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? experience { get; set; }
        public string? designation { get; set; }
        public string? positionapplied { get; set; }
        public string? education { get; set; }
        public string? nationality { get; set; }
        public string? salaryexpected { get; set; }
    }


    public class JobActiononCandidateDto
    {
        public int candidateId { get; set; }
        public int applicationId { get; set; }
        public string? jobAction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ShipJobPortal.Domain.Entities;

public class AppliedCandidatesListModel
{
    public List<JobAppliedModel> CandidatesList { get; set; }
    public PaginationModel pagination { get; set; }
}
public class JobAppliedModel
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? Designation { get; set; }
    public string? SalaryExpected { get; set; }
    public string? Email { get; set; }
    public string? DateOfAvailability { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    public long JobId { get; set; }
    public string? TotalYearsOfExperiance { get; set; }
    public string? AppliedDate {  get; set; }

}

public class JobApplyPostModel
{
    public int UserId { get; set; }
    public int JobId { get; set; }
    public byte[]? CoverLetter { get; set; }
    public byte[]? ResumeFile { get; set; }
    public string? Notes { get; set; }
}


public class AppliedJobsListModel
{
    public List<AppliedJobs> JobsList { get; set; }
    public PaginationModel pagination { get; set; }
}

public class AppliedJobs
{
    public int ApplicationId { get; set; }
    public long JobId { get; set; }//
    public string? JobTitle { get; set; }
    public long NationalityId { get; set; }//
    public string? Nationality { get; set; }
    public string? JobStatus { get; set; }
    public long CompanyId { get; set; }//
    public string? CompanyName {  get; set; }
    public string? AppliedDate { get; set; }
}

public class JobWishlistModel
{
    public int UserId { get; set; }
    public int JobId { get; set; }
    public string? Mode { get; set; }
}

public class SavedJobsListModel
{
    public List<SavedJobsModel> JobsList { get; set; }
    public PaginationModel pagination { get; set; }
}

public class SavedJobsModel
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

public class MatchingCandidateViewModel
{
    public PaginationModel? pagination { get; set; }
    public List<MatchingCandidateModel>? candidates { get; set; }
}
public class MatchingCandidateModel
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

public class JobActionOnCandidateModel
{
    public int candidateId { get; set; }
    public int applicationId { get; set; }
    public string? jobAction { get; set; }
}

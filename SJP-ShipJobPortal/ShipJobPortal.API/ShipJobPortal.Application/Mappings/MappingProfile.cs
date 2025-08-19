using AutoMapper;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //after
            CreateMap<CompanyCreateDto, CompanyCreate>().ReverseMap();
            CreateMap<UserApplicantDto, UserApplicantModel>().ReverseMap();
            CreateMap<ResetPasswordDto, UserApplicantModel>().ReverseMap();
            CreateMap<CompanyDropDto, CompanyDropDetails>().ReverseMap();

            CreateMap<GetCountryListDto, GetCountryList>().ReverseMap();
            CreateMap<GetStateListDto, GetStateList>().ReverseMap();
            CreateMap<GetCityListDto, GetCityList>().ReverseMap();
            CreateMap<VesselTypeDto,VesselTypeDrop>().ReverseMap();
            CreateMap<ContractDurationDto, ContractDurationDrop>().ReverseMap();
            CreateMap<PositionDto, PositionDrop>().ReverseMap();


            CreateMap<JobCreateDto, JobPostModel>().ReverseMap();
            CreateMap<jobViewDto, JobViewModel>().ReverseMap();
            CreateMap<JobPatchDto, JobPostModel>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            CreateMap<JobViewFilteredDto, JobViewFilteredModel>().ReverseMap();
            CreateMap<PaginationDto, PaginationModel>().ReverseMap();
            CreateMap<UserResumeandImageDto,UserFilesModel>().ReverseMap();
            CreateMap<AppliedCandidatesListDto, AppliedCandidatesListModel>().ReverseMap();
            CreateMap<JobApplyDto, JobAppliedModel>().ReverseMap();
            CreateMap<JobApplyPostDto, JobApplyPostModel>().ReverseMap();

            CreateMap<AppliedJobsListDto, AppliedJobsListModel>().ReverseMap();
            CreateMap<AppliedJobDto, AppliedJobs>().ReverseMap();

            CreateMap<CertificatesViewPatchDto, CertificatesViewPatchModel>().ReverseMap();
            CreateMap<SeaExperianceViewPatchDto, SeaExperianceViewPatchModel>().ReverseMap();
            CreateMap<UserProfileViewDto, UserProfileViewModel>().ReverseMap();
            CreateMap<UserProfileDto, UserProfileModel>().ReverseMap();
            CreateMap<UserDetailsDto, UserApplicantPostModel>().ReverseMap();
            CreateMap<DocumentsAndCerticationsDto, DocumentsAndCertications>().ReverseMap();
            CreateMap<PreviousSeaExperianceDto, PreviousSeaExperiance>().ReverseMap();
            CreateMap<UserLoginDto, UserApplicantModel>().ReverseMap();
            CreateMap<ModulePrevilleages, ModulePrevilleagesModel>().ReverseMap();
            CreateMap<JobWishListDto, JobWishlistModel>().ReverseMap();
            CreateMap<SavedJobsListDto, SavedJobsListModel>().ReverseMap();
            CreateMap<SavedJobsDto, SavedJobsModel>().ReverseMap();
            CreateMap<ReferAFriendDto, ReferAFriendModel>();
            CreateMap<JobViewCountDto, JobViewCountModel>();
            CreateMap<MatchingCandidateDto, MatchingCandidateModel>().ReverseMap();
            CreateMap<MatchingCandidateViewDto, MatchingCandidateViewModel>().ReverseMap();
            // In your AutoMapper profile
            CreateMap<userOtpDto, UserOtpCredentialsOtp>()
                .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserOtpVerifyDto, UserOtpVerifyModel>().ReverseMap();
            CreateMap<CompanyViewDto, CompanyViewModel>().ReverseMap();
            CreateMap<JobActiononCandidateDto,JobActionOnCandidateModel>().ReverseMap();
            CreateMap<MobileCountryCodeDto, MobileCountryCodeModel>().ReverseMap();




            //before
            CreateMap<CompanyDto, CompanyDetails>().ReverseMap();


            //CreateMap<UserApplicantModel, UserApplicantDto>().ReverseMap();
            //CreateMap<ResetPasswordDto, UserApplicantModel>().ReverseMap();
            CreateMap<ModuleDto, ModuleModel>().ReverseMap();
            CreateMap<RoleModuleMapDto, RoleModuleMapModel>().ReverseMap();

            //CreateMap<UserProfileDto, UserProfileModel>().ReverseMap();
            CreateMap<NationalityDto, NationalityModel>().ReverseMap();

            CreateMap<RoleDto, RoleModel>().ReverseMap();
            CreateMap<UserRoleMapDto, UserRoleMap>().ReverseMap();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Application.Services;

public class OtpService: IOtpService
{
    private readonly IOtpRepositories _otpRepo;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;
    private readonly IMapper _mapper;



    public OtpService(IOtpRepositories otpRepository,IEmailService email_service, ILogger<OtpService> logger,IMapper mapper)
    {
        _mapper= mapper;
        _logger = logger;
        _otpRepo = otpRepository; 
        _emailService = email_service;
    }

    public async Task<ApiResponse<string>> GenerateAndSaveOtpAsync(userOtpDto otpData)
    {
        try
        {
            // Generate 6-digit OTP
            var otp = new Random().Next(0, 1000000).ToString("D6");

            // Map DTO to DB model
            var otpdata = _mapper.Map<UserOtpCredentialsOtp>(otpData);
            otpdata.OtpCode = otp;

            // Save OTP to DB
            var otpSaveResult = await _otpRepo.fn_UserOtpSave(otpdata);

            if (otpSaveResult.ReturnStatus == "success" && otpSaveResult.ErrorCode == "200")
            {
                // Try sending email
                var emailResult = await _emailService.SendOtpEmailAsync(otpData.Email, otp);

                if (emailResult.Success)
                {
                    return new ApiResponse<string>(
                        success: true,
                        data: "OTP Sent",
                        message: $"OTP sent to email: {otpData.Email}",
                        errorCode: "200"
                    );
                }

                // Email failed
                return new ApiResponse<string>(
                    success: false,
                    data: null,
                    message: emailResult.Message ?? "Email sending failed.",
                    errorCode: emailResult.ErrorCode ?? "EMAIL_FAIL"
                );
            }

            // OTP save failed
            return new ApiResponse<string>(
                success: false,
                data: null,
                message: otpSaveResult.ReturnStatus ?? "Failed to save OTP.",
                errorCode: otpSaveResult.ErrorCode ?? "SAVE_FAIL"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GenerateAndSaveOtpAsync");
            throw;
        }
    }



    public async Task<ApiResponse<string>> UserOtpVerification(UserOtpVerifyDto user)
    {
        try
        {
            // Map input DTO to internal model
            var otpModel = _mapper.Map<UserOtpVerifyModel>(user);

            // Call repository to validate OTP
            var result = await _otpRepo.fn_UserOtpVerify(otpModel);

            if (result.ReturnStatus == "success"&&result.ErrorCode== "200")
            {
                return new ApiResponse<string>(
                    success: true,
                    data: result.Data,
                    message: "OTP verification successful.",
                    errorCode: "200"
                );
            }

            // OTP validation failed
            return new ApiResponse<string>(
                success: false,
                message: "Invalid or expired OTP.",
                errorCode: "INVALID_OTP"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in UserOtpVerification");

            throw;
        }
    }



}

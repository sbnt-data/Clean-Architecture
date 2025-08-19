using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IOtpService
{
    Task<ApiResponse<string>> GenerateAndSaveOtpAsync(userOtpDto otpData);
    Task<ApiResponse<string>> UserOtpVerification(UserOtpVerifyDto user);
}

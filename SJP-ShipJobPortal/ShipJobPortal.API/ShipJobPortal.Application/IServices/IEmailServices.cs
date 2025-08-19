using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.IServices;

public interface IEmailService
{
    Task<ApiResponse<bool>> SendOtpEmailAsync(string toEmail, string otpCode);
}

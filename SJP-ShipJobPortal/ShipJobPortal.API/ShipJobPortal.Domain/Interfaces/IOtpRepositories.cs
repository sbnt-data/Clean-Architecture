using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IOtpRepositories
{
    Task<ReturnResult<string>> fn_UserOtpSave(UserOtpCredentialsOtp otp);
    Task<ReturnResult<string>> fn_UserOtpVerify(UserOtpVerifyModel user);
}

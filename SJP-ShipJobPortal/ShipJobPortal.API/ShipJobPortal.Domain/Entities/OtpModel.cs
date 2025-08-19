using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Domain.Entities;

public class UserOtpCredentialsOtp
{
    public string OtpCode { get; set; }
    public string ToAddress { get; set; }
    public string Email { get; set; } = null;

}
public class UserOtpVerifyModel
{

    public string emailid { get; set; }
    public string otp { get; set; }
    public string IpAddress { get; set; } = null;

}
public class OtpInsertResult
{
    public string ReturnStatus { get; set; }
    public string ErrorCode { get; set; }
    public int OtpId { get; set; }
}

public class OtpValidationResult
{
    public string ReturnStatus { get; set; }
    public string ErrorCode { get; set; }
}

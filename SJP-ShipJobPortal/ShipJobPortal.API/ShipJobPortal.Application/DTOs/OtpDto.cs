using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class userOtpDto 
    {
        public string Email { get; set; }
    }

    public class UserOtpVerifyDto
    {

        public string emailid { get; set; }
        public string otp { get; set; }
        public string IpAddress { get; set; } = null;

    }
}

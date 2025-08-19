using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ShipJobPortal.Application.DTOs;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Application.Services;
using ShipJobPortal.Domain.Constants;
using ShipJobPortal.Domain.Entities;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginServices;
        private readonly ILogger<LoginController> _logger;
        private readonly IDbExceptionLogger _dbExceptionLogger;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;

        public LoginController(ILoginService loginServices, ILogger<LoginController> logger, IDbExceptionLogger dbExceptionLogger, IConfiguration configuration, IOtpService otpService)
        {
            _loginServices = loginServices;
            _logger = logger;
            _dbExceptionLogger = dbExceptionLogger;
            _configuration = configuration;
            _otpService = otpService;
        }

        [HttpPost("UserLogIn")]
        public async Task<IActionResult> UserLogIn([FromBody] UserCredentialsDto user)
        {
            try
            {
                var result = await _loginServices.GetUserAsync(user);
                if (result.Success)
                {
                    var auth = result.Data;

                    int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                    int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);

                    Response.Cookies.Append("AccessToken", auth.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,                      // JS cannot access
                        Secure = true,                        // HTTPS only
                        SameSite = SameSiteMode.Strict,       // Prevent CSRF
                        Expires = DateTime.UtcNow.AddSeconds(jwtExpiryHours)
                    });

                    Response.Cookies.Append("RefreshToken", auth.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(refreshExpiryDays)
                    });

                    var safeData = new
                    {
                        userId = auth.UserId,
                        email = auth.Email,
                        role = auth.Role
                    };

                    return Ok(new ApiResponse<object>(true, safeData, "Login successful.", ErrorCodes.Success));
                }
                return BadRequest(new ApiResponse<object>(false, null, "Invalid email or password.", ErrorCodes.InvalidCredentials));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserLogIn");
                await _dbExceptionLogger.LogExceptionAsync("UserLogIn_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<object>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserApplicantDto userDto)
        {
            try
            {
                var response = await _loginServices.CreateUserAsync(userDto);
                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CreateUser");
                await _dbExceptionLogger.LogExceptionAsync("CreateUser_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<object>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["SjpJwtRefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new ApiResponse<object>(false, null, "Missing refresh token."));
                }
                var result = await _loginServices.RefreshTokenAsync(refreshToken);

                if (!result.Success)
                {
                    return Unauthorized(new ApiResponse<object>(false, null, result.Message));
                }
                var auth = result.Data;
                int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
                Response.Cookies.Append("SjpJwtToken", auth.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,                 // required with SameSite=None
                    SameSite = SameSiteMode.Lax,  // allow cross-site requests
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddHours(jwtExpiryHours)
                });

                Response.Cookies.Append("SjpJwtRefreshToken", auth.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(refreshExpiryDays)
                });
                var safeData = new
                {
                    userId = auth.UserId,
                    email = auth.Email,
                    role = auth.Role
                };
                return Ok(new ApiResponse<object>(true, safeData, "Token refreshed successfully.", ErrorCodes.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefreshToken");
                await _dbExceptionLogger.LogExceptionAsync("RefreshToken_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<AuthResponse>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["SjpJwtRefreshToken"];
                var accesstoken = Request.Cookies["SjpJwtToken"];
                var allowedRoutes = Request.Cookies["allowedRoutes"];
                var navigation = Request.Cookies["navigation"];
                var user = Request.Cookies["user"];
                var userEmail = Request.Cookies["userEmail"];
                var userId = Request.Cookies["userId"];

                //string mesage= "refreshToken:" + refreshToken+ " accesstoken:" + accesstoken+ " allowedRoutes:" + allowedRoutes+ " navigation:" + navigation+ " user:" + user+ " userEmail: " + userEmail+"userId: "+userId;


                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "refreshToken", refreshToken);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "accesstoken", accesstoken);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "allowedRoutes", allowedRoutes);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "navigation", navigation);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "user", user);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "userEmail", userEmail);
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", "userId", userId);


                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new ApiResponse<object>(false, null, "Missing refresh token."));

                var result = await _loginServices.LogoutAsync(refreshToken); // revoke in DB

                if (result)
                {
                    //var del = new CookieOptions
                    //{
                    //    Path = "/",
                    //    HttpOnly = true,
                    //    Secure = true,
                    //    SameSite = SameSiteMode.None
                    //    // Domain = null
                    //};
                    //del.Expires = DateTimeOffset.UnixEpoch;             // expire it
                    //Response.Cookies.Append("SjpRefreshToken", "", del);
                    //Response.Cookies.Append("SjpJwtToken", "", del);

                    Response.Cookies.Delete("SjpJwtRefreshToken");
                    Response.Cookies.Delete("SjpJwtToken");
                    Response.Cookies.Delete("allowedRoutes");
                    Response.Cookies.Delete("navigation");
                    Response.Cookies.Delete("user");
                    Response.Cookies.Delete("userEmail");
                    Response.Cookies.Delete("userId");

                    return Ok(new ApiResponse<object>(true, null, "Logged out successfully.", ErrorCodes.Success));
                }

                return BadRequest(new ApiResponse<object>(false, null, "Invalid or already deleted token.", ErrorCodes.BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Logout");
                await _dbExceptionLogger.LogExceptionAsync("Logout_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<object>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            try
            {
                var result = await _loginServices.ResetPasswordAsync(request);

                if (result.Success)
                    return Ok(new ApiResponse<object>(true, null, result.Message, ErrorCodes.Success));

                return BadRequest(new ApiResponse<object>(false, null, result.Message, ErrorCodes.BadRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword");
                await _dbExceptionLogger.LogExceptionAsync("ResetPassword_Controller", ex.Message, ex.StackTrace);
                return StatusCode(500, new ApiResponse<object>(false, null, "Internal Server Error", ErrorCodes.InternalServerError));
            }
        }

        #region Login

        /// <summary>
        /// this api used to login or signup user for googleoAuth
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>

        [HttpPost("login-Para")] 
        public async Task<IActionResult> loginasync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate invalid combinations according to your rules:
                // 1. Recruiter can only login with "portal"
                if (loginDto.Role.ToLower() == "recruiter" && loginDto.LoginType.ToLower() != "portal")
                {
                    return Forbid("Recruiters can only login with 'portal' login type.");
                }

                if (loginDto.LoginType.ToLower() == "portal" && string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest("Password is required for portal login.");
                }
                var result = await _loginServices.AuthenticateUserAsync_test(loginDto);
                if (result.Data == null)
                {
                    return BadRequest(result);
                }
                if (result.Data.ResumeStatus.ToLower() == "available" || result.Data.ResumeStatus.ToLower() == "recruiter")
                {
                    //tokens
                    int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                    int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
                    string jwttoken = result.Data.AccessToken; string refreshtoken = result.Data.RefreshToken;
                    Response.Cookies.Append("SjpJwtToken", jwttoken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,                 // required with SameSite=None
                        SameSite = SameSiteMode.Lax,  // allow cross-site requests
                        Path = "/",
                        Expires = DateTimeOffset.UtcNow.AddHours(jwtExpiryHours)
                    });

                    Response.Cookies.Append("SjpJwtRefreshToken", refreshtoken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTimeOffset.UtcNow.AddDays(refreshExpiryDays)
                    });

                    result.Data.Email = null; result.Data.Role = null; result.Data.LoginType = null;
                    return Ok(new ApiResponse<AuthenticateResult>(true, result.Data, "Login successful", ErrorCodes.Success));
                }

                return Ok(new ApiResponse<int>(true, result.Data.UserId, "Login successful", ErrorCodes.Success));


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                await _dbExceptionLogger.LogExceptionAsync("Login", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
            // Basic model validation

        }


        [HttpPost("OauthUser")]
        public async Task<IActionResult> googleoauthUserasync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                // Basic model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate invalid combinations according to your rules:
                // 1. Recruiter can only login with "portal"
                if (loginDto.Role.ToLower() == "recruiter" && loginDto.LoginType.ToLower() != "portal")
                {
                    return Forbid("Recruiters can only login with 'portal' login type.");
                }

                // 2. Password is required only for portal login
                if (loginDto.LoginType.ToLower() == "portal" && string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest("Password is required for portal login.");
                }



                // Now delegate login to your service layer (includes verification etc)
                var result = await _loginServices.OauthuserAsync(loginDto);

                if (result.Data == null)
                {
                    return BadRequest(result);
                }
                if (result.Data.ResumeStatus.ToLower() == "available")
                {
                    //tokens
                    int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                    int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
                    string jwttoken = result.Data.AccessToken; string refreshtoken = result.Data.RefreshToken;
                    Response.Cookies.Append("SjpJwtToken", jwttoken, new CookieOptions
                    {
                        HttpOnly = true,                      // JS cannot access
                        Secure = true,                        // HTTPS only
                        SameSite = SameSiteMode.Lax,       // Prevent CSRF
                        Expires = DateTime.UtcNow.AddSeconds(jwtExpiryHours)
                    });

                    Response.Cookies.Append("SjpRefreshToken", refreshtoken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(refreshExpiryDays)
                    });
                    result.Data.Email = null;result.Data.Role = null;result.Data.LoginType = null;
                    return Ok(new ApiResponse<AuthenticateResult>(true, result.Data, "Login successful", ErrorCodes.Success));
                }

                return Ok(new ApiResponse<int>(true, result.Data.UserId, "Login successful", ErrorCodes.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OauthUser");
                await _dbExceptionLogger.LogExceptionAsync("OauthUser", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }

        }

        /// <summary>
        /// this api used to login or signup user for googleoAuth
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>

        [HttpPost("linkedinoauth")]
        public async Task<IActionResult> linkedinuserasync([FromBody] UserLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate invalid combinations according to your rules:
                // 1. Recruiter can only login with "portal"
                if (loginDto.Role.ToLower() == "recruiter" && loginDto.LoginType.ToLower() != "portal")
                {
                    return Forbid("Recruiters can only login with 'portal' login type.");
                }

                if (loginDto.LoginType.ToLower() == "portal" && string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest("Password is required for portal login.");
                }
                var result = await _loginServices.OauthuserAsync(loginDto);
                if (result.Data == null)
                {
                    return BadRequest(result);
                }
                if (result.Data.ResumeStatus.ToLower() == "available")
                {
                    //tokens
                    int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                    int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
                    string jwttoken = result.Data.AccessToken; string refreshtoken = result.Data.RefreshToken;
                    Response.Cookies.Append("SjpJwtToken", jwttoken, new CookieOptions
                    {
                        HttpOnly = true,                      // JS cannot access
                        Secure = true,                        // HTTPS only
                        SameSite = SameSiteMode.Lax,       // Prevent CSRF
                        Expires = DateTime.UtcNow.AddSeconds(jwtExpiryHours)
                    });

                    Response.Cookies.Append("SjpRefreshToken", refreshtoken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(refreshExpiryDays)
                    });
                    result.Data.Email = null; result.Data.Role = null; result.Data.LoginType = null;
                    return Ok(new ApiResponse<AuthenticateResult>(true, result.Data, "Login successful", ErrorCodes.Success));
                }

                return Ok(new ApiResponse<int>(true, result.Data.UserId, "Login successful", ErrorCodes.Success));


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                await _dbExceptionLogger.LogExceptionAsync("Login", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
            // Basic model validation

        }


        /// <summary>
        /// this api used to login or signup user for googleoAuth
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> registerusersync([FromBody] UserLoginDto loginDto) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate invalid combinations according to your rules:
                // 1. Recruiter can only login with "portal"
                if ( loginDto.LoginType.ToLower() != "register")
                {
                    return Forbid("cant register the user");
                }
                
                if (loginDto.LoginType.ToLower() == "register" && string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest("Password is required for portal login.");
                }
                var result = await _loginServices.RegisterAsyns(loginDto);
                if (result.Data == null)
                {
                    return BadRequest(result);
                }
                if (result.Data.ResumeStatus.ToLower() == "available" || result.Data.ResumeStatus.ToLower() == "recruiter")
                {
                    //tokens
                    int jwtExpiryHours = Convert.ToInt32(_configuration["TokenSettings:JwtTokenExpiryHours"]);
                    int refreshExpiryDays = Convert.ToInt32(_configuration["TokenSettings:RefreshTokenExpiryDays"]);
                    
                    string jwttoken = result.Data.AccessToken; 
                    string refreshtoken = result.Data.RefreshToken;
                    
                    Response.Cookies.Append("SjpJwtToken", jwttoken, new CookieOptions
                    {
                        HttpOnly = true,                      // JS cannot access
                        Secure = true,                        // HTTPS only
                        SameSite = SameSiteMode.Lax,       // Prevent CSRF
                        Expires = DateTime.UtcNow.AddSeconds(jwtExpiryHours)
                    });

                    Response.Cookies.Append("SjpRefreshToken", refreshtoken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(refreshExpiryDays)
                    });
                    result.Data.Email = null; result.Data.Role = null; result.Data.LoginType = null;
                    return Ok(new ApiResponse<AuthenticateResult>(true, result.Data, "Login successful", ErrorCodes.Success));
                }

                return Ok(new ApiResponse<int>(true, result.Data.UserId, "Login successful", ErrorCodes.Success));


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                await _dbExceptionLogger.LogExceptionAsync("Login", ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal Server Error");
            }
            // Basic model validation

        }

        [HttpPost("otpSent")]
        public async Task<IActionResult> UserOtpSave([FromBody] userOtpDto otp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(
                    success: false,
                    message: "Invalid input data.",
                    errorCode: "VALIDATION_FAILED"
                ));
            }

            try
            {
                var result = await _otpService.GenerateAndSaveOtpAsync(otp);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result); // Handles both SAVE_FAIL and EMAIL_FAIL
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UserOtpSave");

                return StatusCode(500, new ApiResponse<string>(
                    success: false,
                    message: "An unexpected error occurred while sending OTP.",
                    errorCode: "ERR500"
                ));
            }
        }


        [HttpPost("userOtpVerify")]
        public async Task<IActionResult> UserOtpVerify([FromBody] UserOtpVerifyDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(
                    success: false,
                    message: "Invalid input data.",
                    errorCode: "VALIDATION_FAILED"
                ));
            }
            if (string.IsNullOrEmpty(user.otp)||string.IsNullOrEmpty(user.emailid))
            {
                return BadRequest(new ApiResponse<string>(
                    success: false,
                    message: "otp required",
                    errorCode: "VALIDATION_FAILED"
                ));
            }

            try
            {
                var result = await _otpService.UserOtpVerification(user);

                if (result.Success)
                {
                    return Ok(result);
                }

                return Unauthorized(result); // Handles INVALID_OTP or failure
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UserOtpVerify");

                return StatusCode(500, new ApiResponse<string>(
                    success: false,
                    message: "An unexpected error occurred while verifying OTP.",
                    errorCode: "ERR500"
                ));
            }
        }



        #endregion
    }
}

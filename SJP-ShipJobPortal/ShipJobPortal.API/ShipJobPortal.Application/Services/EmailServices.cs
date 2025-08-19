using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
using ShipJobPortal.Application.IServices;
using ShipJobPortal.Domain.Settings;
using Microsoft.Extensions.Logging;
using ShipJobPortal.Application.DTOs;

namespace ShipJobPortal.Application.Services;




public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;


    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _logger = logger;
        _emailSettings = emailSettings.Value;
    }

    public async Task<ApiResponse<bool>> SendOtpEmailAsync(string toEmail, string otpCode)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromAddress),
                Subject = "Your OTP Code",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            DateTime valid = DateTime.Now.AddMinutes(5);
            message.Body = $@"
        Hi <b>{toEmail}</b><br><br>
        Your OTP code is: <b>{otpCode}</b><br><br>
        Valid only for 5 minutes (until {valid})<br><br>
        Thank you, have a nice day :)";

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_emailSettings.FromAddress, _emailSettings.AppPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);

            return new ApiResponse<bool>(
                success: true,
                data: true,
                message: "OTP email sent successfully.",
                errorCode: "200"
            );
        }
        catch (SmtpFailedRecipientsException ex)
        {
            _logger.LogError(ex, "Failed to deliver email to some or all recipients.");

            return new ApiResponse<bool>(
                success: false,
                data: false,
                message: "One or more recipient email addresses were rejected.",
                errorCode: "SMTP_RECIPIENT_FAIL"
            );
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "SMTP error occurred while sending OTP email.");

            return new ApiResponse<bool>(
                success: false,
                data: false,
                message: "Failed to send email due to SMTP error.",
                errorCode: "SMTP_FAIL"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General exception occurred in SendOtpEmailAsync");

            throw;
        }
    }


}
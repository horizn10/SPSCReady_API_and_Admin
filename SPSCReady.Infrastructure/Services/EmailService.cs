using Microsoft.Extensions.Configuration;
using SPSCReady.Application.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SPSCReady.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOtpEmailAsync(string email, string otpCode)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");
                var senderEmail = smtpSettings["SenderEmail"];
                var senderPassword = smtpSettings["SenderPassword"];

                if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
                {
                    Console.WriteLine("[OTP] EmailSettings.SenderEmail/SenderPassword is missing.");
                    return false;
                }

                using var client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(senderEmail, "SPSCReady"),
                    Subject = "Your OTP for Login - SPSCReady",
                    Body = GetOtpEmailTemplate(otpCode),
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                await client.SendMailAsync(mail);
                Console.WriteLine($"[OTP] Email sent successfully to {email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OTP] Error sending OTP email: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex);
                return false;
            }
        }

        private string GetOtpEmailTemplate(string otpCode)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                            .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
                            .otp-box {{ background-color: #fff; padding: 15px; margin: 20px 0; text-align: center; border: 2px solid #4CAF50; border-radius: 5px; }}
                            .otp-code {{ font-size: 32px; font-weight: bold; color: #4CAF50; letter-spacing: 3px; }}
                            .footer {{ background-color: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 5px 5px; }}
                            .warning {{ color: #ff9800; font-weight: bold; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>SPSCReady - Login Verification</h2>
                            </div>
                            <div class='content'>
                                <p>Hello,</p>
                                <p>Use the following OTP code to complete your login:</p>
                                <div class='otp-box'>
                                    <div class='otp-code'>{otpCode}</div>
                                </div>
                                <p><strong>Important:</strong></p>
                                <ul>
                                    <li>This OTP is valid for <strong>10 minutes only</strong></li>
                                    <li class='warning'>Never share this code with anyone</li>
                                    <li>If you didn't request this login, please ignore this email</li>
                                </ul>
                                <p>Best regards,<br>SPSCReady Team</p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 SPSCReady. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}
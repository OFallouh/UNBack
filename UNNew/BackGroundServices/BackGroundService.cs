using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.Models;
using UNNew.Response;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
namespace UNNew.BackGroundServices
{
    public class BackGroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public BackGroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await AutomaticChangeEmployeeStatus();
                await SendLifeInsuranceNotificationsAsync();
                await SendEndContractNotificationsAsync();

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // تأخير 24 ساعة فقط
            }
        }

        public async Task AutomaticChangeEmployeeStatus()
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UNDbContext>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            //var expiredContracts = await db.EmployeeCoos
            //    .Include(x => x.Emp)
            //    .Where(x => x.EndCont < today && x.IsCancelled==false && x.Emp.Active!=false) // العقود المنتهية فقط
            //    .ToListAsync();

            //foreach (var contract in expiredContracts)
            //{
            //    contract.Emp.Active = false;
            //}
            //var ActiveContracts = await db.EmployeeCoos
            //.Include(x => x.Emp)
            //.Where(x => x.EndCont > today && x.IsCancelled == false && x.Emp.Active == false) // العقود المنتهية فقط
            //.ToListAsync();

            //foreach (var contract in ActiveContracts)
            //{
            //    contract.Emp.Active = true;
            //}
            var AllEmployee=db.UnEmps.ToList();
            foreach (var item in AllEmployee)
            {
                var EmployeeCoo = db.EmployeeCoos.Include(x=>x.Emp).
                    FirstOrDefault(x => x.EmpId == item.RefNo && x.EndCont >= today && x.IsCancelled == false);
                if (EmployeeCoo !=null)
                {
                    item.Active = true;
                }
                else
                {
                    item.Active = false;
                }
            }
            await db.SaveChangesAsync();
        }

        public async Task<ApiResponse<string>> SendLifeInsuranceNotificationsAsync()
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UNDbContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var employees = await db.EmployeeCoos
                    .Include(e => e.Emp)
                    .Where(e =>
                        e.EndLifeDate.HasValue &&
                        (e.EndLifeDate.Value.DayNumber - today.DayNumber) >= 0 &&
                        (e.EndLifeDate.Value.DayNumber - today.DayNumber) <= 10 &&
                        e.Emp.Active &&
                        e.EndCont.HasValue &&
                        e.EndCont.Value >= today) // عقده لا يزال ساري
                    .ToListAsync();

                if (!employees.Any())
                {
                    return new ApiResponse<string>("No employees found with life insurance expiring within 10 days.", null, null);
                }

                var adminEmails = await db.UserRoles
                    .Include(ur => ur.User)
                    .Where(ur => ur.RoleId == 1)
                    .Select(ur => ur.User.Email)
                    .Where(email => !string.IsNullOrEmpty(email))
                    .ToListAsync();

                if (!adminEmails.Any())
                {
                    return new ApiResponse<string>("No admins available to receive notifications.", null, null);
                }

                foreach (var email in adminEmails)
                {
                    await SendEmailNotificationAsync(email, employees, configuration);
                }

                return new ApiResponse<string>("Notifications sent successfully.", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("An error occurred while sending notifications.", null, new List<string> { ex.Message });
            }
        }

        private async Task SendEmailNotificationAsync(string email, List<EmployeeCoo> employees, IConfiguration configuration)
        {
            try
            {
                var body = "<h3>Employees with life insurance expiring within 10 days:</h3><ul>";
                foreach (var employee in employees)
                {
                    body += $"<li>{employee.Emp.EmpName} - Expiry Date: {employee.EndLifeDate}</li>";
                }
                body += "</ul>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(configuration["MailSettings:DisplayName"], configuration["MailSettings:From"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Life Insurance Expiry Notification";
                message.Date = DateTimeOffset.Now;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                try
                {
                    await smtp.ConnectAsync(configuration["MailSettings:Host"], int.Parse(configuration["MailSettings:Port"]), SecureSocketOptions.None);
                    await smtp.AuthenticateAsync(configuration["MailSettings:From"], configuration["MailSettings:Password"]);
                    await smtp.SendAsync(message);
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
            }
        }

        // --------------------------------------------

        public async Task<ApiResponse<string>> SendEndContractNotificationsAsync()
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UNDbContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var employees = await db.EmployeeCoos
                    .Include(e => e.Emp)
                    .Where(e =>
                        e.EndCont.HasValue &&
                        (e.EndCont.Value.DayNumber - today.DayNumber) >= 0 &&
                        (e.EndCont.Value.DayNumber - today.DayNumber) <= 10 &&
                        e.Emp.Active)
                    .ToListAsync();

                if (!employees.Any())
                {
                    return new ApiResponse<string>("No employees found with contract ending within 10 days.", null, null);
                }

                var adminEmails = await db.UserRoles
                    .Include(ur => ur.User)
                    .Where(ur => ur.RoleId == 1)
                    .Select(ur => ur.User.Email)
                    .Where(email => !string.IsNullOrEmpty(email))
                    .ToListAsync();

                if (!adminEmails.Any())
                {
                    return new ApiResponse<string>("No admins available to receive notifications.", null, null);
                }

                foreach (var email in adminEmails)
                {
                    await SendEmailNotificationEndContractAsync(email, employees, configuration);
                }

                return new ApiResponse<string>("Notifications sent successfully.", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("An error occurred while sending notifications.", null, new List<string> { ex.Message });
            }
        }

        private async Task SendEmailNotificationEndContractAsync(string email, List<EmployeeCoo> employees, IConfiguration configuration)
        {
            try
            {
                var body = "<h3>Employees with contracts ending within 10 days:</h3><ul>";
                foreach (var employee in employees)
                {
                    body += $"<li>{employee.Emp.EmpName} - Contract End Date: {employee.EndCont}</li>";
                }
                body += "</ul>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(configuration["MailSettings:DisplayName"], configuration["MailSettings:From"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Contract Expiry Notification";
                message.Date = DateTimeOffset.Now;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                try
                {
                    await smtp.ConnectAsync(configuration["MailSettings:Host"], int.Parse(configuration["MailSettings:Port"]), SecureSocketOptions.None);
                    await smtp.AuthenticateAsync(configuration["MailSettings:From"], configuration["MailSettings:Password"]);
                    await smtp.SendAsync(message);
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
            }
        }



    }
}

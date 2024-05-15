using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using HtmlAgilityPack;
using FastLane.Service.Order;
using FastLane.Repository.Order;
using FastLane.Repository.Employee;
using FastLane.Entities;
using FastLane.Service.Service;
using FastLane.Service.User;

namespace FastLane.Service.Email
{
    public class EmailService : IEmailService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly string _pending_email_Path;
        private readonly string _group_pending_email_Path;
        private readonly string _confirm_email_Path;
        private readonly string _complete_email_Path;
        private readonly string _cancel_email_Path;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmployeeRepository _employeeRespository;
        private readonly IServiceService _serviceService;
        public EmailService(IConfiguration config, IOrderRepository orderService,  IUserService userService,
            IEmployeeRepository employeeRespository, IServiceService serviceService)
        {
            _config = config;
            _orderRepository = orderService;
            _userService = userService;
            _serviceService = serviceService;
            _employeeRespository= employeeRespository;
            _cancel_email_Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "cancelTemplate.html");
            _complete_email_Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "completeTemplate.html");
            _confirm_email_Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "confirmTemplate.html");
            _pending_email_Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "pendingTemplate.html");
            _group_pending_email_Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "group_pendingTemplate.html");
        }
        public async Task SendCancelEmail(Dtos.Email.Email request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;

                // Load HTML content from file
                var doc = new HtmlDocument();
                doc.Load(_cancel_email_Path);

                var order = await _orderRepository.GetOrderById(request.Order_Id);
                string serviceString = order.Service;
                int serviceId = int.Parse(serviceString);

                var service = await _serviceService.GetServiceByIdAsync(serviceId);

                // Replace placeholders with actual data
                ReplacePlaceholder(doc, "[Order_Code]", order.GroupReference);
                ReplacePlaceholder(doc, "[Order_Time]", order.Created_at.ToString());
                ReplacePlaceholder(doc, "[Customer_Name]", order.Name);
                ReplacePlaceholder(doc, "[Email]", order.Email);
                ReplacePlaceholder(doc, "[Phone_Number]", order.Phone);
                ReplacePlaceholder(doc, "[Note]", order.Note);

                // Set up body builder
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = doc.DocumentNode.OuterHtml; // Use modified HTML content as email body

                // Assign body builder to email
                email.Body = bodyBuilder.ToMessageBody();

                // Set up SMTP client
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                email.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending pending email: {ex.Message}");
            }
        }

        public async Task SendPendingEmail(Dtos.Email.Email request)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(request.Order_Id);
                if (order == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                string serviceString = order.Service;
                if (!int.TryParse(serviceString, out int serviceId))
                {
                    Console.WriteLine("Invalid service ID.");
                    return;
                }

                var service = await _serviceService.GetServiceByIdAsync(serviceId);
                if (service == null)
                {
                    Console.WriteLine("Service not found.");
                    return;
                }

                //Get all sale and operator related to airport in this booking
                var sales = await _userService.GetAllSales();
                var operators = await _userService.GetAllOperators(order.AirPort);

                //Send Mail
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                email.To.Add(MailboxAddress.Parse(request.To));
                foreach(var sale in sales.Users)
                {
                    email.To.Add(MailboxAddress.Parse(sale.Email));
                }

                foreach(var operatorItem in operators.Users)
                {
                    email.To.Add(MailboxAddress.Parse(operatorItem.Email));
                }

                email.Subject = request.Subject;

                // Load HTML content from file
                var doc = new HtmlDocument();
                doc.Load(_pending_email_Path);

                // Replace placeholders with actual data
            ReplacePlaceholder(doc, "[Order_Code]", order.GroupReference);
            ReplacePlaceholder(doc, "[Order_Time]", order.Created_at.ToString());
            ReplacePlaceholder(doc, "[Customer_Name]", order.Name);
            ReplacePlaceholder(doc, "[Passport_Number]", order.Passport_Number);
            ReplacePlaceholder(doc, "[Nationality]", order.Nationality);
            ReplacePlaceholder(doc, "[Service]", service.Name);
            ReplacePlaceholder(doc, "[Service_Time]", order.Service_Time.ToString());
            ReplacePlaceholder(doc, "[Flight_Number]", order.Flight_Number);
            ReplacePlaceholder(doc, "[Airport]", order.AirPort);
            ReplacePlaceholder(doc, "[Email]", order.Email);
            ReplacePlaceholder(doc, "[Phone_Number]", order.Phone);
            ReplacePlaceholder(doc, "[Note]", order.Note);  


            // Set up body builder
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = doc.DocumentNode.OuterHtml; 

            // Assign body builder to email
            email.Body = bodyBuilder.ToMessageBody();

            // Set up SMTP client
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            email.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending pending email: {ex.Message}");
            }
        }

        private void ReplacePlaceholder(HtmlDocument doc, string placeholder, string value)
        {
            var spanNodes = doc.DocumentNode.SelectNodes($"//span[contains(text(), '{placeholder}')]");
            if (spanNodes != null)
            {
                foreach (var spanNode in spanNodes)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        spanNode.InnerHtml = spanNode.InnerHtml.Replace(placeholder, value);
                    }
                    else
                    {
                        spanNode.InnerHtml = string.Empty; 
                    }
                }
            }
        }

        public async Task SendCompleteEmail(Dtos.Email.Email request)
        {
            try
            {
                var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            // Load HTML content from file
            var doc = new HtmlDocument();
            doc.Load(_complete_email_Path);

            var order = await _orderRepository.GetOrderById(request.Order_Id);
            var employee = await _employeeRespository.GetEmployeeByIdAsync(order.Employee);
            // Replace placeholders with actual data
            ReplacePlaceholder(doc, "[Customer_Name]", order.Name);

            // Set up body builder
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = doc.DocumentNode.OuterHtml; 

            // Assign body builder to email
            email.Body = bodyBuilder.ToMessageBody();

            // Set up SMTP client
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            email.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending pending email: {ex.Message}");
                // Handle the exception here...
            }
        }

        public async Task SendConfirmEmail(Dtos.Email.Email request)
        {
            try
            {
                var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            // Load HTML content from file
            var doc = new HtmlDocument();
            doc.Load(_confirm_email_Path);

            var order = await _orderRepository.GetOrderById(request.Order_Id);
            var employee = await _employeeRespository.GetEmployeeByIdAsync(order.Employee);
            var user = await _userService.GetUserByIdAsync(employee.User_Id);

            var serviceId = int.Parse(order.Service);
            var service = await _serviceService.GetServiceByIdAsync(serviceId);


            // Replace placeholders with actual data
            ReplacePlaceholder(doc, "[Customer_Name]", order.Name);
            ReplacePlaceholder(doc, "[Service]", service.Name);
            ReplacePlaceholder(doc, "[Passport_Number]", order.Passport_Number);
            ReplacePlaceholder(doc, "[Order_Time]", order.Service_Time.ToString());
            ReplacePlaceholder(doc, "[Flight_Number]", order.Flight_Number);
            ReplacePlaceholder(doc, "[Airport]", order.AirPort);
            ReplacePlaceholder(doc, "[Email]", order.Email);
            ReplacePlaceholder(doc, "[Phone_Number]", order.Phone);
            ReplacePlaceholder(doc, "[Employee_Name]", user.Name);
            ReplacePlaceholder(doc, "[Nationality]", order.Nationality);
            ReplacePlaceholder(doc, "[Order_Time]", order.Created_at.ToString());
            ReplacePlaceholder(doc, "[Order_Code]", order.GroupReference);
            ReplacePlaceholder(doc, "[Note]", order.Note);
            ReplacePlaceholder(doc, "[Person_In_Charge]", user.Name);
            ReplacePlaceholder(doc, "[Contact]", user.Email);

                // Set up body builder
                var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = doc.DocumentNode.OuterHtml; // Use modified HTML content as email body

            // Assign body builder to email
            email.Body = bodyBuilder.ToMessageBody();

            // Set up SMTP client
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            email.Dispose();
        }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending pending email: {ex.Message}");
    }
}
    }
}

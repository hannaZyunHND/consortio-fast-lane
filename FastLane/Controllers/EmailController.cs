using FastLane.Dtos.Email;
using FastLane.Service.Email;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IO;

namespace FastLane.Controllers
{
    [Route("/api/mail")]
    //[ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;


        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendEmail(Email request)
        {
            _emailService.SendPendingEmail(request);
            return Ok("Email sent successfully");
        }
        public IActionResult PreviewEmailTemplate()
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template", "Email", "mailTemplate.html");
            string emailContent = System.IO.File.ReadAllText(templatePath);
            return Content(emailContent, "text/html"); // Xác định kiểu nội dung là HTML
        }
    }
}

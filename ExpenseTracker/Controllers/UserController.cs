using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ExpenseTracker.Database;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;

namespace ExpenseTracker.Controllers
{
    public class UserController : Controller
    {
        SqlContext sc;
        private readonly IConfiguration _configuration;

        public UserController(SqlContext sc1, IConfiguration configuration)
        {
            this.sc = sc1;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Users us)
        {
            if (ModelState.IsValid)
            {

                var emailExist = sc.Users.FirstOrDefault(u => u.User_Email == us.User_Email);
                if (emailExist != null)
                {
                    return Json(new { success = false, message = "Email Already Exists" });
                }

                string otp = new Random().Next(100000, 999999).ToString();

                string hashedPassword = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(us.User_Password)));

                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("User_Name", us.User_Name);
                HttpContext.Session.SetString("User_Email", us.User_Email);
                HttpContext.Session.SetString("User_Password",hashedPassword);
                HttpContext.Session.SetString("OTP_CreatedTime", DateTime.UtcNow.ToString());

                await SendEmailAsync(us.User_Email, "Expense Tracker OTP", $"Your OTP is {otp}");

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }   

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            string fromEmail = _configuration["EmailAddress"];
            string emailPassword = _configuration["Password"];

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, emailPassword)

            };
                
            var message = new MailMessage(fromEmail, toEmail, subject, body);
            await smtpClient.SendMailAsync(message);
        }


        public IActionResult OTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OTP(string otp)
        {
            string storedOTP = HttpContext.Session.GetString("OTP");
            string otpCreatedTimeStr = HttpContext.Session.GetString("OTP_CreatedTime");

            if (DateTime.TryParse(otpCreatedTimeStr, out DateTime otpCreatedTime))
            {
                if((DateTime.UtcNow - otpCreatedTime).TotalMinutes > 1)
                {
                    return Json(new { success = false, message = "OTP Expired" });
                }
            }

            if (otp == storedOTP)
            {
                Users user = new Users
                {
                    User_Name = HttpContext.Session.GetString("User_Name"),
                    User_Email = HttpContext.Session.GetString("User_Email"),
                    User_Password = HttpContext.Session.GetString("User_Password"),
                    OTP = HttpContext.Session.GetString("OTP")
                };

                sc.Users.Add(user);
                sc.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid OTP" });
        }

        [HttpPost]
        public async Task<IActionResult> ResendOTP()
        {
            var email = HttpContext.Session.GetString("User_Email");

            var resendotp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", resendotp);

            HttpContext.Session.SetString("OTP_CreatedTime", DateTime.UtcNow.ToString());

            var subject = "Your New OTP";
            var body = $"Your OTP is: {resendotp}";

            await SendEmailAsync(email, subject, body);

            return Json(new { success = true });

        }


        public IActionResult Login()
        {
            return View();
        }
    }
}
    
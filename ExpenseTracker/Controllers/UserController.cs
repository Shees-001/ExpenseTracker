using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ExpenseTracker.Database;

namespace ExpenseTracker.Controllers
{
    public class UserController : Controller
    {
        SqlContext sc;
        public UserController(SqlContext sc1)
        {
            this.sc = sc1;
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
        public async Task<IActionResult> Register(Users us)
        {
            if (ModelState.IsValid)
            {
                string otp = new Random().Next(100000, 999999).ToString();

                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("User_Name", us.User_Name);
                HttpContext.Session.SetString("User_Email", us.User_Email);
                HttpContext.Session.SetString("User_Password", us.User_Password);

                await SendEmailAsync(us.User_Email, "Your OTP Code is :", $"Your OTP is {otp}");

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("sheessillat@gmail.com", "oxlf dgko qcho qdnz")

            };
                
            var message = new MailMessage("sheessillat@gmail.com", toEmail, subject, body);
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

            if (otp == storedOTP)
            {
                Users user = new Users
                {
                    User_Name = HttpContext.Session.GetString("User_Name"),
                    User_Email = HttpContext.Session.GetString("User_Email"),
                    User_Password = HttpContext.Session.GetString("User_Password")
                };

                sc.Users.Add(user);
                sc.SaveChanges();
                return RedirectToAction("Login");
            }

            // If OTP does not match
            ViewBag.ErrorMessage = "Invalid OTP. Please try again.";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
    
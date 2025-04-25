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

                TempData["OTP"] = otp;
                TempData["UserName"] = us.User_Name;
                TempData["UserEmail"] = us.User_Email;
                TempData["UserPassword"] = us.User_Password;

                await SendEmailAsync(us.User_Email, "Your OTP Code is :", $"Your OTP is {otp}");
                return RedirectToAction("OTP"); 
            }
            return View();
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

        public IActionResult OTP(string otp)
        {
            string storedOTP = TempData["OTP"] as string;

            if (otp == storedOTP) 
            {
                Users user = new Users
                {
                    User_Name = TempData["UserName"] as string,
                    User_Email = TempData["UserEmail"] as string,
                    User_Password = TempData["UserPassword"] as string,
                };

                sc.Users.Add(user);
                sc.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }
    }
}
    
using a_RegistrationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Registration.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mail;

namespace a_RegistrationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersDataContext context;
        public HomeController(UsersDataContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public ActionResult Register(Users user, string cpsw,string job)
        {

            if (userControl(user, cpsw, job))

            {
                user.created_at = DateTime.Now;

                var UserList = context.Users.OrderByDescending(x => x.created_at).ToList();
                double result;
                if(UserList.Count == 0)
                {
                    result = 91; 
                }
                else
                {
                    var newestUser = UserList.First();
                    result = (user.created_at - newestUser.created_at).TotalSeconds;
                }
                if (result > 90)
                {
                    context.Add(user);
                    context.SaveChanges();
                    return View("Index");
                }
                else
                {
                    ViewData["Message"] = "Need to wait 90 sec before new registration.";
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Error()
        {

            return View();
        }
        public bool IsValid(string emailaddress)
        {
            if (!string.IsNullOrEmpty(emailaddress) && new EmailAddressAttribute().IsValid(emailaddress))
                return true;
            else
                return false;
        }
        static bool ValidatePassword(string password)
        {
            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 15;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        ;
            return isValid;

        }

        private bool isExist(String email)
        {
            var exists = context.Users.Any(x => x.Email == email);
            if (exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool userControl(Users user, string cpsw, string job)
        {
            bool resultFlag;

            bool emailValid = IsValid(user.Email);
            bool emailExist = isExist(user.Email);

            if (user.Email != null && user.Password != null && user.Name != null && user.Surname != null)
            {

                if (emailExist)
                {
                    ViewData["Message"] = "Email already exist.";
                    resultFlag = false;
                }
                else
                {
                    if (emailValid)
                    {

                        if (ValidatePassword(user.Password))
                        {
                            if (user.Password == cpsw)
                            {
                                if (user.Email.Contains("@edu") && (job.Equals("Teacher")) || job.Equals("Student"))
                                {
                                    resultFlag = true;
                                }
                                else if(!user.Email.Contains("@edu") && (job.Equals("NA")))
                                {
                                    resultFlag = true;
                                }
                                else
                                {
                                    ViewData["Message"] = "If your email contain @edu please select Teacher of STUDENT";
                                    resultFlag = false;
                                }
                            }
                            else
                            {
                                ViewData["Message"] = "Both passwords must be same.";
                                resultFlag = false;
                            }


                        }
                        else
                        {
                            //MessageBox.Show("Please enter a password which contains at least 1 uppercase letter, 1 lowercase letter and 1 number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ViewData["Message"] = "Please enter a password which contains at least 1 uppercase letter, 1 lowercase letter and 1 number.";
                            resultFlag = false;

                        }
                    }
                    else
                    {
                        ViewData["Message"] = "Please enter valid email adress.";

                        resultFlag = false;

                    }

                }
            }
            else
            {
                //MessageBox.Show("Please enter value in all field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ViewData["Message"] = "Please enter value in all field.";
                resultFlag = false;
            }

            return resultFlag;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using NotesMarketPlace.Models;
using System.Net;
using System.Threading.Tasks;

namespace NotesMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        
        private NotesMarketEntities2 usersEntities = new NotesMarketEntities2();
        public JsonResult IsEmailIdAvailable(string EmailID)
        {
            return Json(!usersEntities.Users.Any(user => user.EmailID == EmailID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]public ActionResult Signup()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Signup( User userobj)
        {
            

            if (usersEntities.Users.Where(model => model.EmailID == userobj.EmailID).Any())
            {
                ModelState.AddModelError("EmailID", "Email Already exists");

                return View();
            }
            else
            {
                userobj.ActivationCode = Guid.NewGuid();

                userobj.RoleID = 1;
                userobj.IsActive = true;
                usersEntities.Users.Add(userobj);

                usersEntities.SaveChanges();
                SendVerificationLinkEmail(userobj.EmailID, userobj.ActivationCode.ToString());
                //ViewBag.result("Record inserted Succesfully");
                TempData["Referrer"] = "SaveRegister";
                return View(userobj);
               

            }
            
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/Home/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "*****"; // Replace with actual password
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (NotesMarketEntities2 dc = new NotesMarketEntities2())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    
                 
                    try
                    {
                        dc.SaveChanges();

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting  
                                // the current instance as InnerException  
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User loginobj)
        {


            NotesMarketEntities2 db = new NotesMarketEntities2();
                var obj1 = db.Users.Where(model => model.EmailID.Equals(loginobj.EmailID)&& model.IsActive.Equals(true)).FirstOrDefault();

                if (obj1 == null)
                {
                    ModelState.AddModelError("EmailID", "This email id does not exist");
                }
                else
                {
                    if (obj1.Password == loginobj.Password)
                    {
                      
                        if (obj1.IsEmailVerified == true)
                        {
                            if (obj1.RoleID == 1)
                            {
                            var obj2 = db.UserProfiles.Where(model => model.UserID.Equals(obj1.ID)).FirstOrDefault();
                                if (obj2 == null)
                                {
                                    Session["UserID"] = obj1.ID;
                               
                                return RedirectToAction("MyProfile", "User");

                                }
                                else
                                {
                                    Session["UserID"] = obj1.ID;
                                    return RedirectToAction("Dashboard","User");

                                }
                            }
                            else if (obj1.RoleID == 2)
                            {
                                Session["UserID"] = obj1.ID;
                                return RedirectToAction("FAQ");
                                //ModelState.AddModelError(" ", "Admin");
                            }
                            else
                            {
                                Session["UserID"] = obj1.ID;
                                return RedirectToAction("Fjhk");

                            }
                        }
                        else
                        {
                            ModelState.AddModelError(" ", "Please verify your email id");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Password");
                    }

                }

            
            return View(loginobj);


           
        }


        public ActionResult FAQ()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SearchNotes()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SellYourNotes()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(User obj)
        {
            var exists = usersEntities.Users.Where(model => model.EmailID.Equals(obj.EmailID)).FirstOrDefault();
            if(exists!=null)
            {

                string pwd=GeneratePassword();
                exists.Password = pwd;
                usersEntities.Configuration.ValidateOnSaveEnabled = false;
                usersEntities.SaveChanges();
                SendPasswordEmail(obj.EmailID, pwd);
                
            }
            else
            {
                ModelState.AddModelError("", "EmailID does not exist");
            }
            return View(obj);

        }
        public string GeneratePassword()
        {
            string PasswordLength = "8";
            string NewPassword = "";

            string allowedChars = "";
            allowedChars = "1,2,3,4,5,6,7,8,9,0";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "!,@,#,*,";


            char[] sep = {
            ','
        };
            string[] arr = allowedChars.Split(sep);


            string IDString = "";
            string temp = "";

            Random rand = new Random();

            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                IDString += temp;
                NewPassword = IDString;

            }
            return NewPassword;
        }

        public void SendPasswordEmail(string emailID, string password)
        {
           
            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = ""; // Replace with actual password
            string subject = "Your new Password is";

            string body = password;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}

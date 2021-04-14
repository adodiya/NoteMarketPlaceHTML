using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using NotesMarketPlace.Models;
using System.Net;
using System.Threading.Tasks;
using PagedList;

namespace NotesMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        
        NotesMarketEntities2 db = new NotesMarketEntities2();
       
        //Returns Home Page
        public ActionResult Index()
        {
            return View();
        }


       
        public ActionResult Signup()
        {
          
            return View();
        }

       
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Signup( User userobj)
        {
            

            if (db.Users.Where(model => model.EmailID == userobj.EmailID).Any())
            {
                ModelState.AddModelError("EmailID", "Email Already exists");

                return View();
            }
            else
            {
                if(ModelState.IsValid)
                { 
                    userobj.ActivationCode = Guid.NewGuid();
                    userobj.RoleID = 1;
                    userobj.IsActive = true;
                    userobj.CreatedDate = DateTime.Now;
                    
                    db.Users.Add(userobj);

                db.SaveChanges();
                SendVerificationLinkEmail(userobj.EmailID, userobj.ActivationCode.ToString());
              
                TempData["Referrer"] = "SaveRegister";
                return View(userobj);
                }

                return View();
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
                                    return RedirectToAction("SearchNotes","User");

                                }
                            }
                            else
                            {
                                Session["UserID"] = obj1.ID;
                                Session["Role"] = obj1.RoleID;
                                return RedirectToAction("Dashboard","Admin");
                                
                            }
                           
                        }
                        else
                        {
                        ViewBag.Message = "Plese verify your email id . Verification link has already been sent to your mail id";
                            return View();
                        }

                    }
                    else
                    {
                    ViewBag.Message = "Password is incorrect";
                    }

                }

            
            return View(loginobj);


           
        }


        public ActionResult FAQ()
        {
           return View();
        }

        public ActionResult SearchNotes(int? page, string searchstring , string searchCategory, string searchType, string searchUniversity, string searchCourse, string searchCountry, string searchRating)
        {
            ViewBag.searchCategory = new SelectList(db.Categories.ToList(), "Name", "Name");
            ViewBag.searchType = new SelectList(db.Types.ToList(), "Name", "Name");
            ViewBag.searchCountry = new SelectList(db.Countries.ToList(), "Name", "Name");
            ViewBag.searchCourse = new SelectList(db.Notes.ToList(), "Course", "Course");
            ViewBag.searchUniversity = new SelectList(db.Notes.ToList(), "UniversityName", "UniversityName");


            ViewBag.Count = db.NotesReviews;
            ViewBag.NoteReview = db.NotesReviews.ToList();
            var notes = db.Notes.Where(model => model.Status == 4).OrderBy(model => model.PublishedDate);
            if (!string.IsNullOrEmpty(searchstring))
            {
                notes = notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring) || model.Price.Equals(searchstring)).OrderBy(model => model.PublishedDate);
            }
           
            if (!string.IsNullOrEmpty(searchCategory))
            {
                notes = notes.Where(model => model.Category.Name.Contains(searchCategory)).OrderBy(model => model.PublishedDate);
            }
           
            if (!string.IsNullOrEmpty(searchType))
            {
                notes = notes.Where(model => model.Type.Name == searchType).OrderBy(model => model.PublishedDate);
            }
            if (!string.IsNullOrEmpty(searchCountry))
            {
                notes = notes.Where(model => model.Country.Name == searchCountry).OrderBy(model => model.PublishedDate);
            }
         if (!string.IsNullOrEmpty(searchCourse))
            {
                notes = notes.Where(model => model.Course.Contains(searchCourse)).OrderBy(model => model.PublishedDate);
            }
            if (!string.IsNullOrEmpty(searchUniversity))
            {
                notes = notes.Where(model => model.UniversityName.Contains(searchUniversity)).OrderBy(model => model.PublishedDate);
            }
            if (!string.IsNullOrEmpty(searchRating))
            {
               
            }
            return View(notes.ToPagedList(page??1,6));
        }

        public ActionResult NoteDetails(int id)
        {
            var obj = db.Notes.Find(id);
            ViewBag.Reviews = db.NotesReviews.Where(model => model.NoteID == id).ToList();
            int count = db.NotesReviews.Where(model => model.NoteID == id).Count();
            int issue= db.ReportedIssues.Where(model => model.NoteID == id).Count();
           
            if(issue==0)
            {
                ViewBag.Issues = 0;
            }
            else
            {
                ViewBag.Issues = issue;
            }
            if(count==0)
            { ViewBag.Count = 0; }
            else
            {
                int count1 = (int)Math.Round(db.NotesReviews.Where(model => model.NoteID == id).Select(model => model.Ratings).Average());
                ViewBag.Count = count1; }
          
            return View(obj);
        }


        public ActionResult SellYourNotes()
        {
           
            return RedirectToAction("Login","Home");
        }

        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]

        public async Task<ActionResult> email(FormCollection form)
        {
            var fullname = form["fname"];
            var email = form["email"];
            var subject = form["subject"];
            var comments = form["comments"];
            var x = await SendEmail(fullname, email, subject, comments);
            if (x == "sent")
                ViewData["esent"] = "Your Message Has Been Sent";
            return RedirectToAction("ContactUs");
        }
        private async Task<string> SendEmail(string fullname, string email, string subject, string comments)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress("ahdodiya99@gmail.com")); // replace with receiver's email id     
            message.From = new MailAddress("ahdodiya99@gmail.com"); // replace with sender's email id     
            message.Subject = email + " - " + subject;
            message.Body = "Hello,\n\n " + comments + "\n\nRegards:,\n " + fullname;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "ahdodiya99@gmail.com", // replace with sender's email id     
                    Password = "" // replace with password     
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                return "sent";
            }
        }


        [HttpGet]
        public ActionResult ForgetPassword()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(User obj)
        {
            var exists = db.Users.Where(model => model.EmailID.Equals(obj.EmailID)).FirstOrDefault();
            if(exists!=null)
            {

                string pwd=GeneratePassword();
                exists.Password = pwd;
                db.Configuration.ValidateOnSaveEnabled = false;
               db.SaveChanges();
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
           
            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Aadi");
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
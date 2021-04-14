using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlace.Models;
using Type = NotesMarketPlace.Models.Type;
using PagedList;
using System.Net;
using System.IO.Compression;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Security;
using Ionic.Zip;

namespace NotesMarketPlace.Controllers
{
    public class AdminController : Controller
    {
        NotesMarketEntities2 db = new NotesMarketEntities2();

      
        public ActionResult Dashboard(string searchstring, string dropdownsearch, string sortby, int? page)
        {
            if (Session["UserID"] != null) {


                var lowerlimit = DateTime.Now.AddMonths(-6);
                var upperlimit = DateTime.Now.AddMonths(6);
                var notes = db.Notes.Where(model => model.Status == 4);

                ViewBag.Submitted = db.Notes.Where(model => model.Status.Equals(5)).Count();
                ViewBag.Downloaded = db.Downloads.Where(model => model.IsAttachmentDownloaded.Equals(true)).Select(model => model.NoteID).Distinct().Count();
                ViewBag.Members = db.Users.Where(model => model.RoleID.Equals(1)).Count();
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
                ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
                ViewBag.SortByPrice = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
                ViewBag.SortByAdmin = string.IsNullOrEmpty(sortby) ? "Admin desc" : "";
                ViewBag.SortBySellType = string.IsNullOrEmpty(sortby) ? "SellType desc" : "";
                if (!string.IsNullOrEmpty(searchstring))
                {
                    notes = notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring));
                }
                if (!string.IsNullOrEmpty(dropdownsearch))
                {
                    DateTime dropdown1 = Convert.ToDateTime(dropdownsearch);
                    notes = notes.Where(model =>model.PublishedDate.Value.Month == dropdown1.Month);
                }
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                    case "Status desc":
                        notes = notes.OrderBy(model => model.ReferenceData.Value);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    case "Admin desc":
                        notes = notes.OrderBy(model => model.User1.FirstName).ThenBy(model => model.User1.LastName);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "SellType desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;

                    default:
                        notes = notes.OrderByDescending(model => model.PublishedDate);
                        break;
                }
                return View(notes.ToPagedList(page ?? 1, 5));
           }
           else
            {
                return RedirectToAction("Login", "Home");
            }
        }

       
        public ActionResult NotesUnderReview(string searchstring, string dropdownsearch, string sortby, int? page)
        {

            if (Session["UserID"] != null) {


                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
                ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
                ViewBag.dropdownsearch = new SelectList(db.Users.Where(model => model.RoleID == 1).ToList(), "FirstName", "FirstName");
                var notes = db.Notes.AsQueryable();
                notes = db.Notes.Where(model => model.Status == 5 || model.Status == 8);

                if (!string.IsNullOrEmpty(searchstring))
                {

                    notes = db.Notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring));

                }
                if (!string.IsNullOrEmpty(dropdownsearch))
                {
                    notes = db.Notes.Where(model => model.User.FirstName.Contains(dropdownsearch)).Where(model => model.Status == 5);
                }
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                    case "Status desc":
                        notes = notes.OrderBy(model => model.ReferenceData.Value);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    default:
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                }
                return View(notes.ToPagedList(page ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Approved(int id)
        {
            if (Session["UserID"] != null) {
                var notes = db.Notes.Single(model => model.ID == id);
                notes.Status = 4;
                notes.PublishedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("NotesUnderReview");
            }
               
              else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Rejected(int id,FormCollection form)
        {
            if (Session["UserID"] != null)
            {
                string remarks = form["Remarks"];
                var notes = db.Notes.Single(model => model.ID == id);
                notes.Status = 6;
                notes.AdminRemarks = remarks;
                notes.ActionedBy = 9;
                db.SaveChanges();

                return RedirectToAction("NotesUnderReview");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult InReview(int id)
        {
            if (Session["UserID"] != null)
            {
                var notes = db.Notes.Single(model => model.ID == id);
                notes.Status = 8;
                notes.PublishedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("NotesUnderReview");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult PublishedNotes(string searchstring, string dropdownsearch, string sortby, int? page)
        {


            if (Session["UserID"] != null)
            {

                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
                ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
                ViewBag.SortByPrice = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
                ViewBag.SortByAdmin = string.IsNullOrEmpty(sortby) ? "Admin desc" : "";
                ViewBag.SortBySellType = string.IsNullOrEmpty(sortby) ? "SellType desc" : "";

                var notes = db.Notes.AsQueryable();
                notes = db.Notes.Where(model => model.Status == 4);

                if (!string.IsNullOrEmpty(searchstring))
                {
                    notes = db.Notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring) || model.Price.Equals(searchstring));
                }
                if (!string.IsNullOrEmpty(dropdownsearch))
                {
                    notes = db.Notes.Where(model => model.User.FirstName == searchstring);
                }
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                    case "Status desc":
                        notes = notes.OrderBy(model => model.ReferenceData.Value);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    case "Admin desc":
                        notes = notes.OrderBy(model => model.User1.FirstName).ThenBy(model => model.User1.LastName);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "SellType desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;

                    default:
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                }
                return View(notes.ToPagedList(page ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Unpublish(int id, FormCollection form)
        {
            if (Session["UserID"] != null)
            {
                var notes = db.Notes.Single(model => model.ID == id);
                string remarks = form["remarks"];
                var admin = db.Users.Find(9);
                notes.Status = 9;
                notes.IsActive = false;
                notes.AdminRemarks = remarks;
                notes.ActionedBy = admin.ID;
                db.SaveChanges();

                SendEmail(remarks, notes);

                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public void SendEmail(string remarks, Note notes)
        {
           


            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Aadi");
            var toEmail = new MailAddress(notes.User.EmailID);
            var fromEmailPassword = ""; // Replace with actual password
            string subject = "Sorry! We need to remove your notes from our portal.";

            string body = "Hello " + notes.User.FirstName + " " + notes.User.LastName + ",\n\n" + "We want to inform you that, your note" + " " + notes.Title + " has been removed from the portal.\n\n" + "Please find our remarks as below -\n\n" + remarks + "\n\nRegards," + "\n Notes Marketplace";

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

        public ActionResult DownloadedNotes(string searchstring, string dropdownsearchNote, string dropdownsearchSeller, string dropdownsearchBuyer, string sortby, int? page)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
            ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
            ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
            ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
            ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
            ViewBag.SortByPrice = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
            ViewBag.SortByBuyer = string.IsNullOrEmpty(sortby) ? "Buyer desc" : "";


            var downloads = db.Downloads.AsQueryable();
            downloads = db.Downloads.Where(model => model.IsAttachmentDownloaded == true);

            if (!string.IsNullOrEmpty(searchstring))
            {
                downloads = db.Downloads.Where(model => model.NoteTitle.Contains(searchstring) || model.NoteCategory.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.User1.FirstName.Contains(searchstring) || model.User1.LastName.Contains(searchstring) || model.Price.Equals(searchstring));
            }
            if (!string.IsNullOrEmpty(dropdownsearchNote))
            {
                downloads = db.Downloads.Where(model => model.NoteTitle == dropdownsearchNote);
            }
            if (!string.IsNullOrEmpty(dropdownsearchSeller))
            {
                downloads = db.Downloads.Where(model => model.User.FirstName.Contains(dropdownsearchSeller) || model.User.LastName.Contains(dropdownsearchSeller));
            }
            if (!string.IsNullOrEmpty(dropdownsearchBuyer))
            {
                downloads = db.Downloads.Where(model => model.User1.FirstName.Contains(dropdownsearchBuyer) || model.User1.LastName.Contains(dropdownsearchBuyer));
            }

            switch (sortby)
            {
                case "Title desc":
                    downloads = downloads.OrderBy(model => model.NoteTitle);
                    break;
                case "Category desc":
                    downloads = downloads.OrderBy(model => model.NoteCategory);
                    break;

                case "Date desc":
                    downloads = downloads.OrderBy(model => model.DownloadDate);
                    break;
                case "Seller desc":
                    downloads = downloads.OrderBy(model => model.User.FirstName).ThenBy(model => model.User.LastName);
                    break;
                case "Buyer desc":
                    downloads = downloads.OrderBy(model => model.User1.FirstName).ThenBy(model => model.User1.LastName);
                    break;
                case "Price desc":
                    downloads = downloads.OrderBy(model => model.Price);
                    break;


                default:
                    downloads = downloads.OrderBy(model => model.DownloadDate);
                    break;
            }
            return View(downloads.ToPagedList(page ?? 1, 5));
        }

            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult RejectedNotes(string searchstring, string dropdownsearch, string sortby, int? page)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
                ViewBag.SortByRejectedBy = string.IsNullOrEmpty(sortby) ? "RejectedBy desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";

                ViewBag.dropdownsearch = new SelectList(db.Users.Where(model => model.RoleID == 1).ToList(), "FirstName", "FirstName");
//var notes = db.Notes.AsQueryable();
                var notes = db.Notes.Where(model => model.Status == 6);

                if (!string.IsNullOrEmpty(searchstring))
                {
                    notes = db.Notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring) || model.Price.Equals(searchstring));
                }
                if (!string.IsNullOrEmpty(dropdownsearch))
                {
                    notes = db.Notes.Where(model => (model.User.FirstName.ToLower() + " " + model.User.LastName.ToLower()).Contains(dropdownsearch));


                }
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                    case "Status desc":
                        notes = notes.OrderBy(model => model.ReferenceData.Value);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    case "Admin desc":
                        notes = notes.OrderBy(model => model.User1.FirstName).ThenBy(model => model.User1.LastName);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "SellType desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;

                    default:
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                }
                return View(notes.ToPagedList(page ?? 1, 5));
            }

            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Members(string searchstring, string sortby, int? page)
        {
            if (Session["UserID"] != null)
            {
                var downloads = db.Users.AsQueryable();
            downloads = db.Users.Where(model => model.RoleID.Equals(1));
            switch (sortby)
            {
                case "Title desc":
                    downloads = downloads.OrderBy(model => model.FirstName);
                    break;
                case "Category desc":
                    downloads = downloads.OrderBy(model => model.LastName);
                    break;

                case "Date desc":
                    downloads = downloads.OrderBy(model => model.EmailID);
                    break;



                default:
                    downloads = downloads.OrderBy(model => model.CreatedDate);
                    break;
            }

            return View(downloads.ToPagedList(page ?? 1, 5)); }

            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult MemberDetails(string searchstring, string dropdownsearch, string sortby, int? page)
        {
            if (Session["UserID"] != null)
            {

                UserProfile obj1 = db.UserProfiles.Where(model => model.UserID == 10).FirstOrDefault();
                User obj = db.Users.Find(10);


                ViewBag.ProfilePic = obj1.ProfilePic;
                ViewBag.FirstName = obj.FirstName;
                ViewBag.LastName = obj.LastName;
                ViewBag.Email = obj.EmailID;
                ViewBag.DOB = obj1.DOB;
                ViewBag.Phone = obj1.Phoneno;
                ViewBag.College = obj1.University;
                ViewBag.Address1 = obj1.AddressL1;
                ViewBag.Address2 = obj1.AddressL2;
                ViewBag.City = obj1.City;
                ViewBag.State = obj1.State;
                ViewBag.Country = obj1.Country;
                ViewBag.Zipcode = obj1.ZipCode;
                var notes = db.Notes.Where(model => model.SellerID == 10 && model.Status != 5);
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
                ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
                ViewBag.SortByPrice = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
                ViewBag.SortByAdmin = string.IsNullOrEmpty(sortby) ? "Admin desc" : "";
                ViewBag.SortBySellType = string.IsNullOrEmpty(sortby) ? "SellType desc" : "";
                if (!string.IsNullOrEmpty(searchstring))
                {
                    notes = db.Notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring) || model.Price.Equals(searchstring));
                }
                if (!string.IsNullOrEmpty(dropdownsearch))
                {
                    notes = db.Notes.Where(model => model.User.FirstName == searchstring);
                }
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                    case "Status desc":
                        notes = notes.OrderBy(model => model.ReferenceData.Value);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    case "Admin desc":
                        notes = notes.OrderBy(model => model.User1.FirstName).ThenBy(model => model.User1.LastName);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "SellType desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;

                    default:
                        notes = notes.OrderBy(model => model.PublishedDate);
                        break;
                }

                return View(notes.ToPagedList(page ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult Deactivate(int id)
        {
            if (Session["UserID"] != null)
            {
                var user = db.Users.Find(id);
                user.IsActive = false;
                db.SaveChanges();
                var notes = db.Notes.Where(model => model.SellerID == id);

                foreach (var note in notes)
                {
                    note.IsActive = false;
                    note.Status = 10;
                    db.SaveChanges();
                }
                return RedirectToAction("Members", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        
        [HttpGet]
        public ActionResult UpdateProfile()
        {
            if (Session["UserID"] != null)
            {

                int id = (int)Session["UserID"];

                var userobj = db.Users.Where(model => model.ID.Equals(id)).FirstOrDefault();
                var userprofileobj = db.UserProfiles.Where(model => model.UserID.Equals(id)).FirstOrDefault();
                User_UserProfile obj = new User_UserProfile();

                if (userprofileobj == null)
                {
                    obj.User = userobj;
                    obj.UserProfile = new UserProfile();
                    obj.UserProfile.UserID = id;
                  
                    obj.UserProfile.CountryCollection = db.Countries.ToList<Country>();
                    return View(obj);
                }
                else
                {
                   
                    userprofileobj.CountryCollection = db.Countries.ToList<Country>();
                    obj.User = userobj;
                    obj.UserProfile = userprofileobj;
                    return View(obj);
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }


        }

        [HttpPost]
        public ActionResult UpdateProfile(User_UserProfile obj, HttpPostedFileBase DisplayImageData)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    var allowedImageExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                    User obj1 = obj.User;
                    UserProfile obj2 = obj.UserProfile;
                    var update1 = db.Users.Find(obj1.ID);
                    db.Entry(update1).CurrentValues.SetValues(obj1);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    var userprofileobj = db.UserProfiles.Where(model => model.UserID.Equals(obj1.ID)).FirstOrDefault();

                    if (userprofileobj == null)
                    {

                        if (DisplayImageData.ContentLength > 0)
                        {

                            string imagefileName = Path.GetFileNameWithoutExtension(DisplayImageData.FileName);
                            string imageextension = Path.GetExtension(DisplayImageData.FileName);
                            if (allowedImageExtensions.Contains(imageextension))
                            {
                                imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                                obj2.ProfilePic = "~/ProfileImage/" + imagefileName;
                                imagefileName = Path.Combine(Server.MapPath("~/ProfileImage/"), imagefileName);
                                DisplayImageData.SaveAs(imagefileName);



                            }
                            else
                            {
                                ModelState.AddModelError("ProfilePic", "Add an image");
                            }
                        }
                        db.UserProfiles.Add(obj2);
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "User");
                    }
                    else
                    {

                        if (DisplayImageData != null && DisplayImageData.ContentLength > 0)
                        {
                            userprofileobj = obj2;
                            string imagefileName = Path.GetFileNameWithoutExtension(DisplayImageData.FileName);
                            string imageextension = Path.GetExtension(DisplayImageData.FileName);
                            if (allowedImageExtensions.Contains(imageextension))
                            {

                                imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                                userprofileobj.ProfilePic = "~/ProfileImage/" + imagefileName;
                                imagefileName = Path.Combine(Server.MapPath("~/ProfileImage/"), imagefileName);
                                DisplayImageData.SaveAs(imagefileName);
                            }
                            else
                            {
                                ModelState.AddModelError("ProfilePic", "only png/jpg formet allowed");
                            }

                        }
                        else
                        {
                            string count = Request.Form["IsRemoved"];
                            if (count == "Removed")
                            {
                                obj2.ProfilePic = null;
                                userprofileobj = obj2;


                            }
                            else
                            {
                                obj2.ProfilePic = userprofileobj.ProfilePic;
                                userprofileobj = obj2;

                            }
                        }

                        obj2.ModifiedBy = (int)Session["UserID"];
                        obj2.ModifiedDate = DateTime.Now;
                        var update = db.UserProfiles.Find(obj2.ID);
                        db.Entry(update).CurrentValues.SetValues(userprofileobj);


                        db.SaveChanges();
                        return RedirectToAction("SearchNotes");

                    }

                }

                else
                {
                    return View();
                }


            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult NoteDetails(int id)
        {
            if (Session["UserID"] != null)
            {
                int userid = (int)Session["UserID"];
                var obj = db.Notes.Find(id);
                ViewBag.Reviews = db.NotesReviews.Where(model => model.NoteID == id).ToList();
                int count = db.NotesReviews.Where(model => model.NoteID == id).Count();
                int issue = db.ReportedIssues.Where(model => model.NoteID == id).Count();

                if (issue == 0)
                {
                    ViewBag.Issues = 0;
                }
                else
                {
                    ViewBag.Issues = issue;
                }
                if (count == 0)
                { ViewBag.Count = 0; }
                else
                {
                    int count1 = (int)Math.Round(db.NotesReviews.Where(model => model.NoteID == id).Select(model => model.Ratings).Average());
                    ViewBag.Count = count1;
                }
                ViewBag.Isdownloaded = db.Downloads.Where(model => model.NoteID == id && model.IsAttachmentDownloaded == false && model.DownloadAllowed == true && model.Downloader == userid).Count();


                return View(obj);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ChangePassword()
        {
            if (Session["UserID"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            if (Session["UserID"] != null)
            {
                User userobj = db.Users.Find(9);
                string oldpassword = form["oldpwd"];
                string newpassword = form["newpwd"];
                string confirmpassword = form["confirmpwd"];
                if (oldpassword == userobj.Password)
                {
                    if (newpassword == confirmpassword)
                    {
                        userobj.Password = newpassword;
                        db.Users.Attach(userobj);
                        db.Entry(userobj).Property(x => x.Password).IsModified = true;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {

                        ViewBag.Message = "Password doesn't match";
                        return View();
                    }
                }

                else
                {

                    ViewBag.Message = "Please enter corect old password";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }


        public ActionResult ManageAdmin(int?page,string searchstring,string sortby)
        {
            if (Session["UserID"]!=null)
            {
                ViewBag.SortByFirstName= string.IsNullOrEmpty(sortby) ? "Firstname" : "";
                ViewBag.SortByLastName = string.IsNullOrEmpty(sortby) ? "Lastname" : "";
                ViewBag.SortByEmailID = string.IsNullOrEmpty(sortby) ? "EmailID" : "";
               
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date" : "";
                var admin = db.Users.Where(model => model.RoleID.Equals(2));
                if(!string.IsNullOrEmpty(searchstring))
                {
                    admin = admin.Where(model => model.FirstName.Contains(searchstring) || model.LastName.Contains(searchstring)||model.EmailID.Contains(searchstring));
                }
                
                switch (sortby)
                {
                    case "Firstname":
                        admin = admin.OrderBy(model => model.FirstName);
                        break;
                    case "Lastname":
                        admin = admin.OrderBy(model => model.LastName);
                        break;
                    case "EmailID":
                        admin = admin.OrderBy(model => model.EmailID);
                        break;
                    case "Date":
                        admin = admin.OrderBy(model => model.CreatedDate);
                        break;
                 
                    default:
                        admin = admin.OrderBy(model => model.CreatedDate);
                        break;
                }
                return View(admin.ToPagedList(page??1,5));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult AddAdmin(int? id)
        {
            if (Session["UserID"] != null && (int)Session["RoleID"] == 3)
            {
                if (id == null)
                {
                    User_UserProfile obj = new User_UserProfile();
                    obj.User = new User();
                    obj.UserProfile = new UserProfile();
                    obj.UserProfile.CountryCollection = db.Countries.ToList<Country>();
                    return View(obj);
                }
                else
                {
                    var userobj = db.Users.Find(id);
                    var userprofileobj = db.UserProfiles.Where(model => model.UserID == id).FirstOrDefault();
                    User_UserProfile obj = new User_UserProfile();
                    obj.User = userobj;
                    obj.UserProfile = userprofileobj;
                    obj.UserProfile.CountryCollection = db.Countries.ToList<Country>();
                    return View(obj);
                }
            }
            else { return RedirectToAction("Login", "Home"); }
        }
       
        public ActionResult AddAdmin(User_UserProfile obj)
        {

            if (Session["UserID"] != null && (int)Session["RoleID"] == 3)
            {

                if (db.Users.Where(model => model.EmailID == obj.User.EmailID).Any())
                {
                    ModelState.AddModelError("EmailID", "Email Already exists");

                    return View();
                }
                if(ModelState.IsValid)
                {
                    int id = obj.User.ID;
                    var user = db.Users.Find(id);
                    if (user == null)
                    {
                        User userobj = obj.User;
                        UserProfile userprofile = obj.UserProfile;
                        db.Users.Add(userobj);
                        db.SaveChanges();
                        var obj1 = db.Users.Find(userobj.ID);
                        userprofile.UserID = obj1.ID;
                        db.UserProfiles.Add(userprofile);
                        db.SaveChanges();
                        return RedirectToAction("ManageAdmin");
                    }
                    else
                    {

                        user = obj.User;
                        db.SaveChanges();
                        return RedirectToAction("ManageAdmin");
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public FileResult DownloadNotes(int id)
        {
            
            //Note noteobj = db.Notes.Find(id);
            int count = db.NotesAttachments.Where(model => model.NoteID == id).Count();

            // NotesAttachment obj = db.NotesAttachments.Where(model => model.NoteID.Equals(id)).FirstOrDefault();

            if (count > 1)
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    foreach (var file in db.NotesAttachments.Where(model => model.NoteID == id))
                    {
                        zip.AddFile(@Url.Content(file.FilePath), "Files");

                    }
                    string zipName = String.Format("FilesZip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        zip.Save(memoryStream);
                        return File(memoryStream.ToArray(), "application/zip", zipName);
                    }
                }
            }

            else
            {
                var obj = db.NotesAttachments.Find(id);
                byte[] fileBytes = System.IO.File.ReadAllBytes(Url.Content(obj.FilePath));
                string filename = obj.FileName;
                return File(fileBytes, "application/pdf", filename);
            }
         




        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteReview(int id)
        {
            var obj = db.NotesReviews.Find(id);
            db.NotesReviews.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("NoteDetails", id);
        }

    }
}
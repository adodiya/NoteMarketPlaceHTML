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

namespace NotesMarketPlace.Controllers
{
    public class AdminController : Controller
    {
        NotesMarketEntities2 db = new NotesMarketEntities2();

       
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult Dashboard(string searchstring, string dropdownsearch, string sortby, int? page)
        {
            //var notes = db.Notes.AsQueryable();
            var lowerlimit = DateTime.Now.AddMonths(-6);
            var upperlimit = DateTime.Now.AddMonths(6);
            var notes = db.Notes.Where(model => model.Status ==4 &&(model.PublishedDate>lowerlimit&&model.PublishedDate<upperlimit));
         
            ViewBag.Submitted= db.Notes.Where(model => model.Status.Equals(5)).Count();
            ViewBag.Downloaded = db.Downloads.Where(model => model.IsAttachmentDownloaded.Equals(true)).Select(model=>model.NoteID).Distinct().Count();
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
                notes = notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring) || model.Price.Equals(searchstring));
            }
            if (!string.IsNullOrEmpty(dropdownsearch))
            {
                notes = notes.Where(model => model.User.FirstName == searchstring);
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

       
        public ActionResult NotesUnderReview(string searchstring, string dropdownsearch, string sortby, int? page)
        {

            ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
            ViewBag.SortByCategory= string.IsNullOrEmpty(sortby) ? "Category desc" : "";
            ViewBag.SortBySeller= string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
            ViewBag.SortByStatus= string.IsNullOrEmpty(sortby) ? "Status desc" : "";
            ViewBag.SortByDate= string.IsNullOrEmpty(sortby) ? "Date desc" : "";
            ViewBag.dropdownsearch = new SelectList(db.Users.Where(model => model.RoleID == 1).ToList(), "FirstName", "FirstName");
            var notes = db.Notes.AsQueryable();
            notes = db.Notes.Where(model => model.Status == 5 || model.Status == 8);

            if (!string.IsNullOrEmpty(searchstring))
            {

                notes = db.Notes.Where(model =>model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring) || model.ReferenceData.Value.Contains(searchstring));
                
            }
            if (!string.IsNullOrEmpty(dropdownsearch))
            {
                notes = db.Notes.Where(model=>model.User.FirstName==dropdownsearch).Where(model=>model.Status==5);
            }
            switch(sortby)
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
            return View(notes.ToPagedList(page??1,5));
        }

        public ActionResult Approved(int id)
        {
            var notes = db.Notes.Single(model=>model.ID==id);
            notes.Status = 4;
            notes.PublishedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }

        public ActionResult Rejected(int id,FormCollection form)
        {
            string remarks = form["Remarks"];
            var notes = db.Notes.Single(model=>model.ID==id);
            notes.Status = 6;
            notes.AdminRemarks = remarks;
            notes.ActionedBy = 9;
            db.SaveChanges();
           
            return RedirectToAction("NotesUnderReview");
        }

        public ActionResult InReview(int id)
        {
            var notes = db.Notes.Single(model => model.ID == id);
            notes.Status = 8;
            notes.PublishedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }
        public ActionResult PublishedNotes(string searchstring, string dropdownsearch, string sortby, int? page)
        {

            


            ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
            ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
            ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
            ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
            ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
            ViewBag.SortByPrice= string.IsNullOrEmpty(sortby) ? "Price desc" : "";
            ViewBag.SortByAdmin= string.IsNullOrEmpty(sortby) ? "Admin desc" : "";
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

        public ActionResult Unpublish(int id, FormCollection form)
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

            ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
            ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
            ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
            ViewBag.SortByStatus = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
            ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
            ViewBag.SortByPrice = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
            ViewBag.SortByBuyer = string.IsNullOrEmpty(sortby) ? "Buyer desc" : "";
          

            var downloads = db.Downloads.AsQueryable();
            downloads = db.Downloads.Where(model => model.IsAttachmentDownloaded==true);

            if (!string.IsNullOrEmpty(searchstring))
            {
                downloads = db.Downloads.Where(model => model.NoteTitle.Contains(searchstring) || model.NoteCategory.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring)|| model.User1.FirstName.Contains(searchstring) || model.User1.LastName.Contains(searchstring) || model.Price.Equals(searchstring));
            }
            if (!string.IsNullOrEmpty(dropdownsearchNote))
            {
               downloads = db.Downloads.Where(model => model.NoteTitle==dropdownsearchNote);
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

        public ActionResult RejectedNotes(string searchstring, string dropdownsearch, string sortby, int? page)
        {

            ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
            ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
            ViewBag.SortBySeller = string.IsNullOrEmpty(sortby) ? "Seller desc" : "";
            ViewBag.SortByRejectedBy = string.IsNullOrEmpty(sortby) ? "RejectedBy desc" : "";
            ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";
           

            var notes = db.Notes.AsQueryable();
            notes = db.Notes.Where(model => model.Status == 6);

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

        public ActionResult Members(string searchstring,int?page,string sortby)
        {
            var users = db.Users.AsQueryable();
            users = db.Users.Where(model => model.RoleID.Equals(1));
           
            return View(users.ToPagedList(page ?? 1, 5));
        }

        public ActionResult MemberDetails(string searchstring, string dropdownsearch, string sortby, int? page)
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
            var notes = db.Notes.Where(model => model.SellerID == 10 && model.Status!=5);
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

            return View(notes.ToPagedList(page??1,5));
        }

        [HttpGet]
        public ActionResult UpdateProfile()
        {
            int id = (int)Session["id"];
            var userobj = db.Users.Find(id);
            var userprofileobj = db.UserProfiles.Where(model => model.UserID.Equals(id)).FirstOrDefault();
            User_UserProfile obj = new User_UserProfile();
            if(userprofileobj!=null)
            {
                obj.User = userobj;
                obj.UserProfile = new UserProfile();
                return View(obj);
            }
            else
            {
                obj.User = userobj;
                    obj.UserProfile = userprofileobj;
                return View(obj);
            }
            

            
        }

        [HttpPost]
        public ActionResult UpdateProile(User_UserProfile obj)
        {
            return View();
        }

        public ActionResult NoteDetails(int id)
        {
            Note obj1 = db.Notes.Where(model => model.ID.Equals(id)).FirstOrDefault();
            return View(obj1);
        }


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }
    }
}
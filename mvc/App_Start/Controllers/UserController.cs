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
using Ionic.Zip;
using System.Web.Security;

namespace NotesMarketPlace.Controllers
{
    public class UserController : Controller
    {
        NotesMarketEntities2 db = new NotesMarketEntities2();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        //Edit the user information
        public ActionResult MyProfile()
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
                    obj.UserProfile.GenderList = db.ReferenceDatas.Where(model => model.RefCategory.Equals("Gender")).ToList<ReferenceData>();
                    obj.UserProfile.CountryCollection = db.Countries.ToList<Country>();
                    return View(obj);
                }
                else
                {
                    userprofileobj.GenderList = db.ReferenceDatas.Where(model => model.RefCategory.Equals("Gender")).ToList<ReferenceData>();
                    userprofileobj.CountryCollection = db.Countries.ToList<Country>();
                    obj.User = userobj;
                    obj.UserProfile = userprofileobj;
                    return View(obj);
                }
            }

            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost] public ActionResult MyProfile(User_UserProfile obj, HttpPostedFileBase DisplayImageData)
        {
            if (Session["UserID"] != null) 
            {
                if(ModelState.IsValid)
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

                else {
                    return View();
                }
               

            }


            else
            {
                return RedirectToAction("Login", "User");
            }

        }





        //Add or edit the notes
        [HttpGet] public ActionResult AddNote(int? id)
        {
            if (Session["UserID"] != null)
            {
                var noteobj2 = db.Notes.Find(id);
                Notes_NotesAttachment noteobj = new Notes_NotesAttachment();

                if (noteobj2 != null)
                {

                    noteobj2.CategoryCollection = db.Categories.ToList<Category>();
                    noteobj2.CountryCollection = db.Countries.ToList<Country>();
                    noteobj2.TypeCollection = db.Types.ToList<Type>();

                    noteobj.AttachmentList = db.NotesAttachments.Where(model => model.NoteID == id).ToList();
                    noteobj.Note = noteobj2;
                    noteobj.NotesAttachment = new NotesAttachment();

                    return View(noteobj);

                }
                else
                {

                    noteobj.AttachmentList = new List<NotesAttachment>();
                    noteobj.Note = new Note();
                    noteobj.Note.CategoryCollection = db.Categories.ToList<Category>();
                    noteobj.Note.CountryCollection = db.Countries.ToList<Country>();
                    noteobj.Note.TypeCollection = db.Types.ToList<Type>();
                    noteobj.Note.SellerID = (int)Session["UserID"];
                    noteobj.NotesAttachment = new NotesAttachment();
                    return View(noteobj);
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }


        }

        //Save notes
        [HttpPost]
        public ActionResult AddNote(Notes_NotesAttachment noteobj1, HttpPostedFileBase previewfile, HttpPostedFileBase displaypic, HttpPostedFileBase[] attachmentfiles)
        {
           
            var allowedImageExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
            var allowedFileExtensions = new[] { ".pdf" };
            Note noteobj = noteobj1.Note;
            NotesAttachment attachobj = new NotesAttachment();
            noteobj.SellerID = (int)Session["UserID"];


            if (noteobj.IsPaid == true && noteobj.Price == null)
            {
                ModelState.AddModelError("Price", "Please enter the price");
            }
            if (noteobj.IsPaid == true && noteobj.Price == 0)
            {
                ModelState.AddModelError("Price", "Price cannot be 0 for paid notes");
            }
            if (noteobj.IsPaid == true && noteobj.Preview == null)
            {
                ModelState.AddModelError("", "Please add a preview for paid notes");
            }


            var noteExists = db.Notes.Where(model => model.ID.Equals(noteobj.ID)).FirstOrDefault();
            //Add Note
            if (noteExists == null)
            {
                if (noteobj.IsPaid == false)
                {
                    noteobj.Price = 0;
                }
                noteobj.Status = 4;
                if (previewfile != null)
                {
                    string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                    string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);

                    //Condition to allow only pdfs for preview file 
                    if (allowedFileExtensions.Contains(_PreviewFileExtension))
                    {
                        //Condition to allow only images

                        _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                        noteobj.Preview = _path;
                        previewfile.SaveAs(_path);

                    }
                    else
                    {
                        ModelState.AddModelError("", "Only pdfs are allowed");
                    }
                }
                if (displaypic != null)
                {

                    string imagefileName = Path.GetFileNameWithoutExtension(displaypic.FileName);
                    string imageextension = Path.GetExtension(displaypic.FileName);


                    //Condition to allow only images
                    if (allowedImageExtensions.Contains(imageextension))
                    {
                        imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                        noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                        imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                        displaypic.SaveAs(imagefileName);



                    }
                    else
                    {
                        ModelState.AddModelError("", "Only pdfs are allowed");
                    }
                }
                noteobj.PublishedDate = DateTime.Now;
                noteobj.CreatedBy = (int)Session["UserID"];
                db.Notes.Add(noteobj);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                int NoteId = noteobj.ID;

                //Multiple file upload start
                foreach (var attachment in attachmentfiles)
                {
                    if (attachment.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                        string _FileExtension = Path.GetExtension(previewfile.FileName);

                        //Condition to allow only pdfs for notes attachment
                        if (allowedFileExtensions.Contains(_FileExtension))
                        {
                            _FileName = _FileName + DateTime.Now.ToString("yymmssff") + _FileExtension;
                            string _attachmentpath = Path.Combine(Server.MapPath("~/UploadedFiles/"), _FileName);

                            attachobj.NoteID = NoteId;
                            attachobj.FileName = _FileName;
                            attachobj.FilePath = _attachmentpath;
                            attachment.SaveAs(_attachmentpath);
                            db.NotesAttachments.Add(attachobj);
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                        }


                        else
                        {
                            ModelState.AddModelError("", "Please choose only Image file");
                        }
                    }
                }
                //multiple file upload end

                return View();



            }

            //Edit Note
            else
            {
                noteobj.ModifiedBy = (int)Session["UserID"];
                noteobj.ModifiedDate = DateTime.Now;
                string forpreview = Request.Form["Preview"];
                string fordisplay = Request.Form["Preview"];
                if (noteobj.IsPaid == true && (previewfile == null && forpreview == "Removed"))
                {
                    ModelState.AddModelError("", "Add a file");
                }

                if (previewfile != null)
                {
                    noteExists = noteobj;
                    string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                    string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);

                    if (allowedFileExtensions.Contains(_PreviewFileExtension))
                    {

                        _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                        noteExists.Preview = _path;
                        previewfile.SaveAs(_path);
                    }

                    else
                    {
                        ModelState.AddModelError("", "");
                    }
                }

                else
                {
                    if (forpreview == "Removed")
                    {
                        noteExists = noteobj;
                        noteExists.Preview = null;
                    }
                    else
                    {
                        noteobj.Preview = noteExists.Preview;
                        noteExists = noteobj;
                    }

                }
                if (displaypic != null)
                {

                    noteExists = noteobj;

                    string imagefileName = Path.GetFileNameWithoutExtension(displaypic.FileName);
                    string imageextension = Path.GetExtension(displaypic.FileName);

                    if (allowedImageExtensions.Contains(imageextension))
                    {
                        imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                        noteExists.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                        imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                        displaypic.SaveAs(imagefileName);





                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                    }
                }

                else
                {
                    if (fordisplay == "Removed")
                    {
                        noteExists = noteobj;
                    }
                    else
                    {
                        noteobj.DisplayPic = noteExists.DisplayPic;
                        noteExists = noteobj;
                    }

                }

                int count = db.NotesAttachments.Where(model => model.NoteID.Equals(noteobj.ID)).Count();
                if (attachmentfiles != null)
                {
                    foreach (var attachment in attachmentfiles)
                    {
                        if (attachment.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                            string _FileExtension = Path.GetExtension(previewfile.FileName);
                            if (allowedFileExtensions.Contains(_FileExtension))
                            {
                                _FileName = _FileName + DateTime.Now.ToString("yymmssff") + _FileExtension;
                                string _attachmentpath = Path.Combine(Server.MapPath("~/UploadedFiles/"), _FileName);

                                attachobj.NoteID = noteobj.ID;
                                attachobj.FileName = _FileName;
                                attachobj.FilePath = _attachmentpath;
                                attachment.SaveAs(_attachmentpath);
                                db.NotesAttachments.Add(attachobj);
                                db.Configuration.ValidateOnSaveEnabled = false;
                                db.SaveChanges();

                                var update = db.Notes.Find(noteobj.ID);
                                db.Entry(update).CurrentValues.SetValues(noteExists);

                                //db.Entry(userprofileobj).State = EntityState.Modified;
                                db.Configuration.ValidateOnSaveEnabled = false;
                                db.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", "Please choose only Image file");
                            }
                        }
                    }
                }
                else
                {
                   if(count==0)
                    {
                        ModelState.AddModelError("", "");
                    }
                    
                }

            }
            return RedirectToAction("SearchNotes");


        }

        //Publish notes
        public ActionResult PublishNote(int id)
        {

            var obj = db.Notes.Find(id);
            obj.Status = 4;
            db.SaveChanges();
            
           // SendPublishEmail(obj.Title, obj.User.FullName);
            return RedirectToAction("SearchNotes");

        }

        public void SendPublishEmail(string title, string sellername)
        {


            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress("ahdodiya99@gmail.com");

            var fromEmailPassword = "*****"; // Replace with actual password
            string subject = sellername + "sent their note for review";

            string body = "Hello Admins\n\n" + "We want to inform you that, " + sellername + ",\nsent his note" + title + "for review. Please look at the notes and take required actions.\n\n" + "Regards,\n" + "Notes Marketplace";



            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (MailMessage message = new MailMessage())
            {
                message.From = fromEmail;
                message.To.Add("");
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);

            };

        }

        //Delete previously stored notes attachmensts
        [HttpPost]
        public JsonResult DeleteFile(int id)
        {
            
            NotesAttachment file = db.NotesAttachments.Find(id);
           
            db.NotesAttachments.Remove(file);
            
            db.SaveChanges();
            return Json(new { Result = "OK" });
        }

        public ActionResult SearchNotes(int? page, string searchstring, string searchCategory, string searchType, string searchUniversity, string searchCourse, string searchCountry, string searchRating)
        {
            ViewBag.searchCategory = new SelectList(db.Categories.ToList(), "Name", "Name");
            ViewBag.searchType = new SelectList(db.Types.ToList(), "Name", "Name");
            ViewBag.searchCountry = new SelectList(db.Countries.ToList(), "Name", "Name");
            ViewBag.searchCourse = new SelectList(db.Notes.ToList(), "Course", "Course");


            var notes = db.Notes.Where(model => model.Status == 5).OrderBy(model => model.PublishedDate);
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

            return View(notes.ToPagedList(page ?? 1, 5));
        }

        public ActionResult AddRequest(int id)
        {
            var obj = db.Notes.Find(id);
            Download request = new Download();
            request.NoteID = id;
            request.NoteTitle = obj.Title;
            request.NoteCategory = obj.Category.Name;
            request.DownloadAllowed = false;
            request.Downloader = (int)Session["UserID"];
            request.Seller = obj.SellerID;
            request.Price = obj.Price;
            request.IsPaid = obj.IsPaid;
            request.IsAttachmentDownloaded = false;
            request.CreatedBy = (int)Session["UserID"];
            request.CreatedDate = DateTime.Now;
            db.Downloads.Add(request);
            db.SaveChanges();
            var user = db.Users.Find(obj.SellerID);
            var buyer = db.Users.Find((int)Session["UserID"]);
            SendEmailToSeller(buyer,user);
            ViewBag.Message = "Request Sent";
            return RedirectToAction("NotesDetails", "User", new { id=id});
        }
        public void SendEmailToSeller(User buyer,User seller)
        {
           

            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Aadi");
            var toEmail = new MailAddress(seller.EmailID);
            var fromEmailPassword = ""; // Replace with actual password
            string subject = buyer.FullName + "wants to purchase your notes";

            string body = "Hello" + seller.FullName+",\nWe would like to inform you that," + buyer.FullName + "wants to purchase your notes. Please see Buyer Requests tab and allow download access to Buyer if you have received the payment from him.\n\n" + "Regards,\n" + "Notes Marketplace";



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

        public ActionResult AllowDownload(int id)
        {
            var note = db.Downloads.Find(id);
            note.DownloadAllowed = true;
            note.ModifiedBy = (int)Session["UserID"];
            note.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            var buyer = db.Users.Find(note.Downloader);
            var seller = db.Users.Find((int)Session["UserID"]);
            //SendEmailToBuyer(buyer, seller);
            return RedirectToAction("BuyerRequest");
        }

        public void SendEmailToBuyer(User buyer, User seller)
        {


            var fromEmail = new MailAddress("ahdodiya99@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(seller.EmailID);
            var fromEmailPassword = "*****"; // Replace with actual password
            string subject = seller.FullName + " Allows you to download a note";

            string body = "Hello" + buyer.FullName + ",\nWe would like to inform you that," + seller.FullName + " Allows you to download a note. Please login and see My Download tabs to download particular note.\n\n" + "Regards,\n" + "Notes Marketplace";



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
        public ActionResult BuyerRequest(int? page, string searchstring , string sortby)
        {
            if (Session["UserID"] != null)
            {
                int id = (int)Session["UserID"];
                var downloads = db.Downloads.Where(model => model.DownloadAllowed.Equals(false)).ToList().ToPagedList(page ?? 1, 3);
                return View(downloads.ToPagedList(page ?? 1, 2));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }

        [HttpGet]public ActionResult NotesDetails(int id)
        {
            if(Session["UserID"]!=null)
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

        public ActionResult FileDownload(int id)
        {
            if(Session["UserID"]!=null)
            {
                int userid = (int)Session["UserID"];
                var obj1 = db.Downloads.Where(model => model.NoteID.Equals(id) && model.Downloader.Equals(userid) && model.IsAttachmentDownloaded == true).FirstOrDefault();
                if (obj1 != null)
                {
                    if (obj1.IsAttachmentDownloaded == false)
                    {
                        obj1.IsAttachmentDownloaded = true;
                        db.SaveChanges();
                        return DownloadNotes(id);
                    }
                    else
                    {
                        return DownloadNotes(id);
                    }
                }
                else
                {
                    Note noteobj = db.Notes.Find(id);
                  
                    var obj = db.NotesAttachments.Where(model => model.NoteID.Equals(id)).FirstOrDefault();
                    Download entry = new Download();
                    entry.NoteID = noteobj.ID;
                    entry.Seller = noteobj.SellerID;
                    entry.Downloader = (int)Session["UserID"];
                    entry.DownloadAllowed = true;
                    entry.IsAttachmentDownloaded = true;
                    entry.AttachmentPath = obj.FilePath;
                    entry.IsPaid = noteobj.IsPaid;
                    entry.Price = noteobj.Price;
                    entry.NoteTitle = noteobj.Title;
                    entry.NoteCategory = noteobj.Category.Name;
                    db.Downloads.Add(entry);

                    db.SaveChanges();

                    return DownloadNotes(id);




                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public FileResult DownloadNotes(int id)
        {
            //Note noteobj = db.Notes.Find(id);
            int count= db.NotesAttachments.Where(model => model.NoteID==id).Count();
           
           // NotesAttachment obj = db.NotesAttachments.Where(model => model.NoteID.Equals(id)).FirstOrDefault();
         
           if(count>1)
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

            else {
                var obj = db.NotesAttachments.Where(model=>model.NoteID==id).FirstOrDefault();
                byte[] fileBytes = System.IO.File.ReadAllBytes(Url.Content(obj.FilePath));
                string filename = obj.FileName;
                return File(fileBytes, "application/pdf", filename);
            }






        }

        public ActionResult Dashboard(int?page1,int?page2,string searchString1,string sortby1, string searchString2, string sortby2)
        {
           if(Session!=null)
            {
                int id = 10;
                ViewBag.Count = db.Downloads.Where(model => model.Seller.Equals(id) && model.IsAttachmentDownloaded.Equals(true)).Select(model => model.NoteID).Distinct().Count();
                ViewBag.Sum = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed.Equals(true) && model.IsPaid.Equals(true)).Select(model => model.Price).Sum();

                ViewBag.MyDownloads = db.Downloads.Where(model => model.Downloader.Equals(id) && model.IsAttachmentDownloaded.Equals(true)).Count();
                ViewBag.MyRejected = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).Count();

              ViewBag.BuyerRequest = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed == false).Count();
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby1) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby1) ? "Category desc" : "";
                ViewBag.SortByPrice = string.IsNullOrEmpty(sortby1) ? "Price desc" : "";
                ViewBag.SortByType = string.IsNullOrEmpty(sortby1) ? "Type desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby1) ? "Date desc" : "";

                ViewBag.Title = string.IsNullOrEmpty(sortby2) ? "Title" : "";
                ViewBag.Category = string.IsNullOrEmpty(sortby2) ? "Category" : "";
                ViewBag.Status = string.IsNullOrEmpty(sortby2) ? "Status" : "";
                ViewBag.Date = string.IsNullOrEmpty(sortby2) ? "Date" : "";
                Note note = new Note();
                note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderByDescending(model => model.PublishedDate).ToPagedList(page1 ?? 1, 5);
                note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).OrderByDescending(model => model.PublishedDate).ToPagedList(page2 ?? 1, 5);
                if(!string.IsNullOrEmpty(searchString1))
                {
                    note.Publishednotes = note.Publishednotes.Where(model => model.Title.Contains(searchString1)||model.Category.Name.Contains(searchString1)||model.ReferenceData.Value.Contains(searchString1)).ToPagedList(page1 ?? 1, 5);
                }
            if (!string.IsNullOrEmpty(searchString2))
            {
                note.Inprogressnotes = note.Inprogressnotes.Where(model => model.Title.Contains(searchString2) || model.Category.Name.Contains(searchString2)).OrderByDescending(model => model.PublishedDate).ToPagedList(page2 ?? 1, 5);
            }   
                switch (sortby1)
                {
                    case "Title desc":
                        note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.Title).ToPagedList(page1 ?? 1, 5);
                        break;
                    case "Category desc":
                        note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.Category.Name).ToPagedList(page1 ?? 1, 5);
                        break;
                    case "Sell desc":
                        note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.IsPaid).ToPagedList(page1 ?? 1, 5);
                        break;

                    case "Price desc":
                        note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.Price).ToPagedList(page1 ?? 1, 5);
                        break;
                    case "Date desc":
                        note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.PublishedDate).ToPagedList(page1 ?? 1, 5);
                        break;
                    
                    default:
                        note.Publishednotes = note.Publishednotes.OrderBy(model => model.PublishedDate).ToPagedList(page1 ?? 1, 5);
                        break;
                }

                switch (sortby2)
                {
                    case "Title":
                        note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).OrderBy(model => model.Title).ToPagedList(page2 ?? 1, 5);
                        break; 
                    case "Category":
                        note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).OrderBy(model => model.Category.Name).ToPagedList(page2?? 1, 5);
                        break;
                    case "Status":
                        note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).OrderBy(model => model.IsPaid).ToPagedList(page2 ?? 1, 5);
                        break;

                    
                    case "Date":
                        note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).OrderBy(model => model.PublishedDate).ToPagedList(page2 ?? 1, 5);
                        break;

                    default:
                        note.Inprogressnotes = note.Inprogressnotes.OrderByDescending(model => model.PublishedDate).ToPagedList(page2 ?? 1, 5);
                        break;
                }
                return View();
            }

           else
            {
                return RedirectToAction("Login", "User");
            }



        }

     
        public ActionResult Publish(int?page1,string searchString1,string sortby1)
        {
            ViewBag.SortByTitle = string.IsNullOrEmpty(sortby1) ? "Title desc" : "";
            ViewBag.SortByCategory = string.IsNullOrEmpty(sortby1) ? "Category desc" : "";
            ViewBag.SortByPrice = string.IsNullOrEmpty(sortby1) ? "Price desc" : "";
            ViewBag.SortByType = string.IsNullOrEmpty(sortby1) ? "Type desc" : "";
            ViewBag.SortByDate = string.IsNullOrEmpty(sortby1) ? "Date desc" : "";
            var note= db.Notes.Where(model => model.SellerID.Equals(10) && model.Status == 4);
            if (!string.IsNullOrEmpty(searchString1))
            {
                note = note.Where(model => model.Title.Contains(searchString1) || model.Category.Name.Contains(searchString1) || model.ReferenceData.Value.Contains(searchString1));
            }
            switch (sortby1)
            {
                case "Title desc":
                    note = note.OrderBy(model => model.Title);
                    break;
                case "Category desc":
                    note = note.OrderBy(model => model.Category.Name);
                    break;
                case "Sell desc":
                    note= note.OrderBy(model => model.IsPaid);
                    break;

                case "Price desc":
                    note= note.OrderBy(model => model.Price);
                    break;
                case "Date desc":
                    note = note.OrderBy(model => model.PublishedDate);
                    break;
                    
                default:
                    note = note.OrderBy(model => model.PublishedDate);
                    break;
            }

            return PartialView("_InProgressDashboard",note.ToPagedList(page1 ?? 1, 5));
        }

       
      
        
        public ActionResult MyDownloads(int? page, string searchstring, string sortby)
        {
           if (Session["UserID"]!=null)
           {
                int id = (int)Session["UserID"];
                var notes = db.Downloads.Where(model => model.Downloader.Equals(id) && (model.IsAttachmentDownloaded.Equals(true) || model.DownloadAllowed.Equals(true)));
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortByBuyer = string.IsNullOrEmpty(sortby) ? "Buyer desc" : "";
                ViewBag.Type = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.Price = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date" : "Date desc";
               
               

                if (!string.IsNullOrEmpty(searchstring))
                {

                    notes = notes.Where(model => model.NoteTitle.Contains(searchstring) || model.NoteCategory.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring));

                }
               
                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.NoteTitle);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.NoteCategory);
                        break;
                    case "Buyer desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    case "Type desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.DownloadDate);
                        break;
                    case "Seller desc":
                        notes = notes.OrderBy(model => model.User.FirstName);
                        break;
                    default:
                        notes = notes.OrderByDescending(model => model.DownloadDate);
                        break;
                }
                return View(notes.ToPagedList(page??1,5));
           }
            else
           {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ReportedIssues(FormCollection form, int id)
        {
            if (Session["UserID"] != null)
            {

                int userid = (int)Session["UserID"];
                    var downloadobj = db.Downloads.Find(id);
                    ReportedIssue obj = new ReportedIssue();
                    obj.ReportedByID =userid;
                    obj.Remarks = form["Remarks"];
                   
                    obj.DownloadID = downloadobj.ID;
                    obj.NoteID = downloadobj.NoteID;
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = userid;
                
                    db.ReportedIssues.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("MyDownloads", "User");
                
                
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Addreview(FormCollection form, int id)

        {
            if (Session["UserID"] != null)
            {
                int userid = (int)Session["UserID"];
                var obj = db.Downloads.Find(id);
                var obj1 = db.NotesReviews.Where(model => model.ReviewedBy == userid).FirstOrDefault();
                string rate = form["rate"];
                string comments = form["Comments"];
                if (obj==null)
                {
                    NotesReview review = new NotesReview();
                    review.Ratings = decimal.Parse(rate);
                    review.ReviewedBy = userid;
                    review.CreatedDate = DateTime.Now;
                    review.CreatedBy = userid;
                    review.DownloadID = id;
                    review.NoteID = obj.NoteID;
                    review.Comments = comments;
                    review.IsActive = true;
                    db.NotesReviews.Add(review);
                    db.SaveChanges();
                }
                else
                {
                    obj1.Ratings = decimal.Parse(rate);
                    obj1.Comments = comments;
                    obj1.ModifiedBy = userid;
                    obj1.ModifiedDate = DateTime.Now;
                    obj1.IsActive = true;
                    db.SaveChanges();
                }
               
               
                return RedirectToAction("MyDownloads", "User");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult MySoldNotes(int? page, string searchstring, string sortby)
        {
            if(Session["UserID"] != null)
            {
                int id = (int)Session["UserID"];
                var notes = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed == true);
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                ViewBag.SortByBuyer = string.IsNullOrEmpty(sortby) ? "Buyer desc" : "";
                ViewBag.Type = string.IsNullOrEmpty(sortby) ? "Status desc" : "";
                ViewBag.Price = string.IsNullOrEmpty(sortby) ? "Price desc" : "";
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";



                if (!string.IsNullOrEmpty(searchstring))
                {

                    notes = notes.Where(model => model.NoteTitle.Contains(searchstring) || model.NoteCategory.Contains(searchstring) || model.User.FirstName.Contains(searchstring) || model.User.LastName.Contains(searchstring));

                }

                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.NoteTitle);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.NoteCategory);
                        break;
                    case "Buyer desc":
                        notes = notes.OrderBy(model => model.User1.FirstName);
                        break;
                    case "Type desc":
                        notes = notes.OrderBy(model => model.IsPaid);
                        break;
                    case "Price desc":
                        notes = notes.OrderBy(model => model.Price);
                        break;
                    case "Date desc":
                        notes = notes.OrderBy(model => model.DownloadDate);
                        break;
                   
                    default:
                        notes = notes.OrderByDescending(model => model.DownloadDate);
                        break;
                }
                return View(notes.ToPagedList(page??1,5));
           }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult MyRejectedNotes(int? page, string searchstring, string sortby)
        {
            if (Session["UserID"] != null)
            {
                int id = (int)Session["UserID"];
                var notes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status.Equals(6));
                ViewBag.SortByTitle = string.IsNullOrEmpty(sortby) ? "Title desc" : "";
                ViewBag.SortByCategory = string.IsNullOrEmpty(sortby) ? "Category desc" : "";
                
               
                ViewBag.SortByDate = string.IsNullOrEmpty(sortby) ? "Date desc" : "";



                if (!string.IsNullOrEmpty(searchstring))
                {

                    notes = notes.Where(model => model.Title.Contains(searchstring) || model.Category.Name.Contains(searchstring));

                }

                switch (sortby)
                {
                    case "Title desc":
                        notes = notes.OrderBy(model => model.Title);
                        break;
                    case "Category desc":
                        notes = notes.OrderBy(model => model.Category.Name);
                        break;
                 
                    case "Date desc":
                        notes = notes.OrderBy(model => model.ModifiedDate);
                        break;

                    default:
                        notes = notes.OrderByDescending(model => model.ModifiedDate);
                        break;
                }
                return View(notes.ToPagedList(page ?? 1, 5));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ContactUs()
        {
            if (Session["UserID"] != null)
            {
                User obj = db.Users.Find((int)Session["UserID"]);
                return View(obj);        
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
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
            message.Subject = email+" - "+ subject;
            message.Body = "Hello,\n\n " + comments + "\n\nRegards:,\n " + fullname ;
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
            User userobj = db.Users.Find(9);
            string oldpassword = form["oldpwd"];
            string newpassword = form["newpwd"];
            string confirmpassword = form["confirmpwd"];
            if (oldpassword==userobj.Password) {
                if(newpassword==confirmpassword)
                { 
                    userobj.Password = newpassword;
                    db.Users.Attach(userobj);
                    db.Entry(userobj).Property(x => x.Password).IsModified = true;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard");
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


      
      
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }

    }







}

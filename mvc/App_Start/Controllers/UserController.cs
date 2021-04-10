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

            var allowedImageExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
            User obj1 = obj.User;
             UserProfile obj2 = obj.UserProfile;
            db.Entry(obj1).State = EntityState.Modified;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            
            var userprofileobj = db.UserProfiles.Where(model => model.UserID.Equals(obj1.ID)).FirstOrDefault();
            
            if (userprofileobj == null)
            {

                if (DisplayImageData.ContentLength >0)
                {

                    string imagefileName = Path.GetFileNameWithoutExtension(DisplayImageData.FileName);
                    string imageextension = Path.GetExtension(DisplayImageData.FileName);
                    //if (allowedImageExtensions.Contains(imageextension))
                    //{

                        imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                        obj2.ProfilePic = "~/ProfileImage/" + imagefileName;
                        imagefileName = Path.Combine(Server.MapPath("~/ProfileImage/"), imagefileName);
                        DisplayImageData.SaveAs(imagefileName);
                    
                        db.UserProfiles.Add(obj2);
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "User");

                   // }
                    //else
                    //{
                      //  return RedirectToAction("SearchNotes", "User");
                    //}
                }
                else
                {
                    
                    db.UserProfiles.Add(obj2);
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "User");
                }
               
            }
            else
            {
                
                if(DisplayImageData!= null && DisplayImageData.ContentLength>0 )
                {
                    userprofileobj = obj2;
                    string imagefileName = Path.GetFileNameWithoutExtension(DisplayImageData.FileName);
                    string imageextension = Path.GetExtension(DisplayImageData.FileName);
                    //if (allowedImageExtensions.Contains(imageextension))
                    //{

                    imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                    userprofileobj.ProfilePic = "~/ProfileImage/" + imagefileName;
                    imagefileName = Path.Combine(Server.MapPath("~/ProfileImage/"), imagefileName);
                    DisplayImageData.SaveAs(imagefileName);
                    var update = db.UserProfiles.Find(obj2.ID);
                    db.Entry(update).CurrentValues.SetValues(userprofileobj);

                    //db.Entry(userprofileobj).State = EntityState.Modified;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return RedirectToAction("SearchNotes", "User");
                }
                else
                {
                    string count = Request.Form["Country"];
                    if(count=="India")
                    {
                        obj2.ProfilePic = null;
                        var update = db.UserProfiles.Find(obj2.ID);
                        db.Entry(update).CurrentValues.SetValues(obj2);
                       
                        db.Entry(update).CurrentValues.SetValues(obj2);
                        //db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "User");

                    }
                    else {
                        obj2.ProfilePic = userprofileobj.ProfilePic;
                        var update = db.UserProfiles.Find(obj2.ID);
                        db.Entry(update).CurrentValues.SetValues(obj2);
                        //db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        return RedirectToAction("AddNote", "User");

                    }
                }
              
            }

          
        }




      
        //Add or edit the notes
        [HttpGet] public ActionResult AddNote(int? id)
        {
            if (Session["UserID"] != null)
            {
                var noteobj2 = db.Notes.Find(id);
                Notes_NotesAttachment noteobj = new Notes_NotesAttachment();
                
                if (noteobj2!=null)
                {
                   
                    noteobj2.CategoryCollection = db.Categories.ToList<Category>(); 
                    noteobj2.CountryCollection= db.Countries.ToList<Country>();
                    noteobj2.TypeCollection = db.Types.ToList<Type>();
                   
                    noteobj.AttachmentList = db.NotesAttachments.Where(model => model.NoteID==id).ToList();
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
            var allowedImageExtensions = new[]{".Jpg", ".png", ".jpg", "jpeg"};
            
            var allowedFileExtensions = new[]{".pdf"};
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
           

            var noteExists = db.Notes.Where(model=>model.ID.Equals(noteobj.ID)).FirstOrDefault();
            //Add Note
            if(noteExists==null)
            {
                if(noteobj.IsPaid==false)
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
                string fordisplay= Request.Form["Preview"];
                if (noteobj.IsPaid==true && (previewfile == null && forpreview == "Removed"))
                {
                    ModelState.AddModelError("", "Add a file");
                }

                if(previewfile!=null)
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
                if (displaypic!=null)
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
                if (attachmentfiles == null && count==0)
                {
                   
                    
                        ModelState.AddModelError("Price", "Add Attachments");
                    

                }
                else
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
             
               

            }
           
            
            
            
            
            
            
           

         
           
            noteobj.CategoryCollection = db.Categories.ToList<Category>();
            noteobj.CountryCollection = db.Countries.ToList<Country>();
            noteobj.TypeCollection = db.Types.ToList<Type>();
            return View(noteobj);
            
           
        }

        //Publish notes
        public ActionResult PublishNote(int id)
        {
            
          var obj = db.Notes.Find(id);
                obj.Status = 4;
                db.SaveChanges();
                return RedirectToAction("AddNote", "User", new { id = obj.ID });
                
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


        public ActionResult BuyerRequest(int? page)
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

            var obj1 = db.Notes.Where(model=>model.ID.Equals(id)).FirstOrDefault();
            return View(obj1);
        }

        public ActionResult FileDownload(int id)
        {
            var obj1 = db.Downloads.Where(model => model.NoteID.Equals(id) && model.Downloader.Equals(5) && model.IsAttachmentDownloaded == true).FirstOrDefault();
            if (obj1 != null) 
            { 
                return DownloadNotes(id); }
            else
            {
                Note noteobj = db.Notes.Find(id);
                int count = db.NotesAttachments.Where(model => model.NoteID == id).Count();
                NotesAttachment obj = db.NotesAttachments.Where(model => model.NoteID.Equals(id)).FirstOrDefault();
                Download entry = new Download();
                entry.NoteID = noteobj.ID;
                entry.Seller = noteobj.SellerID;
                entry.Downloader = (int)Session["UserID"];
                entry.DownloadAllowed = true;
                entry.IsAttachmentDownloaded = true;
                entry.AttachmentPath = obj.FilePath;
                entry.IsPaid = noteobj.IsPaid;
                entry.Price = 0;
                entry.NoteTitle = noteobj.Title;
                entry.NoteCategory = noteobj.Category.Name;
                db.Downloads.Add(entry);

                db.SaveChanges();
                
                 return DownloadNotes(id);
                
                

                
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
                var obj = db.NotesAttachments.Find(id);
                byte[] fileBytes = System.IO.File.ReadAllBytes(Url.Content(obj.FilePath));
                string filename = obj.FileName;
                return File(fileBytes, "application/pdf", filename);
            }






        }

        public ActionResult Dashboard(int?page1, int? page2)
        {
            int id = 10;


            
          
           
           ViewBag.Count = db.Downloads.Where(model=>model.Seller.Equals(id)&&model.IsAttachmentDownloaded.Equals(true)).Select(model => model.NoteID).Distinct().Count();
            ViewBag.Sum = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed.Equals(true) && model.IsPaid.Equals(true)).Select(model => model.Price).Sum();

            ViewBag.MyDownloads = db.Downloads.Where(model => model.Downloader.Equals(id) && model.IsAttachmentDownloaded.Equals(true)).Count();
            ViewBag.MyRejected = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).Count();

            
            ViewBag.BuyerRequest = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed==false).Count();
            Note note = new Note();
            note.Publishednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 4).OrderBy(model => model.PublishedDate).ToPagedList(page1 ?? 1, 3);
            note.Inprogressnotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status == 5).OrderBy(model => model.PublishedDate).ToPagedList(page2 ?? 1, 3);


            return View(note);
        }
       
       

        public ActionResult MyDownloads(int? page)
        {
           if (Session["UserID"]!=null)
           {
                int id = (int)Session["UserID"];
                var downloads = db.Downloads.Where(model => model.Downloader.Equals(10) && (model.IsAttachmentDownloaded.Equals(true) || model.DownloadAllowed.Equals(true)));
                return View(downloads.ToPagedList(page??1,2));
           }
            else
           {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult MySoldNotes(int? page)
        {
            if(Session["UserID"] != null)
            {
                int id = (int)Session["UserID"];
                var soldnotes = db.Downloads.Where(model => model.Seller.Equals(id) && model.DownloadAllowed == true);
                return View(soldnotes);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult MyRejectedNotes(int? page)
        {
            if (Session["UserID"] != null)
            {
                int id = (int)Session["UserID"];
                var rejectednotes = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status.Equals(6));
                return View(rejectednotes);
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


      
        public ActionResult ReportedIssues(FormCollection form, int id)
        {
            var find = db.ReportedIssues.Where(model => model.ReportedByID.Equals((int)Session["UserID"])).FirstOrDefault();
            if(find==null)
            {
                var downloadobj = db.Downloads.Where(model => model.NoteID.Equals(id)).FirstOrDefault();
                ReportedIssue obj = new ReportedIssue();
                obj.ReportedByID = (int)Session["UserID"];
                obj.Remarks = form["Comments"];
                obj.ID = id;
                obj.DownloadID = downloadobj.ID;
                obj.NoteID = id;
                db.ReportedIssues.Add(obj);
                db.SaveChanges();
                return View();
            }
            else
            {
                ViewBag.Message = "You have already given your feedback";
                return View();
            }
            

        }

        public ActionResult Addreview(FormCollection form, int id)

        {
            if(Session["UserID"]!=null)
            {
                int userid = (int)Session["UserID"];
                var downloadId = db.Downloads.Where(model => model.Seller.Equals(userid) && model.NoteID.Equals(id)).FirstOrDefault();
               string rate = form["rate"];
                string comments = form["comments"];
                NotesReview review = new NotesReview();
                review.Ratings = decimal.Parse(rate);
                review.ReviewedBy = userid;
                review.CreatedDate = DateTime.Now;
                review.CreatedBy= userid;
                review.DownloadID = downloadId.ID;
                review.NoteID = id;
                review.Comments = comments;
                db.NotesReviews.Add(review);
                db.SaveChanges();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
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

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

       
    public ActionResult MyProfile()
        {
            if (Session["UserID"] != null)
            {



                int id = (int)Session["UserID"];
                //User_UserProfile mymodel = new User_UserProfile();

                // mymodel.UserProfile = db.UserProfiles.Find(9);
                // mymodel.User = db.Users.Find(9);
                // mymodel.User = db.Users.Find();*/

                /* var getlist = db.Countries.ToList();
                SelectList list = new SelectList(getlist, "CountryCode", "Name");
                ViewBag.countrylist = list;
               var genderlist = db.ReferenceDatas.Where(model => model.RefCategory.Equals("Gender")).ToList();
                SelectList gender = new SelectList(genderlist, "ID", "Value");
                ViewBag.data = genderlist;

                var phonecode = db.Countries.ToList();
                SelectList code = new SelectList(phonecode, "CountryCode", "Name");
                ViewBag.countrycode = code;*/

                //  mymodel.UserProfile.CountryList = db.Countries.ToList<Country>();
                // mymodel.UserProfile.GenderList = db.ReferenceDatas.ToList<ReferenceData>();
                //  return View(mymodel);
                //}
                //else
                //{

                //}
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




        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                UserProfile detail = db.UserProfiles.Find(id);
              
                if (detail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }
                UserProfile obj = detail;
                obj.ProfilePic = null;
                detail.ProfilePic = null;
                //Remove from database
               
                db.Entry(detail).CurrentValues.SetValues(obj);
                db.SaveChanges();

              
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpGet] public ActionResult AddNote(int? noteid)
        {
            if (Session["UserID"] != null)
            {
                Notes_NotesAttachment noteobj = new Notes_NotesAttachment();
               /*Note noteobj = new Note();
                noteobj.CategoryCollection = db.Categories.ToList<Category>();
                noteobj.CountryCollection = db.Countries.ToList<Country>();
                noteobj.TypeCollection = db.Types.ToList<Type>();
                noteobj.SellerID = (int)Session["UserID"];
              return View(noteobj);*/
                if(noteid!=null)
                {
                    var noteobj2 = db.Notes.Where(model => model.ID.Equals(noteid)).FirstOrDefault();
                    noteobj2.CategoryCollection = db.Categories.ToList<Category>(); 
                    noteobj2.CountryCollection= db.Countries.ToList<Country>();

                    noteobj.AttachmentList = db.NotesAttachments.Where(model => model.NoteID.Equals(noteid)).ToList();
                    noteobj.Note = noteobj2;
                    return View(noteobj);
                }
                else
                {
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

        [HttpPost]
        public ActionResult AddNote(Notes_NotesAttachment noteobj1, HttpPostedFileBase previewfile, HttpPostedFileBase displaypic, HttpPostedFileBase[] attachmentfiles)
        {
            var allowedImageExtensions = new[]{".Jpg", ".png", ".jpg", "jpeg"};
            
            var allowedFileExtensions = new[]{".pdf"};
            Note noteobj = noteobj1.Note;
            NotesAttachment attachobj = noteobj1.NotesAttachment;
            noteobj.SellerID = (int)Session["UserID"];
            noteobj.Status = 4;

            if (noteobj.IsPaid == true && noteobj.Price == null)
            {
                ModelState.AddModelError("Price", "Please enter the price");
            }
            if (noteobj.IsPaid == true && noteobj.Price == 0)
            {
                ModelState.AddModelError("Price", "Price cannot be 0 for paid notes");
            }
            if (noteobj.IsPaid == false && noteobj.Price > 0)
            {
                ModelState.AddModelError("", "Price for free notes will be 0");
            }
            if (noteobj.IsPaid == true && noteobj.Preview == null)
            {
                ModelState.AddModelError("", "Please add a preview fo rpaid notes");
            }
           
            var noteExists = db.Notes.Find(noteobj.ID);
            if(noteExists==null)
            {
                //Conditions for image and file upload
                if (previewfile!=null && displaypic!=null)
                {
                    string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                    string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);
                    string imagefileName = Path.GetFileNameWithoutExtension(displaypic.FileName);
                    string imageextension = Path.GetExtension(displaypic.FileName);
                    if (allowedFileExtensions.Contains(_PreviewFileExtension))
                    {
                        if (allowedImageExtensions.Contains(imageextension))
                        {
                            imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                            noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                            imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                            displaypic.SaveAs(imagefileName);
                            _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                            noteobj.Preview = _path;
                            previewfile.SaveAs(_path);
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
                            return RedirectToAction("Login", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                    }
                }


                else if (previewfile ==null && displaypic!=null)
                {
                    string imagefileName = Path.GetFileNameWithoutExtension(displaypic.FileName);
                    string imageextension = Path.GetExtension(displaypic.FileName);
                    if (allowedImageExtensions.Contains(imageextension))
                    {
                        imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                        noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                        imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                        displaypic.SaveAs(imagefileName);

                        db.Notes.Add(noteobj);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        int NoteId = noteobj.ID;

                       
                        //Multiple file upload start
                        foreach (var attachment in attachmentfiles)
                        {
                            if (attachment.ContentLength > 0)
                            {
                                NotesAttachment attachobj1 = new NotesAttachment();
                                string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                                string _FileExtension = Path.GetExtension(attachment.FileName);
                                if (allowedFileExtensions.Contains(_FileExtension))
                                {
                                    _FileName = _FileName + DateTime.Now.ToString("yymmssff") + _FileExtension;
                                    string _attachmentpath = Path.Combine(Server.MapPath("~/UploadedFiles/"), _FileName);
                                    attachobj1.NoteID = NoteId;
                                    attachobj1.FileName = _FileName;
                                    attachobj1.FilePath = _attachmentpath;
                                    attachment.SaveAs(_attachmentpath);
                                    db.NotesAttachments.Add(attachobj1);
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
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                    }
                }

                else if (previewfile.ContentLength > 0 && displaypic.ContentLength < 0)
                {
                    string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                    string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);
                    if (allowedFileExtensions.Contains(_PreviewFileExtension))
                    {
                        _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                        noteobj.Preview = _path;
                        previewfile.SaveAs(_path);
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

                    }
                    else
                    {
                        ModelState.AddModelError("", "Please choose only Image file");
                    }
                }


                else
                {
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
                }
            }

            else
            {
                
            }
           
            
            
            
            
            
            
           

            //Conditions for image and file upload
            /*if (previewfile.ContentLength > 0 && noteobj.DisplayImageData != null)
            {
                string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);
                string imagefileName = Path.GetFileNameWithoutExtension(noteobj.DisplayImageData.FileName);
                string imageextension = Path.GetExtension(noteobj.DisplayImageData.FileName);
                if (allowedFileExtensions.Contains(_PreviewFileExtension))
                {
                    if (allowedImageExtensions.Contains(imageextension))
                    {
                        imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                        noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                        imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                        noteobj.DisplayImageData.SaveAs(imagefileName);
                        _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                        noteobj.Preview = _path;
                        previewfile.SaveAs(_path);
                        db.Notes.Add(noteobj);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        int NoteId = noteobj.ID;

                        //Multiple file upload start
                        foreach (var attachment in attachobj.attachmentfiles)
                        {
                            if (attachment.ContentLength > 0)
                            {
                                string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                                string _FileExtension = Path.GetExtension(previewfile.FileName);
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
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "");
                }
            }
            
            
            else if (previewfile.ContentLength < 0 && noteobj.DisplayImageData != null)
            {
                string imagefileName = Path.GetFileNameWithoutExtension(noteobj.DisplayImageData.FileName);
                string imageextension = Path.GetExtension(noteobj.DisplayImageData.FileName);
                if (allowedImageExtensions.Contains(imageextension))
                {
                    imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                    noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                    imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                    noteobj.DisplayImageData.SaveAs(imagefileName);
                   
                    db.Notes.Add(noteobj);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    int NoteId = noteobj.ID;

                    //Multiple file upload start
                    foreach (var attachment in attachobj.attachmentfiles)
                    {
                        if (attachment.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                            string _FileExtension = Path.GetExtension(previewfile.FileName);
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
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "");
                }
            }
            
            else if (previewfile.ContentLength > 0 && noteobj.DisplayImageData == null)
            {
                string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);
                if (allowedFileExtensions.Contains(_PreviewFileExtension))
                {
                    _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                    noteobj.Preview = _path;
                    previewfile.SaveAs(_path);
                    db.Notes.Add(noteobj);

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    int NoteId = noteobj.ID;



                    //Multiple file upload start
                    foreach (var attachment in attachobj.attachmentfiles)
                    {
                        if (attachment.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                            string _FileExtension = Path.GetExtension(previewfile.FileName);
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

                }
                else
                {
                    ModelState.AddModelError("", "Please choose only Image file");
                }
               }
           
            
            else
            {
                db.Notes.Add(noteobj);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                int NoteId = noteobj.ID;

                //Multiple file upload start
                foreach (var attachment in attachobj.attachmentfiles)
                {
                    if (attachment.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                        string _FileExtension = Path.GetExtension(previewfile.FileName);
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
            } */

           /* if (previewfile.ContentLength > 0)
            {
                string _PreviewFileName = Path.GetFileNameWithoutExtension(previewfile.FileName);
                string _PreviewFileExtension = Path.GetExtension(previewfile.FileName);
                if (allowedFileExtensions.Contains(_PreviewFileExtension)) 
                {
                    if(noteobj.DisplayImageData != null)
                    {
                        string imagefileName = Path.GetFileNameWithoutExtension(noteobj.DisplayImageData.FileName);
                        string imageextension = Path.GetExtension(noteobj.DisplayImageData.FileName);
                        if (allowedImageExtensions.Contains(imageextension))
                        {
                            imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                            noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                            imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                            noteobj.DisplayImageData.SaveAs(imagefileName);
                            _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                            noteobj.Preview = _path;
                            previewfile.SaveAs(_path);
                            db.Notes.Add(noteobj);
                           
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            int NoteId = noteobj.ID;
                          
                           

                            //Multiple file upload start
                            foreach (var attachment in attachobj.attachmentfiles)
                            {
                                if (attachment.ContentLength > 0)
                                {
                                    string _FileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                                    string _FileExtension = Path.GetExtension(previewfile.FileName);
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
                        }
                        else
                        {
                            ModelState.AddModelError("DisplayPic", "Please choose only Image file");
                        }
                        
                    }
                    else
                    {
                        _PreviewFileName = _PreviewFileName + DateTime.Now.ToString("yymmssff") + _PreviewFileExtension;
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _PreviewFileName);
                        noteobj.Preview = _path;
                        previewfile.SaveAs(_path);
                        db.Notes.Add(noteobj);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        int NoteId = noteobj.ID;
                       
                        //Multiple file upload start
                        foreach(var attachment in attachobj.attachmentfiles)
                        {
                            if(attachment.ContentLength>0)
                            {
                                string _FileName= Path.GetFileNameWithoutExtension(attachment.FileName);
                                string _FileExtension = Path.GetExtension(previewfile.FileName);
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
                        
                        return RedirectToAction("Login", "Home");
                    }
                   
                }
                else
                {
                    ModelState.AddModelError("DisplayPic", "Only pdfs are allowed");
                }
            }*/




            

            /*      if (noteobj.DisplayImageData != null) 
            { 
                string imagefileName = Path.GetFileNameWithoutExtension(noteobj.DisplayImageData.FileName);
                string imageextension = Path.GetExtension(noteobj.DisplayImageData.FileName);
                if (allowedImageExtensions.Contains(imageextension))
                {
                    imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
                    noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
                    imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
                    noteobj.DisplayImageData.SaveAs(imagefileName);
                }
                else
                {
                    ModelState.AddModelError("DisplayPic", "Please choose only Image file");
                }
            }*/
           
            noteobj.CategoryCollection = db.Categories.ToList<Category>();
            noteobj.CountryCollection = db.Countries.ToList<Country>();
            noteobj.TypeCollection = db.Types.ToList<Type>();
            return View(noteobj);
            
           
        }

       [HttpPost]
        public JsonResult Delete(int id)
        {
            NotesAttachment file = db.NotesAttachments.Find(id);
            db.NotesAttachments.Remove(file);
            db.SaveChanges();
            return Json(new { Result = "Done" });
        }
        public FileResult DownloadFile(String p, String d)
        {
            return File(p, System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        public ActionResult SearchNotes(int? page)
        {


            
            return View(db.Notes.ToList().ToPagedList(page??1,6));
        }

      
        public ActionResult BuyerRequest(int? page)
        {
            return View(db.Downloads.Where(model=>model.DownloadAllowed.Equals(false)).ToList().ToPagedList(page??1,3));
        }

        [HttpGet]public ActionResult NotesDetails(int id)
        {
           
           
            return View(db.Notes.Single(model => model.ID == id));
        }

        public FileResult DownloadNote(int bookid)
        {
            Note noteobj = db.Notes.Find(bookid);
            int count= db.NotesAttachments.Where(model => model.NoteID.Equals(bookid)).Count(); 
            Download entry = new Download();
            entry.NoteID = noteobj.ID;
            entry.Seller = noteobj.SellerID;
            entry.Downloader = (int)Session["UserID"];
            entry.DownloadAllowed = true;
            entry.IsAttachmentDownloaded = true;
           
                NotesAttachment obj = db.NotesAttachments.Where(model => model.NoteID.Equals(bookid)).FirstOrDefault();
                byte[] fileBytes = System.IO.File.ReadAllBytes(Url.Content(obj.FilePath));
                string filename = obj.FileName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

           
            
           
        }

        public ActionResult Dashboard()
        {
            int id = (int)Session["UserID"];

            UserProfile img = new UserProfile();
            img = db.UserProfiles.Where(model => model.UserID.Equals(id)).FirstOrDefault();
          
          //  var viewmodel = new MyViewModel();
           // viewmodel.count = db.Downloads.Where(model => model.Seller.Equals(id) && model.IsAttachmentDownloaded.Equals(true)).Count();
            //viewmodel.sum =(decimal)db.Downloads.Where(model => model.Seller.Equals(id) && model.IsAttachmentDownloaded.Equals(true) && model.IsPaid.Equals(true)).Select(model => model.Price).Sum();
            //viewmodel.ListA = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status != 4).ToList();
            //viewmodel.ListB = db.Notes.Where(model => model.SellerID.Equals(id) && model.Status.Equals(6)).ToList();
            return View(img);
        }
        public class MyViewModel
        {
            public List<Note> ListA { get; set; }
            public List<Note> ListB { get; set; }

            public int count { get; set; }

            public decimal sum { get; set; }
        }
        /*  public List<Country> GetCountry()
          {
              List<Country> countries = db.Countries.ToList();
              return countries;
          }



          public List<Type> GetTypelist()
          {
              List<Type> types = db.Types.ToList();
              return types;
          }*/


        public ActionResult MyDownloads()
        {
            if (Session["UserID"]!=null)
            {
                return View(db.Downloads.Where(model=>model.Downloader.Equals((int)Session["UserID"]) && (model.IsAttachmentDownloaded.Equals(true) || model.DownloadAllowed.Equals(true))).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult MySoldNotes()
        {
            if(Session["UserID"] != null)
            {
                return View(db.Downloads.Where(model=>model.Seller.Equals((int)Session["UserID"])&& model.DownloadAllowed.Equals(true)).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult MyRejectedNotes()
        {
            if (Session["UserID"] != null)
            {
                return View(db.Notes.Where(model=>model.SellerID.Equals((int)Session["UserID"])&&model.Status.Equals(6)).ToList());
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
           // if (Session["UserID"] != null)
            //{
               
                return View();
            //}
            //else
            //{
             //   return RedirectToAction("Login", "Home");
            //}

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

    }







}

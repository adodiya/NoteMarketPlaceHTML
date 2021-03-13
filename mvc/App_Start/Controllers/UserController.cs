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

       
       [HttpGet] public ActionResult MyProfile()
        {
            if (Session["UserID"] != null)
            {
                

                int id = (int)Session["UserID"];
                User_UserProfile mymodel = new User_UserProfile();
                mymodel.UserProfile = db.UserProfiles.Find(id);
                mymodel.User = db.Users.Find(id);
                var getlist = db.Countries.ToList();
                SelectList list = new SelectList(getlist, "CountryCode", "Name");
                ViewBag.PhonenoCountryCode = list;
                return View(mymodel);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }

        }

        [HttpPost] public ActionResult MyProfile(User obj1, UserProfile obj2)
        {
            if(ModelState.IsValid)
            {
                db.UserProfiles.Add(obj2);
               
            }
            return View();
        }

        [HttpGet] public ActionResult AddNote(int id)
        {
            if (Session["UserID"] != null)
            {
                Note noteobj = new Note();
                noteobj.CategoryCollection = db.Categories.ToList<Category>();
                noteobj.CountryCollection = db.Countries.ToList<Country>();
                noteobj.TypeCollection = db.Types.ToList<Type>();
               //  var Countrylist = db.Countries.ToList();


//                 var Typelist = db.Types.ToList();

                 noteobj.SellerID = id;
  //               noteobj.Categorylist = new SelectList(Categorylist, "CategoryID", "Name");
    //             noteobj.Countrylist = new SelectList(Countrylist, "CountryID", "Name");
     //            noteobj.Typelist = new SelectList(Typelist, "typeID", "Name");*/

                noteobj.SellerID = id;
                //List<Category> categories = db.Categories.ToList();
                //Category category = new Category();
                //ViewBag.Category = new SelectList(db.Categories, "CategoryID", "Name");

                return View(noteobj);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
               
            
        }

        [HttpPost]
        public ActionResult AddNote(Note noteobj)
        {
            //  int catid = noteobj.CategoryID;
            // noteobj.SellerID = (int)Session["UserID"];
            
            noteobj.Status = 4;
            string imagefileName = Path.GetFileNameWithoutExtension(noteobj.DisplayImageData.FileName);
            string imageextension = Path.GetExtension(noteobj.DisplayImageData.FileName);
            imagefileName = imagefileName + DateTime.Now.ToString("yymmssff") + imageextension;
            noteobj.DisplayPic = "~/Uploaded_Images/" + imagefileName;
            imagefileName = Path.Combine(Server.MapPath("~/Uploaded_Images/"), imagefileName);
           
            //noteobj.DisplayPic.SaveAs(fileName);
            noteobj.DisplayImageData.SaveAs(imagefileName);

            string fileName = Path.GetFileNameWithoutExtension(noteobj.FileData.FileName);
            string extension = Path.GetExtension(noteobj.FileData.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
            noteobj.Preview = "~/UploadedFiles/" + imagefileName;
            imagefileName = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);

            //noteobj.DisplayPic.SaveAs(fileName);
            noteobj.DisplayImageData.SaveAs(imagefileName);
            
            db.Notes.Add(noteobj);
            db.Configuration.ValidateOnSaveEnabled = false;
                
                db.SaveChanges();
            ModelState.Clear();
            ViewBag.Message = "Done";
            return View();
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

        public ActionResult DownloadNote(int bookid)
        {
            return View();
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

    }

  
    

   
}

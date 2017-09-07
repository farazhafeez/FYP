using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Data.Entity;
using System.IO;

namespace FYP.Controllers
{
    public class TeacherController : Controller
    {
        List<int> randomNumber = new List<int>();
        List<Question> question_mcq = new List<Question>();
        List<Question> question_tf = new List<Question>();
        Random rand = new Random();
        //
        // GET: /Teacher/
        FYP_DB_Entities obj = new FYP_DB_Entities();
        public ActionResult Index()
        {
            if(Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Subjects()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var user = (string)Session["User_Id"];
                    User u = obj.Users.First(a => a.User_Id == user);
                    List<Subject> subject = u.Subjects.ToList();
                    return View(subject);
                }
                catch
                {
                    return RedirectToAction("Index", "Teacher");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public ActionResult Manage_MCQ(string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Subject sub = obj.Subjects.First(a => a.Subject_Id.Equals(Subject_Id));
                ViewBag.Subject = sub.Subject_Name;
                ViewBag.subject_id = Subject_Id;
                var mcqs = sub.Questions.Where(a => a.Type.Equals("MCQ-E") || a.Type.Equals("MCQ-M") || a.Type.Equals("MCQ-D"));
                return View(mcqs);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult View_MCQ_Detail(int Question_Id,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == Question_Id).ToList();
                Subject sub = obj.Subjects.First(a => a.Subject_Id == Subject_Id);
                ViewBag.subject = sub.Subject_Name;
                ViewBag.subject_id = Subject_Id;
                return View(qtn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Del_MCQ(int Question_Id, string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Question question = obj.Questions.First(a => a.Question_Id == Question_Id);
                obj.Questions.Attach(question);
                obj.Questions.Remove(question);
                obj.SaveChanges();
                return RedirectToAction("Manage_MCQ", "Teacher", new { @Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Edit_MCQ(int Question_Id, string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == Question_Id).ToList();
                ViewBag.subject_id = Subject_Id;
                return View(qtn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Edit_MCQ(FormCollection fc , int Question_Id , string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                obj.Options.Where(a => a.Question_Id == Question_Id).ToList().ForEach(a => obj.Options.Remove(a));
                obj.SaveChanges();

                Question qtn = new Question();
                Option optn = new Option();
                qtn = obj.Questions.First(a => a.Question_Id == Question_Id);
                qtn.Questions = fc["Questions"];
                qtn.Type = fc["Type"];
                obj.SaveChanges();

                int counter = 2;
                int count = fc.Count;
                for (int i = 1; i <= (count - counter); i++)
                {
                    qtn = obj.Questions.First(a => a.Question_Id == Question_Id);
                    optn.Question_Id = qtn.Question_Id;
                    optn.Options = fc["Option" + i];
                    if (fc["" + i] != null)
                    {
                        optn.Correct_Answer = "yes";
                        counter++;
                    }
                    else
                    {
                        optn.Correct_Answer = " ";
                    }

                    obj.Options.Add(optn);
                    obj.SaveChanges();
                }

                return RedirectToAction("Manage_MCQ", "Teacher", new { @Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }             
        }

        public ActionResult Add_MCQ(string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                ViewBag.subject_id = Subject_Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Add_MCQ(FormCollection fc,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Option opt = new Option();
                Question qtn = new Question();
                int counter = 2;
                string question1 = fc["Questions"];
                string type1 = fc["Type"];
                qtn.Questions = question1;
                qtn.Subject_Id = Subject_Id;
                qtn.Type = type1;
                obj.Questions.Add(qtn);
                obj.SaveChanges();
                int count = fc.Count;
                for (int i = 1; i <= (count - counter); i++)
                {
                    qtn = obj.Questions.OrderByDescending(a => a.Question_Id).First(a => a.Questions.Equals(question1));
                    opt.Options = fc["Option" + i];
                    opt.Question_Id = qtn.Question_Id;
                    if (fc["check" + i] != null)
                    {
                        opt.Correct_Answer = "yes";
                        counter++;
                    }
                    else
                    {
                        opt.Correct_Answer = " ";
                    }


                    obj.Options.Add(opt);
                    obj.SaveChanges();
                }
                return RedirectToAction("Manage_MCQ", "Teacher", new { @Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Manage_TrueFalse(string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Subject sub = obj.Subjects.First(a => a.Subject_Id.Equals(Subject_Id));
                ViewBag.Subject = sub.Subject_Name;
                ViewBag.subject_id = Subject_Id;
                var TrueFalseQuestions = sub.Questions.Where(a => a.Type.Equals("True/False-E") || a.Type.Equals("True/False-M") || a.Type.Equals("True/False-D"));
                return View(TrueFalseQuestions);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult View_TrueFalseDetail(int Question_Id,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == Question_Id).ToList();
                Subject sub = obj.Subjects.First(a => a.Subject_Id == Subject_Id);
                ViewBag.subject = sub.Subject_Name;
                ViewBag.subject_id = Subject_Id;
                return View(qtn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Del_TrueFalse(int Question_Id,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Question qtn = obj.Questions.First(a => a.Question_Id.Equals(Question_Id));
                obj.Questions.Attach(qtn);
                obj.Questions.Remove(qtn);
                obj.SaveChanges();
                return RedirectToAction("Manage_TrueFalse", "Teacher", new { @Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Edit_TrueFalse(int Question_Id,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id.Equals(Question_Id)).ToList();
                ViewBag.subject_id = Subject_Id;
                return View(qtn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Edit_TrueFalse(FormCollection fc,int Question_Id,string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Question qtn = new Question();
                Option optn = new Option();
                qtn = obj.Questions.First(a => a.Question_Id.Equals(Question_Id));
                qtn.Questions = fc["Questions"];
                qtn.Type = fc["Type"];
                obj.SaveChanges();
                optn = obj.Options.First(a => a.Question_Id == Question_Id);
                optn.Correct_Answer = fc["radio1"];
                obj.SaveChanges();
                return RedirectToAction("Manage_TrueFalse", "Teacher", new { @Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Add_TrueFalse(string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                ViewBag.subject_id = Subject_Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Add_TrueFalse(FormCollection fc, string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Question q = new Question();
                Option o = new Option();
                string question = fc["questions"];
                string option = fc["radio1"];
                string type = fc["Type"];
                q.Questions = question;
                q.Subject_Id = Subject_Id;
                q.Type = type;
                obj.Questions.Add(q);
                obj.SaveChanges();
                q = obj.Questions.OrderByDescending(a => a.Question_Id).First(a => a.Questions.Equals(question));
                o.Correct_Answer = option;
                o.Question_Id = q.Question_Id;
                obj.Options.Add(o);
                obj.SaveChanges();
                return RedirectToAction("Manage_TrueFalse", "Teacher", new { Subject_Id = Subject_Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult TeacherProfile()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                string imageurl = "";
                var user1 = (string)Session["User_Id"];
                User user = obj.Users.First(a => a.User_Id == user1);
                string imageurlforJPG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".jpg"));
                string imageurlforPNG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".png"));
                if (System.IO.File.Exists(imageurlforJPG))
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".jpg");
                }
                else if (System.IO.File.Exists(imageurlforPNG))
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".png");
                }
                else
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName("PlaceHolder") + System.IO.Path.GetExtension(".jpg");
                }
                ViewBag.imageURL = imageurl;
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult EditProfile(string Success_Message)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                string imageurl="";
                var user1 = (string)Session["User_Id"];
                User user = obj.Users.First(a => a.User_Id == user1);
                string imageurlforJPG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".jpg"));
                string imageurlforPNG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".png"));
                if(System.IO.File.Exists(imageurlforJPG))
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".jpg");
                }
                else if(System.IO.File.Exists(imageurlforPNG))
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".png");
                }
                else
                {
                    imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName("PlaceHolder") + System.IO.Path.GetExtension(".jpg");
                }
                ViewBag.ImageURL = imageurl;
                ViewBag.Message = Success_Message;
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult EditProfile(FormCollection fc)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var user1 = (string)Session["User_Id"];
                User user = obj.Users.First(a => a.User_Id == user1);
                var Contact_No = fc["contact_no"];
                var Password = fc["password"];
                var Gender = fc["gender"];
                user.Contact_No = Contact_No;
                user.Password = Password;
                user.Gender = Gender;
                //todo  
                obj.SaveChanges();
                return RedirectToAction("EditProfile","Teacher",new { Success_Message = "Edit Profile Successfully"});
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public JsonResult Password_IsAvailable(string OldPassword)
        {
            var user1 = (string)Session["User_Id"];
            return Json(obj.Users.Any(a => a.User_Id == user1 && a.Password == OldPassword));
        }
        
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string user = (string)Session["User_Id"];
                    string imageurlforJPG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user) + Path.GetExtension(".jpg"));
                    string imageurlforPNG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user) + Path.GetExtension(".png"));
                    if (System.IO.File.Exists(imageurlforJPG)) 
                    {
                        System.IO.File.Delete(imageurlforJPG);
                    }
                    else if(System.IO.File.Exists(imageurlforPNG))
                    {
                        System.IO.File.Delete(imageurlforPNG);
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Pictures"), Path.GetFileName(user) + Path.GetExtension(file.FileName));
                    file.SaveAs(path);
                    return RedirectToAction("EditProfile", "Teacher");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("User_Id");
            Session.Remove("User_Password");
            return RedirectToAction("Index", "Home");
        }

    }
}

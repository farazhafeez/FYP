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
    public class StudentController : Controller
    {
        public static List<int> rand_question = new List<int>();
        public static List<int> marks = new List<int>();
        Random random = new Random();
        FYP_DB_Entities obj = new FYP_DB_Entities();
        //
        // GET: /Student/

        public ActionResult Index(int? id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                ViewBag.message = id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }            
        }
        [HttpPost]
        public JsonResult Index()
        {
            int exam_Id = 0;
            string subject_Name="";
            TimeSpan timeduration1 = TimeSpan.Zero;
            var user = (string)Session["User_Id"];
            var enrolled = obj.Enrolleds.Where(a => a.User_Id == user);
            if (Session["ExamSession"] == null)
            {
                foreach (var i in enrolled)
                {
                    Schedule schedule = i.Exam.Schedules.First();
                    if (timeduration1 == TimeSpan.Zero)
                    {
                        if (DateTime.Now > schedule.Time_From && DateTime.Now < schedule.Time_To)
                        {
                            var result = new { Exam_Id = i.Exam_Id, day = 0, hour = 0, minute = 0, second = 0 };
                            return Json(result);
                        }
                        else if (DateTime.Now < schedule.Time_From)
                        {
                            timeduration1 = Convert.ToDateTime(schedule.Time_From.ToString()) - DateTime.Now;
                            exam_Id = Convert.ToInt32(i.Exam_Id);
                            subject_Name = i.Exam.Subject.Subject_Name;
                        }
                    }
                    else
                    {
                        if (DateTime.Now > schedule.Time_From && DateTime.Now < schedule.Time_To)
                        {
                            var result = new { Exam_Id = i.Exam_Id, day = 0, hour = 0, minute = 0, second = 0 };
                            return Json(result);
                        }
                        else if (DateTime.Now < schedule.Time_From)
                        {
                            TimeSpan timeduration2 = Convert.ToDateTime(schedule.Time_From.ToString()) - DateTime.Now;
                            if (timeduration1 > timeduration2)
                            {
                                timeduration1 = timeduration2;
                                exam_Id = Convert.ToInt32(i.Exam_Id);
                                subject_Name = i.Exam.Subject.Subject_Name;
                            }
                        }
                    }
                }
            }
            else
            {
                int exam_id = (int)Session["ExamSession"];
                Exam exam = obj.Exams.First(a => a.Exam_Id == exam_id);
                foreach (var i in enrolled)
                {
                    if (i.Exam_Id == exam.Exam_Id)
                    {
                        Schedule schedule = i.Exam.Schedules.First();
                        if (DateTime.Now > schedule.Time_To)
                        {
                            Session.Remove("ExamSession");
                        }
                    }
                    else
                    {
                        Schedule schedule = i.Exam.Schedules.First();
                        if (timeduration1 == TimeSpan.Zero)
                        {
                            if (DateTime.Now > schedule.Time_From && DateTime.Now < schedule.Time_To)
                            {
                                var result = new { Exam_Id = i.Exam_Id, day = 0, hour = 0, minute = 0, second = 0 };
                                return Json(result);
                            }
                            else if (DateTime.Now < schedule.Time_From)
                            {
                                timeduration1 = Convert.ToDateTime(schedule.Time_From.ToString()) - DateTime.Now;
                                exam_Id = Convert.ToInt32(i.Exam_Id);
                                subject_Name = i.Exam.Subject.Subject_Name;
                            }
                        }
                        else
                        {
                            if (DateTime.Now > schedule.Time_From && DateTime.Now < schedule.Time_To)
                            {
                                var result = new { Exam_Id = i.Exam_Id, day = 0, hour = 0, minute = 0, second = 0 };
                                return Json(result);
                            }
                            else if (DateTime.Now < schedule.Time_From)
                            {
                                TimeSpan timeduration2 = Convert.ToDateTime(schedule.Time_From.ToString()) - DateTime.Now;
                                if (timeduration1 > timeduration2)
                                {
                                    timeduration1 = timeduration2;
                                    exam_Id = Convert.ToInt32(i.Exam_Id);
                                    subject_Name = i.Exam.Subject.Subject_Name;
                                }
                            }
                        }
                    }
                }
            }
            if(timeduration1 != TimeSpan.Zero)
            {
                var result = new { Exam_Id = exam_Id, Subject_Name = subject_Name, day = timeduration1.Days, hour = timeduration1.Hours, minute = timeduration1.Minutes, second = timeduration1.Seconds };
                return Json(result);
            }
            else
            {
                var result = new { Exam_Id = 0, day = 0, hour = 0, minute = 0, second = 0 };
                return Json(result);
            }            
        }
        public ActionResult Instructions(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                ViewBag.Exam_Id = Exam_Id;
                var questions = obj.Papers.Where(a => a.Exam_Id == Exam_Id);
                Schedule schedule = obj.Schedules.First(a => a.Exam_Id == Exam_Id);
                TimeSpan timeduration = Convert.ToDateTime(schedule.Time_To.ToString()) - Convert.ToDateTime(schedule.Time_From.ToString());
                ViewBag.timeduration = timeduration.TotalMinutes + " Minutes";
                ViewBag.questions = questions.Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            //Session["Marks"] = 0;
            
        }
        public ActionResult ExamSchedule()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Exam> exams = new List<Exam>();
                var user = (string)Session["User_Id"];
                var enrolled = obj.Enrolleds.Where(a => a.User_Id == user);
                foreach (var i in enrolled)
                {
                    Exam exam = obj.Exams.Include(a => a.Schedules).First(b => b.Exam_Id == i.Exam_Id);
                    exams.Add(exam);
                }
                ViewBag.Schedule = exams;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Start_Exam(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                marks.Clear();
                rand_question.Clear();
                //Create session for exam
                Session["ExamSession"] = Exam_Id;

                var total_questions = obj.Papers.Where(a => a.Exam_Id == Exam_Id);
                var count_total_questions = total_questions.Count();
                int count_questions = rand_question.Count();
                int number;
                do
                {
                    number = random.Next(1, (count_total_questions + 1));
                } while (rand_question.Contains(number));
                rand_question.Add(number);
                var question = obj.Papers.OrderBy(a => a.Question_Id).Where(b => b.Exam_Id == Exam_Id).Skip(number - 1).Take(1).ToList();
                foreach (var s in question)
                {
                    var pick_question = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == s.Question_Id).ToList();
                    ViewBag.question1 = pick_question;
                }
                ViewBag.Exam_Id = Exam_Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            //if(Session["Exam_Session"] == null)
            //{
            //    marks.Clear();
            //    rand_question.Clear();
            //}
        }
        [HttpPost]
        public JsonResult Start_Exam(string selected_answer , int Exam_Id, int Question_ID)
        {
            var correct_answer = obj.Options.Where(a => a.Question_Id == Question_ID);
            var Question_marks = obj.Papers.First(a => a.Question_Id == Question_ID && a.Exam_Id == Exam_Id);
            foreach (var i in correct_answer)
            {
                if ((i.Options == selected_answer && i.Correct_Answer == "yes") || (i.Correct_Answer == selected_answer))
                {
                    marks.Add(1);
                    //Session["Marks"] = Convert.ToInt32(Session["Marks"]) + Question_marks.Question_Marks;
                }
                //else
                //{
                //    Session["Marks"] = Convert.ToInt32(Session["Marks"]) + 0;
                //}
            }
            var total_questions = obj.Papers.Where(a => a.Exam_Id == Exam_Id);
            var count_total_questions = total_questions.Count();
            int count_questions = rand_question.Count();

            if (count_questions < count_total_questions)
            {
                int number;
                do
                {
                    number = random.Next(1, (count_total_questions + 1));
                } while (rand_question.Contains(number));
                rand_question.Add(number);
                int pick = number - 1;
                var question = obj.Papers.OrderBy(a => a.Question_Id).Where(b => b.Exam_Id == Exam_Id).Skip(pick).Take(1).ToList();
                foreach (var s in question)
                {
                    var pick_question = obj.Options.Where(a => a.Question_Id == s.Question_Id).Select(b => new { b.Options, b.Question.Questions,b.Question_Id });
                    return Json(pick_question);
                }
            }
            else
            {
                return Json("");
            }
            return null;
        }
        public ActionResult Count_Marks(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                int flag = 0;
                int a = 0;
                foreach (var i in marks)
                {
                    a = a + i;
                }
                Result new_result = new Result();
                try
                {
                    Result result = obj.Results.First(c => c.Exam_Id == Exam_Id);
                    if (result.Mid_Marks != null && result.Final_Marks == null)
                    {
                        result.Final_Marks = a;
                        obj.SaveChanges();
                        //Session.Remove("Marks");
                    }
                }
                catch
                {
                    var user = (string)Session["User_Id"];
                    new_result.User_Id = user;
                    new_result.Exam_Id = Exam_Id;
                    new_result.Mid_Marks = a;
                    obj.Results.Add(new_result);
                    obj.SaveChanges();
                    //Session.Remove("Marks");
                    flag++;
                }
                return RedirectToAction("Index", "Student");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public JsonResult ExamTimeDuration(int Exam_Id)
        {
            Schedule schedule = obj.Schedules.First(a => a.Exam_Id == Exam_Id);
            TimeSpan timeduration = Convert.ToDateTime(schedule.Time_To.ToString()) - DateTime.Now;
            var result = new { Hours = timeduration.Hours, Minutes = timeduration.Minutes, Seconds = timeduration.Seconds };
            return Json(result);
        }
        
        public ActionResult StudentProfile()
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
                return RedirectToAction("EditProfile", "Student",new { Success_Message = "Edit Profile Successfully"});
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
                    else if (System.IO.File.Exists(imageurlforPNG))
                    {
                        System.IO.File.Delete(imageurlforPNG);
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Pictures"), Path.GetFileName(user) + Path.GetExtension(file.FileName));
                    file.SaveAs(path);
                    return RedirectToAction("EditProfile", "Student");
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

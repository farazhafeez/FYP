using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Data.Entity;
using System.IO;
using System.Web.UI;

namespace FYP.Controllers
{
    public class ExamControllerController : Controller
    {
        //
        // GET: /ExamController/

        FYP_DB_Entities obj = new FYP_DB_Entities();
        #region ExamController HomePage
        public ActionResult HomePage()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region ManageExam
        public ActionResult ManageExam()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var exams = obj.Exams.Include(a => a.Schedules).Where(x => x.Status != "Mid_Exam" && x.Status != "Final_Exam");
                ViewBag.midexamerrors = TempData["midexamerrors"];
                ViewBag.finalexamerrors = TempData["finalexamerrors"];
                return View(exams);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region CreateExam
        public ActionResult CreateExam()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View(obj.Departments.ToList());
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        [HttpPost]
        public ActionResult CreateExam(string[] subjects , string examTerm , string examSession)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Subject subject = new Subject();
                Exam exam = new Exam();
                Schedule schedule = new Schedule();
                Batch batch = new Batch();
                int marks = 0;
                List<string> examError = new List<string>();

                var students = obj.Users.Where(x => x.Role.Equals("Student") && x.Status.Equals("Active"));

                List<Enrolled> enrollmentList = new List<Enrolled>();

                var conductedExams = obj.Exams.Where(x => x.Status.Equals("Mid_Exam") || x.Status.Equals("Final_Exam"));

                //getting all enrollment rows that have exams conducted
                foreach (var i in conductedExams)
                {
                    //var allEnrolledsForConductedExams = obj.Enrolleds.Where(x => x.Exam_Id == i.Exam_Id);
                    //var allDropOutForConductedExams = obj.Drop_Out.Where(x => x.Exam_Id == i.Exam_Id);
                    var allSchedulesForConductedExams = obj.Schedules.Where(x => x.Exam_Id == i.Exam_Id);

                    foreach (var j in allSchedulesForConductedExams)
                    {
                        if (DateTime.Now > j.Time_To)
                        {
                            var allEnrolleds = obj.Enrolleds.Where(x => x.Exam_Id == j.Exam_Id);
                            foreach (var k in allEnrolleds)
                            {
                                obj.Enrolleds.Remove(k);
                            }
                            var allDropOuts = obj.Drop_Out.Where(x => x.Exam_Id == j.Exam_Id);
                            foreach (var l in allDropOuts)
                            {
                                obj.Drop_Out.Remove(l);
                            }
                            obj.Schedules.Remove(j);
                            if (i.Total_Marks == 30)
                                i.Status = "Mid_Conducted";
                            else if (i.Total_Marks == 50)
                                i.Status = "Final_Conducted";
                        }
                    }
                }

                obj.SaveChanges();


                if (examTerm.Equals("Mid"))
                {
                    marks = 30;

                    foreach (var i in subjects)
                    {
                        try
                        {
                            var check = obj.Exams.First(x => x.Subject_Id == i && x.Exam_Session == examSession && x.Total_Marks == 30);
                            examError.Add(check.Subject_Id);
                        }
                        catch
                        {
                            subject = obj.Subjects.First(x => x.Subject_Id.Equals(i));
                            batch = obj.Batches.First(x => x.Batch_Id.Equals(subject.Batch_Id));

                            exam.Subject_Id = subject.Subject_Id;
                            exam.Batch_Id = subject.Batch_Id;
                            exam.Department_Id = batch.Department_Id;
                            exam.Total_Marks = marks;
                            exam.Exam_Session = examSession;
                            exam.Status = "Enable";
                            schedule.Exam_Id = exam.Exam_Id;
                            obj.Exams.Add(exam);
                            obj.Schedules.Add(schedule);
                            obj.SaveChanges();

                            foreach (var j in students)
                            {
                                Enrolled enrollment = new Enrolled();
                                if (j.Batch_Id == batch.Batch_Id && subject.Section.Contains(j.Section))
                                {
                                    enrollment.User_Id = j.User_Id;
                                    enrollment.Exam_Id = exam.Exam_Id;

                                    enrollmentList.Add(enrollment);
                                }
                            }

                        }
                    }
                    TempData["midexamerrors"] = examError;
                }

                else if (examTerm.Equals("Final"))
                {
                    marks = 50;
                    var midExams = obj.Exams.Where(x => x.Exam_Session.Equals(examSession) && x.Status.Equals("Mid_Conducted"));
                    foreach (var i in subjects)
                    {
                        try
                        {
                            var mid = midExams.First(x => x.Subject_Id == i);
                            mid.Total_Marks = marks;
                            mid.Status = "Enable";

                            schedule.Exam_Id = mid.Exam_Id;
                            obj.Schedules.Add(schedule);

                            foreach (var j in students)
                            {
                                Enrolled enrollment = new Enrolled();
                                if (j.Batch_Id == mid.Batch_Id && mid.Subject.Section.Contains(j.Section))
                                {
                                    enrollment.User_Id = j.User_Id;
                                    enrollment.Exam_Id = mid.Exam_Id;

                                    enrollmentList.Add(enrollment);
                                }
                            }
                        }
                        catch
                        {
                            examError.Add(i);
                        }
                    }
                    TempData["finalexamerrors"] = examError;
                    obj.SaveChanges();
                }


                foreach (var i in enrollmentList)
                {
                    obj.Enrolleds.Add(i);
                    obj.SaveChanges();
                }

                return RedirectToAction("ManageExam", "ExamController");
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region Enrolled students
        public ActionResult Enrolleds(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var enrolleds = obj.Enrolleds.Where(x => x.Exam_Id == Exam_Id);
                return View(enrolleds);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region Dropout Students
        public ActionResult DropOuts(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var dropOuts = obj.Drop_Out.Where(x => x.Exam_Id == Exam_Id);
                return View(dropOuts);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region ExamSchedule
        public ActionResult ExamSchedule(int Exam_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var schedule = obj.Schedules.Where(x => x.Exam_Id == Exam_Id);
                    Schedule s = new Schedule();
                    int a = 0;
                    List<string> rooms = new List<string>();
                    foreach (var i in schedule)
                    {
                        if (a == 0)
                        {
                            s.Exam_Id = i.Exam_Id;
                            s.Time_From = i.Time_From;
                            s.Time_To = i.Time_To;
                            rooms.Add(i.Room_Id);
                            a++;
                        }
                        else
                        {
                            rooms.Add(i.Room_Id);
                        }
                    }
                    ViewBag.rooms = rooms;
                    return View(s);
                }
                catch
                {
                    Schedule schedule = new Schedule();
                    return View(schedule);
                }
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        [HttpPost]
        public ActionResult ExamSchedule(Schedule schedule, string[] rooms)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var schedules = obj.Schedules.Where(x => x.Exam_Id == schedule.Exam_Id);
                foreach (var i in schedules)
                {
                    obj.Schedules.Remove(i);
                }
                obj.SaveChanges();
                if (rooms != null)
                {
                    foreach (var i in rooms)
                    {
                        Schedule s = new Schedule();
                        s.Exam_Id = schedule.Exam_Id;
                        s.Time_From = schedule.Time_From;
                        s.Time_To = schedule.Time_To;
                        s.Room_Id = i;
                        obj.Schedules.Add(s);
                    }
                }
                else
                {
                    Schedule s = new Schedule();
                    s.Exam_Id = schedule.Exam_Id;
                    s.Time_From = schedule.Time_From;
                    s.Time_To = schedule.Time_To;
                    s.Room_Id = schedule.Room_Id;
                    obj.Schedules.Add(s);
                }
                obj.SaveChanges();
                return RedirectToAction("ManageExam", "ExamController");
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region LocalAPIs

        [HttpPost]
        public JsonResult IsUser_IdAvailable(string User_Id)
        {
            return Json(!obj.Users.Any(a => a.User_Id.Equals(User_Id)));
        }
        [HttpPost]
        public JsonResult Drop(int Enrolled_Id)
        {
            try
            {
                var e = obj.Enrolleds.Find(Enrolled_Id);
                Drop_Out d = new Drop_Out();
                d.Exam_Id = e.Exam_Id;
                d.User_Id = e.User_Id;

                obj.Drop_Out.Add(d);
                obj.Enrolleds.Remove(e);
                obj.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult Enroll(int Drop_Out_Id)
        {
            try
            {
                var d = obj.Drop_Out.Find(Drop_Out_Id);
                Enrolled e = new Enrolled();
                e.Exam_Id = d.Exam_Id;
                e.User_Id = d.User_Id;

                obj.Enrolleds.Add(e);
                obj.Drop_Out.Remove(d);
                obj.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForDepartment()
        {
            //IEnumerable<Department> departmentList = Enumerable.Empty<Department>();

            try
            {
                var departmentList = obj.Departments.Select(x => x.Department_Id);
                return Json(departmentList);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForBatch(string[] departments)
        {
            //IEnumerable<Batch> batchList = Enumerable.Empty<Batch>();
            try
            {
                var b = departments.SelectMany(d => obj.Batches.Where(x => x.Department_Id == d && x.Status.Equals("Active")));
                var batchList = b.Select(x => x.Batch_Id);
                return Json(batchList);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForSubject(string[] batches)
        {

            try
            {
                var subjects = batches.SelectMany(d => obj.Subjects.Where(x => x.Batch_Id == d));
                var subjectList = subjects.Select(x => new { x.Subject_Id, x.Subject_Name, x.Section, x.Batch_Id });
                return Json(subjectList);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForExamSchedule()
        {
            try
            {
                var e = obj.Exams.Where(x => x.Status != "Conducted");
                var examList = e.Select(x => new { x.Exam_Id, x.Subject_Id, x.Subject.Subject_Name, x.Batch_Id });
                return Json(examList);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForRoom()
        {
            try
            {
                var rooms = obj.Rooms.ToList();
                var roomList = rooms.Select(x => new { x.Room_Id });
                return Json(roomList);
            }
            catch
            {
                return Json(null);
            }
        }
        #endregion
        #region ManageProfile
        public ActionResult ExamControllerProfile()
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
                ViewBag.Message = TempData["EditProfile"];
                return View(user);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        public ActionResult EditContactNumber()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var userId = Session["User_Id"].ToString();
                User user = obj.Users.First(a => a.User_Id == userId);
                return View(user);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        [HttpPost]
        public ActionResult EditContactNumber(string contact_no)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var userId = Session["User_Id"].ToString();
                var user = obj.Users.First(x => x.User_Id == userId);
                user.Contact_No = contact_no;

                obj.SaveChanges();

                TempData["EditProfile"] = "Contact number was changed successfully!";

                return RedirectToAction("ExamControllerProfile", "ExamController");
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        public ActionResult EditGender()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var userId = Session["User_Id"].ToString();
                User user = obj.Users.First(a => a.User_Id == userId);
                return View(user);
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        [HttpPost]
        public ActionResult EditGender(string gender)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var userId = Session["User_Id"].ToString();
                var user = obj.Users.First(x => x.User_Id == userId);
                user.Gender = gender;

                obj.SaveChanges();

                TempData["EditProfile"] = "Gender was changed successfully!";

                return RedirectToAction("ExamControllerProfile", "ExamController");
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
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

                    return RedirectToAction("ExamControllerProfile", "ExamController");
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

        public ActionResult ChangePassword()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string password)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var userId = Session["User_Id"].ToString();
                var user = obj.Users.First(x => x.User_Id == userId);
                user.Password = password;

                obj.SaveChanges();

                TempData["EditProfile"] = "Password was changed successfully !";

                return RedirectToAction("ExamControllerProfile", "ExamController");
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }

        #endregion
        #region Logout
        public ActionResult Logout()
        {
            Session.Remove("User_Id");
            Session.Remove("Password");
            return RedirectToAction("LoginPage", "Login");
        }
        #endregion
    }
}

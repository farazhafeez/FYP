using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using FYP.Models;
using System.IO;
using FYP.CacheModel;
using System.Web.UI;

namespace FYP.Controllers
{
    public class SuperUserController : Controller
    {
        //
        // GET: /SuperUser/

        FYP_DB_Entities obj = new FYP_DB_Entities();

        public ActionResult Index()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }





        #region ManageTeachers

        public ActionResult ManageTeacher()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var u = obj.Users.Where(a => a.Role.Equals("Teacher") && a.Status.Equals("Active"));
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddTeacher()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddTeacher(User user)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = new User();
                u = user;
                u.Status = "Active";
                u.Role = "Teacher";

                obj.Users.Add(u);
                obj.SaveChanges();
                return RedirectToAction("ManageTeacher", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditTeacher(string User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = obj.Users.Find(User_Id);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditTeacher(User teacher)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var teacherToUpdate = obj.Users.First(x => x.User_Id.Equals(teacher.User_Id));
                teacherToUpdate.First_Name = teacher.First_Name;
                teacherToUpdate.Last_Name = teacher.Last_Name;
                teacherToUpdate.Password = teacher.Password;
                teacherToUpdate.Contact_No = teacher.Contact_No;
                teacherToUpdate.Department = teacher.Department;
                obj.SaveChanges();

                return RedirectToAction("ManageTeacher", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageStudents

        public ActionResult ManageStudent()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var u = obj.Users.Where(a => a.Role.Equals("Student") && a.Status.Equals("Active"));

                    return View(u);
                }
                catch
                {
                    return RedirectToAction("Index", "SuperUser");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddStudent()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddStudent(User user)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = new User();
                u = user;
                u.Status = "Active";
                u.Role = "Student";

                obj.Users.Add(u);
                obj.SaveChanges();
                return RedirectToAction("ManageStudent", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditStudent(String User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = obj.Users.Find(User_Id);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditStudent(User student)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var studentToUpdate = obj.Users.First(x => x.User_Id.Equals(student.User_Id));
                studentToUpdate.First_Name = student.First_Name;
                studentToUpdate.Last_Name = student.Last_Name;
                studentToUpdate.Password = student.Password;
                studentToUpdate.Contact_No = student.Contact_No;
                studentToUpdate.Section = student.Section;
                obj.SaveChanges();

                return RedirectToAction("ManageStudent", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageExamController

        public ActionResult ManageExamController()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var u = obj.Users.Where(a => a.Role.Equals("ExamController") && a.Status.Equals("Active"));

                    return View(u);
                }
                catch
                {
                    return RedirectToAction("Index", "SuperUser");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddExamController()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddExamController(User user)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = new User();
                u = user;
                u.Status = "Active";
                u.Role = "ExamController";

                obj.Users.Add(u);
                obj.SaveChanges();
                return RedirectToAction("ManageExamController", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditExamController(string User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                User u = obj.Users.Find(User_Id);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditExamController(User examController)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var examControllerToUpdate = obj.Users.First(x => x.User_Id.Equals(examController.User_Id));
                examControllerToUpdate.First_Name = examController.First_Name;
                examControllerToUpdate.Last_Name = examController.Last_Name;
                examControllerToUpdate.Password = examController.Password;
                examControllerToUpdate.Contact_No = examController.Contact_No;
                obj.SaveChanges();

                return RedirectToAction("ManageExamController", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageSubjects

        public ActionResult ManageSubject()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var s = obj.Subjects.Where(x => x.Status.Equals("Active"));
                return View(s);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult AddSubject()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddSubject(Subject sub, string[] Section)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                sub.Section = null;

                Subject s = new Subject();
                s.Subject_Id = sub.Subject_Id;
                int a = 1;
                foreach (var i in Section)
                {
                    if (a == 1)
                    {
                        s.Section = i;
                        a++;
                    }
                    else
                    {
                        s.Section = s.Section + "," + i;
                    }
                }
                s.Subject_Name = sub.Subject_Name;
                s.User_Id = sub.User_Id;
                s.Batch_Id = sub.Batch_Id;
                s.Status = "Active";
                obj.Subjects.Add(s);
                obj.SaveChanges();

                return RedirectToAction("ManageSubject");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditSubject(string Subject_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var s = obj.Subjects.First(x => x.Subject_Id.Equals(Subject_Id));
                    return View(s);
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditSubject(Subject subject)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var SubjectToUpdate = obj.Subjects.First(x => x.Subject_Id.Equals(subject.Subject_Id));
                if (TryUpdateModel(SubjectToUpdate, "", new string[] { "Subject_Name", "User_Id", "Batch_Id", "Section" }))
                {
                    obj.SaveChanges();
                }
                return RedirectToAction("ManageSubject", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ShowTeacherSubjects(string User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                ViewBag.user = User_Id + "'s";
                var subjects = obj.Subjects.Where(x => x.User_Id.Equals(User_Id) && x.Status.Equals("Active"));
                return View(subjects);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageDepartments

        public ActionResult ManageDepartment()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View(obj.Departments.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddDepartment()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddDepartment(string Department_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Department d = new Department();
                d.Department_Id = Department_Id;
                obj.Departments.Add(d);
                obj.SaveChanges();
                return RedirectToAction("ManageDepartment", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageBatches

        public ActionResult ManageBatch()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var b = obj.Batches.Where(x => x.Status.Equals("Active"));
                return View(b);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddBatch()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddBatch(Batch batch)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Batch b = new Batch();
                batch.Status = "Active";
                b = batch;
                obj.Batches.Add(b);
                obj.SaveChanges();
                return RedirectToAction("ManageBatch", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageRooms

        public ActionResult ManageRoom()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View(obj.Rooms.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddRoom()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddRoom(Room r)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                obj.Rooms.Add(r);
                obj.SaveChanges();
                return RedirectToAction("ManageRoom", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditRoom(string Room_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var r = obj.Rooms.Find(Room_Id);
                return View(r);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditRoom(Room room)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var r = obj.Rooms.First(x => x.Room_Id == room.Room_Id);
                r.Room_Capacity = room.Room_Capacity;
                obj.SaveChanges();
                return RedirectToAction("ManageRoom", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ChangePassword()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
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

                return RedirectToAction("SuperUserProfile", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region ManageProfile

        public ActionResult SuperUserProfile()
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
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditContactNumber()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
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

                return RedirectToAction("SuperUserProfile", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditGender()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
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

                return RedirectToAction("SuperUserProfile", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region FreezeStudents

        public ActionResult ManageFreezeStudents()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var students = obj.Users.Where(x => x.Batch.Status == "Active" && x.Status == "Active" || x.Status == "Freeze");
                ViewBag.freezeerror = TempData["freezeerror"];
                return View(students);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult FreezeStudent(string User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                try
                {
                    var f = obj.Freezes.First(x => x.Student_Id == User_Id);
                    TempData["freezeerror"] = "Cannot freeze " + f.Student_Id + " again because this facility was already availed in the past!";
                    return RedirectToAction("ManageFreezeStudents", "SuperUser");
                }
                catch
                {

                    var u = obj.Users.First(x => x.User_Id == User_Id);
                    return View(u);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult FreezeStudent(string User_Id, DateTime Freezing_Date)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Freeze freeze = new Freeze();
                var u = obj.Users.First(x => x.User_Id == User_Id);
                freeze.Student_Id = u.User_Id;
                freeze.Freezing_Date = Freezing_Date;
                freeze.Status = "Freeze";
                u.Status = "Freeze";
                obj.Freezes.Add(freeze);
                obj.SaveChanges();

                return RedirectToAction("ManageFreezeStudents", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UnFreezeStudent(string User_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var u = obj.Users.First(x => x.User_Id == User_Id);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UnFreezeStudent(string User_Id, DateTime UnFreezing_Date, string Batch_Id)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var u = obj.Users.First(x => x.User_Id == User_Id);

                var f = obj.Freezes.First(x => x.Student_Id == User_Id);

                u.Status = "Active";
                u.Batch_Id = Batch_Id;

                f.Unfreezing_Date = UnFreezing_Date;
                f.Status = "Active";

                obj.SaveChanges();

                return RedirectToAction("ManageFreezeStudents", "SuperUser");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion





        #region LocalApis

        //Activate or Deactivate Subject
        public JsonResult AjaxMethodForDisablingSubject(string Subject_Id)
        {
            try
            {
                var s = obj.Subjects.First(a => a.Subject_Id.Equals(Subject_Id));
                s.Status = "Inactive";
                obj.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(null);
            }
        }

        //Deactivate Users
        public JsonResult AjaxMethodForDeactivatingUser(string User_Id)
        {
            try
            {
                User u = obj.Users.Find(User_Id);
                u.Status = "Inactive";
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
            try
            {
                var dep = obj.Departments.ToList();
                return Json(dep.Select(x => new { x.Department_Id }));
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForBatch(string Department_Id)
        {
            try
            {
                var batches = obj.Batches.Where(x => x.Status.Equals("Active") && x.Department_Id.Equals(Department_Id));
                return Json(batches.Select(x => new { x.Batch_Id }));
            }
            catch
            {
                return Json(null);
            }
        }


        [HttpPost]
        public JsonResult AjaxMethodForBatchInFreeze(string User_Id)
        {
            try
            {
                var s = obj.Users.First(x => x.User_Id == User_Id);
                var b = obj.Batches.Where(x => x.Department_Id == s.Department_Id);

                return Json(b.Select(x => new { x.Batch_Id }));
            }
            catch
            {
                return Json(null);
            }
        }


        [HttpPost]
        public JsonResult AjaxMethodForBatchInAddSubject()
        {
            try
            {
                var batches = obj.Batches.Where(x => x.Status.Equals("Active"));
                return Json(batches.Select(x => new { x.Batch_Id }));
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForCompleteBatch(string Batch_Id)
        {
            try
            {
                Batch b = obj.Batches.Find(Batch_Id);
                b.Status = "Completed";
                obj.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForGettingTeachers()
        {
            try
            {
                var teacher = obj.Users.Where(x => x.Role.Equals("Teacher") && x.Status.Equals("Active"));
                return Json(teacher.Select(x => new { x.User_Id, x.First_Name, x.Last_Name }));
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult AjaxMethodForSection()
        {
            List<string> sections = new List<string>();
            sections.Add("a");
            sections.Add("b");
            sections.Add("c");
            sections.Add("d");
            sections.Add("e");
            sections.Add("f");
            sections.Add("g");
            sections.Add("h");
            sections.Add("i");
            sections.Add("j");
            sections.Add("k");
            sections.Add("l");
            sections.Add("m");
            sections.Add("n");
            sections.Add("o");
            sections.Add("p");
            sections.Add("q");
            sections.Add("r");
            sections.Add("s");
            sections.Add("t");
            sections.Add("u");
            sections.Add("v");
            sections.Add("w");
            sections.Add("x");
            sections.Add("y");
            sections.Add("z");
            return Json(sections);
        }

        [HttpPost]
        public JsonResult AjaxMethodForRemovingRoom(string Room_Id)
        {
            try
            {
                var r = obj.Rooms.Find(Room_Id);
                obj.Rooms.Attach(r);
                obj.Rooms.Remove(r);
                obj.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(null);
            }
        }





        //RemoteValidations
        [HttpPost]
        public JsonResult IsUser_IdAvailable(string User_Id)
        {
            return Json(!obj.Users.Any(a => a.User_Id.Equals(User_Id)));
        }

        [HttpPost]
        public JsonResult IsDepartment_IdAvailable(string Department_Id)
        {
            return Json(!obj.Departments.Any(a => a.Department_Id.Equals(Department_Id)));
        }

        [HttpPost]
        public JsonResult IsBatch_IdAvailable(string Batch_Id)
        {
            return Json(!obj.Batches.Any(a => a.Batch_Id.Equals(Batch_Id)));
        }

        [HttpPost]
        public JsonResult IsSubject_IdAvailable(string Subject_Id)
        {
            return Json(!obj.Subjects.Any(a => a.Subject_Id.Equals(Subject_Id)));
        }

        [HttpPost]
        public JsonResult IsRoom_IdAvailable(string Room_Id)
        {
            return Json(!obj.Rooms.Any(a => a.Room_Id.Equals(Room_Id)));
        }

        [HttpPost]
        public JsonResult Password_IsAvailable(string OldPassword)
        {
            var user1 = (string)Session["User_Id"];
            return Json(obj.Users.Any(a => a.User_Id == user1 && a.Password == OldPassword));
        }

        #endregion





        #region Logout

        public ActionResult Logout()
        {

            Session.Remove("User_Id");
            Session.Remove("User_Password");
            return RedirectToAction("Index", "Home");
        }

        #endregion




        #region UploadImage

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

                    return RedirectToAction("SuperUserProfile", "SuperUser");
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

        #endregion





        //public ActionResult EditProfile(string Success_Message)
        //{
        //    if (Session["User_Id"] != null && Session["User_Password"] != null)
        //    {
        //        string imageurl = "";
        //        var user1 = (string)Session["User_Id"];
        //        User user = obj.Users.First(a => a.User_Id == user1);
        //        string imageurlforJPG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".jpg"));
        //        string imageurlforPNG = Request.MapPath("~/Content/Images/Pictures/" + Path.GetFileName(user1) + Path.GetExtension(".png"));
        //        if (System.IO.File.Exists(imageurlforJPG))
        //        {
        //            imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".jpg");
        //        }
        //        else if (System.IO.File.Exists(imageurlforPNG))
        //        {
        //            imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName(user1) + System.IO.Path.GetExtension(".png");
        //        }
        //        else
        //        {
        //            imageurl = "~/Content/Images/Pictures/" + System.IO.Path.GetFileName("PlaceHolder") + System.IO.Path.GetExtension(".jpg");
        //        }
        //        ViewBag.ImageURL = imageurl;
        //        ViewBag.Message = Success_Message;
        //        return View(user);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        //[HttpPost]
        //public ActionResult EditProfile(FormCollection fc)
        //{
        //    if (Session["User_Id"] != null && Session["User_Password"] != null)
        //    {
        //        var user1 = (string)Session["User_Id"];
        //        User user = obj.Users.First(a => a.User_Id == user1);
        //        var Contact_No = fc["contact_no"];
        //        var Password = fc["password"];
        //        var Gender = fc["gender"];
        //        user.Contact_No = Contact_No;
        //        user.Password = Password;
        //        user.Gender = Gender;
        //        //todo  
        //        obj.SaveChanges();
        //        return RedirectToAction("EditProfile", "SuperUser", new { Success_Message = "Edit Profile Successfully" });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
    }
}
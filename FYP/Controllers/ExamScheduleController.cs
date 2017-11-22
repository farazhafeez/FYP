using FYP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace FYP.Controllers
{
    public class ExamScheduleController : Controller
    {
        //
        // GET: /ExamSchedule/

        FYP_DB_Entities obj = new FYP_DB_Entities();

        #region Exam Schedule For Student
        public ActionResult ExamScheduleForStudent()
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
        #endregion

    }
}

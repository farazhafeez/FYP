using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using FYP.Models;

namespace FYP.Controllers
{
    public class ExamResultController : Controller
    {
        //
        // GET: /ExamResult/
        FYP_DB_Entities obj = new FYP_DB_Entities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Student_ExamResult()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var user = (string)Session["User_Id"];
                var users = obj.Results.Where(a => a.User_Id == user);
                ViewBag.user = users;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Students_ExamResult()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                List<Result> result = new List<Result>();
                var user = (string)Session["User_Id"];
                var subjects = obj.Subjects.Where(a => a.User_Id == user);
                foreach (var i in subjects)
                {

                    var students = obj.Users.Where(a => a.Batch_Id == i.Batch_Id && i.Section.Contains(a.Section));
                    foreach (var s in students)
                    {
                        var student_results = obj.Results.Where(a => a.User_Id == s.User_Id);
                        foreach (var j in student_results)
                        {
                            result.Add(j);
                        }
                    }
                }
                return View(result);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult ResultForSuperUser()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View(obj.Results.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ResultForExamController()
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                return View(obj.Results.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}

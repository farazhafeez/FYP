using FYP.Models;
using FYP.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        FYP_DB_Entities obj = new FYP_DB_Entities();

        public ActionResult LoginPage()
        {
            User user;
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var user_id = (string)Session["User_Id"];
                var password = (string)Session["User_Password"];
                user = obj.Users.First(a => a.User_Id == user_id && a.Password == password);

                if (user.Role == "Teacher")
                {
                    return RedirectToAction("HomePage", "Teacher");
                }
                else if (user.Role == "Student")
                {
                    return RedirectToAction("HomePage", "Student");
                }
                else if (user.Role == "SuperUser")
                {
                    return RedirectToAction("HomePage", "SuperUser");
                }
                else if (user.Role == "ExamController")
                {
                    return RedirectToAction("HomePage", "ExamController");
                }
            }
            else
            {
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult LoginPage(ClassForLogin u)
        {
            User us = new User();
            if (ModelState.IsValid)
            {
                try
                {
                    us = obj.Users.First(a => a.User_Id.Equals(u.User_Id) && a.Password.Equals(u.Password));
                    if (us.Role.Equals("Teacher"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Password"] = us.Password;
                        Session["User_Name"] = us.First_Name + " " + us.Last_Name;
                        return RedirectToAction("HomePage", "Teacher");
                    }
                    else if (us.Role.Equals("Student"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Password"] = us.Password;
                        Session["User_Name"] = us.First_Name + " " + us.Last_Name;
                        return RedirectToAction("HomePage", "Student");
                    }
                    else if (us.Role.Equals("SuperUser"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Password"] = us.Password;
                        Session["User_Name"] = us.First_Name + " " + us.Last_Name;
                        return RedirectToAction("HomePage", "SuperUser");
                    }
                    else if (us.Role.Equals("ExamController"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Password"] = us.Password;
                        Session["User_Name"] = us.First_Name + " " + us.Last_Name;
                        return RedirectToAction("HomePage", "ExamController");
                    }
                }
                catch
                {
                    ViewBag.Message = "User not found";
                    return View();
                }
            }
            else
            {
                return View();
            }
            return View();
        }

    }
}

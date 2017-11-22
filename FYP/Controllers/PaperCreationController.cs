using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;

namespace FYP.Controllers
{
    public class PaperCreationController : Controller
    {
        //
        // GET: /PaperCreation/
        FYP_DB_Entities obj = new FYP_DB_Entities();
        List<int> randomNumber = new List<int>();
        List<Question> question_mcq = new List<Question>();
        List<Question> question_tf = new List<Question>();
        Random rand = new Random();

        #region Check Exam Status
        public ActionResult Exam_Status(string Success_Message)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                var user = (string)Session["User_Id"];
                User u = obj.Users.First(a => a.User_Id == user);
                List<Subject> subjects = u.Subjects.ToList();
                ViewBag.total_subjects = subjects;
                ViewBag.Success_Message = Success_Message;
                return View();
            }
            else
            {
                return RedirectToAction("LoginPage", "Login");
            }
        }
        #endregion
        #region Set Criteria For Paper
        public ActionResult Create_Criteria(string Subject_Id, string Error_Message)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                Subject subject = obj.Subjects.First(a => a.Subject_Id == Subject_Id);
                Exam exam = obj.Exams.First(a => a.Subject_Id == Subject_Id);

                // pick easy , medium and difficult questions for MCQ's and True/False
                var easymcq = subject.Questions.Where(a => a.Type.Equals("MCQ-E"));
                var mediummcq = subject.Questions.Where(a => a.Type.Equals("MCQ-M"));
                var difficultmcq = subject.Questions.Where(a => a.Type.Equals("MCQ-D"));
                var easytf = subject.Questions.Where(a => a.Type.Equals("True/False-E"));
                var mediumtf = subject.Questions.Where(a => a.Type.Equals("True/False-M"));
                var difficulttf = subject.Questions.Where(a => a.Type.Equals("True/False-D"));
                var totalmcq = subject.Questions.Where(a => a.Type.Equals("MCQ-E") || a.Type.Equals("MCQ-M") || a.Type.Equals("MCQ-D"));
                var totaltf = subject.Questions.Where(a => a.Type.Equals("True/False-E") || a.Type.Equals("True/False-M") || a.Type.Equals("True/False-D"));

                int easymcq_count = easymcq.Count();
                int mediummcq_count = mediummcq.Count();
                int difficultmcq_count = difficultmcq.Count();
                int easytf_count = easytf.Count();
                int mediumtf_count = mediumtf.Count();
                int difficulttf_count = difficulttf.Count();
                int totalmcq_count = totalmcq.Count();
                int totaltf_count = totaltf.Count();

                ViewBag.easymcq = easymcq_count;
                ViewBag.mediummcq = mediummcq_count;
                ViewBag.difficultmcq = difficultmcq_count;
                ViewBag.easytf = easytf_count;
                ViewBag.mediumtf = mediumtf_count;
                ViewBag.difficulttf = difficulttf_count;
                ViewBag.totalmcq = totalmcq_count;
                ViewBag.totaltf = totaltf_count;


                // Calculate percentage for each easy , medium and difficult MCQ's and True/False Questions
                if (totalmcq_count > 0 && totaltf_count > 0)
                {
                    float easymcqpercentage = ((easymcq_count * 100) / totalmcq_count);
                    float mediummcqpercentage = ((mediummcq_count * 100) / totalmcq_count);
                    float difficultmcqpercentage = ((difficultmcq_count * 100) / totalmcq_count);
                    float easytfpercentage = ((easytf_count * 100) / totaltf_count);
                    float mediumtfpercentage = ((mediumtf_count * 100) / totaltf_count);
                    float difficulttfpercentage = ((difficulttf_count * 100) / totaltf_count);

                    ViewBag.easymcqpercentage = easymcqpercentage;
                    ViewBag.mediummcqpercentage = mediummcqpercentage;
                    ViewBag.difficultmcqpercentage = difficultmcqpercentage;
                    ViewBag.easytfpercentage = easytfpercentage;
                    ViewBag.mediumtfpercentage = mediumtfpercentage;
                    ViewBag.difficulttfpercentage = difficulttfpercentage;
                }
                else
                {
                    ViewBag.easymcqpercentage = 0;
                    ViewBag.mediummcqpercentage = 0;
                    ViewBag.difficultmcqpercentage = 0;
                    ViewBag.easytfpercentage = 0;
                    ViewBag.mediumtfpercentage = 0;
                    ViewBag.difficulttfpercentage = 0;
                }

                ViewBag.subject = subject.Subject_Name;
                ViewBag.subject_id = subject.Subject_Id;
                ViewBag.Error_Message = Error_Message;
                ViewBag.Marks = exam.Total_Marks;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
        #region Create Paper For Specific Exam
        public ActionResult Create_Paper(FormCollection fc)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                int Subject_Id = int.Parse(fc["subject_id"]);
                string subject_name = fc["subject"];
                int total = int.Parse(fc["totalmarks"]);
                float mcq = int.Parse(fc["mcqs"]);
                float truefalse = int.Parse(fc["truefalse"]);
                int mcqmark = int.Parse(fc["mcqmark"]);
                float truefalsemark = int.Parse(fc["truefalsemark"]);
                float TotalMcq = (mcq / 100) * total;
                float Totaltruefalse = (truefalse / 100) * total;
                float mcqQuestion1 = (TotalMcq / mcqmark);
                float truefalseQuestion1 = (Totaltruefalse / truefalsemark);
                ViewBag.eachmcqmark = mcqmark;
                ViewBag.eachtfmark = truefalsemark;

                int mcqQuestions = (int)Math.Round(mcqQuestion1, MidpointRounding.AwayFromZero);
                int truefalseQuestions = total - ((mcqQuestions) * mcqmark);

                var user1 = (string)Session["User_Id"];
                User user = obj.Users.First(a => a.User_Id == user1);
                Subject subject = obj.Subjects.First(a => a.Subject_Name == subject_name && a.User_Id == user.User_Id);

                var queryforallmcq = subject.Questions.Where(a => a.Type == "MCQ-E" || a.Type == "MCQ-M" || a.Type == "MCQ-D");
                var queryforeasymcq = subject.Questions.Where(a => a.Type == "MCQ-E");
                var queryformediummcq = subject.Questions.Where(a => a.Type == "MCQ-M");
                var queryfordifficultmcq = subject.Questions.Where(a => a.Type == "MCQ-D");

                var queryforalltf = subject.Questions.Where(a => a.Type == "True/False-E" || a.Type == "True/False-M" || a.Type == "True/False-D");
                var queryforeasytf = subject.Questions.Where(a => a.Type == "True/False-E");
                var queryformediumtf = subject.Questions.Where(a => a.Type == "True/False-M");
                var queryfordifficulttf = subject.Questions.Where(a => a.Type == "True/False-D");

                int easymcq_count = queryforeasymcq.Count();
                int mediummcq_count = queryformediummcq.Count();
                int difficultmcq_count = queryfordifficultmcq.Count();
                int easytf_count = queryforeasytf.Count();
                int mediumtf_count = queryformediumtf.Count();
                int difficulttf_count = queryfordifficulttf.Count();
                int allmcq_count = queryforallmcq.Count();
                int alltf_count = queryforalltf.Count();

                if (fc["mcqe"] != null && fc["tfe"] == null)
                {
                    float mcqeasy = int.Parse(fc["mcqe"]);
                    float mcqmedium = int.Parse(fc["mcqm"]);
                    float mcqdifficult = int.Parse(fc["mcqd"]);

                    var easymcq_Questions = (mcqeasy / 100) * mcqQuestions;
                    var mediummcq_Questions = (mcqmedium / 100) * mcqQuestions;
                    var difficultmcq_Questions = (mcqdifficult / 100) * mcqQuestions;

                    if (((easymcq_Questions + 1) <= easymcq_count) && ((mediummcq_Questions + 1) <= mediummcq_count) && ((difficultmcq_Questions + 1) <= difficultmcq_count) && (truefalseQuestions <= alltf_count))
                    {
                        float a = easymcq_Questions % 1;
                        float b = mediummcq_Questions % 1;
                        float c = difficultmcq_Questions % 1;
                        float d = a + b + c;
                        if (d == 0.0)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(mediummcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficultmcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediummcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediummcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficultmcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                        else if (d == 1)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                        else if (d == 2)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                                randomNumber.Clear();
                                for (int i = 0; i < truefalseQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, alltf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                    }
                    else
                    {
                        if ((easymcq_Questions + 1 > easymcq_count) && (mediummcq_Questions + 1 > mediummcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium , Difficult MCQ's and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (mediummcq_Questions + 1 > mediummcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium MCQ and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Difficult MCQ and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediummcq_Questions + 1 > mediummcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Medium , Difficult MCQ and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (mediummcq_Questions + 1 > mediummcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy and Medium MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy and Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Easy MCQ's and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediummcq_Questions + 1 > mediummcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count))
                        {
                            string ErrorMessage = "You have not enough Medium and Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediummcq_Questions + 1 > mediummcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Medium MCQ's and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((difficultmcq_Questions + 1 > difficultmcq_count) && (truefalseQuestions + 1 > alltf_count))
                        {
                            string ErrorMessage = "You have not enough Difficult MCQ's and True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (easymcq_Questions + 1 > easymcq_count)
                        {
                            string ErrorMessage = "You have not enough Easy MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mediummcq_Questions + 1 > mediummcq_count)
                        {
                            string ErrorMessage = "You have not enough Medium MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (difficultmcq_Questions + 1 > difficultmcq_count)
                        {
                            string ErrorMessage = "You have not enough Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (truefalseQuestions + 1 > alltf_count)
                        {
                            string ErrorMessage = "You have not enough True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                    }
                }
                else if (fc["mcqe"] == null && fc["tfe"] != null)
                {
                    float tfeasy = int.Parse(fc["tfe"]);
                    float tfmedium = int.Parse(fc["tfm"]);
                    float tfdifficult = int.Parse(fc["tfd"]);

                    var easytf_Questions = (tfeasy / 100) * truefalseQuestions;
                    var mediumtf_Questions = (tfmedium / 100) * truefalseQuestions;
                    var difficulttf_Questions = (tfdifficult / 100) * truefalseQuestions;

                    if (((easytf_Questions + 1) <= easymcq_count) && ((mediumtf_Questions + 1) <= mediummcq_count) && ((difficulttf_Questions + 1) <= difficultmcq_count) && (mcqQuestions <= allmcq_count))
                    {
                        float a = easytf_Questions % 1;
                        float b = mediumtf_Questions % 1;
                        float c = difficulttf_Questions % 1;
                        float d = a + b + c;
                        if (d == 0.0)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(mediumtf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficulttf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediumtf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediumtf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficulttf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                        else if (d == 1)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                        else if (d == 2)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                                randomNumber.Clear();
                                for (int i = 0; i < mcqQuestions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, allmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                    }
                    else
                    {
                        if ((easytf_Questions + 1 > easytf_count) && (mediumtf_Questions + 1 > mediumtf_count) && (difficulttf_Questions + 1 > difficulttf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium , Difficult True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (mediumtf_Questions + 1 > mediumtf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (difficulttf_Questions + 1 > difficulttf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Difficult True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediumtf_Questions + 1 > mediumtf_count) && (difficulttf_Questions + 1 > difficulttf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Medium , Difficult True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (mediumtf_Questions + 1 > mediumtf_count))
                        {
                            string ErrorMessage = "You have not enough Easy and Medium True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (difficulttf_Questions + 1 > difficulttf_count))
                        {
                            string ErrorMessage = "You have not enough Easy and Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediumtf_Questions + 1 > mediumtf_count) && (difficulttf_Questions + 1 > difficulttf_count))
                        {
                            string ErrorMessage = "You have not enough Medium and Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediumtf_Questions + 1 > mediumtf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Medium True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((difficulttf_Questions + 1 > difficulttf_count) && (mcqQuestions + 1 > allmcq_count))
                        {
                            string ErrorMessage = "You have not enough Difficult True/False and MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (easytf_Questions + 1 > easytf_count)
                        {
                            string ErrorMessage = "You have not enough Easy True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mediumtf_Questions + 1 > mediumtf_count)
                        {
                            string ErrorMessage = "You have not enough Medium True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (difficulttf_Questions + 1 > difficulttf_count)
                        {
                            string ErrorMessage = "You have not enough Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mcqQuestions + 1 > allmcq_count)
                        {
                            string ErrorMessage = "You have not enough MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                    }
                }
                else if (fc["mcqe"] != null && fc["tfe"] != null)
                {
                    float mcqeasy = int.Parse(fc["mcqe"]);
                    float mcqmedium = int.Parse(fc["mcqm"]);
                    float mcqdifficult = int.Parse(fc["mcqd"]);
                    var easymcq_Questions = (mcqeasy / 100) * mcqQuestions;
                    var mediummcq_Questions = (mcqmedium / 100) * mcqQuestions;
                    var difficultmcq_Questions = (mcqdifficult / 100) * mcqQuestions;

                    float tfeasy = int.Parse(fc["tfe"]);
                    float tfmedium = int.Parse(fc["tfm"]);
                    float tfdifficult = int.Parse(fc["tfd"]);
                    var easytf_Questions = (tfeasy / 100) * truefalseQuestions;
                    var mediumtf_Questions = (tfmedium / 100) * truefalseQuestions;
                    var difficulttf_Questions = (tfdifficult / 100) * truefalseQuestions;

                    if (((easymcq_Questions + 1) <= easymcq_count) && ((mediummcq_Questions + 1) <= mediummcq_count) && ((difficultmcq_Questions + 1) <= difficultmcq_count) && ((easytf_Questions + 1) <= easytf_count) && ((mediumtf_Questions + 1) <= mediumtf_count) && ((difficulttf_Questions + 1) <= difficulttf_count))
                    {
                        float m1 = easymcq_Questions % 1;
                        float m2 = mediummcq_Questions % 1;
                        float m3 = difficultmcq_Questions % 1;
                        float m = m1 + m2 + m3;

                        if (m == 0.0)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(mediummcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficultmcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediummcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easymcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediummcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficultmcq_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                        else if (m == 1)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                        else if (m == 2)
                        {
                            if (easymcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (mediummcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else if (difficultmcq_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easymcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediummcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficultmcq_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.MCQ = question_mcq;
                            }
                        }
                        float t1 = easytf_Questions % 1;
                        float t2 = mediumtf_Questions % 1;
                        float t3 = difficulttf_Questions % 1;
                        float t = t1 + t2 + t3;

                        if (t == 0.0)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(mediumtf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficulttf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediumtf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < easytf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < mediumtf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < difficulttf_Questions; i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                        else if (t == 1)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                        else if (t == 2)
                        {
                            if (easytf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }

                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (mediumtf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else if (difficulttf_Questions == 0)
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                            else
                            {
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, easytf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, mediumtf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                randomNumber.Clear();
                                for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                                {
                                    int number;
                                    do
                                    {
                                        number = rand.Next(1, difficulttf_count + 1);
                                    } while (randomNumber.Contains(number));
                                    randomNumber.Add(number);
                                    var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                    foreach (var put in qtn)
                                    {
                                        question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                    }
                                }
                                ViewBag.TrueFalse = question_tf;
                            }
                        }
                    }
                    else
                    {
                        if (((easymcq_Questions + 1) > easymcq_count) && ((mediummcq_Questions + 1) > mediummcq_count) && ((difficultmcq_Questions + 1) > difficultmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium , Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (((easytf_Questions + 1) > easytf_count) && ((mediumtf_Questions + 1) > mediumtf_count) && ((difficulttf_Questions + 1) > difficulttf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium , Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (mediummcq_Questions + 1 > mediummcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easymcq_Questions + 1 > easymcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediummcq_Questions + 1 > mediummcq_count) && (difficultmcq_Questions + 1 > difficultmcq_count))
                        {
                            string ErrorMessage = "You have not enough Medium , Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (mediumtf_Questions + 1 > mediumtf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Medium True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((easytf_Questions + 1 > easytf_count) && (difficulttf_Questions + 1 > difficulttf_count))
                        {
                            string ErrorMessage = "You have not enough Easy , Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if ((mediumtf_Questions + 1 > mediumtf_count) && (difficulttf_Questions + 1 > difficulttf_count))
                        {
                            string ErrorMessage = "You have not enough Medium , Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (easymcq_Questions + 1 > easymcq_count)
                        {
                            string ErrorMessage = "You have not enough Easy MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mediummcq_Questions + 1 > mediummcq_count)
                        {
                            string ErrorMessage = "You have not enough Medium MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (difficultmcq_Questions + 1 > difficultmcq_count)
                        {
                            string ErrorMessage = "You have not enough Difficult MCQ's to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (easytf_Questions + 1 > easytf_count)
                        {
                            string ErrorMessage = "You have not enough Easy True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mediumtf_Questions + 1 > mediumtf_count)
                        {
                            string ErrorMessage = "You have not enough Medium True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (difficulttf_Questions + 1 > difficulttf_count)
                        {
                            string ErrorMessage = "You have not enough Difficult True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                    }
                }
                else if (fc["mcqe"] == null && fc["tfe"] == null)
                {
                    if (mcqQuestions <= allmcq_count && truefalseQuestions <= alltf_count)
                    {
                        randomNumber.Clear();
                        for (int i = 0; i < mcqQuestions; i++)
                        {
                            int number;
                            do
                            {
                                number = rand.Next(1, allmcq_count + 1);
                            } while (randomNumber.Contains(number));
                            randomNumber.Add(number);
                            var qtn = obj.Questions.Where(a => a.Type == "MCQ-E" || a.Type == "MCQ-M" || a.Type == "MCQ-D").OrderBy(b => b.Question_Id).Skip(number - 1).Take(1).ToList();
                            foreach (var put in qtn)
                            {
                                question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                            }
                        }
                        ViewBag.MCQ = question_mcq;
                        randomNumber.Clear();
                        for (int i = 0; i < truefalseQuestions; i++)
                        {
                            int number;
                            do
                            {
                                number = rand.Next(1, alltf_count + 1);
                            } while (randomNumber.Contains(number));
                            randomNumber.Add(number);

                            var tfs = obj.Questions.Where(a => a.Type == "True/False-E" || a.Type == "True/False-M" || a.Type == "True/False-D").OrderBy(b => b.Question_Id).Skip(number - 1).Take(1).ToList();
                            foreach (var put in tfs)
                            {
                                question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                            }
                        }
                        ViewBag.TrueFalse = question_tf;
                    }
                    else
                    {
                        if ((mcqQuestions > allmcq_count) && (truefalseQuestions > alltf_count))
                        {
                            string ErrorMessage = "You have not enough questons for MCQ'S and True/False to meet the Criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (mcqQuestions > allmcq_count)
                        {
                            string ErrorMessage = "You have not enough questons for MCQ'S to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                        else if (truefalseQuestions > alltf_count)
                        {
                            string ErrorMessage = "You have not enough questons for True/False to meet the criteria";
                            return RedirectToAction("Create_Criteria", "PaperCreation", new { Subject_Id = Subject_Id, Error_Message = ErrorMessage });
                        }
                    }
                }
                ViewBag.Subject_Id = Subject_Id;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
        #region Send Paper To ExamController For Schedule
        public ActionResult Send_Paper(FormCollection fc)
        {
            if (Session["User_Id"] != null && Session["User_Password"] != null)
            {
                string Subject_Id = fc["Subject_Id"];
                Exam exam = obj.Exams.First(a => a.Subject_Id == Subject_Id);
                Paper paper = new Paper();
                for (int i = 1; i <= ((fc.Count) - 3); i++)
                {
                    paper.Question_Id = int.Parse(fc["question_" + i]);
                    paper.Question_Marks = int.Parse(fc["mark_" + i]);
                    paper.Exam_Id = exam.Exam_Id;
                    obj.Papers.Add(paper);
                    obj.SaveChanges();
                }
                if (exam.Total_Marks == 30)
                {
                    exam.Status = "Mid Exam";
                    obj.SaveChanges();
                }
                else if (exam.Total_Marks == 50)
                {
                    exam.Status = "Final Exam";
                    obj.SaveChanges();
                }
                string success_message = "Paper Has Been Created Successfully for " + exam.Subject.Subject_Name;
                return RedirectToAction("Exam_Status", "PaperCreation", new { Success_Message = success_message });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

    }
}

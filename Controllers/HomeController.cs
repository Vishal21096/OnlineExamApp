using OnlineQuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;
using System.Data.Entity;

namespace OnlineQuizApp.Controllers
{
    public class HomeController : Controller
    { 
        DbQuizEntities db = new DbQuizEntities();

        //Student registration 
        [HttpGet]
        public ActionResult studentRegister()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult studentRegister(TBL_STUDENT student,HttpPostedFileBase imgfile)
        {
            TBL_STUDENT stu = new TBL_STUDENT();
            stu.S_NAME = student.S_NAME;
            stu.S_PASSWORD = student.S_PASSWORD;
            stu.S_IMAGE = uploadimage(imgfile);
            db.TBL_STUDENT.Add(stu);
            db.SaveChanges();
            TempData["msg"] = "You have registered Successfully.....";
            TempData.Keep();
            return RedirectToAction("slogin");
        }
        public string uploadimage(HttpPostedFileBase imgfile)
        {
            string path = "-1";
            try
            {
                if(imgfile!=null && imgfile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imgfile.FileName);
                    if (extension.ToLower().Equals("jpg") || extension.ToLower().Equals("jpeg") || extension.ToLower().Equals("png")) ;
                    Random r = new Random();
                    path= Path.Combine(Server.MapPath("/Content/img"),r+Path.GetFileName(imgfile.FileName));
                    imgfile.SaveAs(path);
                    path = "/Content/img" + r + Path.GetFileName(imgfile.FileName);
                }
                   
                else
                {

                }
            }
            catch(Exception)
            {

            }
            return path;
        }

        //Logout
        public ActionResult LogOut()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
          
        }
        //Admin Login
        [HttpGet]
        public ActionResult tlogin()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult tlogin(TBL_ADMIN admin)
        {
            TBL_ADMIN ad = db.TBL_ADMIN.Where( x => x.AD_NAME==admin.AD_NAME && x.AD_PASSWORD == admin.AD_PASSWORD).SingleOrDefault();
            if(ad!=null)
            {
                Session["AD_ID"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid username or password";
            }
            return View();
        }

        //Student Login
        [HttpGet]
        public ActionResult slogin()
        {          
            return View();
        }
        [HttpPost]
        public ActionResult slogin(TBL_STUDENT student)
        {
            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_NAME == student.S_NAME && x.S_PASSWORD == student.S_PASSWORD).SingleOrDefault();
            if(std==null)
            {
                ViewBag.msg = "Invalid Username or password";
                
            }
            else
            {
                Session["std_id"] = std.S_ID;
                return RedirectToAction("StudentExam");
            }
            return View();
        }

        //to enter room number
        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            return View();
        }
        [HttpPost]
        public ActionResult StudentExam( string room)           
        {
           List< TBL_CATEGORY> list = db.TBL_CATEGORY.ToList();
            foreach(var item in list)
            {
                if(item.room_number==room)
                {
                    List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.QUESTION_FK_CATID == item.CAT_ID).ToList();
                    Queue<TBL_QUESTIONS> queue = new Queue<TBL_QUESTIONS>();
                    foreach(TBL_QUESTIONS a in li)
                    {
                        queue.Enqueue(a);
                    }
                    TempData["examid"] = item.CAT_ID;
                    TempData["questions"] = queue;
                    TempData["score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Room Found.....";
                }
            }
            return View();
        }
        //Exam End
        public ActionResult EndExam()
        {
            return View();
        }

        // code to start quiz
        [HttpGet]
        public ActionResult QuizStart()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }
            TBL_QUESTIONS q = null;
            if(TempData["questions"]!=null)
            {
                Queue<TBL_QUESTIONS> qlist = (Queue<TBL_QUESTIONS>)TempData["questions"];
                if(qlist.Count>0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["questions"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }

                return View(q);
        }
        [HttpPost]
        public ActionResult QuizStart(TBL_QUESTIONS q)
        {
            string correctans = null;
            if(q.OPA!=null)
            {
                correctans = "A";
            }
            else if(q.OPB!=null)
            {
                correctans = "B";
            }
            else if(q.OPC!=null)
            {
                correctans = "C";

            }
            else if(q.OPD!=null)
            {
                correctans = "D";
            }
            if(correctans.Equals(q.COP))
            {
                TempData["score"] = Convert.ToInt32(TempData["score"]) +1;
            }
            TempData.Keep();

            return RedirectToAction("QuizStart");
        }

            public ActionResult Dashboard()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // Add category of exam 
        [HttpGet]
        public ActionResult AddCategory()
        {
            Session["AD_ID"] = 1;//it has to be remove...
            int adminid= Convert.ToInt32(Session["AD_ID"].ToString());
            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where( x => x.CAT_FK_ADMIN_ID==adminid) .OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = li;
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(TBL_CATEGORY cat)
        {
            List<TBL_CATEGORY> li = db.TBL_CATEGORY. OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = li;

            TBL_CATEGORY c = new TBL_CATEGORY();
            c.CAT_NAME = cat.CAT_NAME;
            c.room_number = cat.room_number;
            c.CAT_FK_ADMIN_ID = Convert.ToInt32(Session["AD_ID"].ToString());
            db.TBL_CATEGORY.Add(c);
            db.SaveChanges();
            return RedirectToAction("AddCategory");
        }

        //Add Question 
        [HttpGet]
        public ActionResult AddQuestion()
        {
            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADMIN_ID == sid).ToList();
            ViewBag.list = new SelectList(li, "CAT_ID", "CAT_NAME");

            return View();
        }
        [HttpPost]
        public ActionResult AddQuestion(TBL_QUESTIONS questions)
        {
            try
            {
                int sid = Convert.ToInt32(Session["AD_ID"]);
                List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADMIN_ID == sid).ToList();
                ViewBag.list = new SelectList(li, "CAT_ID", "CAT_NAME");

                TBL_QUESTIONS QA = new TBL_QUESTIONS();
                QA.Q_TEXT = questions.Q_TEXT;
                QA.OPA = questions.OPA;
                QA.OPB = questions.OPB;
                QA.OPC = questions.OPC;
                QA.OPD = questions.OPD;
                QA.COP = questions.COP;
                QA.QUESTION_FK_CATID = questions.QUESTION_FK_CATID;

                db.TBL_QUESTIONS.Add(QA);
                db.SaveChanges();
                TempData["msg"] = "Question Added Successfully.....";              
            }
            catch(Exception)
            {
                TempData["msg"] = "You are entering same question again...please enter different question.";
                
            }
            return RedirectToAction("AddQuestion");
        }
        public ActionResult ViewAllQuestions(int?id,int?page)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("tlogin");
            }
            if (id == null)
            {
                return RedirectToAction("Dashboard");
            }

            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.QUESTION_FK_CATID == id).ToList();
            IPagedList<TBL_QUESTIONS> stu = li.ToPagedList(pageindex, pagesize);
          
            return View(stu);
        }
        //Edit the Questions
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            TBL_QUESTIONS questions = db.TBL_QUESTIONS.Find(id);
            if (questions == null)
            {
                return HttpNotFound();
            }
            return View(questions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TBL_QUESTIONS questions)
        {
            //int sid = Convert.ToInt32(Session["AD_ID"]);
            // List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADMIN_ID == sid).ToList();
            //ViewBag.list = new SelectList(li);
            if (ModelState.IsValid)
            {
                db.Entry(questions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewAllQuestions");
            }
            return View(questions);
        }
        public ActionResult Delete(int?id)
        {
            TBL_QUESTIONS getquestion = db.TBL_QUESTIONS.Find(id);
            if (getquestion == null)
            {
                return HttpNotFound();
            }
            return View(getquestion);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int?id)
        {
            TBL_QUESTIONS questions = db.TBL_QUESTIONS.Find(id);
            db.TBL_QUESTIONS.Remove(questions);
            db.SaveChanges();
            return RedirectToAction("ViewAllQuestions");
        }

        //Dashboard
        public ActionResult Index()
        {
            if(Session["AD_ID"]!=null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
    }

}
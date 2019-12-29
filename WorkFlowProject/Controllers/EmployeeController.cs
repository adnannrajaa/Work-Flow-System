using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.Admin;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Home;
using WorkFlowProject.ViewModels;
using WorkFlowProject.ViewModels.Account;
using WorkFlowProject.ViewModels.Employee;

namespace WorkFlowProject.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        DownloadFiles obj;
        public EmployeeController()
        {
            obj = new DownloadFiles();
        }
        // GET: Employee
        public ActionResult Index()
        {
            obj.GetFiles();
            WorkFlowDbContext db = new WorkFlowDbContext();
            var AdminId = db.Users.Where(s => s.Account == "Principle").Select(f => f.UserId).FirstOrDefault();
            var fullNAme = new SelectList(db.UsersDetails.Where(s=>(s.UserId != WebSecurity.CurrentUserId && s.UserId != AdminId)).ToList(), "UserId", "FirstName");
            ViewData["Users"] = fullNAme;

            var ProjectName = new SelectList(db.Projects.Where(s => s.Status !=true).ToList(), "ProjectId", "ProjectTitle");
            ViewData["ProjectData"] = ProjectName;


            return View(GetTotalCommentsLikesAndPosts());
        }
        private EmployeeViewModel GetTotalCommentsLikesAndPosts()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                EmployeeViewModel totaldata = EmployeeViewModel.GetRecord(db);
                ViewBag.TotalPosts = totaldata.totalPostList;
                ViewBag.TotalLikes = totaldata.totalLikesList;
                ViewBag.TotalComments = totaldata.totalCommentsList;
                ViewBag.TotalUsers = totaldata.totalUsersList;
                ViewBag.TotalCompletedProjects = totaldata.totalProjects;
                return totaldata;
            }
        }
        [HttpPost]
        public ActionResult CreateProject(EmployeeViewModel Addproject)
        {
            var projectdata = Addproject.CreateNewProject;
            if (ModelState.IsValid)
            {
               
                projectdata.AssignedBy = WebSecurity.CurrentUserName;
                projectdata.CreatedDate = DateTime.Now.ToShortDateString();
                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    db.ProjectTasks.Add(projectdata);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Employee");
        }


        public FileResult Download(int FileID)
        {
            int CurrentFileID = FileID;
            var filesCol = obj.GetFiles();
            string CurrentFileName = (from fls in filesCol
                                      where fls.PaperId == CurrentFileID
                                      select fls.FilePath).First();

            string contentType = string.Empty;

            if (CurrentFileName.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }

            else if (CurrentFileName.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            return File(CurrentFileName, contentType, CurrentFileName);
        }


        public ActionResult RequestLeave()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestLeave(Leaf leav)
        {
            
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                if(ModelState.IsValid)
                {
                    leav.UserId = WebSecurity.CurrentUserId;
                    db.Leaves.Add(leav);
                    db.SaveChanges();
                    return RedirectToAction("ViewLeaveStatus","Employee");
                }
               

            }
            return View();
        }

        public ActionResult ViewLeaveStatus()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var query = db.Leaves.Join(db.UsersDetails, l => l.UserId, u => u.UserId, (x, y) => new
                {
                    Leav = x,
                    Use = y,

                }).Where(s=> s.Leav.UserId == WebSecurity.CurrentUserId).Select(s => new ListLeaves
                {
                    FullName = s.Use.FirstName + " " + s.Use.LastName,
                    ImagePath = s.Use.ImagePath,
                    Reason = s.Leav.Reason,
                    LeavePeriod = s.Leav.LeavePeriod,
                    Status = s.Leav.Status,
                }).ToList();
                return View(query);
            }
        }
        public ActionResult GetUserDetailsAndNotifications()
        {
            int UserId = WebSecurity.CurrentUserId;
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                NotificationViewModel NotifyMe = NotificationViewModel.UserNotifyList(db, UserId);
                ViewBag.count = NotifyMe.totalCount;
                return PartialView("NavPartial", NotifyMe);
            }
        }

        public ActionResult RequestForPaperApprovel()
        {
            return View();
        }

        public ActionResult RequestPaperApprovel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestPaperApprovel(Paper paperModel)
        {
            if (ModelState.IsValid)
            {
                if (paperModel.fileUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(paperModel.fileUpload.FileName);
                    string extension = Path.GetExtension(paperModel.fileUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    paperModel.FilePath = "~/AppFiles/Files/" + fileName;
                    paperModel.fileUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Files/"), fileName));
                }

                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    paperModel.UserId = WebSecurity.CurrentUserId;
                    db.Papers.Add(paperModel);
                    db.SaveChanges();
                    ViewBag.Message = "Data added";

                    return RedirectToAction("PaperStatus", "Employee");
                }
            }
            return View();

        }

        public ActionResult CompletedTask(int id)
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var verab = db.ProjectTasks.Find(id);
                verab.Status = true;
                db.SaveChanges();

            }
            return RedirectToAction("Index", "Employee");
        }

        public ActionResult Completed(int id)
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var verab = db.Projects.Find(id);
                verab.Status = true;
                db.SaveChanges();

            }
            return RedirectToAction("Index", "Employee");
            
        }

        public ActionResult PaperStatus()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                obj.GetFiles();
                var NewQuery = db.Papers.Join(db.UsersDetails, p => p.UserId, u => u.UserId, (p, u) => new { p, u }).Where(s => (s.p.PaperStatus != true && s.p.PaperStatus != false && s.u.UserId == WebSecurity.CurrentUserId)).Select(f => new PaperModel
                {
                    PaperId = f.p.PaperId,
                    Name = f.u.FirstName,
                    PaperStandard = f.p.PaperStandard,
                    PaperSubject = f.p.PaperSubject,
                    FilePath = f.p.FilePath,
                    PaperStatus = f.p.PaperStatus,


                }).ToList();

                return View(NewQuery);
            }
        }

    }
}
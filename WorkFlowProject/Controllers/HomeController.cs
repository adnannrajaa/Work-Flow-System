using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Home;
using WorkFlowProject.ViewModels;
using WorkFlowProject.ViewModels.Account;

namespace WorkFlowProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DownloadFiles obj;
        public HomeController()
        {
            obj = new DownloadFiles();
        }
        // GET: Home
        public ActionResult Index()
        {
            obj.GetFiles();
            WorkFlowDbContext db = new WorkFlowDbContext();
            var fullNAme = new SelectList(db.UsersDetails.Where(s=>s.UserId !=WebSecurity.CurrentUserId).ToList(), "UserId", "FirstName");
            ViewData["Users"] = fullNAme;
            return View(GetTotalCommentsLikesAndPosts());
        }

        private TotalLikesPostsCommentsModel GetTotalCommentsLikesAndPosts()
        {
            using (WorkFlowDbContext db  = new WorkFlowDbContext())
            {
                TotalLikesPostsCommentsModel totaldata = TotalLikesPostsCommentsModel.GetRecord(db);
                ViewBag.TotalPosts = totaldata.totalPostList;
                ViewBag.TotalLikes = totaldata.totalLikesList;
                ViewBag.TotalComments = totaldata.totalCommentsList;
                ViewBag.TotalUsers = totaldata.totalUsersList;
                ViewBag.TotalCompletedProjects = totaldata.totalProjects;
                return totaldata;
            }
        }

        public ActionResult GetUserDetailsAndNotifications()
        {
            int UserId = WebSecurity.CurrentUserId;
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                NotificationViewModel NotifyMe = NotificationViewModel.UserNotifyList(db,UserId);
                ViewBag.count = NotifyMe.totalCount;
                return PartialView("NavPartial", NotifyMe);
            }
        }

        [HttpPost]
        public ActionResult BlockComments(int CommentId)
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var finddata = db.PostComments.Find(CommentId);
                finddata.Status = false;
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult CreateProject(TotalLikesPostsCommentsModel Addproject)
        {
            var projectdata = Addproject.CreateNewProject;
            if (ModelState.IsValid)
            {
                if (projectdata.fileUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(projectdata.fileUpload.FileName);
                    string extension = Path.GetExtension(projectdata.fileUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    projectdata.FilePath = "~/AppFiles/ProjectFiles/" + fileName;
                    projectdata.fileUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/ProjectFiles/"), fileName));
                }

                projectdata.CreatedBy = WebSecurity.CurrentUserName;
                projectdata.CreatedDate = DateTime.Now.ToShortDateString();
                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    db.Projects.Add(projectdata);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index","Home");
        }

  
        public FileResult Download(int FileID)
        {
            int CurrentFileID = FileID;
            var filesCol = obj.GetFiles();
            string CurrentFileName = (from fls in filesCol
                                      where fls.PaperId == CurrentFileID
                                      select fls.FilePath).FirstOrDefault();

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


    }
}
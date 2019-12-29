using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.Admin;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.ViewModels;
using WorkFlowProject.ViewModels.Account;

namespace WorkFlowProject.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        DownloadFiles obj;
        public AdminController()
        {
            obj = new DownloadFiles();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            GetRolesForCurrentUsers();
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserRegister registerModel)
        {
            GetRolesForCurrentUsers();
            if (ModelState.IsValid)
            {

                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    bool isUserExist = WebSecurity.UserExists(registerModel.UserName);
                    if (isUserExist)
                    { ModelState.AddModelError("UserName", "UserName already exist"); }
                    else
                    {
                        registerModel.Account = registerModel.Role;
                        WebSecurity.CreateUserAndAccount(registerModel.UserName, registerModel.Password, new { registerModel.Account, status = 1 });
                        Roles.AddUserToRole(registerModel.UserName, registerModel.Role);

                        var userid = (db.Users.OrderByDescending(s => s.UserId).Select(s => s.UserId)).FirstOrDefault();

                        UsersDetail userModel = new UsersDetail();
                        userModel.UserId = userid;
                        db.UsersDetails.Add(userModel);
                        db.SaveChanges();
                        ViewBag.Message = "User Added";
                    }
                }

            }
            return View();

        }
        public ActionResult ManageFaculty()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var query = db.Users.Join(db.UsersDetails, u => u.UserId, ud => ud.UserId, (x, y) => new
                {
                    Use = x,
                    Used = y,
                }).Select(s => new ManageFaculty
                {
                    UserId = s.Use.UserId,
                    FullName = s.Used.FirstName + " " + s.Used.LastName,
                    UserName = s.Use.UserName,
                    Mobile = s.Used.Mobile,
                    Email = s.Used.Email,
                    Status = s.Use.Status
                }).ToList();
                
                return View(query);
            }

        }

        public ActionResult Block(int id)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var Isblock = db.Users.Where(x => (x.UserId == id && x.Status == true));
                User u = db.Users.Find(id);
                if (Isblock.Count() > 0)
                {
                    updateUserStatus(id, db, u, false);
                }
                else
                {

                    updateUserStatus(id, db, u, true);
                }
            }
            return RedirectToAction("ManageFaculty", "Admin");
        }
        public ActionResult CreateMeeting()
        {
            GetTopicsForCurrentPost();
            return View();
        }
        [HttpPost]
        public ActionResult CreateMeeting(Meeting addNewMeeting)
        {
            GetTopicsForCurrentPost();
            if(ModelState.IsValid)
            {
                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    string meetingCreatedBy = String.Empty;
                    if (User.IsInRole("Principle"))
                    {
                        meetingCreatedBy = "Principle";
                    }
                    else if (User.IsInRole("HOD")) { meetingCreatedBy = "HOD"; }
                    addNewMeeting.CreatedBy = meetingCreatedBy;
                    db.Meetings.Add(addNewMeeting);
                    db.SaveChanges();
                    ViewBag.Message = "Meeting Added";
                }
            }
            return Json(new { status = true , message="Meeting Created successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AproveLeave()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var query = db.Leaves.Join(db.UsersDetails, l => l.UserId, u => u.UserId, (x, y) => new
                {
                    Leav = x,
                    Use = y,

                }).Where(s => (s.Leav.Status != true && s.Leav.Status != false)).Select(s => new ListLeaves
                {
                    LeaveId = s.Leav.LeaveId,
                    FullName = s.Use.FirstName + " " + s.Use.LastName,
                    ImagePath = s.Use.ImagePath,
                    Reason = s.Leav.Reason,
                    LeavePeriod = s.Leav.LeavePeriod,
                    Status = s.Leav.Status,
                }).ToList();
                return View(query);
            }
        }
        public ActionResult LeaveApproved(Leaf leave , int id)
        {
            
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                leave = db.Leaves.Find(id);
                leave.Status = true;
                db.Entry(db.Leaves.Where(x => x.LeaveId == id).FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AproveLeave", "Admin");
            }
        }
        public ActionResult LeaveReject(Leaf leave, int id)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                leave = db.Leaves.Find(id);
                leave.Status = false;
                db.Entry(db.Leaves.Where(x => x.LeaveId == id).FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AproveLeave", "Admin");
            }
        }
        private void GetRolesForCurrentUsers()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var item = new SelectList(db.webpages_Roles.Where(s=>s.RoleName !="Principle").ToList(), "RoleName", "RoleName");
                ViewData["Roles"] = item;
            }
        }
        private static void updateUserStatus(int id, WorkFlowDbContext db, User u, bool _status)
        {
            u.Status = _status;
            db.Entry(db.Users.Where(x => x.UserId == id).FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        private void GetTopicsForCurrentPost()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var item = new SelectList(db.Topics.ToList(), "TopicId", "TopicName");
                ViewData["Topics"] = item;

                var fullNAme = new SelectList(db.UsersDetails.Where(s=>s.UserId != WebSecurity.CurrentUserId).ToList(), "UserId", "FirstName");
                ViewData["Users"] = fullNAme;
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

        public ActionResult ApprovePaper()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                obj.GetFiles();
                var NewQuery = db.Papers.Join(db.UsersDetails, p => p.UserId, u => u.UserId, (p, u) => new { p, u }).Select(f => new PaperModel
                {
                    PaperId = f.p.PaperId,
                    Name = f.u.FirstName,
                    PaperStandard = f.p.PaperStandard,
                    PaperSubject = f.p.PaperSubject,
                    FilePath = f.p.FilePath,
                    PaperStatus = f.p.PaperStatus,


                }).Where(s => (s.PaperStatus != true && s.PaperStatus != false)).ToList();

                return View(NewQuery);
            }
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

        public ActionResult PaperApproved(Paper pap, int id)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                pap = db.Papers.Find(id);
                pap.PaperStatus = true;
                db.Entry(db.Papers.Where(x => x.PaperId== id).FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ApprovePaper", "Admin");
            }
        }
        public ActionResult PaperReject(Paper pap, int id)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                pap = db.Papers.Find(id);
                pap.PaperStatus = false;
                db.Entry(db.Papers.Where(x => x.PaperId == id).FirstOrDefault()).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ApprovePaper", "Admin");
            }
        }
    }
}
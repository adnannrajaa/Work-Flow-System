using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.ViewModels.Account;


namespace WorkFlowProject.Controllers
{
    public class AccountController : Controller
    {
      
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin userLogin)
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                if (ModelState.IsValid)
                {
                    var Isblock = db.Users.Where(x => (x.UserName == userLogin.UserName && x.Status == false));

                    bool isAthenciated = WebSecurity.Login(userLogin.UserName, userLogin.Password);
                    if (isAthenciated)
                    {
                        if (Isblock.Count() == 0)
                        {
                            string returnUrl = Request.QueryString["ReturnUrl"];
                            var UserAccount = db.Users.Where(s => s.UserName == userLogin.UserName).Select(f=>f.Account).FirstOrDefault();
                            if (returnUrl == null)
                            {
                                if (UserAccount == "Principle")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Employee");
                                }
                            }
                            else
                            {
                                if (UserAccount == "Principle")
                                {
                                    return Redirect(Url.Content(returnUrl));
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Employee");
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Your Account is block by Admin");
                        }


                    }
                    else
                    {
                        ModelState.AddModelError("", "User Name or password is invalid");
                    }
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            WebSecurity.Logout();
            return RedirectToAction("Login", "Account");
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
        [HttpPost]
        public ActionResult updateViewdNotification(Meeting m)
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                
                var status = db.Meetings.Where(s=>(s.UserId == WebSecurity.CurrentUserId && s.IsViewd !=true)).Select(s => s.IsViewd).FirstOrDefault();
                if(status != true)
                {
                    var id = db.Meetings.Where(s => (s.UserId == WebSecurity.CurrentUserId && s.IsViewd != true)).Select(s => s.MeetingId).ToList();
                    foreach(var itemid in id)
                    {
                        m = db.Meetings.Find(itemid);
                        m.IsViewd = true;
                        db.Entry(m).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                   

                    return Json(new { success = true  }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult UserProfile()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var data = db.UsersDetails.Where(x => x.UserId == WebSecurity.CurrentUserId).Select( s=> new UserDetail {
                    UserDetailId = s.UserDetailId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Mobile = s.Mobile,
                    UserAddress = s.UserAddress,
                    About = s.About,
                    ImagePath = s.ImagePath


                }).FirstOrDefault();
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult UserProfile(UserDetail UsersModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var imageUpdate = "";
                    if (UsersModel.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(UsersModel.ImageUpload.FileName);
                        string extension = Path.GetExtension(UsersModel.ImageUpload.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        UsersModel.ImagePath = "~/AppFiles/Images/" + fileName;
                        UsersModel.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                    }

                    else
                    {
                        using (WorkFlowDbContext db = new WorkFlowDbContext())
                        {
                            imageUpdate = db.UsersDetails.Where(s => s.UserId == WebSecurity.CurrentUserId).Select(s => s.ImagePath).FirstOrDefault();
                        }
                    }
            

                   

                    using (WorkFlowDbContext db = new WorkFlowDbContext())
                    {
                        if(UsersModel.ImageUpload == null)
                        {
                            UsersModel.ImagePath = imageUpdate;
                        }
                        UsersModel.UserId = WebSecurity.CurrentUserId;
                        UsersDetail ud = new UsersDetail();
                        ud.UserDetailId = UsersModel.UserDetailId;
                        ud.UserId = UsersModel.UserId;
                        ud.FirstName = UsersModel.FirstName;
                        ud.LastName = UsersModel.LastName;
                        ud.Email = UsersModel.Email;
                        ud.Mobile = UsersModel.Mobile;
                        ud.UserAddress = UsersModel.UserAddress;
                        ud.About = UsersModel.About;
                        ud.ImagePath = UsersModel.ImagePath;
                        db.Entry(ud).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Message = "Your profile has been updated!";


                    }

                }
                else
                {
                   
                    return View(UsersModel);
                }

                return View(UsersModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangePassword(ChangePasswordModel ResetPassword)
        {
            if (ModelState.IsValid)
            {
                bool isPasswordChange = WebSecurity.ChangePassword(WebSecurity.CurrentUserName, ResetPassword.OldPassword, ResetPassword.NewPassword);
                if (isPasswordChange)
                {
                    return Json(new { success = true, message = "Password Changed Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Old Password is not correct" }, JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("UserProfile", "Account");
        }


    }
       
}
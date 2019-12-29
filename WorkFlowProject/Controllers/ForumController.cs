using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.ViewModels.Account;
using WorkFlowProject.ViewModels.Forum;

namespace WorkFlowProject.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        // GET: Forum
        public ActionResult AddPost()
        {
            GetTopicsForCurrentPost();
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var ListData = db.Topics.ToList();
                var dispaly = new PostTopicViewModel
                {
                    displayTopic = ListData
                };
                return View(dispaly);
            }
        }
        private void GetTopicsForCurrentPost()
        {
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var item = new SelectList(db.Topics.ToList(), "TopicId", "TopicName");
                ViewData["Topics"] = item;

                
            }
        }

        [HttpPost]
        public ActionResult AddPost(Post AddNewPost)
        {
            GetTopicsForCurrentPost();
            if (ModelState.IsValid)
            {
                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    AddNewPost.UserId = WebSecurity.CurrentUserId;
                    AddNewPost.CreatedTime = DateTime.Now.ToShortDateString();
                    AddNewPost.Status = true;
                    db.Posts.Add(AddNewPost);
                    db.SaveChanges();
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddTopic(string TopicName)
        {
            if(TopicName != "")
            {
                using (WorkFlowDbContext db = new WorkFlowDbContext())
                {
                    Topic AddTopicData = new Topic();
                    AddTopicData.TopicName = TopicName;
                    AddTopicData.CreatedBy = WebSecurity.CurrentUserName;
                    db.Topics.Add(AddTopicData);
                    db.SaveChanges();
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult AddNewComment(int PostId , string Description)
        {
            PostComment addnewcomment = new PostComment();
            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                string CurrentUserName = db.UsersDetails.Where(s => s.UserId == WebSecurity.CurrentUserId).Select(s=>(s.FirstName +" "+s.LastName)).FirstOrDefault();
                addnewcomment.PostId = PostId;
                addnewcomment.UserId = WebSecurity.CurrentUserId;
                addnewcomment.CommentContent = Description;
                addnewcomment.CommentTime = DateTime.Now.ToShortDateString();
                addnewcomment.Status = true;
                addnewcomment.UserFullName = CurrentUserName;
                db.PostComments.Add(addnewcomment);
                db.SaveChanges();
                var newComment = db.PostComments.Join(db.UsersDetails , pc => pc.UserId , ud => ud.UserId ,(pc,ud) => new { pc,ud}).OrderByDescending(p => p.pc.CommentId).Select(s => new { commentContent = s.pc.CommentContent , name =(s.ud.FirstName +" " + s.ud.LastName) }).FirstOrDefault();
                return Json(new { status = true, mycomment = newComment }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ViewPost()
        {
            WorkFlowDbContext db = new WorkFlowDbContext();
            var posts = from p in db.Posts
                        join u in db.UsersDetails on p.UserId equals u.UserId
                        join a in db.Users on p.UserId equals a.UserId
                        select new ShowPostViewModel
                        {
                            Post = p,
                            FirstName = u.FirstName + " " + u.LastName,
                            ImagePath = u.ImagePath,
                            Account = a.Account
                        };
            return View(posts.OrderByDescending(s=>s.Post.PostId).ToList());

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

        public ActionResult LikePost(int PostId)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {

                var isLike = db.PostLikes.Where(s => (s.PostLike1 == true && s.UserId == WebSecurity.CurrentUserId && s.PostId == PostId));
                var isdisLike = db.PostLikes.Where(s => (s.PostLike1 == false && s.UserId == WebSecurity.CurrentUserId && s.PostId == PostId));

                if (isdisLike.Count() > 0 && isLike.Count() == 0)
                {
                    updateLikeDislikePost(db, PostId, true);
                }
                else if (isLike.Count() > 0)
                {
                    return Json(new { success = true, message = "You already like this post" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    InsertRecord(db, PostId, true);
                    int likescount = GetUpdatedLikeDisLikeCounts(PostId, db, true);
                    return Json(new { success = true, postLikeCount = likescount, postId = PostId }, JsonRequestBehavior.AllowGet);

                }
                int likecount = GetUpdatedLikeDisLikeCounts(PostId, db,true);
                int Dislikecount = GetUpdatedLikeDisLikeCounts(PostId, db, false);
                //return RedirectToAction("ViewPost","Forum");
                return Json(new { success = true, postLikeCount = likecount, dislikecount = Dislikecount, postId = PostId }, JsonRequestBehavior.AllowGet);

            }

        }

        private static int GetUpdatedLikeDisLikeCounts(int PostId, WorkFlowDbContext db,bool status)
        {
            return db.PostLikes.Where(s => (s.PostId == PostId && s.PostLike1 == status)).ToList().Count();
        }

        public ActionResult DislikePost(int PostId)
        {

            using (WorkFlowDbContext db = new WorkFlowDbContext())
            {
                var isdisLike = db.PostLikes.Where(s => (s.PostLike1 == false && s.UserId == WebSecurity.CurrentUserId && s.PostId == PostId));
                var isLike = db.PostLikes.Where(s => (s.PostLike1 == true && s.UserId == WebSecurity.CurrentUserId && s.PostId == PostId));

                if (isLike.Count() > 0 && isdisLike.Count() == 0)
                {
                    updateLikeDislikePost(db, PostId, false);
                }
                else if (isdisLike.Count() > 0)
                {
                    //return Json(new { success = true, message = "You already Dislike this post" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    InsertRecord(db, PostId, false);

                }
                int likecount = GetUpdatedLikeDisLikeCounts(PostId, db, true);
                int Dislikecount = GetUpdatedLikeDisLikeCounts(PostId, db, false);
                return Json(new { success = true, dislikecount = Dislikecount, postLikeCount = likecount, postId = PostId }, JsonRequestBehavior.AllowGet);

            }



        }

        private void updateLikeDislikePost(WorkFlowDbContext db, int PId, bool v)
        {
            var UpdateId = db.PostLikes.Where(x => (x.PostId == PId && x.UserId == WebSecurity.CurrentUserId)).FirstOrDefault();
            UpdateId.PostLike1 = v;
            db.SaveChanges();
        }

        private void InsertRecord(WorkFlowDbContext db, int likeId ,bool status)
        {
            PostLike NewLikeDislike = new PostLike();
            NewLikeDislike.PostId = likeId;
            NewLikeDislike.UserId = WebSecurity.CurrentUserId;
            NewLikeDislike.PostLike1 = status;

            db.PostLikes.Add(NewLikeDislike);
            db.SaveChanges();
        }

        public ActionResult AllPost()
        {
            WorkFlowDbContext db = new WorkFlowDbContext();
            var posts = from p in db.Posts
                        join u in db.UsersDetails on p.UserId equals u.UserId
                        join a in db.Users on p.UserId equals a.UserId
                        select new ShowPostViewModel
                        {
                            Post = p,
                            FirstName = u.FirstName +" "+ u.LastName,
                            ImagePath = u.ImagePath,
                            Account = a.Account
                        };
            return View(posts.ToList());
        }
      
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Forum;

namespace WorkFlowProject.Models.Home
{
    public class TotalLikesPostsCommentsModel
    {
        public int totalPostList { get; set; }
        public int totalLikesList { get; set; }
        public int totalCommentsList { get; set; }
        public int totalUsersList { get; set; }

        public int totalProjects { get; set; }

        public IList<ManageFaculty> ActiveUsers { get; set; }
        public IList<CommentModel> commentModel { get; set; }

        public IList<ProjectListModel> ProjectModel { get; set; }

        public IList<ProjectTasKModel> projectTaskModel { get; set; }



        public Project CreateNewProject { get; set; }
        public static TotalLikesPostsCommentsModel GetRecord(WorkFlowDbContext db)
        {
            var totalPosts = db.Posts.Where(s=>s.Status==true).ToList().Count();
            var totalLikes = db.PostLikes.Where(s=>s.PostLike1==true).ToList().Count();
            var totalComments = db.PostComments.Where(s => s.Status == true).ToList().Count();
            var totalUsers = db.Users.Where(s => s.Status == true).ToList().Count();

            var activeUsers = db.Users.Join(db.UsersDetails, u => u.UserId, ud => ud.UserId, (x, y) => new
            {
                Use = x,
                Used = y,
            }).Where(s => s.Use.Status == true).Select(s => new ManageFaculty
            {
                UserId = s.Use.UserId,
                FullName = s.Used.FirstName + " " + s.Used.LastName,
                UserName = s.Use.UserName,
                Mobile = s.Used.Mobile,
                Email = s.Used.Email,
                ImagePath = s.Used.ImagePath,
                Status = s.Use.Status,

            }).ToList();

            var CommentResult = db.PostComments.Join(db.Users, pc => pc.UserId, u => u.UserId, (x, y) => new
            {
                pcom = x,
                use = y
            }).Join(db.UsersDetails, us => us.use.UserId, ud => ud.UserId, (us, ud) => new
            {
                us.pcom,
                us.use,
                ud


            }).Where(s => s.pcom.Status != false).Select(f => new CommentModel
            {
                CommentId = f.pcom.CommentId,
                PostId = f.pcom.PostId,
                UserName = f.ud.FirstName + " " + f.ud.LastName,
                Account = f.use.Account,
                CommentContent = f.pcom.CommentContent,
                CommentTime = f.pcom.CommentTime,
                ImagePath = f.ud.ImagePath

            }).ToList();

            var projectResult = db.Projects.Join(db.UsersDetails, p => p.UserId, u => u.UserId, (p, u) => new
            { p, u }).Where(s => s.p.Status != false).Select(f => new ProjectListModel
            {
                ProjectId = f.p.ProjectId,
                FilePath = f.p.FilePath,
                FullName = f.u.FirstName,
                status = f.p.Status,
            }).Take(3).ToList();
            var projectsCompleted = db.Projects.ToList().Count();


            var projectTaskDetail = db.ProjectTasks.Join(db.UsersDetails, s => s.UserId, u => u.UserId, (s, u) => new
            { s, u }).Select(f => new ProjectTasKModel
            {
                FullName = f.u.FirstName,
                TaskTitle = f.s.TaskTitle,
            }).ToList();

            var ReturnRecordData = new TotalLikesPostsCommentsModel
            {
                totalPostList = totalPosts,
                totalLikesList = totalLikes,
                totalCommentsList = totalComments,
                totalUsersList = totalUsers,
                ActiveUsers = activeUsers,
                commentModel = CommentResult,
                ProjectModel = projectResult,
                totalProjects = projectsCompleted,
                projectTaskModel = projectTaskDetail

            };
            return ReturnRecordData;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Employee;
using WorkFlowProject.Models.Forum;
using WorkFlowProject.Models.Home;

namespace WorkFlowProject.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public int totalPostList { get; set; }
        public int totalLikesList { get; set; }
        public int totalCommentsList { get; set; }
        public int totalUsersList { get; set; }

        public int totalProjects { get; set; }

        public IList<ManageFaculty> ActiveUsers { get; set; }
        public IList<CommentModel> commentModel { get; set; }

        public IList<Project> ProjectModel { get; set; }

        public IList<TaskModel> projectTaskModel { get; set; }



        public ProjectTask CreateNewProject { get; set; }
        public static EmployeeViewModel GetRecord(WorkFlowDbContext db)
        {
            var totalPosts = db.Posts.Where(s => s.Status == true).ToList().Count();
            var totalLikes = db.PostLikes.Where(s => s.PostLike1 == true).ToList().Count();
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

            var projectResult = db.Projects.Where(s=>s.UserId == WebSecurity.CurrentUserId).ToList();

            var projectsCompleted = db.Projects.ToList().Count();

            

            var projectTaskDetail =  db.ProjectTasks.Join(db.Projects, s => s.ProjectId, u => u.ProjectId, (s, u) => new
            { s, u }).Select(f => new TaskModel
            {
                TaskId = f.s.TaskId,
                ProjectTitle = f.u.ProjectTitle,
                TaskTitle = f.s.TaskTitle,
                DueDate = f.s.DueDate,
                Description = f.s.Description,
                Status = f.s.Status
            }).ToList();

            var ReturnRecordData = new EmployeeViewModel
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
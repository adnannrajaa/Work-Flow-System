using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.DataBaseModel;
using WorkFlowProject.Models.Home;
using static System.Net.WebRequestMethods;

namespace WorkFlowProject.ViewModels.Account
{
    public class NotificationViewModel
    {
        public UsersDetail userModel { get; set; }
        public int totalCount { get; set; }
        public IList<MeetingModel> meetingModel { get; set; }
        public static NotificationViewModel UserNotifyList(WorkFlowDbContext db, int id)
        {
            var UserResult = db.UsersDetails.Where(s => s.UserId == id).FirstOrDefault();

            var MeetingResult = db.Users.Join(db.Meetings, ud => ud.UserId, m => m.UserId, (ud, m) => new
            {
                Use = ud,
                meet = m
            }).Select(s => new MeetingModel
            {
                MeetingId = s.meet.MeetingId,
                UserId = s.meet.UserId,
                MeetingPoints = s.meet.MeetingPoints,
                MeetingTime = s.meet.MeetingTime,
                MeetingCreatedBy = s.meet.CreatedBy,
                IsViewd = s.meet.IsViewd,
            }).Where(s => s.UserId == id).OrderByDescending(s=>s.MeetingId).Take(5).ToList();
             var totalNotification = MeetingResult.Where(s=>s.IsViewd !=true).Count();
           
            var UserNotifyList = new NotificationViewModel
            {
                userModel = UserResult,
                meetingModel = MeetingResult,
                totalCount = totalNotification

            };
            return UserNotifyList;
        }
    }
}
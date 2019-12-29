using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Home
{
    public class MeetingModel
    {
        public int? MeetingId { get; set; }
        public int? UserId { get; set; }
        public string MeetingCreatedBy { get; set; }

        public string MeetingPoints { get; set; }
        public string MeetingTime { get; set; }

        public Nullable<bool> IsViewd { get; set; }



    }
}
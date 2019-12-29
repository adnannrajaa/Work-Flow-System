using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.Models.Forum
{
    public class PostModel
    {
        public int PostId { get; set; }
        public string FullName { get; set; }
        public string ImagePath { get; set; }
        public string Account { get; set; }
        public string CreatedTime { get; set; }
        public string PostContent { get; set; }

        public int PostLikes { get; set; }
    }
}
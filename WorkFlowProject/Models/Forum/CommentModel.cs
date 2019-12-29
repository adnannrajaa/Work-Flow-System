using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Forum
{
    public class CommentModel
    {
        public int? CommentId { get; set; }
        public int? PostId { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public string CommentContent { get; set; }
        public string CommentTime { get; set; }
        public string ImagePath { get; set; }

    }
}
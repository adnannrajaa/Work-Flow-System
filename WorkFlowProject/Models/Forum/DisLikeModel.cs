using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Forum
{
    public class DisLikeModel
    {
        public int? PostId { get; set; }
       // public Nullable<bool> TotalLikes { get; set; }
         public int TotalDisLikes { get; set; }
    }
}
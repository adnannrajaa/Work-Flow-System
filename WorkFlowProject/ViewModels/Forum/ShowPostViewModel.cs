using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.ViewModels.Forum
{
    public class ShowPostViewModel
    {
        public Post Post { get; set; }
        public string FirstName { get; set; }
        public string ImagePath { get; set; }
        public string Account { get; set; }

    }
}
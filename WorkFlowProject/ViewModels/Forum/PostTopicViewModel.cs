using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.ViewModels.Forum
{
    public class PostTopicViewModel
    {
        public Post addPost { get; set; }
        public IList<Topic> displayTopic { get; set; }
        public Topic insertTopic { get; set; }
    }
}
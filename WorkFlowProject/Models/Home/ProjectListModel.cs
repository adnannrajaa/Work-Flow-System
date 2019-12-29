using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Home
{
    public class ProjectListModel
    {
        public int? ProjectId { get; set; }
        [DisplayName("File")]
        public string FilePath { get; set; }

        [DisplayName("Assigned to")]
        public string  FullName { get; set; }
        [DisplayName("Status")]
        public  Nullable<bool> status { get; set; }
    }
}
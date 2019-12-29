using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Employee
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string ProjectTitle { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        
        public Nullable<bool> Status { get; set; }

       
    }
}
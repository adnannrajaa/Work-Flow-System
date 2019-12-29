using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Account
{
    public class ManageFaculty
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ImagePath { get; set; }
        public bool? Status { get; set; }


    }
}
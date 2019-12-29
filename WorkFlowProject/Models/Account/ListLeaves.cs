using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Account
{
    public class ListLeaves
    {
        public int LeaveId { get; set; }
        [Display(Name ="Name")]
        public string  FullName { get; set; }
        [Display(Name = "Picture")]
        public string ImagePath { get; set; }
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Display(Name = "Leave Period")]
        public string LeavePeriod { get; set; }
        [Display(Name = "Status")]
        public bool? Status { get; set; }
    }
}
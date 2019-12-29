using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Account
{
    public class UserRegister
    {
        public int UserId { get; set; }
        [Display(Name ="User Name")]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        //[Display(Name = "Repeat Password")]
        //[Required(ErrorMessage = "Repeat Password is required.")]
        //[Compare(otherProperty: "Password" , ErrorMessage ="Password did not matched") ]
        //public string RepeatPassword { get; set; }

        public string Account { get; set; }
        public Nullable<bool> Status { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
    }
}
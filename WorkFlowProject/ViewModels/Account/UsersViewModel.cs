using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlowProject.Models.Account;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.ViewModels.Account
{
    public class UsersViewModel
    {
        public UsersDetail userDetail { get; set; }
        public UserRegister userRegister { get; set; }

    }
}
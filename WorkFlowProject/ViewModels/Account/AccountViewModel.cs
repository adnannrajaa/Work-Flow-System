using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.ViewModels.Account
{
    public class AccountViewModel
    {
        public IList<webpages_Roles> rolesModel { get; set; }
        public IList<User> registerModel { get; set; }
    }
}
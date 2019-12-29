using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;

namespace WorkFlowProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            initializeMemberShip();
        }

        private void initializeMemberShip()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("dbcon", "Users", "UserId", "UserName", true);
                //WebSecurity.CreateUserAndAccount("Admin", "Admin");
                //Roles.CreateRole("Principle");
                //Roles.CreateRole("HOD");
                //Roles.CreateRole("Staff");
                ////Roles.CreateRole("Alumni");
                //Roles.AddUserToRole("Admin", "Principle");

            }
        }
    }
}

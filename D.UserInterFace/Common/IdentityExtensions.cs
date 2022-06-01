using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace D.UserInterFace
{
    public static class IdentityExtensions
    {
        public static IEmployee AppsUser;
        public static string[] EmpRights;

        public static string GetFullName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static void UserInfo(this IIdentity identity)
        {
            EmployeeDAL dal = new EmployeeDAL();
            AppsUser = dal.GetByAppLogin(identity.Name);
            string[] data = AppsUser.EmpRights.Select(x => x.Code).ToArray();
            EmpRights = data!=null?data:new string[0];
        }
        
    }
    
}
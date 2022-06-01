using System.Web.Mvc;

namespace D.UserInterFace.Areas.SCM
{
    public class SCMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SCM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SCM_default",
                "SCM/{controller}/{action}/{id}",
                new {controller="Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
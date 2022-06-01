using System.Web.Mvc;

namespace D.UserInterFace.Areas.ATM
{
    public class ATMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ATM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ATM_default",
                "ATM/{controller}/{action}/{id}",
                new { controller="Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
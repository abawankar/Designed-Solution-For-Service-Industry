using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(D.UserInterFace.Startup))]
namespace D.UserInterFace
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

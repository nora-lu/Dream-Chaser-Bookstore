using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GaryBookstore.Web.Startup))]
namespace GaryBookstore.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

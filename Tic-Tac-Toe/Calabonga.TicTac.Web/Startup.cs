using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Calabonga.TicTac.Web.Startup))]
namespace Calabonga.TicTac.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

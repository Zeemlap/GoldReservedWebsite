using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TonsOfGoldPerCountry.Web.Startup))]
namespace TonsOfGoldPerCountry.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

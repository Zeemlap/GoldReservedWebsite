using GoldReserves.Data;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using System.Linq;

[assembly: OwinStartup(typeof(GoldReserves.Web.Startup))]
namespace GoldReserves.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.CreatePerOwinContext(() =>
            {
                var connStrConfig2 = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Single(connStrConfig1 => connStrConfig1.Name == "GoldReserves");
                var connStr = connStrConfig2.ConnectionString;
                GoldReservesDbContext d = null;
                bool ex = true;
                try
                {
                    d = new GoldReservesDbContext(connStr);
                    ex = false;
                }
                finally
                {
                    if (ex)
                    {
                        d.Dispose();
                    }
                }
                return d;
            });
        }
    }
}

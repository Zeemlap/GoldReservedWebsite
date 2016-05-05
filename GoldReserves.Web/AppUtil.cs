using GoldReserves.Data;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace GoldReserves.Web
{
    public class AppUtil
    {

        public static GoldReservesDbContext DbContext
        {
            get
            {
                var oc = HttpContext.Current.GetOwinContext();
                return oc.Get<GoldReservesDbContext>();
            }
        }

    }
}
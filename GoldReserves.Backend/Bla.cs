using GoldReserves.Data;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class Bla
    {   
        public async Task<WorldOfficialGoldHoldingReport> GetWorldGoldReservesReportAsync()
        {
            return await new WorldOfficialGoldHoldingReportScraper().RunAsync();
        }
    }
}

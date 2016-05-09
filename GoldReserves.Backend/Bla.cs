using GoldReserves.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class Bla
    {   
        public List<WorldOfficialGoldHoldingReport_Raw> GetWorldGoldReservesReports_UpTo2000()
        {
            return new WorldOfficialGoldHoldingReportsScraper_UpTo2000().Run();
        }

        // The tons of gold is AT THE END OF EACH QUARTER (to retrieve last known tons of gold at quarter 2, look at quarter 1).
        public List<WorldOfficialGoldHoldingReport_Raw> GetWorldGoldReservesReports_2000To2015()
        {
            return new WorldOfficialGoldHoldingReportsScraper_2000To2015().Run();
        }

        // We're assuming the same time blabla holds at GetWorldGoldReservesReports_2000To2015.
        public async Task<WorldOfficialGoldHoldingReport_Raw> GetWorldGoldReservesReportAsync_Latest()
        {
            return await new WorldOfficialGoldHoldingReportScraper_2016AndOnwards().RunAsync();
        }
    }
}

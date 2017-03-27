using System;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class ServerReporter : ReportMaker<BaseServerStatistics, Tuple<int, int>, ServerAverageMatchesPerDayRateStat>
    {
        public ServerReporter(IService<BaseServerStatistics> service) : base(service)
        {
        }

        public override bool Filter(BaseServerStatistics arg)
        {
            return true;
        }

        public override ServerAverageMatchesPerDayRateStat Transformer(BaseServerStatistics arg)
        {
            int totalDaysCount = Math.Max(arg.LastMatchPlayed.Subtract(arg.FirstMatchPlayed).Days, 1);
            return new ServerAverageMatchesPerDayRateStat(arg.EndPoint, arg.Name, (double)arg.TotalMatchesPlayed / totalDaysCount);
        }

        public override Tuple<int, int> Selector(BaseServerStatistics arg)
        {
            int totalDaysCount = Math.Max(arg.LastMatchPlayed.Subtract(arg.FirstMatchPlayed).Days, 1);
            return Tuple.Create(totalDaysCount, arg.TotalMatchesPlayed);
        }
    }
}

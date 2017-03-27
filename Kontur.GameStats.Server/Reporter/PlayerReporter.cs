using System;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class PlayerReporter : ReportMaker<BasePlayerStatistics, Tuple<int, int>, PlayersKillToDeathRateStat>
    {
        public PlayerReporter(IService<BasePlayerStatistics> service) : base(service)
        {
        }

        public override bool Filter(BasePlayerStatistics arg)
        {
            return arg.TotalMatchesPlayed >= 10 && arg.TotalDeaths != 0;
        }

        public override PlayersKillToDeathRateStat Transformer(BasePlayerStatistics arg)
        {
            return new PlayersKillToDeathRateStat(arg.Name, (double)arg.TotalKills / arg.TotalDeaths);
        }

        public override Tuple<int, int> Selector(BasePlayerStatistics arg)
        {
            return Tuple.Create(arg.TotalKills, arg.TotalDeaths);
        }
    }
}
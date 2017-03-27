using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public interface IStatisticController
    {
        void RecalculateStatsByAdditionalMatch(Match match);
        ServerStatistics GetServerStats(string endpoint);
        PlayerStatistics GetPlayerStats(string playerName);
    }
}
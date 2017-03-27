using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class ServerStatistics
    {
        public readonly int totalMatchesPlayed;
        public readonly int maximumMatchesPerDay;
        public readonly double averageMatchesPerDay;
        public readonly int maximumPopulation;
        public readonly double averagePopulation;
        public readonly IReadOnlyCollection<string> top5GameModes;
        public readonly IReadOnlyCollection<string> top5Maps;

        public ServerStatistics(BaseServerStatistics stats)
        {
            this.totalMatchesPlayed = stats.TotalMatchesPlayed;
            this.maximumMatchesPerDay = stats.MaximumMatchesPerDay;
            this.averageMatchesPerDay = (double) stats.TotalMatchesPlayed / Math.Max(stats.LastMatchPlayed.Subtract(stats.FirstMatchPlayed).Days, 1);
            this.maximumPopulation = stats.MaximumPopulation;
            this.averagePopulation = (double) stats.TotalPlayersInMatches / stats.TotalMatchesPlayed;
            this.top5GameModes = stats.TopGameModes.Take(5).Select(pair => pair.Key).ToList().AsReadOnly();
            this.top5Maps = stats.TopMaps.Take(5).Select(pair => pair.Key).ToList().AsReadOnly();
        }
    }
}
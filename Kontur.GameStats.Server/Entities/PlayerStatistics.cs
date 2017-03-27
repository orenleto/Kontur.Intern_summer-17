using System;
using System.Linq;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class PlayerStatistics
    {
        public readonly int totalMatchesPlayed;
        public readonly int totalMatchesWon;
        public readonly string favoriteServer;
        public readonly int uniqueServers;
        public readonly string favoriteGameMode;
        public readonly double averageScoreboardPercent;
        public readonly int maximumMatchesPerDay;
        public readonly double averageMatchesPerDay;
        public readonly DateTime lastMatchPlayed;
        public readonly double killToDeathRatio;

        public PlayerStatistics(BasePlayerStatistics stats)
        {
            totalMatchesPlayed = stats.TotalMatchesPlayed;
            totalMatchesWon = stats.TotalMatchesWon;
            favoriteServer = stats.Servers
                .FirstOrDefault(pair => pair.Value == stats.Servers.Max(valuePair => valuePair.Value)).Key;
            uniqueServers = stats.UniqueServers;
            favoriteGameMode = stats.GameModes
                .FirstOrDefault(pair => pair.Value == stats.GameModes.Max(valuePair => valuePair.Value)).Key;
            averageScoreboardPercent = stats.TotalScoreboardPercent / stats.TotalMatchesPlayed;
            maximumMatchesPerDay = stats.MaximumMatchesPerDay;
            averageMatchesPerDay = stats.TotalMatchesPlayed / Math.Max(Math.Ceiling(stats.LastMatchPlayed.Subtract(stats.FirstMatchPlayed).TotalDays), 1.0);
            lastMatchPlayed = stats.LastMatchPlayed;
            killToDeathRatio = (double)stats.TotalKills / stats.TotalDeaths;
        }
    }
}
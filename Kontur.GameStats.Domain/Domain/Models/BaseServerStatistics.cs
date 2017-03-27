using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Kontur.GameStats.Server;

namespace Kontur.GameStats.Domain
{
    public class BaseServerStatistics : IStatistics
    {
        public string EndPoint { get; private set; }
        public string Name { get; private set; }
        public int TotalMatchesPlayed { get; private set; }
        public int TotalMatchesToday { get; private set; }
        public int MaximumMatchesPerDay { get; private set; }
        public int MaximumPopulation { get; private set; }
        public int TotalPlayersInMatches { get; private set; }
        public DateTime LastMatchPlayed { get; private set; }
        public DateTime FirstMatchPlayed { get; private set; }
        public IDictionary<string, int> TopGameModes { get; private set; }
        public IDictionary<string, int> TopMaps { get; private set; }

        private BaseServerStatistics()
        {
            EndPoint = string.Empty;
            Name = string.Empty;
            TotalMatchesPlayed = 0;
            TotalMatchesToday = 0;
            MaximumMatchesPerDay = 0;
            MaximumPopulation = 0;
            TotalPlayersInMatches = 0;
            LastMatchPlayed = FirstMatchPlayed = DateTime.UtcNow;
            TopGameModes = new Dictionary<string, int>();
            TopMaps = new Dictionary<string, int>();
        }
        public BaseServerStatistics(string endpoint, string name)
        {
            EndPoint = endpoint;
            Name = name;
            TotalMatchesPlayed = 0;
            TotalMatchesToday = 0;
            MaximumMatchesPerDay = 0;
            MaximumPopulation = 0;
            TotalPlayersInMatches = 0;
            LastMatchPlayed = FirstMatchPlayed = DateTime.UtcNow;
            TopGameModes = new Dictionary<string, int>();
            TopMaps = new Dictionary<string, int>();
        }

        private BaseServerStatistics(string endPoint, string name, int totalMatchesPlayed, int totalMatchesToday, int maximumMatchesPerDay, int maximumPopulation, int totalPlayersInMatches, DateTime lastMatchPlayed, DateTime firstMatchPlayed, IDictionary<string, int> topGameModes, IDictionary<string, int> topMaps)
        {
            EndPoint = endPoint;
            Name = name;
            TotalMatchesPlayed = totalMatchesPlayed;
            TotalMatchesToday = totalMatchesToday;
            MaximumMatchesPerDay = maximumMatchesPerDay;
            MaximumPopulation = maximumPopulation;
            TotalPlayersInMatches = totalPlayersInMatches;
            LastMatchPlayed = lastMatchPlayed;
            FirstMatchPlayed = firstMatchPlayed;
            TopGameModes = topGameModes;
            TopMaps = topMaps;
        }

        public BaseServerStatistics RecalculateWithAdditional(Match match)
        {
            return new BaseServerStatistics(
                endPoint: EndPoint,
                name: Name,
                totalMatchesPlayed: TotalMatchesPlayed + 1,
                totalMatchesToday: LastMatchPlayed.Date == match.Timestamp.Date ? TotalMatchesToday + 1 : 1,
                maximumMatchesPerDay: Math.Max(MaximumMatchesPerDay, LastMatchPlayed.Date == match.Timestamp.Date ? TotalMatchesToday + 1 : 1),
                maximumPopulation: Math.Max(MaximumPopulation, match.Results.Scoreboard.Count()),
                totalPlayersInMatches: TotalPlayersInMatches + match.Results.Scoreboard.Count(),
                firstMatchPlayed: TotalMatchesPlayed == 0 ? match.Timestamp : FirstMatchPlayed,
                lastMatchPlayed: match.Timestamp,
                topGameModes: TopGameModes.Increment(match.Results.GameMode),
                topMaps: TopMaps.Increment(match.Results.Map)
            );
        }

        public BaseServerStatistics Trim()
        {
            return new BaseServerStatistics()
            {
                EndPoint = EndPoint,
                Name = Name,
                TotalMatchesPlayed = TotalMatchesPlayed,
                TotalMatchesToday = TotalMatchesToday,
                TotalPlayersInMatches = TotalPlayersInMatches,
                MaximumPopulation = MaximumPopulation,
                MaximumMatchesPerDay = MaximumMatchesPerDay,
                FirstMatchPlayed = FirstMatchPlayed,
                LastMatchPlayed = LastMatchPlayed,
                TopGameModes = TopGameModes
                        .OrderByDescending(pair => pair.Value)
                        .Take(5)
                        .ToDictionary(pair => pair.Key, pair => pair.Value),
                TopMaps = TopMaps
                        .OrderByDescending(pair => pair.Value)
                        .Take(5)
                        .ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server;

namespace Kontur.GameStats.Domain
{
    public class BasePlayerStatistics : IStatistics
    {
        public string Name { get; private set; }
        public int TotalMatchesPlayed { get; private set; }
        public int TotalMatchesWon { get; private set; }
        public int TotalKills { get; private set; }
        public int TotalDeaths { get; private set; }
        public int TotalMatchesToday { get; private set; }
        public int MaximumMatchesPerDay { get; private set; }
        public int UniqueServers { get; private set; }
        public double TotalScoreboardPercent { get; private set; }
        public DateTime FirstMatchPlayed { get; private set; }
        public DateTime LastMatchPlayed { get; private set; }
        public IDictionary<string, int> Servers { get; private set; }
        public IDictionary<string, int> GameModes { get; private set; }

        private BasePlayerStatistics()
        {
            Name = string.Empty;
            TotalMatchesPlayed = 0;
            TotalMatchesWon = 0;
            TotalKills = 0;
            TotalDeaths = 0;
            TotalMatchesToday = 0;
            MaximumMatchesPerDay = 0;
            UniqueServers = 0;
            TotalScoreboardPercent = .0;
            FirstMatchPlayed = LastMatchPlayed = DateTime.UtcNow;
            Servers = new Dictionary<string, int>();
            GameModes = new Dictionary<string, int>();
        }
        public BasePlayerStatistics(string name)
        {
            Name = name;
            TotalMatchesPlayed = 0;
            TotalMatchesWon = 0;
            TotalKills = 0;
            TotalDeaths = 0;
            TotalMatchesToday = 0;
            MaximumMatchesPerDay = 0;
            UniqueServers = 0;
            TotalScoreboardPercent = .0;
            FirstMatchPlayed = LastMatchPlayed = DateTime.UtcNow;
            Servers = new Dictionary<string, int>();
            GameModes = new Dictionary<string, int>();
        }

        public BasePlayerStatistics(string name, int totalMatchesPlayed, int totalMatchesWon, int totalKills, int totalDeaths, int totalMatchesToday, int maximumMatchesPerDay, int uniqueServers, double totalScoreboardPercent, DateTime firstMatchPlayed, DateTime lastMatchPlayed, IDictionary<string, int> servers, IDictionary<string, int> gameModes)
        {
            Name = name;
            TotalMatchesPlayed = totalMatchesPlayed;
            TotalMatchesWon = totalMatchesWon;
            TotalKills = totalKills;
            TotalDeaths = totalDeaths;
            TotalMatchesToday = totalMatchesToday;
            MaximumMatchesPerDay = maximumMatchesPerDay;
            UniqueServers = uniqueServers;
            TotalScoreboardPercent = totalScoreboardPercent;
            FirstMatchPlayed = firstMatchPlayed;
            LastMatchPlayed = lastMatchPlayed;
            Servers = servers;
            GameModes = gameModes;
        }


        public BasePlayerStatistics RecalculateWithAdditional(Match match)
        {
            var playerResult =
                match.Results.Scoreboard
                    .OrderBy(score => score)
                    .Select((score, index) => new { Position = index + 1, Name = score.Name, Kills = score.Kills, Deaths = score.Deaths })
                    .First(arg => arg.Name == Name);
            var totalPlayers = match.Results.Scoreboard.Count();
            var servers = Servers.Increment(match.Server);

            return new BasePlayerStatistics(
                name: Name,
                totalMatchesPlayed: TotalMatchesPlayed + 1,
                totalMatchesWon: TotalMatchesWon + playerResult.Position == 1 ? 1 : 0,
                totalKills: TotalKills + playerResult.Kills,
                totalDeaths: TotalDeaths + playerResult.Deaths,
                totalMatchesToday: LastMatchPlayed.Date == match.Timestamp.Date ? TotalMatchesToday + 1 : 1,
                uniqueServers: servers.Count,
                maximumMatchesPerDay: Math.Max(MaximumMatchesPerDay, LastMatchPlayed.Date == match.Timestamp.Date ? TotalMatchesToday + 1 : 1),
                totalScoreboardPercent: TotalScoreboardPercent + (double) (totalPlayers - playerResult.Position) / (totalPlayers - 1) * 100.0,
                firstMatchPlayed: TotalMatchesPlayed == 0 ? match.Timestamp : FirstMatchPlayed,
                lastMatchPlayed: match.Timestamp,
                servers: servers,
                gameModes: GameModes.Increment(match.Results.GameMode)
            );
        }

        public BasePlayerStatistics Trim()
        {
            return new BasePlayerStatistics
            {
                Name = Name,
                TotalMatchesPlayed = TotalMatchesPlayed,
                TotalMatchesWon = TotalMatchesWon,
                TotalMatchesToday = TotalMatchesToday,
                TotalKills = TotalKills,
                TotalDeaths = TotalDeaths,
                TotalScoreboardPercent = TotalScoreboardPercent,
                MaximumMatchesPerDay = MaximumMatchesPerDay,
                UniqueServers = UniqueServers,
                FirstMatchPlayed = FirstMatchPlayed,
                LastMatchPlayed = LastMatchPlayed,
                Servers = Servers
                        .OrderByDescending(pair => pair.Value)
                        .Take(1)
                        .ToDictionary(pair => pair.Key, pair => pair.Value),
                GameModes = GameModes
                        .OrderByDescending(pair => pair.Value)
                        .Take(1)
                        .ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }
    }
}

using System;
using System.Linq;

namespace Kontur.GameStats.Domain
{
    public class Match
    {
        public string Server { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MatchInfo Results { get; private set; }

        private Match()
        {
        }

        public Match(string server, DateTime timestamp, MatchInfo results)
        {
            if (results.TimeElapsed > results.TimeLimit)
                throw new Exception(string.Format("Match cannot durate more than {0}.", results.TimeLimit));
            if (results.FragLimit < results.Scoreboard.Max(score => score.Frags))
                throw new Exception(string.Format("One or more player has more than {0} frags.", results.FragLimit));
            if (results.Scoreboard.Sum(score => score.Kills) > results.Scoreboard.Sum(score => score.Deaths))
                throw new Exception("Sum of players Deaths less than summary Kills.");

            Server = server;
            Timestamp = timestamp;
            Results = results;
        }

        public override bool Equals(object obj)
        {
            var match = obj as Match;

            if (match == null)
                return false;

            return Server.Equals(match.Server)
                   && Timestamp.Equals(match.Timestamp)
                   && Results.Equals(match.Results);
        }

        public override int GetHashCode()
        {
            return Server.GetHashCode()
                   ^ Timestamp.GetHashCode()
                   ^ Results.GetHashCode();
        }
    }
}


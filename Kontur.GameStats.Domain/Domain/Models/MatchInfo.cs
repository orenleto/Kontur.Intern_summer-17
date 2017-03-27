using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Domain
{
    public class MatchInfo
    {
        public string Map { get; private set; }
        public string GameMode { get; private set; }
        public int FragLimit { get; private set; }
        public int TimeLimit { get; private set; }
        public double TimeElapsed { get; private set; }
        public IEnumerable<Score> Scoreboard { get; private set; }

        private MatchInfo()
        {
        }

        public MatchInfo(string map, string gameMode, int fragLimit, int timeLimit, double timeElapsed, IEnumerable<Score> scoreboard)
        {
            if (fragLimit < scoreboard.Max(score => score.Frags))
                throw new ArgumentException("Max frags in scoreboard should be less or equal to FragLimit");
            if (timeLimit < timeElapsed)
                throw new ArgumentException("Match duration should be less or equal to TimeLimit");
            if (scoreboard.Sum(score => score.Kills) > scoreboard.Sum(score => score.Deaths))
                throw new ArgumentException("Sum of Kills should be less or equal to sum of Deaths");

            Map = map;
            GameMode = gameMode;
            FragLimit = fragLimit;
            TimeLimit = timeLimit;
            TimeElapsed = timeElapsed;
            Scoreboard = scoreboard;
        }

        public override bool Equals(object obj)
        {
            var info = obj as MatchInfo;

            if (ReferenceEquals(info, null))
                return false;

            return Map.Equals(info.Map)
                   && GameMode.Equals(info.GameMode)
                   && FragLimit == info.FragLimit
                   && TimeLimit == info.TimeLimit
                   && TimeElapsed == info.TimeElapsed
                   && Scoreboard.SequenceEqual(info.Scoreboard);
        }

        public override int GetHashCode()
        {
            var scoreboardHash = 0;
            foreach (var score in Scoreboard)
            {
                scoreboardHash ^= score.GetHashCode();
            }

            return Map.GetHashCode()
                   ^ GameMode.GetHashCode()
                   ^ scoreboardHash
                   ^ FragLimit
                   ^ TimeLimit
                   ^ TimeElapsed.GetHashCode();
        }
    }
}
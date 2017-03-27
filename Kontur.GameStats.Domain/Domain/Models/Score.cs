using System;
using System.CodeDom;

namespace Kontur.GameStats.Domain
{
    public class Score : IComparable<Score>
    {
        public string Name { get; private set; }
        public int Frags { get; private set; }
        public int Kills { get;private set; }
        public int Deaths { get; private set; }

        private Score()
        {}

        public Score(string name, int frags, int kills, int deaths)
        {
            Name = name;
            Frags = frags;
            Kills = kills;
            Deaths = deaths;
        }

        public override bool Equals(object obj)
        {
            var score = obj as Score;

            if (ReferenceEquals(score, null))
                return false;

            return Name.Equals(score.Name)
                   && Frags == score.Frags
                   && Kills == score.Kills
                   && Deaths == score.Deaths;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Frags ^ Kills ^ Deaths;
        }

        public int CompareTo(Score other)
        {
            if (this.Frags == other.Frags)
            {
                if (this.Kills == other.Kills)
                {
                    return other.Deaths.CompareTo(Deaths);
                }
                return other.Kills.CompareTo(Kills);
            }
            return other.Frags.CompareTo(Frags);
        }
    }
}
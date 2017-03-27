namespace Kontur.GameStats.Server
{
    public class PlayersKillToDeathRateStat
    {
        public readonly string name;
        public readonly double killToDeathRatio;

        public PlayersKillToDeathRateStat(string name, double killToDeathRatio)
        {
            this.name = name;
            this.killToDeathRatio = killToDeathRatio;
        }
    }
}

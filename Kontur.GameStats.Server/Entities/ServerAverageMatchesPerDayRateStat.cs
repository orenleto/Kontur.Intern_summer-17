namespace Kontur.GameStats.Server
{
    public class ServerAverageMatchesPerDayRateStat
    {
        public readonly string endpoint;
        public readonly string name;
        public readonly double averageMatchesPerDay;

        public ServerAverageMatchesPerDayRateStat(string endpoint, string name, double averageMatchesPerDay)
        {
            this.endpoint = endpoint;
            this.name = name;
            this.averageMatchesPerDay = averageMatchesPerDay;
        }
    }
}

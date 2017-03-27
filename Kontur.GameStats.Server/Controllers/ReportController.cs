using System.Collections.Generic;
using System.Web.Http;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server.Controllers
{
    public class ReportController : ApiController
    {
        private IService<Match> matchService;
        private IService<BasePlayerStatistics> playerService;
        private IService<BaseServerStatistics> serverService;

        public ReportController(IService<Match> matchService, IService<BasePlayerStatistics> playerService, IService<BaseServerStatistics> serverService)
        {
            this.matchService = matchService;
            this.playerService = playerService;
            this.serverService = serverService;
        }

        [HttpGet]
        [ActionName("RecentMatches")]
        public IEnumerable<Match> GetRecentMatches(int count)
        {
            MatchReporter reporter = new MatchReporter(matchService);
            return reporter.Build(count);
        }

        [HttpGet]
        [ActionName("TopPlayers")]
        public IEnumerable<PlayersKillToDeathRateStat> GetTopPlayers(int count)
        {
            PlayerReporter reporter = new PlayerReporter(playerService);
            return reporter.Build(count);
        }

        [HttpGet]
        [ActionName("PopularServers")]
        public IEnumerable<ServerAverageMatchesPerDayRateStat> GetTopServers(int count)
        {
            ServerReporter reporter = new ServerReporter(serverService);
            return reporter.Build(count);
        }
    }
}

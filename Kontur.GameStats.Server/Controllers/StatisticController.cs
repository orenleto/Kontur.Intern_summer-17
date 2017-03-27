using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class StatisticController : ApiController, IStatisticController
    {
        private IServerService _serverService;
        private IService<BaseServerStatistics> _serverStatisticService;
        private IService<BasePlayerStatistics> _playerStatisticService;

        public StatisticController(IServerService serverService, IService<BaseServerStatistics> serverStatisticService, IService<BasePlayerStatistics> playerStatisticService)
        {
            _serverService = serverService;
            _serverStatisticService = serverStatisticService;
            _playerStatisticService = playerStatisticService;
        }

        public void RecalculateStatsByAdditionalMatch(Match match)
        {
            _serverStatisticService.Save(match);
            _playerStatisticService.Save(match);
        }
        [HttpGet]
        [ActionName("ServersStats")]
        public ServerStatistics GetServerStats(string endpoint)
        {
            try
            {
                _serverService.Get(endpoint);
            }
            catch (NullReferenceException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
            return new ServerStatistics(_serverStatisticService.Get(new ServerStatisticsServiceParameter(endpoint)));
             
        }
        [HttpGet]
        [ActionName("PlayersStats")]
        public PlayerStatistics GetPlayerStats(string name)
        {
            try
            {
                var stats = _playerStatisticService.Get(new PlayerStatisticServiceParameters(name));
                return new PlayerStatistics(stats);
            }
            catch
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(string.Format("Player {0} not compete in match.", name));
                throw new HttpResponseException(response);
            }
        }
    }
}

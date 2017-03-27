using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class MatchController : ApiController
    {
        private readonly IService<Match> matchService;
        private readonly IServerService serverService;
        private readonly IStatisticController statisticController;

        public MatchController(IService<Match> matchService, IServerService serverService, IStatisticController statisticController)
        {
            this.matchService = matchService;
            this.serverService = serverService;
            this.statisticController = statisticController;
        }

        [HttpPut]
        public HttpResponseMessage Save(string endpoint, DateTime timestamp, MatchInfo results)
        {
            try
            {
                serverService.Get(endpoint);
                Match match = new Match(endpoint, timestamp, results);
                matchService.Save(match);
                statisticController.RecalculateStatsByAdditionalMatch(match);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (NullReferenceException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
            catch (ArgumentException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        public MatchInfo Get(string endpoint, DateTime timestamp)
        {
            try
            {
                var param = new MatchParameters(endpoint, timestamp);
                return matchService.Get(param).Results;
            }
            catch (NullReferenceException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
        }
    }
}

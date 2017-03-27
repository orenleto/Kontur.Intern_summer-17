using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class ServerController : ApiController
    {
        private readonly IServerService serverService;

        public ServerController(IServerService serverService)
        {
            this.serverService = serverService;
        }

        [HttpPut]
        public HttpResponseMessage Save(string endpoint, ServerInfo info)
        {
            Domain.Server server = new Domain.Server(endpoint, info);
            serverService.Save(server);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public ServerInfo Get(string endpoint)
        {
            try { return serverService.Get(endpoint).Info; }
            catch (NullReferenceException ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (Request != null) response.RequestMessage = Request;
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        public IEnumerable<Domain.Server> GetAll()
        {
            return serverService.GetAll();
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Kontur.GameStats.Server
{
    public class ErrorHandler : DelegatingHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    ResponseHandler(task);

                    var response = task.Result;
                    return response;
                }
            );
        }

        public void ResponseHandler(Task<HttpResponseMessage> task)
        {
            var result = task.Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                var url = result.RequestMessage.RequestUri;
                var message = result.Content.ReadAsStringAsync().Result;
                logger.Error("URL: {0}\n{1}", url, message);
            }
        }

    }
}

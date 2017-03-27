using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Routing;
using Kontur.GameStats.Domain;
using Kontur.GameStats.Storage;
using Ninject;
using Ninject.Syntax;
using Owin;

namespace Kontur.GameStats.Server
{
    public class Startup
    {
        private const string EndpointRegExp =
            @"^((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])|(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]))-([1-5]?\d{1,4}|6[0-4]\d{3}|65[0-4]\d{2}|655[0-2]\d|6553[0-6])$";

        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();
            app.UseWebApi(webApiConfiguration);
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ServerInfo",
                routeTemplate: "servers/{endpoint}/info",
                defaults: new {controller = "server"},
                constraints: new {httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Put), endpoint = EndpointRegExp }
            );

            config.Routes.MapHttpRoute(
                name: "ServersInfo",
                routeTemplate: "servers/info",
                defaults: new {controller = "server"},
                constraints: new {httpMethod = new HttpMethodConstraint(HttpMethod.Get)}
            );

            config.Routes.MapHttpRoute(
                name: "MatchInfo",
                routeTemplate: "servers/{endpoint}/matches/{timestamp}",
                defaults: new {controller = "match"},
                constraints: new {httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Put), endpoint = EndpointRegExp }
            );

            config.Routes.MapHttpRoute(
                name: "ServersStats",
                routeTemplate: "servers/{endpoint}/stats",
                defaults: new { controller = "statistic" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), endpoint = EndpointRegExp }
            );

            config.Routes.MapHttpRoute(
                name: "PlayersStats",
                routeTemplate: "players/{name}/stats",
                defaults: new { controller = "statistic" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.Routes.MapHttpRoute(
                name: "RecentMatches",
                routeTemplate: "reports/recent-matches/{count}",
                defaults: new {controller = "report", action = "RecentMatches", count = 5 },
                constraints: new {httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.Routes.MapHttpRoute(
                name: "TopPlayers",
                routeTemplate: "reports/best-players/{count}",
                defaults: new { controller = "report", action = "TopPlayers", count = 5 },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.Routes.MapHttpRoute(
                name: "PopularServers",
                routeTemplate: "reports/popular-servers/{count}",
                defaults: new { controller = "report", action = "PopularServers", count = 5 },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.DependencyResolver = new NinjectResolver(NinjectConfig.CreateKernel());
            config.MessageHandlers.Add(new ErrorHandler());

            return config;
        }
    }

    public static class NinjectConfig
    {
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IServerRepository>()
                .To<ServerRepository>()
                .InSingletonScope();
            kernel.Bind<IMatchRepository>()
                .To<MatchRepository>()
                .InSingletonScope();
            kernel.Bind<IStatisticRepository<BaseServerStatistics>>()
                .To<ServerStatRepository>();
            kernel.Bind<IStatisticRepository<BasePlayerStatistics>>()
                .To<PlayerStatRepository>();

            kernel.Bind<IService<BaseServerStatistics>>()
                .To<ServerStatisticService>()
                .InSingletonScope();
            kernel.Bind<IService<BasePlayerStatistics>>()
                .To<PlayerStatisticService>()
                .InSingletonScope();
            kernel.Bind<IService<Match>>()
                .To<MatchService>()
                .InSingletonScope();
            kernel.Bind<IServerService>()
                .To<ServerService>()
                .InSingletonScope();

            kernel.Bind<IStatisticController>()
                .To<StatisticController>();

            return kernel;
        }
    }

    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            Contract.Assert(resolver != null);

            this.resolver = resolver;
        }

        public void Dispose()
        {
            resolver = null;
        }

        public object GetService(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");

            return resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");

            return resolver.GetAll(serviceType);
        }
    }

    public class NinjectResolver : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;

        public NinjectResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel);
        }
    }
}

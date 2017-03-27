using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class MatchService : IService<Match>
    {
        private static Timer _timer;
        private static object synclock = new object();

        private readonly IMatchRepository matchRepository;
        private readonly IServerService serverService;
        private ConcurrentBag<Match> cache;

        public MatchService(IMatchRepository matchRepository, IServerService serverService)
        {
            this.matchRepository = matchRepository;
            this.serverService = serverService;
            cache = new ConcurrentBag<Match>();

            foreach (Match match in matchRepository.GetAll())
            {
                cache.Add(match);
                if (cache.Count > 1000)
                    cache = new ConcurrentBag<Match>(cache.OrderByDescending(m => m.Timestamp).Take(50));
            }

            _timer = new Timer(UpdateCache, null, 30000, 30000);
        }

        private void UpdateCache(object obj)
        {
            lock (synclock)
            {
                cache = new ConcurrentBag<Match>(cache.OrderByDescending(m => m.Timestamp).Take(50));
            }
        }

        public void Save(Match match)
        {
            Domain.Server server = serverService.Get(match.Server);
            if (!server.Info.GameModes.Contains(match.Results.GameMode))
                throw new ArgumentException(string.Format("GameMode {0} is not available for {1}",
                    match.Results.GameMode, match.Server));

            matchRepository.Save(match);
            cache.Add(match);
        }

        public Match Get(IParams param)
        {
            MatchParameters matchParameters = param as MatchParameters;

            string endpoint = matchParameters.EndPoint;
            DateTime timestamp = matchParameters.Timestamp;

            serverService.Get(endpoint);
            return matchRepository.Get(endpoint, timestamp);
        }

        public IEnumerable<Match> CreateReport(int count)
        {
            return cache.OrderByDescending(match => match.Timestamp).Take(5);
        }

        public IEnumerable<Match> GetAll()
        {
            return matchRepository.GetAll();
        }
    }

    public class MatchParameters : IParams
    {
        public string EndPoint { get; private set; }
        public DateTime Timestamp { get; private set; }

        public MatchParameters(string endPoint, DateTime timestamp)
        {
            EndPoint = endPoint;
            Timestamp = timestamp;
        }

        public override bool Equals(object obj)
        {
            var param = obj as MatchParameters;
            if (param == null)
                return false;

            return EndPoint.Equals(param.EndPoint)
                   && Timestamp.Equals(param.Timestamp);
        }

        public override int GetHashCode()
        {
            return EndPoint.GetHashCode() ^ Timestamp.GetHashCode();
        }
    }
}

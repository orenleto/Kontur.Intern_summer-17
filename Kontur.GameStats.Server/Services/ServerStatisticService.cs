using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class ServerStatisticService : IService<BaseServerStatistics>
    {
        private readonly IStatisticRepository<BaseServerStatistics> _repository;
        private readonly IServerService _serverService;
        private ConcurrentDictionary<string, BaseServerStatistics> cache;

        public ServerStatisticService(IStatisticRepository<BaseServerStatistics> repository, IServerService serverService)
        {
            _repository = repository;
            _serverService = serverService;

            cache = new ConcurrentDictionary<string, BaseServerStatistics>();

            foreach (BaseServerStatistics stats in _repository.GetAll())
            {
                var trimStat = stats.Trim();
                cache.TryAdd(trimStat.EndPoint, trimStat);
            }
        }

        public void Save(Match match)
        {
            var endpoint = match.Server;
            string name = _serverService.Get(endpoint).Info.Name;

            BaseServerStatistics stats;
            try { stats = _repository.Get(endpoint); }
            catch { stats = new BaseServerStatistics(endpoint, name); }

            stats = stats.RecalculateWithAdditional(match);
            _repository.Save(stats);

            var trimStat = stats.Trim();
            cache.AddOrUpdate(endpoint, trimStat, (_name, _stats) => trimStat);
        }

        public BaseServerStatistics Get(IParams param)
        {
            var p = param as ServerStatisticsServiceParameter;
            
            return cache[p.Endpoint];
        }

        public IEnumerable<BaseServerStatistics> GetAll()
        {
            return cache.Values;
        }
    }

    public class ServerStatisticsServiceParameter : IParams
    {
        public string Endpoint { get; private set; }

        public ServerStatisticsServiceParameter(string endpoint)
        {
            Endpoint = endpoint;
        }

        public override bool Equals(object obj)
        {
            var param = obj as ServerStatisticsServiceParameter;
            if (param == null)
                return false;
            return Endpoint == param.Endpoint;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class PlayerStatisticService : IService<BasePlayerStatistics>
    {
        private readonly IStatisticRepository<BasePlayerStatistics> _repository;
        private readonly ConcurrentDictionary<string, BasePlayerStatistics> cache;


        public PlayerStatisticService(IStatisticRepository<BasePlayerStatistics> repository)
        {
            _repository = repository;
            cache = new ConcurrentDictionary<string, BasePlayerStatistics>(new List<KeyValuePair<string, BasePlayerStatistics>>(), StringComparer.OrdinalIgnoreCase);

            foreach (BasePlayerStatistics stats in _repository.GetAll())
            {
                var trimStats = stats.Trim();
                cache.TryAdd(trimStats.Name, trimStats);
            }
        }

        public void Save(Match match)
        {
            var players = match.Results.Scoreboard.Select(score => score.Name);
            foreach (var name in players)
            {
                BasePlayerStatistics stats;
                try { stats = _repository.Get(name); }
                catch { stats = new BasePlayerStatistics(name); }

                stats = stats.RecalculateWithAdditional(match);
                _repository.Save(stats);

                var trimStats = stats.Trim();
                cache.AddOrUpdate(name, trimStats, (_name, _stats) => trimStats);
            }
        }

        public BasePlayerStatistics Get(IParams param)
        {
            try { return cache[(param as PlayerStatisticServiceParameters).Name]; }
            catch
            {
                throw new NullReferenceException(string.Format("Player {0} is not played in match.", (param as PlayerStatisticServiceParameters).Name));
            }
        }

        public IEnumerable<BasePlayerStatistics> GetAll()
        {
            return cache.Values;
        }
    }

    public class PlayerStatisticServiceParameters : IParams
    {
        public string Name { get; private set; }

        public PlayerStatisticServiceParameters(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var param = obj as PlayerStatisticServiceParameters;
            if (param == null)
                return false;
            return Name == param.Name;
        }
    }
}

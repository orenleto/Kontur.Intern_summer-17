using System.Collections.Generic;

namespace Kontur.GameStats.Domain
{
    public interface IStatisticRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(string key);
        void Save(T statistics);
    }
}
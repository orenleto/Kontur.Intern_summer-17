using System.Collections.Generic;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public interface IService<T>
    {
        void Save(Match match);
        T Get(IParams param);
        IEnumerable<T> GetAll();
    }
}

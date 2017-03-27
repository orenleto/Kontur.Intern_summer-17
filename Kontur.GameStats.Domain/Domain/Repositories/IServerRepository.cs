using System.Collections.Generic;

namespace Kontur.GameStats.Domain
{
    public interface IServerRepository
    {
        Server Get(string endpoint);
        void SaveOrUpdate(Server server);
        IEnumerable<Server> GetAll();
    }
}

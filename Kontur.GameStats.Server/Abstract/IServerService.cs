using System.Collections.Generic;

namespace Kontur.GameStats.Server
{
    public interface IServerService
    {
        void Save(Domain.Server server);
        Domain.Server Get(string endpoint);
        IEnumerable<Domain.Server> GetAll();
    }
}

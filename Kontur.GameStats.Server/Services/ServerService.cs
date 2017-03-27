using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    class ServerService : IServerService
    {
        private static Timer _timer;
        private static object synclock = new object();

        private IServerRepository serverRepository;
        private ConcurrentDictionary<string, Domain.Server> _storage;

        public ServerService(IServerRepository serverRepository)
        {
            this.serverRepository = serverRepository;

            _storage = new ConcurrentDictionary<string, Domain.Server>();
            foreach (Domain.Server server in serverRepository.GetAll())
            {
                _storage.TryAdd(server.Endpoint, server);
            }

            _timer = new Timer(FlushData, null, 60000, 60000);
        }

        private void FlushData(object obj)
        {
            lock (synclock)
            {
                foreach (Domain.Server server in _storage.Select(record => record.Value))
                {
                    serverRepository.SaveOrUpdate(server);
                }

            }
        }

        public void Save(Domain.Server server)
        {
            _storage.AddOrUpdate(server.Endpoint, server, (_endpoint, _server) => server);
        }

        public Domain.Server Get(string endpoint)
        {
            try { return _storage[endpoint]; }
            catch { throw new NullReferenceException(string.Format("Server {0} not send advertise-request", endpoint));}
        }

        public IEnumerable<Domain.Server> GetAll()
        {
            return _storage.Select(pair => pair.Value);
        }

        ~ServerService()
        {
            FlushData(null);
        }
    }
}

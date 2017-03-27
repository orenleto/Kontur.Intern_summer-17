using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kontur.GameStats.Domain;
using LiteDB;

namespace Kontur.GameStats.Storage
{
    public class ServerRepository : IServerRepository
    {
        public Domain.Server Get(string endpoint)
        {
            using (var db = ConnectionFactory.Repository)
            {
                return db.Query<Domain.Server>().Where(Query.EQ("EndPoint", endpoint)).First();
            }
        }

        public void SaveOrUpdate(Domain.Server server)
        {
            using (var db = ConnectionFactory.Repository)
            {
                if (!db.Update(server))
                    db.Insert(server);
            }
        }

        public IEnumerable<Domain.Server> GetAll()
        {
            using (var db = ConnectionFactory.Repository)
            {
                return db.Query<Domain.Server>().ToEnumerable();
            }
        }
    }
}

using Kontur.GameStats.Domain;
using LiteDB;

namespace Kontur.GameStats.Storage
{
    public static class ConnectionFactory
    {
        public static LiteRepository Repository
        {
            get { return new LiteRepository("My.db", mapper); }
        }

        private static readonly BsonMapper mapper;
        static ConnectionFactory()
        {
            mapper = BsonMapper.Global;

            mapper.Entity<Domain.Server>()
                .Id(server => server.Endpoint);

            mapper.Entity<BaseServerStatistics>()
                .Id(statistics => statistics.EndPoint);
            mapper.Entity<BasePlayerStatistics>()
                .Id(statistics => statistics.Name);
        }
    }
}

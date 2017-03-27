using System;
using System.Collections.Generic;
using Kontur.GameStats.Domain;
using LiteDB;

namespace Kontur.GameStats.Storage
{
    public class MatchRepository : IMatchRepository
    {
        public void Save(Match match)
        {
            using (var db = ConnectionFactory.Repository)
            {
                if (db.Query<Match>().Where(Query.And(Query.EQ("Server", match.Server), Query.EQ("Timestamp", match.Timestamp))).Exists())
                    throw new ArgumentException(string.Format("Match {0}:{1} exist.", match.Server, match.Timestamp));

                db.Insert(match);
            }
        }

        public Match Get(string endpoint, DateTime timestamp)
        {
            using (var db = ConnectionFactory.Repository)
            {
                if (!db.Query<Match>().Where(Query.And(Query.EQ("Server", endpoint), Query.EQ("Timestamp", timestamp))).Exists())
                    throw new NullReferenceException(string.Format("Match {0}:{1} is not exist.", endpoint, timestamp));

                var matchInCollection = db.Query<Match>()
                        .Where(Query.And(Query.EQ("Server", endpoint), Query.EQ("Timestamp", timestamp)))
                        .First();
                return matchInCollection;
            }
        }

        public IEnumerable<Match> GetAll()
        {
            using (var db = ConnectionFactory.Repository)
            {
                return db.Query<Match>().ToEnumerable();
            }
        }
    }
}

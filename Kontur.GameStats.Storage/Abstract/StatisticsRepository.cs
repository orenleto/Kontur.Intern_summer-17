using System;
using System.Collections.Generic;
using Kontur.GameStats.Domain;
using LiteDB;

namespace Kontur.GameStats.Storage
{
    public abstract class StatisticsRepository<T> : IStatisticRepository<T>
    {

        public T Get(string key)
        {
            using (var db = ConnectionFactory.Repository)
            {
                return db.Query<T>().Where(Query.EQ("_id", key)).First();
            }
        }
        public void Save(T statistics)
        {
            using (var db = ConnectionFactory.Repository)
            {
                if (!db.Update(statistics))
                    db.Insert(statistics);
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (var db = ConnectionFactory.Repository)
            {
                return db.Query<T>().ToEnumerable();
            }
        }
    }
}

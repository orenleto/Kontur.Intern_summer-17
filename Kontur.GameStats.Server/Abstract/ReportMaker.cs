using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Server
{
    public abstract class ReportMaker<T, TKey, U>
    {
        public abstract bool Filter(T arg);
        public abstract U Transformer(T arg);
        public abstract TKey Selector(T arg);
        public IService<T> Service { get; private set; }
        protected ReportMaker(IService<T> service)
        {
            Service = service;
        }

        public IEnumerable<U> Build(int count)
        {
            return Service
                .GetAll()
                .Where(arg => Filter(arg))
                .OrderByDescending(Selector)
                .Select(arg => Transformer(arg))
                .Take(Math.Max(0, Math.Min(50, count)));
        }
    }
}

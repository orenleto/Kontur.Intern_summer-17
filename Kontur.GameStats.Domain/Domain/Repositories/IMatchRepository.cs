using System;
using System.Collections.Generic;

namespace Kontur.GameStats.Domain
{
    public interface IMatchRepository
    {
        void Save(Match match);
        Match Get(string endpointString, DateTime timestamp);
        IEnumerable<Match> GetAll();
    }
}

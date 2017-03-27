using System;
using Kontur.GameStats.Domain;

namespace Kontur.GameStats.Server
{
    public class MatchReporter : ReportMaker<Match, DateTime, Match>
    {
        public MatchReporter(IService<Match> service) : base(service)
        {
        }

        public override bool Filter(Match arg)
        {
            return true;
        }

        public override Match Transformer(Match arg)
        {
            return arg;
        }

        public override DateTime Selector(Match arg)
        {
            return arg.Timestamp;
        }
    }
}
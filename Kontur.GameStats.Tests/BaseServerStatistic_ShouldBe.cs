using System;
using System.Collections.Generic;
using FluentAssertions;
using Kontur.GameStats.Domain;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
    [TestFixture]
    public class BaseServerStatistic_ShouldBe
    {
        private Match match;
        [SetUp]
        public void Initialize()
        {
            var endpointString = "192.168.0.1-8080";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            match = new Match(endpointString, timestamp, matchInfo);
        }

        [Test]
        public void Recalculated_WithAdditionalMatch()
        {
            var stats = new BaseServerStatistics("192.168.0.1-8080", "TestServer");
            var updatedStats = stats.RecalculateWithAdditional(match) as BaseServerStatistics;

            Assert.AreNotEqual(updatedStats, null);

            updatedStats.TotalMatchesPlayed.ShouldBeEquivalentTo(1);
            updatedStats.MaximumMatchesPerDay.ShouldBeEquivalentTo(1);
            updatedStats.TotalMatchesToday.ShouldBeEquivalentTo(1);
            updatedStats.LastMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.FirstMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.MaximumPopulation.ShouldBeEquivalentTo(2);
            updatedStats.TotalPlayersInMatches.ShouldBeEquivalentTo(2);
            updatedStats.TopGameModes["TestModeA"].ShouldBeEquivalentTo(1);
            updatedStats.TopMaps["TestMapA"].ShouldBeEquivalentTo(1);
        }
    }
}

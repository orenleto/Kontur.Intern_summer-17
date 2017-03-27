using System;
using System.Collections.Generic;
using FluentAssertions;
using Kontur.GameStats.Domain;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
    [TestFixture]
    public class BasePlayerStatistic_ShouldBe
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
        public void RecalculatedAsWinner_WithAdditionalMatch()
        {
            var stats = new BasePlayerStatistics("PlayerA");
            var updatedStats = stats.RecalculateWithAdditional(match);

            Assert.AreNotEqual(updatedStats, null);

            updatedStats.TotalMatchesPlayed.ShouldBeEquivalentTo(1);
            updatedStats.TotalMatchesWon.ShouldBeEquivalentTo(1);
            updatedStats.TotalKills.ShouldBeEquivalentTo(3);
            updatedStats.TotalDeaths.ShouldBeEquivalentTo(1);
            updatedStats.MaximumMatchesPerDay.ShouldBeEquivalentTo(1);
            updatedStats.TotalMatchesToday.ShouldBeEquivalentTo(1);
            updatedStats.TotalScoreboardPercent.ShouldBeEquivalentTo(100.0);
            updatedStats.FirstMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.LastMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.Servers["192.168.0.1-8080"].ShouldBeEquivalentTo(1);
            updatedStats.GameModes["TestModeA"].ShouldBeEquivalentTo(1);
        }

        [Test]
        public void RecalculatedAsCompetitor_WithAdditionalMatch()
        {
            var stats = new BasePlayerStatistics("PlayerB");
            var updatedStats = stats.RecalculateWithAdditional(match);

            Assert.AreNotEqual(updatedStats, null);

            updatedStats.TotalMatchesPlayed.ShouldBeEquivalentTo(1);
            updatedStats.TotalMatchesWon.ShouldBeEquivalentTo(0);
            updatedStats.TotalKills.ShouldBeEquivalentTo(1);
            updatedStats.TotalDeaths.ShouldBeEquivalentTo(3);
            updatedStats.MaximumMatchesPerDay.ShouldBeEquivalentTo(1);
            updatedStats.TotalMatchesToday.ShouldBeEquivalentTo(1);
            updatedStats.TotalScoreboardPercent.ShouldBeEquivalentTo(0.0);
            updatedStats.FirstMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.LastMatchPlayed.ShouldBeEquivalentTo(new DateTime(2017, 01, 01));
            updatedStats.Servers["192.168.0.1-8080"].ShouldBeEquivalentTo(1);
            updatedStats.GameModes["TestModeA"].ShouldBeEquivalentTo(1);
        }
    }
}

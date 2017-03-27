using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using FakeItEasy;
using Kontur.GameStats.Domain;
using NUnit.Framework;
using Kontur.GameStats.Server;

namespace Kontur.GameStats.Tests
{
    [TestFixture]
    public class StatisticController_Should
    {
        private IService<BaseServerStatistics> _serverStatisticService;
        private IService<BasePlayerStatistics> _playerStatisticService;
        private StatisticController _controller;

        [SetUp]
        public void Initialize()
        {
            _serverStatisticService = A.Fake<IService<BaseServerStatistics>>();
            _playerStatisticService = A.Fake<IService<BasePlayerStatistics>>();
            var _serverRepository = A.Fake<IServerService>();

            _controller = new StatisticController(_serverRepository, _serverStatisticService, _playerStatisticService);

            A.CallTo(() => _serverRepository.Get("Not.Existing.EndPoint")).Throws<NullReferenceException>();
            A.CallTo(() => _playerStatisticService.Get(new PlayerStatisticServiceParameters("Test.Player"))).Throws<NullReferenceException>();
        }

        [Test]
        public void ThrowNotFoundException_IfEndpointNotExist()
        {
            var exception = Assert.Throws<HttpResponseException>(() =>_controller.GetServerStats("Not.Existing.EndPoint"));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void CallServerStatisticService_IfEndpointExist()
        {
            var stats = _controller.GetServerStats("Existing.EndPoint");
            A.CallTo(() => _serverStatisticService.Get(new ServerStatisticsServiceParameter("Existing.EndPoint"))).MustHaveHappened();
        }

        [Test]
        public void ThrowNotFoundException_IfPlayerNotPlayed()
        {
            var exception = Assert.Throws<HttpResponseException>(() => _controller.GetPlayerStats("Test.Player"));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void CallPlayerStatisticService()
        {
            A.CallTo(() => _playerStatisticService.Get(new PlayerStatisticServiceParameters("Player"))).Returns(A.Fake<BasePlayerStatistics>());
            _controller.GetPlayerStats("Player");
            A.CallTo(() => _playerStatisticService.Get(new PlayerStatisticServiceParameters("Player"))).MustHaveHappened();
        }

        [Test]
        public void CallUpdateInAllService_WhenPutMatch()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345,
                new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);

            _controller.RecalculateStatsByAdditionalMatch(match);

            A.CallTo(() => _serverStatisticService.Save(match)).MustHaveHappened();
            A.CallTo(() => _playerStatisticService.Save(match)).MustHaveHappened();
        }
    }
}

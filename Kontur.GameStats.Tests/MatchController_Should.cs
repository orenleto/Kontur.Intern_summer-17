using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using FakeItEasy;
using FluentAssertions;
using Kontur.GameStats.Domain;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
    [TestFixture]
    public class MatchController_Should
    {
        private IServerService _serverRepository;
        private IService<Match> _matchRepository;
        private IStatisticController _statisticController;
        private MatchController _controller;

        [SetUp]
        public void Initialize()
        {
            var serverInfo = new ServerInfo("TestServer", new[] {"TestModeA", "TestModeB"});
            var server = new Domain.Server("192.168.0.1-80", serverInfo);

            _serverRepository = A.Fake<IServerService>();
            _matchRepository = A.Fake<IService<Match>>();
            _statisticController = A.Fake<IStatisticController>();

            _controller = new MatchController(_matchRepository, _serverRepository, _statisticController);

            A.CallTo(() => _serverRepository.Get("192.168.0.1-80")).Returns(server);
            A.CallTo(() => _serverRepository.Get("192.168.0.2-80")).Throws(() => new NullReferenceException());
        }

        [Test]
        public void PutMatchInRepository_WhenCorrectData()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score ("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);

            _controller.Save(endpointString, timestamp, matchInfo);

            A.CallTo(() => _matchRepository.Save(match)).MustHaveHappened();
            A.CallTo(() => _statisticController.RecalculateStatsByAdditionalMatch(match)).MustHaveHappened();
        }

        [Test]
        public void ThrowBadRequestExceptionOnPut_WhenEndpointNotExist()
        {
            var endpointString = "192.168.0.2-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);

            var exception = Assert.Throws<HttpResponseException>(() => _controller.Save(endpointString, timestamp, matchInfo));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void ThrowBadRequestExceptionOnPut_WhenServerNotPlayedGameMode()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeC", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);

            A.CallTo(() => _matchRepository.Save(match)).Throws<ArgumentException>();

            var exception = Assert.Throws<HttpResponseException>(() => _controller.Save(endpointString, timestamp, matchInfo));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void ThrowBadRequestExceptionOnPut_WhenMatchExist()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);
            A.CallTo(() => _matchRepository.Save(match)).Throws<ArgumentException>();

            var exception = Assert.Throws<HttpResponseException>(() => _controller.Save(endpointString, timestamp, matchInfo));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void ReturnMatchInfoOnGet_WhenMatchExist()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);
            var matchInfo = new MatchInfo("TestMapA", "TestModeA", 20, 20, 12.345, new List<Score> { new Score("PlayerA", 20, 3, 1), new Score("PlayerB", 3, 1, 3) });

            var match = new Match(endpointString, timestamp, matchInfo);

            A.CallTo(() => _matchRepository.Get(new MatchParameters(endpointString, timestamp))).Returns(match);

            var result = _controller.Get(endpointString, timestamp);
            result.ShouldBeEquivalentTo(matchInfo);
        }

        [Test]
        public void ThrowNotFoundExceptionOnGet_WhenEndpointNotExist()
        {
            var endpointString = "192.168.0.2-80";
            var timestamp = new DateTime(2017, 01, 01);

            var exception = Assert.Throws<HttpResponseException>(() => _controller.Get(endpointString, timestamp));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void ThrowNotFoundExceptionOnGet_WhenMatchNotExist()
        {
            var endpointString = "192.168.0.1-80";
            var timestamp = new DateTime(2017, 01, 01);

            A.CallTo(() => _matchRepository.Get(new MatchParameters(endpointString, timestamp))).Throws(() => new NullReferenceException());

            var exception = Assert.Throws<HttpResponseException>(() => _controller.Get(endpointString, timestamp));
            Assert.AreEqual(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}

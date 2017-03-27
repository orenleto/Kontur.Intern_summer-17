using FakeItEasy;
using FluentAssertions;
using Kontur.GameStats.Domain;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
    [TestFixture]
    public class ServerController_Should
    {
        [Test]
        public void ReturnAllServers()
        {
            var repo = A.Fake<IServerService>();
            ServerController controller = new ServerController(repo);
            controller.GetAll();

            A.CallTo(() => repo.GetAll()).MustHaveHappened();
        }

        [Test]
        public void ReturnServerInfoOnGet_WhenEndPointExist()
        {
            var endpointString = "192.168.0.1-80";
            var serverInfo = new ServerInfo("TestServer", new[] {"TestModeA", "TestModeB"});
            var server = new Domain.Server(endpointString, serverInfo);

            var repo = A.Fake<IServerService>();
            A.CallTo(() => repo.Get(endpointString)).Returns(server);
            ServerController controller = new ServerController(repo);

            var result = controller.Get(endpointString);

            result.ShouldBeEquivalentTo(new ServerInfo("TestServer", new[] {"TestModeA", "TestModeB"}));
        }

        [Test]
        public void PutServerInRepository_WhenCorrectData()
        {
            var endpointString = "192.168.0.1-80";
            var serverInfo = new ServerInfo("TestServer", new[] {"TestModeA", "TestModeB"});
            var server = new Domain.Server(endpointString, serverInfo);
            
            var repo = A.Fake<IServerService>();
            ServerController controller = new ServerController(repo);

            controller.Save(endpointString, serverInfo);

            A.CallTo(() => repo.Save(server)).MustHaveHappened();
        }
    }
}

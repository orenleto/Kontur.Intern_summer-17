using System;
using System.Net;

namespace Kontur.GameStats.Domain
{
    public class Server
    {
        public string Endpoint { get; private set; }
        public ServerInfo Info { get; private set; }

        private Server() { }
        public Server(string endpoint, ServerInfo info)
        {
            Endpoint = endpoint;
            Info = info;
        }

        public override bool Equals(object obj)
        {
            var server = obj as Server;

            if (ReferenceEquals(server, null))
                return false;

            return Endpoint.Equals(server.Endpoint)
                   && Info.Equals(server.Info);
        }

        public override int GetHashCode()
        {
            return Endpoint.GetHashCode() ^ Info.GetHashCode();
        }
    }
}

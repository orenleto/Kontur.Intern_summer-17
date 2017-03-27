using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Domain
{
    public class ServerInfo
    {
        public string Name { get; private set; }
        public IEnumerable<string> GameModes { get; private set; }

        private ServerInfo()
        {
            
        }

        public ServerInfo(string name, IEnumerable<string> gameModes)
        {
            Name = name;
            GameModes = gameModes;
        }

        public override bool Equals(object obj)
        {
            var info = obj as ServerInfo;

            if (ReferenceEquals(info, null))
                return false;

            return Name == info.Name &&
                GameModes.SequenceEqual(info.GameModes);
        }

        public override int GetHashCode()
        {
            int gameModeHashCode = 0;
            foreach (var mode in GameModes)
            {
                gameModeHashCode ^= mode.GetHashCode();
            }

            return Name.GetHashCode() ^
                   gameModeHashCode;
        }
    }
}

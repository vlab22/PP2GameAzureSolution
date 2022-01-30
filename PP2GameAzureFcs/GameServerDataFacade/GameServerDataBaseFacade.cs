using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    internal abstract class GameServerDataBaseFacade
    {
        public abstract Task<List<GameServerDetailData>> GetGameServerListAsync();
        public abstract int Insert(string serverName, string serverDns, int maxPlayers, int playerCount);
        public abstract Task<int> InsertAsync(string serverName, string serverDns, int maxPlayers, int playerCount);
        public abstract Task<int> UpdateAsync(int id, int newPlayerCount);
    }
}

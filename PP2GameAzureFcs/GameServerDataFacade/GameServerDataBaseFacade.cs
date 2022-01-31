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
        public abstract int Insert(string serverName, string serverDns, int port, string region, int maxPlayers, int playerCount, string status);
        public abstract Task<int> InsertAsync(string serverName, string serverDns, int port, string region, int maxPlayers, int playerCount, string status);
        public abstract Task<int> UpdatePlayeCountAsync(int id, int newPlayerCount);
        public abstract Task<int> UpdateStatusAsync(int id, string newStatus);
        public abstract Task<int> UpdateAsync(int id, string newName, string newDns, int newPort, string newRegion, int newMaxPlayers, int newPlayerCount, string newStatus);
        public abstract Task<int> InsertForIdAsync();

        public abstract Task<int> DeleteAsync(int pServerId);
    }
}

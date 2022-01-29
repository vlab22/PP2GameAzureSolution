using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    internal abstract class GameServerDataBaseFacade
    {
        public abstract int Insert(int playerCount);
        public abstract Task<int> InsertAsync(int playerCount);
        public abstract Task<int> UpdateAsync(int id, int newPlayerCount);
    }
}

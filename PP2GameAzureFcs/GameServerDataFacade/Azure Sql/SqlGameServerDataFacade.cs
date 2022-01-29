using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs.GameServerDataFacade
{
    internal class SqlGameServerDataFacade : GameServerDataBaseFacade
    {
        public override int Insert(int playerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"INSERT INTO GameServer(player_count) VALUES({playerCount}) SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        var result = cmd.ExecuteScalar();

                        return Convert.ToInt32(result);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return -1;
                    }
                }
            }
        }

        public override async Task<int> InsertAsync(int playerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"INSERT INTO GameServer(player_count) VALUES({playerCount}) SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        var result = await cmd.ExecuteScalarAsync();

                        return Convert.ToInt32(result);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return -1;
                    }
                }
            }
        }

        public override async Task<int> UpdateAsync(int id, int newPlayerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"UPDATE GameServer SET player_count = {newPlayerCount} WHERE id = {id}";

                using (var cmd = new SqlCommand(text, conn))
                {
                    try
                    {
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return -1;
                    }
                }
            }
        }
    }
}

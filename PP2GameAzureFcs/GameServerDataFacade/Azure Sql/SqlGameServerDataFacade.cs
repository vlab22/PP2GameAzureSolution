using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs.GameServerDataFacade
{
    internal class SqlGameServerDataFacade : GameServerDataBaseFacade
    {
        public override async Task<List<GameServerDetailData>> GetGameServerListAsync()
        {
            var resultList =new List<GameServerDetailData>();

            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            var conn = new SqlConnection(str);

            var sql = $"SELECT [Id] ,[Name] ,[Dns] ,[Max_Players_Limit] ,[Players_Count] FROM [dbo].[GameServer]";

            await conn.OpenAsync();

            var cmd = new SqlCommand(sql, conn);


            using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
            {
                while (await reader.ReadAsync())
                {
                    resultList.Add(new GameServerDetailData()
                    {
                        name = reader["Name"].ToString(),
                        dns = reader["Dns"].ToString(),
                        maxPlayers = Convert.ToInt32(reader["Max_Players_Limit"]),
                        playerCount = Convert.ToInt32(reader["Players_Count"])
                    });
                }
            }

            return resultList;
        }

        public override int Insert(string serverName, string serverDns, int maxPlayers, int playerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"INSERT INTO [dbo].[GameServer] ([Name], [Dns], [Max_Players_Limit], [Players_Count]) VALUES" +
                    $" (N'{serverName}', N'{serverDns}', {maxPlayers}, {playerCount})";

                using (var cmd = new SqlCommand(sql, conn))
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

        public override async Task<int> InsertAsync(string serverName, string serverDns, int maxPlayers, int playerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"INSERT INTO [dbo].[GameServer] ([Name], [Dns], [Max_Players_Limit], [Players_Count]) VALUES" +
                    $" (N'{serverName}', N'{serverDns}', {maxPlayers}, {playerCount})";

                using (var cmd = new SqlCommand(sql, conn))
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
                var text = $"UPDATE GameServer SET Player_Count = {newPlayerCount} WHERE id = {id}";

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

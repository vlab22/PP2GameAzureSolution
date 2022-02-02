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
        public override async Task<GameServerDetailData> GetGameServerByIdAsync(int pId)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            var conn = new SqlConnection(str);

            var sql = $"SELECT [Id] ,[Name] ,[Dns], [Port] ,[Region] " +
                        $",[Max_Players_Limit] ,[Players_Count], [Status], [Created_At] " +
                        $"FROM [dbo].[GameServer] " +
                        $"WHERE [Id] = {pId}";

            await conn.OpenAsync();

            var cmd = new SqlCommand(sql, conn);

            GameServerDetailData serverData = null;

            using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
            {
                while (await reader.ReadAsync())
                {
                    serverData = new GameServerDetailData()
                    {
                        id = Convert.ToInt32(reader["Id"]),
                        name = reader["Name"].ToString(),
                        dns = reader["Dns"].ToString(),
                        port = Convert.ToInt32(reader["Port"]),
                        region = reader["Region"].ToString(),
                        maxPlayers = Convert.ToInt32(reader["Max_Players_Limit"]),
                        playersCount = Convert.ToInt32(reader["Players_Count"]),
                        status = reader["Status"].ToString(),
                        created_at = (DateTime)reader["Created_At"]

                    };
                }
            }

            return serverData;
        }

        public override async Task<int> DeleteAsync(int pServerId)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"DELETE FROM [dbo].[GameServer] WHERE [Id] = {pServerId}";

                using (var cmd = new SqlCommand(sql, conn))
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

        public override async Task<List<GameServerDetailData>> GetGameServerListAsync()
        {
            var resultList = new List<GameServerDetailData>();

            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            var conn = new SqlConnection(str);

            var sql = $"SELECT [Id] ,[Name] ,[Dns], [Port] ,[Region] ,[Max_Players_Limit] ,[Players_Count], [Status], [Created_At] FROM [dbo].[GameServer]";

            await conn.OpenAsync();

            var cmd = new SqlCommand(sql, conn);


            using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
            {
                while (await reader.ReadAsync())
                {
                    resultList.Add(new GameServerDetailData()
                    {
                        id = Convert.ToInt32(reader["Id"]),
                        name = reader["Name"].ToString(),
                        dns = reader["Dns"].ToString(),
                        port = Convert.ToInt32(reader["Port"]),
                        region = reader["Region"].ToString(),
                        maxPlayers = Convert.ToInt32(reader["Max_Players_Limit"]),
                        playersCount = Convert.ToInt32(reader["Players_Count"]),
                        status = reader["Status"].ToString(),
                        created_at = (DateTime)reader["Created_At"]

                    });
                }
            }

            return resultList;
        }

        public override int Insert(string serverName, string serverDns, int port, string region, int maxPlayers, int playerCount, string status)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"INSERT INTO [dbo].[GameServer] ([Name], [Dns], [Port], [Region] [Max_Players_Limit], [Players_Count], [Status]) VALUES" +
                    $" (N'{serverName}', N'{serverDns}', {port}, N'{region}', {maxPlayers}, {playerCount}, N'{status}')";

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


        public override async Task<int> InsertAsync(string serverName, string serverDns, int port, string region, int maxPlayers, int playerCount, string status)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"INSERT INTO [dbo].[GameServer] ([Name], [Dns], [Port], [Region], [Max_Players_Limit], [Players_Count]) VALUES" +
                    $" (N'{serverName}', N'{serverDns}', {port}, N'{region}', {maxPlayers}, {playerCount}, N'{status}')";

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

        public override async Task<int> InsertForIdAsync()
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var sql = $"INSERT INTO [dbo].[GameServer] ([Status]) VALUES" +
                    $" (N''); SELECT SCOPE_IDENTITY();";

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

        public override async Task<int> UpdateAsync(int id, string newName, string newDns, int newPort, string newRegion, int newMaxPlayers, int newPlayerCount, string newStatus)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"UPDATE GameServer SET [Name] = N'{newName}', [Dns] = N'{newDns}', [Port] = {newPort}" +
                    $", [Region] = N'{newRegion}', [Max_Players_Limit] = {newMaxPlayers}, [Players_Count] = {newPlayerCount}" +
                    $", [Status] = N'{newStatus}' WHERE id = {id}";

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
        public override async Task<int> UpdateAndSetLastPortAsync(int id, string newName, string newDns, string newRegion, int newMaxPlayers, int newPlayerCount, string newStatus)


        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"UPDATE GameServer SET [Name] = N'{newName}', [Dns] = N'{newDns}', [Port] = (SELECT MAX([Port])+1 FROM GameServer)" +
                    $", [Region] = N'{newRegion}', [Max_Players_Limit] = {newMaxPlayers}, [Players_Count] = {newPlayerCount}" +
                    $", [Status] = N'{newStatus}' WHERE id = {id}";

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

        public override async Task<int> UpdatePlayeCountAsync(int id, int newPlayerCount)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"UPDATE GameServer SET [Players_Count] = {newPlayerCount} WHERE id = {id}";

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

        public override async Task<int> UpdateStatusAsync(int id, string newStatus)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (var conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"UPDATE GameServer SET [Status] = N'{newStatus}' WHERE id = {id}";

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

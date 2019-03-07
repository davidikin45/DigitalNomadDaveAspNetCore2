using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Helpers;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Hangfire
{
    public static class HangfireInitializationHelper
    {
        public static async Task EnsureTablesDeletedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var tableNames = new List<string>();

            tableNames.Add($"[HangFire].[AggregatedCounter]");
            tableNames.Add($"[HangFire].[Counter]");
            tableNames.Add($"[HangFire].[Hash]");
            tableNames.Add($"[HangFire].[Job]");
            tableNames.Add($"[HangFire].[JobParameter]");
            tableNames.Add($"[HangFire].[JobQueue]");
            tableNames.Add($"[HangFire].[List]");
            tableNames.Add($"[HangFire].[Schema]");
            tableNames.Add($"[HangFire].[Server]");
            tableNames.Add($"[HangFire].[Set]");
            tableNames.Add($"[HangFire].[State]");

            var commands = new List<String>();
            if (string.IsNullOrEmpty(connectionString))
            {

            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                bool dbExists = await DbInitializationHelper.ExistsAsync(connectionString, cancellationToken);

                if (dbExists)
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        await conn.OpenAsync(cancellationToken);

                        using (SqliteTransaction transaction = conn.BeginTransaction())
                        {
                            //Drop tables
                            foreach (var tableName in tableNames)
                            {
                                foreach (var t in tableNames)
                                {
                                    try
                                    {
                                        var commandSql = $"DROP TABLE IF EXISTS {t.Replace("].[",".")};";
                                        using (var command = new SqliteCommand(commandSql, conn, transaction))
                                        {
                                            await command.ExecuteNonQueryAsync(cancellationToken);
                                        }

                                        commands.Add(commandSql);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            transaction.Rollback();
                        }

                        using (SqliteTransaction transaction = conn.BeginTransaction())
                        {
                            foreach (var commandSql in commands)
                            {
                                using (var command = new SqliteCommand(commandSql, conn, transaction))
                                {
                                    await command.ExecuteNonQueryAsync(cancellationToken);
                                }
                            }

                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    await DbInitializationHelper.EnsureDestroyedAsync(connectionString, cancellationToken);
                }
            }
            else
            {
                bool dbExists = await DbInitializationHelper.ExistsAsync(connectionString, cancellationToken);

                if (dbExists)
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync(cancellationToken);

                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            //Drop tables
                            foreach (var tableName in tableNames)
                            {
                                foreach (var t in tableNames)
                                {
                                    try
                                    {
                                        var commandSql = $"DROP TABLE IF EXISTS {t}";
                                        using (var command = new SqlCommand(commandSql, conn, transaction))
                                        {
                                            await command.ExecuteNonQueryAsync(cancellationToken);
                                        }

                                        commands.Add(commandSql);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            transaction.Rollback();
                        }

                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            foreach (var commandSql in commands)
                            {
                                using (var command = new SqlCommand(commandSql, conn, transaction))
                                {
                                    await command.ExecuteNonQueryAsync(cancellationToken);
                                }
                            }

                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    await DbInitializationHelper.EnsureDestroyedAsync(connectionString, cancellationToken);
                }
            }
        }

        public static Task<bool> EnsureDbCreatedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            return DbInitializationHelper.EnsureCreatedAsync(connectionString, cancellationToken);
        }

        public static async Task EnsureDbAndTablesCreatedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            await EnsureDbCreatedAsync(connectionString, cancellationToken);
            if(string.IsNullOrEmpty(connectionString))
            {

            }
            else if(ConnectionStringHelper.IsSQLite(connectionString))
            {
                var options = new SQLiteStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                };

                //Initialize Schema
                var storage = new SQLiteStorage(connectionString, options);
            }
            else
            {
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                };

                //Initialize Schema
                var storage = new SqlServerStorage(connectionString, options);
            }      
        }

        public static Task EnsureDbDestroyedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            return DbInitializationHelper.EnsureDestroyedAsync(connectionString, cancellationToken);
        }
    }
}

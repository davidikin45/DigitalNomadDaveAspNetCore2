using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Helpers;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AspNetCore.Base.Hangfire
{
    public static class HangfireInitializationHelper
    {
        public static void EnsureTablesDeleted(string connectionString)
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
                bool dbExists = DbInitializationHelper.Exists(connectionString);

                if (dbExists)
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        conn.Open();

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
                                            command.ExecuteNonQuery();
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
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    DbInitializationHelper.EnsureDestroyed(connectionString);
                }
            }
            else
            {
                bool dbExists = DbInitializationHelper.Exists(connectionString);

                if (dbExists)
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

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
                                            command.ExecuteNonQuery();
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
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    DbInitializationHelper.EnsureDestroyed(connectionString);
                }
            }
        }

        public static bool EnsureDbCreated(string connectionString)
        {
            return DbInitializationHelper.EnsureCreated(connectionString);
        }

        public static void EnsureDbAndTablesCreated(string connectionString)
        {
            EnsureDbCreated(connectionString);
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

        public static void EnsureDbDestroyed(string connectionString)
        {
            DbInitializationHelper.EnsureDestroyed(connectionString);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.Base.Data.Helpers
{
    public static class DbContextInitializationExtensions
    {
        #region ConnectionString
        public static string GetConnectionString(this DbContext context)
        {
            if (context.Database.IsSqlServer() || context.Database.IsSqlite())
            {
                var dbConnection = context.Database.GetDbConnection();
                var connectionString = dbConnection.ConnectionString;
                return connectionString;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Ensure Db and Tables Created at a DbContext Level
        /// <summary>Adds objects that are used by the model for this context</summary>
        public static void EnsureDbAndTablesCreated(this DbContext context)
        {
            var created = context.Database.EnsureCreated();
            if (!created)
            {
                //https://docs.microsoft.com/en-us/ef/core/managing-schemas/ensure-created
                //CreateTables would throw exception if any table already exists. 
                //var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();
                //databaseCreator.CreateTables();

                if (context.Database.IsSqlServer() || context.Database.IsSqlite())
                {
                    var dependencies = context.Database.GetService<RelationalDatabaseCreatorDependencies>();
                    var createTablesCommands = dependencies.MigrationsSqlGenerator.Generate(dependencies.ModelDiffer.GetDifferences(null, dependencies.Model), dependencies.Model);
                    foreach (var createTableCommand in createTablesCommands)
                    {
                        try
                        {
                            dependencies.MigrationCommandExecutor.ExecuteNonQuery(new MigrationCommand[] { createTableCommand }, dependencies.Connection);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
        #endregion

        #region Delete Tables and Migrations DbContent Level
        /// <summary>Just removes the database objects that are used by the model for this context.</summary>
        /// <param name="context">The context.</param>
        public static void EnsureTablesAndMigrationsDeleted(this DbContext context)
        {
            bool dbExists = false;
            try
            {
                if (context.Database.Exists())
                    dbExists = true;
            }
            catch
            {

            }

            if (dbExists)
            {
                if (context.Database.IsSqlServer() || context.Database.IsSqlite())
                {
                    var modelTypes = context.GetModelTypes();

                    var tableNames = new List<string>();
                    foreach (var entityType in modelTypes)
                    {
                        var mapping = context.Model.FindEntityType(entityType).Relational();
                        var schema = mapping.Schema;
                        var tableName = mapping.TableName;

                        var schemaAndTableName = $"[{tableName}]";
                        tableNames.Add(schemaAndTableName);
                    }

                    var commands = new List<String>();
                    using (var transaction = context.Database.BeginTransaction())
                    {

                        //Drop tables
                        foreach (var tableName in tableNames)
                        {
                            foreach (var t in tableNames)
                            {
                                try
                                {
                                    var command = $"DROP TABLE IF EXISTS {t}";
                                    context.Database.ExecuteSqlCommand(new RawSqlString(command));
                                    commands.Add(command);
                                }
                                catch
                                {

                                }
                            }
                        }

                        //Delete migrations
                        if (TableExists(context.Database, "__EFMigrationsHistory"))
                        {
                            foreach (var migrationId in context.Database.Migrations())
                            {
                                try
                                {
                                    var command = $"DELETE FROM [__EFMigrationsHistory] WHERE MigrationId = '{migrationId}'";
                                    context.Database.ExecuteSqlCommand(command);
                                    commands.Add(command);
                                }
                                catch
                                {

                                }
                            }
                        }
                        
                        transaction.Rollback();
                    }

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var command in commands)
                        {
                            context.Database.ExecuteSqlCommand(new RawSqlString(command));
                        }

                        transaction.Commit();
                    }
                }
                else if (context.Database.IsInMemory())
                {
                    context.Database.EnsureDeleted();
                }
            }
            else
            {
                //As long as the Db is online this will physically delete db.
                context.Database.EnsureDeleted();
            }
        }
        #endregion

        #region Clear Data at a DbContext Level
        public static void ClearData(this DbContext context)
        {
            var modelTypes = context.GetModelTypes();
            foreach (var entityType in modelTypes)
            {
                var dbSet = Set(context, entityType);

                var paramType = typeof(IEnumerable<>).MakeGenericType(entityType);
                var removeRangeMethod = typeof(DbSet<>).MakeGenericType(entityType).GetMethod("RemoveRange", new[] { paramType });
                removeRangeMethod.Invoke(dbSet, new object[] { dbSet });
            }
        }

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));
        private static IQueryable Set(this DbContext context, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
        #endregion

        #region Exists
        public static bool Exists(this DatabaseFacade databaseFacade)
        {
            var relationalDatabaseCreator = databaseFacade.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (relationalDatabaseCreator != null)
            {
                return relationalDatabaseCreator.Exists();

            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Table Exists
        public static bool TableExists(this DatabaseFacade databaseFacade, string tableName)
        {
            var relationalDatabaseCreator = databaseFacade.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (relationalDatabaseCreator != null)
            {
                if (databaseFacade.IsSqlite())
                {
                    using (var command = databaseFacade.GetDbConnection().CreateCommand())
                    {
                        if (databaseFacade.CurrentTransaction != null)
                            command.Transaction = databaseFacade.CurrentTransaction.GetDbTransaction();

                        command.CommandText = $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM ""sqlite_master"" WHERE ""type"" = 'table' AND ""rootpage"" IS NOT NULL AND ""tbl_name"" = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;";
                        var exists = (long)command.ExecuteScalar();
                        return exists == 1;
                    }
                }
                else
                {
                    using (var command = databaseFacade.GetDbConnection().CreateCommand())
                    {
                        if (databaseFacade.CurrentTransaction != null)
                            command.Transaction = databaseFacade.CurrentTransaction.GetDbTransaction();

                        command.CommandText = $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;";
                        var exists = (bool)command.ExecuteScalar();
                        return exists;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Create Tables Script
        public static string GenerateCreateTablesScript(this DbContext context)
        {
            var sql = context.Database.GenerateCreateScript();
            return sql;
        }
        #endregion

        #region Migration Script
        public static string GenerateMigrationScript(this DatabaseFacade database, string fromMigration = null, string toMigration = null)
        {
            return database.GetService<IMigrator>().GenerateScript(fromMigration, toMigration);
        }
        #endregion

        #region Pending Migrations 
        public static IEnumerable<string> PendingMigrations(this DatabaseFacade database)
        {
            return database.GetPendingMigrations().ToList();
        }
        #endregion

        #region Migrations 
        public static IEnumerable<string> Migrations(this DatabaseFacade database)
        {
            return database.GetMigrations().ToList();
        }
        #endregion

        #region Apply Pending Migrations
        public static void EnsureMigratedStepByStep(this DatabaseFacade database)
        {
            var pendingMigrations = PendingMigrations(database);
            if (pendingMigrations.Any())
            {
                var migrator = database.GetService<IMigrator>();
                foreach (var targetMigration in pendingMigrations)
                {
                    var sql = migrator.GenerateScript(targetMigration, targetMigration);
                    migrator.Migrate(targetMigration);
                }
            }
        }
        #endregion

        #region Get Entity Model Types
        public static IEnumerable<IEntityType> GetEntityTypes(this DbContext context)
        {
            return context.Model.GetEntityTypes();
        }

        public static IEnumerable<Type> GetModelTypes(this DbContext context)
        {
            return context.Model.GetEntityTypes().Select(t => t.ClrType);
        }
        #endregion

        #region Ensure Destroyed
        public static void EnsureDestroyed(this DbContext context)
        {
            if (context.Database.IsSqlServer() || context.Database.IsSqlite())
            {
                var dependencies = context.Database.GetService<RelationalDatabaseCreatorDependencies>();
                string masterConnectionString = "";
                using (var masterConnection = ((ISqlServerConnection)dependencies.Connection).CreateMasterConnection())
                {
                    masterConnectionString = masterConnection.ConnectionString;
                }

                var connectionString = context.GetConnectionString();

                DbInitializationHelper.EnsureDestroyed(connectionString);
            }
            else
            {
                context.ClearData();
            }
        }
        #endregion
    }
}

﻿namespace AspNetCore.Base.Data.NoSql.Initializers
{
    public static class DbContextNoSqlInitializationExtensions
    {
        public static  bool EnsureCollectionsDeleted(this DbContextNoSql context)
        {
            foreach (var collectionName in context.Database.GetCollectionNames())
            {
                context.Database.DropCollection(collectionName);
            }

            return true;
        }
    }
}

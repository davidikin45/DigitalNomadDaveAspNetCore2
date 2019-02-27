﻿using AspnetCore.Base.Data.Initializers;

namespace DND.Data.Initializers
{
    public class AppContextInitializerMigrate : ContextInitializerMigrate<AppContext>
    {
        public override void Seed(AppContext context, string tenantId)
        {
            context.Seed();
        }
    }
}

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Transactions;

namespace AspNetCore.Testing
{
    public class Isolated : Attribute, ITestAction
    {
        private TransactionScope _transactionScope;

        public void BeforeTest(ITest test)
        {
            _transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, 
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
        }

        public void AfterTest(ITest test)
        {
            if (_transactionScope != null)
            {
                _transactionScope.Dispose();
            }
            _transactionScope = null;
        }

        public ActionTargets Targets
        {
            get
            {
                return ActionTargets.Test;
            }
        }

    }
}

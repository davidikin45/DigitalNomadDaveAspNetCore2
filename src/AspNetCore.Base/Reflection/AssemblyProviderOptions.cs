using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Reflection
{
    public class AssemblyProviderOptions
    {
        public string BinPath { get; set; }
        public Func<string, Boolean> AssemblyFilter { get; set; }
    }
}

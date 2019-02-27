using System.Diagnostics;

namespace AspNetCore.Testing.Processes
{
    public static class ProcessExtensions
    {
        public static void KillProcessAndChildren(this Process process)
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
    }
}

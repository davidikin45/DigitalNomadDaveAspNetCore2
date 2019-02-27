using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.Testing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureRazorViewEngineForTestServer(this IServiceCollection services, Assembly assembly, string netVersion)
        {
            //https://github.com/aspnet/Hosting/issues/954
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                var previous = options.CompilationCallback;
                options.CompilationCallback = (context) =>
                {
                    previous?.Invoke(context);

                    var assemblies = new List<PortableExecutableReference>();
                    foreach (var x in assembly.GetReferencedAssemblies())
                    {
                        var path = Assembly.Load(x).Location;
                        var refAssembly = MetadataReference.CreateFromFile(path);
                        assemblies.Add(refAssembly);
                    }

                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Http.Features")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.Extensions.Primitives")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.LoadFrom(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\" + netVersion + @"\Facades\netstandard.dll").Location));

                    context.Compilation = context.Compilation.AddReferences(assemblies);
                };
            });
        }
    }
}

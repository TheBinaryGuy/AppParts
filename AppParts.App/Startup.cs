using System.IO;
using System.Linq;
using System.Runtime.Loader;
using AppParts.App.Helpers;
using AppParts.App.Providers;
using AppParts.App.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppParts.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionDescriptorChangeProvider>(PluginActionDescriptorChangeProvider.Instance);
            services.AddSingleton<IPluginService, PluginService>();
            services.AddMvc().AddRazorRuntimeCompilation().ConfigureApplicationPartManager(ConfigureApplicationParts);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        private void ConfigureApplicationParts(ApplicationPartManager apm)
        {
            var pluginsDirectory = Path.Combine("Plugins");

            foreach (var file in FileSystemHelpers.GetDllPaths(pluginsDirectory, "*.dll"))
            {
                if (apm.ApplicationParts.FirstOrDefault(part => part.Name == Path.GetFileNameWithoutExtension(file)) == null)
                {
                    if (Path.GetFileName(file).Contains(".Views.dll"))
                    {
                        // Add assembly to ApplicationParts assuming it's a compiled razor assembly as it ends with *.Views.dll
                        apm.ApplicationParts.Add(new CompiledRazorAssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                    else
                    {
                        // Add assembly to ApplicationParts assuming
                        apm.ApplicationParts.Add(new AssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                }
            }
        }
    }
}

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using AppParts.App.Helpers;
using AppParts.App.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace AppParts.App.Services
{
    public class PluginService : IPluginService
    {
        private readonly ApplicationPartManager _partManager;
        private readonly IWebHostEnvironment _env;

        public PluginService(ApplicationPartManager partManager, IWebHostEnvironment env)
        {
            _partManager = partManager;
            _env = env;
        }

        public async Task<bool> SaveAndExtractPlugin(PluginUploadViewModel model)
        {
            try
            {
                var zipDirectory = Path.Combine(_env.ContentRootPath, "Zips");
                Directory.CreateDirectory(zipDirectory);
                string uploadedZip = Path.Combine(zipDirectory, $"{model.PluginName}.zip");
                using (var stream = new FileStream(uploadedZip, FileMode.Create))
                {
                    await model.PluginZip.CopyToAsync(stream);
                }
                var pluginsDirectory = Path.Combine(_env.ContentRootPath, "Plugins");
                Directory.CreateDirectory(pluginsDirectory);
                string zipDestDir = Path.Combine(pluginsDirectory, $"{model.PluginName}");
                ZipFile.ExtractToDirectory(uploadedZip, zipDestDir, true);
                File.Delete(uploadedZip);
                return true;
            }
            catch { throw; }
        }

        public bool LoadPlugin(string pluginName)
        {
            var pluginDirectory = Path.Combine("Plugins", pluginName);

            foreach (var file in FileSystemHelpers.GetDllPaths(pluginDirectory, "*.dll"))
            {
                if (_partManager.ApplicationParts.FirstOrDefault(part => part.Name == Path.GetFileNameWithoutExtension(file)) == null)
                {
                    if (Path.GetFileName(file).Contains(".Views.dll"))
                    {
                        // Add assembly to ApplicationParts assuming it's a compiled razor assembly as it ends with *.Views.dll
                        _partManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                    else
                    {
                        // Add assembly to ApplicationParts assuming
                        _partManager.ApplicationParts.Add(new AssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                }
            }

            return true;
        }

        public bool LoadAllPlugins(ApplicationPartManager appPartManager)
        {
            var pluginsDirectory = Path.Combine("Plugins");
            foreach (var file in FileSystemHelpers.GetDllPaths(pluginsDirectory, "*.dll"))
            {
                if (_partManager.ApplicationParts.FirstOrDefault(part => part.Name == Path.GetFileNameWithoutExtension(file)) == null)
                {
                    if (Path.GetFileName(file).Contains(".Views.dll"))
                    {
                        // Add assembly to ApplicationParts assuming it's a compiled razor assembly as it ends with *.Views.dll
                        appPartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                    else
                    {
                        // Add assembly to ApplicationParts assuming
                        appPartManager.ApplicationParts.Add(new AssemblyPart(AssemblyLoadContext.Default.LoadFromAssemblyPath(file)));
                    }
                }
            }

            return true;
        }
    }
}
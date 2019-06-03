using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using AppParts.App.Providers;
using AppParts.App.ViewModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

public class PluginService : IPluginService
{
    private readonly ApplicationPartManager _partManager;

    public PluginService(ApplicationPartManager partManager)
    {
        _partManager = partManager;
    }

    public async Task<bool> SaveAndExtractZip(PluginUploadViewModel model)
    {
        try
        {
            string uploadedZip = Path.Combine("Zips", $"{model.PluginName}.zip");
            using (var stream = new FileStream(uploadedZip, FileMode.Create))
            {
                await model.PluginZip.CopyToAsync(stream);
            }
            string zipDestDir = Path.Combine("Plugins", $"{model.PluginName}");
            ZipFile.ExtractToDirectory(uploadedZip, zipDestDir, true);
            File.Delete(uploadedZip);
            return true;
        }
        catch { throw; }
    }

    public bool LoadPlugin(string pluginName)
    {
        var pluginDirectory = Path.Combine("Plugins", pluginName);

        // Get path of all dlls in the plugin directory
        var files = GetDllPaths(pluginDirectory, "*.dll").ToList();
        var assembliesToLoad = new List<Assembly>();
        foreach (var file in files)
        {
            assembliesToLoad.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(file));
        }

        foreach (var assembly in assembliesToLoad)
        {
            if (assembly.EscapedCodeBase.Contains(".Views.dll"))
            {
                // Add assembly to ApplicationParts assuming it's a compiled razor assembly as it ends with *.Views.dll
                _partManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(assembly));
            }
            else
            {
                // Add assembly to ApplicationParts assuming
                _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
        }

        return true;
    }

    private IEnumerable<string> GetDllPaths(string rootFolderPath, string fileSearchPattern)
    {
        Queue<string> pending = new Queue<string>();
        pending.Enqueue(rootFolderPath);
        string[] tmp;
        while (pending.Count > 0)
        {
            rootFolderPath = pending.Dequeue();
            try
            {
                tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            for (int i = 0; i < tmp.Length; i++)
            {
                yield return Path.GetFullPath(tmp[i]);
            }
            tmp = Directory.GetDirectories(rootFolderPath);
            for (int i = 0; i < tmp.Length; i++)
            {
                pending.Enqueue(tmp[i]);
            }
        }
    }
}
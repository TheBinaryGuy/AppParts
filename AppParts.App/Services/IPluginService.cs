using System.Threading.Tasks;
using AppParts.App.ViewModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

public interface IPluginService
{
    Task<bool> SaveAndExtractZip(PluginUploadViewModel model);
    bool LoadPlugin(string pluginName);
}
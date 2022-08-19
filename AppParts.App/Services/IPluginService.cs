using System.Threading.Tasks;
using AppParts.App.ViewModels;

namespace AppParts.App.Services
{
    public interface IPluginService
    {
        Task<bool> SaveAndExtractPlugin(PluginUploadViewModel model);
        bool LoadPlugin(string pluginName);
    }
}
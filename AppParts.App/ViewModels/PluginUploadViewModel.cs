using Microsoft.AspNetCore.Http;

namespace AppParts.App.ViewModels
{
    public class PluginUploadViewModel
    {
        public string PluginName { get; set; }
        public IFormFile PluginZip { get; set; }
    }
}
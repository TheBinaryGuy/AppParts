﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppParts.App.Providers;
using AppParts.App.Services;
using AppParts.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AppParts.App.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationPartManager _partManager;
        private readonly IPluginService _pluginService;

        public HomeController(ApplicationPartManager partManager, IPluginService pluginService)
        {
            _partManager = partManager;
            _pluginService = pluginService;
        }

        [HttpGet("/plugins")]
        public IEnumerable<string> PluginList()
        {
            var partNames = new List<string>();
            foreach (var part in _partManager.ApplicationParts)
            {
                partNames.Add(part.Name);
            }
            return partNames;
        }

        [HttpPost("/plugins")]
        public async Task<IActionResult> Add([FromForm] PluginUploadViewModel model)
        {
            if (model.PluginZip.Length > 0)
            {
                try
                {
                    if (await _pluginService.SaveAndExtractPlugin(model))
                    {
                        if (_pluginService.LoadPlugin(model.PluginName))
                        {
                            PluginActionDescriptorChangeProvider.Instance.HasChanged = true;
                            PluginActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                            return Ok();
                        }
                    }
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is ArgumentException || ex is IOException)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        [HttpDelete("/plugins")]
        public IActionResult Remove(string partName)
        {
            var pluginToRemove = _partManager.ApplicationParts.FirstOrDefault(p => p.Name == partName);
            if (pluginToRemove != null)
            {
                _partManager.ApplicationParts.Remove(pluginToRemove);
                PluginActionDescriptorChangeProvider.Instance.HasChanged = true;
                PluginActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return Ok();
            }
            return NotFound();
        }
    }
}

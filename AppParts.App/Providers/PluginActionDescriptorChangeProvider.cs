using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace AppParts.App.Providers
{
    public class PluginActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static PluginActionDescriptorChangeProvider Instance { get; } = new PluginActionDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
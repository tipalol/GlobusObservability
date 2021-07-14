using System.Linq;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Helpers
{
    public static class MetricsNameHelper
    {
        public static string GenerateName(JsonMetricsModel metric)
        {

            var networks = metric.SubNetworks.Aggregate("", (current, network) => current + (network + ","));

            networks = networks.Remove(networks.Length - 1);

            return $"{metric.Date:u}-{metric.NodeName}-{networks}";
        }
    }
}
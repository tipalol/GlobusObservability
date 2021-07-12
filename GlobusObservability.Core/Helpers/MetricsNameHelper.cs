using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Helpers
{
    public static class MetricsNameHelper
    {
        public static string GenerateName(JsonMetricsModel metric)
            => $"{metric.Date}-{metric.NodeName}-{metric.SubNetworks}";
    }
}
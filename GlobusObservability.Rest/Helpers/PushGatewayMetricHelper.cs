using Prometheus;

namespace GlobusObservability.Rest.Helpers
{
    public class PushGatewayMetricHelper
    {
        private readonly MetricServer _server = new MetricServer(hostname: "172.24.217.5", port: 9091);
        
        public void StartPush()
        {
            _server.Start();
            var metric = Metrics.CreateGauge("HateThisSoft", "Help!", new[] { "omg", "im tired" });
            metric.Set(9999);
            metric.Publish();
        }
    }
}
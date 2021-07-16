using System.Collections.Generic;
using System.Linq;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Infrastructure.Helpers
{
    public class MetricSimplifierHelper
    {
        public List<GlobusMetric> MakeSimple(JsonMetricsModel metricModel)
        {
            var result = new List<GlobusMetric>();
            foreach (var model in metricModel.Metrics)
            {
                foreach (var measures in from values in model.Value from value in values from measures in values select measures)
                {
                    foreach (var (key, ints) in measures.Value)
                    {
                        var name = key;
                        var time = model.Duration;
                        var networks = metricModel.SubNetworks.Aggregate("", (current, net) => current + (net + "-"));
                        var valueType = $"{metricModel.NodeName}-{networks}-{model.Id}-{model.Duration}";

                        result.AddRange(ints.Select(measureValue => new GlobusMetric(name, time, valueType, measureValue)));
                    }
                }
            }

            return result;
        }
    }
}
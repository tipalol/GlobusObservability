using System.Collections.Generic;
using System.Linq;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Infrastructure.Helpers
{
    public class MetricSimplifierHelper
    {
        public List<(string name, string time, string valueType, int value)> MakeSimple(JsonMetricsModel metricModel)
        {
            var result = new List<(string name, string time, string valueType, int value)>();
            foreach (var model in metricModel.Metrics)
            {
                foreach (var measures in from values in model.Value from value in values from measures in values select measures)
                {
                    foreach (var (key, ints) in measures.Value)
                    {
                        var name = key;
                        var time = metricModel.Date.ToString("yyyy-MM-dd-HH-mm-ss");
                        var networks = metricModel.SubNetworks.Aggregate("", (current, net) => current + (net + "-"));
                        var valueType = $"{metricModel.NodeName}-{networks}-{model.Id}-{model.Duration}";

                        result.AddRange(ints.Select(measureValue => (name, time, valueType, measureValue)));
                    }
                }
            }

            return result;
        }
    }
}
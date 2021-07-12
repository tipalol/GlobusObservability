using System.Xml;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing
{
    public interface IParser
    {
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml = null);
    }
}
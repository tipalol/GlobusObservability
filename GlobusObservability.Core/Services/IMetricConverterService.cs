using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Services
{
    public interface IMetricConverterService
    {
        public JsonMetricsModel ConvertToJson(XmlMetricDto xml);
    }
}
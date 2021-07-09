using GlobusObservability.Core;

namespace GlobusObservability.Rest.Services
{
    public interface IMetricConverterService
    {
        public Metric ConvertToJson(string xml);
    }
}
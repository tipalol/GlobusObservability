using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Services
{
    public interface IMetricConverterService
    {
        public Metric ConvertToJson(XmlMetricDto xml);
    }
}
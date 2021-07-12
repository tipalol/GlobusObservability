using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Parsing;
using Serilog;

namespace GlobusObservability.Core.Services
{
    public class MetricConverterService : IMetricConverterService
    {
        private readonly ILogger _logger;
        private readonly ParsingFacade _parsingFacade;

        public MetricConverterService(ILogger logger)
        {
            _logger = logger;
            _parsingFacade = new ();
        }
        
        public JsonMetricsModel ConvertToJson(XmlMetricDto xmlMetric)
        {
            // TODO Detect metric type
            // For specific metric file type
            // use specific ParsingFacade method

            var metric = ParsingFacade.ParseMetricsTypeA(xmlMetric);

            return metric;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Helpers;
using GlobusObservability.Core.Parsing.Parsers;

namespace GlobusObservability.Core.Parsing
{
    public class ParsingFacade
    {
        public static JsonMetricsModel ParseMetricsTypeA(XmlMetricDto xmlMetric)
        {
            var metric = new JsonMetricsModel()
            {
                Name = xmlMetric.FileName
            };

            var parsers = new List<IParser>()
            {
                new FileNameDateParser(),
                new FileNameSubNetworkParser(),
                new FileNameNodeParser(),
                new TypeValuesParser()
            };


            metric = ParseWith(metric, xmlMetric, parsers);

            metric.Name = MetricsNameHelper.GenerateName(metric);

            return metric;
        }

        private static JsonMetricsModel ParseWith(JsonMetricsModel metric, XmlMetricDto xml, IEnumerable<IParser> parsers) 
            => parsers.Aggregate(metric, (current, parser) => parser.ParseValue(current, xml));
    }
}
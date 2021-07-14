using System.Text.RegularExpressions;
using System.Xml;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class FileNameNodeParser : IParser
    {
        private const string Pattern = "MeContext=\\S*_";
        
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            var fileName = xml.FileName;

            var node = Regex.Match(fileName, Pattern).Value;

            node = node.Replace("MeContext=", string.Empty);
            node = node.Replace("_", string.Empty);

            metricsModel.NodeName = node;

            return metricsModel;
        }
    }
}
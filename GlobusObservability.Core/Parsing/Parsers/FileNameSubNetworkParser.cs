using System.Text.RegularExpressions;
using System.Xml;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class FileNameSubNetworkParser : IParser
    {
        private const string Pattern = "SubNetwork=[\\S]*,";
        
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            var fileName = xml.FileName;

            var subNetworks = Regex.Match(fileName, Pattern).Value;

            subNetworks = subNetworks.Replace("SubNetwork=", string.Empty);

            // removing last comma
            subNetworks = subNetworks.Remove(subNetworks.Length - 1);

            metricsModel.SubNetworks = subNetworks;

            return metricsModel;
        }
    }
}
using System;
using System.Text.RegularExpressions;
using System.Xml;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class FileNameDateParser : IParser
    {
        private const string Pattern = "20[0-9]{2}[0-1][0-9][0-9]{2}";
        
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            var fileName = xml.FileName;

            var date = Regex.Match(fileName, Pattern).Value;

            metricsModel.Date = Convert.ToDateTime(date);

            return metricsModel;
        }
    }
}
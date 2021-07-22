using System;
using System.Text.RegularExpressions;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class FileNameDateParser : IParser
    {
        private const string Pattern = "20[0-9]{2}[0-1][0-9][0-9]{2}.[0-9]{4}";
        
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            var fileName = xml.FileName;

            var date = Regex.Match(fileName, Pattern).Value;

            try
            {
                metricsModel.Date = DateTime.ParseExact(date, "yyyyMMdd.HHmm", null);
            }
            catch (Exception e)
            {
                throw new Exception($"Cant parse date from file name. File name: {fileName} xml: {xml.FileContent.Length} json: {metricsModel}");
            }
           
            
            return metricsModel;
        }
    }
}
using System;
using System.Xml;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class TypeValuesParser : IParser
    {
        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            throw new NotImplementedException();
        }
        
        private JsonMetricsModel BuildMetrics(XmlDocument xml)
        {
            var metric = new MetricModel();

            foreach (XmlNode node in xml.ChildNodes)
                BuildStep(node, ref metric);

            metric.Id = "It works!";

            return null; //metric;
        }

        private void BuildStep(XmlNode node, ref MetricModel metricModel)
        {
            /*if (node.Name == XmlKeys["Date"].nodeName) 
                metricModel.Date = Convert.ToDateTime(node.Attributes?[XmlKeys["Date"].propertyName]?.Value);
            
            if (node.Name == XmlKeys["SubNetwork"].nodeName)
                metricModel.SubNetwork = node.Attributes?[XmlKeys["SubNetwork"].propertyName]?.Value;

            if (node.Name == XmlKeys["ValueBlock"].nodeName)
            {
                foreach (XmlNode value in node.ChildNodes)
                {
                    if (value.Name == XmlKeys["Value"].nodeName)
                    {
                        
                    }
                }
            }

            for (var i = 0; i < node.ChildNodes.Count; i++)
                BuildStep(node.ChildNodes[i], ref metricModel);*/
        }
    }
}
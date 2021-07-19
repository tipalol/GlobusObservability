using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Helpers;
using Newtonsoft.Json;

namespace GlobusObservability.Core.Parsing.Parsers
{
    public class TypeValuesParser : IParser
    {
        // Xml Block Keys
        private const string MeasureBlockKey = "measInfo";
        private const string MeasureTypeBlockKey = "measType";
        private const string MeasureValueBlockKey = "measValue";
        private const string MeasureDurationBlock = "granPeriod";
        
        // Xml Property Keys
        private const string MeasureIdProperty = "measInfoId";
        private const string MeasureDurationProperty = "endTime";
        private const string MeasureTypeProperty = "p";
        private const string MeasureNodeProperty = "measObjLdn";

        public JsonMetricsModel ParseValue(JsonMetricsModel metricsModel, XmlMetricDto xml)
        {
            var metrics = BuildMetrics(metricsModel, xml);

            return metrics;
        }
        
        private JsonMetricsModel BuildMetrics(JsonMetricsModel metric, XmlMetricDto xmlDto)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlDto.FileContent);
            
            foreach (XmlNode node in xml.ChildNodes)
                BuildStep(node, ref metric);

            return metric;
        }

        private void BuildStep(XmlNode node, ref JsonMetricsModel metricModel)
        {
            // <measInfo> block
            if (node.Name == MeasureBlockKey)
            {
                // Parse metric date
                var metricDuration = DateTime.Now;
                foreach (XmlNode durationExpected in node.ChildNodes)
                {
                    if (durationExpected.Name != MeasureDurationBlock) continue;
                    
                    metricDuration = DateTime.Parse(durationExpected.Attributes?[MeasureDurationProperty]?.InnerText);
                    break;
                }
                
                var metric = new MetricModel()
                {
                    Id = MetricIdHelper.GetMetricId(node, MeasureIdProperty),
                    Duration = metricDuration,
                    Value = new List<Dictionary<string, Dictionary<string, long[]>>>()
                };

                // <measType> block values
                var measureTypes = new Dictionary<int, string>();
                // <measValue> block values
                var nodeMetrics = new Dictionary<string, Dictionary<string, long[]>>();
                
                // Parsing block inside measInfo block
                foreach (XmlNode measureBlock in node.ChildNodes)
                {
                    // Parsing of <measType> block
                    if (measureBlock.Name == MeasureTypeBlockKey)
                        measureTypes.Add(
                            Convert.ToInt32(measureBlock.Attributes?[MeasureTypeProperty]?.InnerText), 
                            measureBlock.InnerText);
                    
                    if (measureBlock.Name != MeasureValueBlockKey) continue;
                    
                    // Here goes <measValue> block parsing

                    // Specify measureNode
                    var measureNode = measureBlock.Attributes?[MeasureNodeProperty]?.Value;
                    
                    // Parsing of measValue blocks
                    // Result will be like {"Type": 1, "Value": [1,2,3,4]}
                    var measureValues = new List<(int, long[])> ();
                    var debugValue = Array.Empty<string>();

                    try
                    {
                        measureValues =
                        (
                            from XmlNode valueNode in measureBlock.ChildNodes
                            let values = valueNode.InnerText?.Split(',')
                            let debug = debugValue = valueNode.InnerText?.Split(',')
                            select (Array.ConvertAll(values, long.Parse), valueNode) into measValues
                            select (Convert.ToInt32(measValues.valueNode.Attributes?[MeasureTypeProperty]?.InnerText), measValues.Item1)
                        ).ToList();
                    }
                    catch (FormatException e)
                    {
                        Debug.WriteLine(metricModel.Name);

                        foreach (var value in measureValues)
                            Debug.WriteLine("MeasureType: " + value.Item1 + " MeasureValues: " + JsonConvert.SerializeObject(value.Item2));

                        foreach (var s in debugValue)
                            Debug.WriteLine(s);

                        measureValues = (
                            from XmlNode valueNode in measureBlock.ChildNodes
                            select (Convert.ToInt32(valueNode.Attributes?[MeasureTypeProperty]?.InnerText), new long[] { 0 })
                        ).ToList();

                        throw;
                    }

                    // Join measureTypes list with measureValues to specify types
                    // Result will be like {"Type", [0,1,2,3]}
                    var typedMetrics = 
                        measureValues.ToDictionary(measure 
                            => measureTypes[measure.Item1], measure 
                            => measure.Item2);
                    
                    nodeMetrics.Add(measureNode, typedMetrics);
                }
                
                // When measInfo block ended
                metric.Value.Add(nodeMetrics);
                
                metricModel.Metrics.Add(metric);
            }
            
            

            for (var i = 0; i < node.ChildNodes.Count; i++)
                BuildStep(node.ChildNodes[i], ref metricModel);
        }
    }
}
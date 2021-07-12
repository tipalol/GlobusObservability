using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Xml;
using GlobusObservability.Core.Entities;
using Serilog;

namespace GlobusObservability.Core.Services
{
    public class MetricConverterService : IMetricConverterService
    {
        private readonly Dictionary<string, (string nodeName, string propertyName)> XmlKeys =
            new ()
        {
            {
                "Date",
                ("granPeriod", "endTime")
            },
            {
                "SubNetwork",
                ("fileHeader", "dnPrefix")
            },
            {
                "ValueBlock",
                ("measInfo", "measInfoId")
            },
            {
                "Value",
                ("measValue", "")
            }
        };
        
        private readonly ILogger _logger;

        public MetricConverterService(ILogger logger)
        {
            _logger = logger;
        }
        
        public Metric ConvertToJson(XmlMetricDto xmlMetric)
        {
            var xmlDocument = new XmlDocument();
            
            xmlDocument.LoadXml(xmlMetric.FileContent);

            var metric = BuildMetric(xmlDocument);

            return metric;
        }

        private Metric BuildMetric(XmlDocument xml)
        {
            var metric = new Metric();

            foreach (XmlNode node in xml.ChildNodes)
                BuildStep(node, ref metric);

            metric.Id = "It works!";

            return metric;
        }

        private void BuildStep(XmlNode node, ref Metric metric)
        {
            if (node.Name == XmlKeys["Date"].nodeName) 
                metric.Date = Convert.ToDateTime(node.Attributes?[XmlKeys["Date"].propertyName]?.Value);
            
            if (node.Name == XmlKeys["SubNetwork"].nodeName)
                metric.SubNetwork = node.Attributes?[XmlKeys["SubNetwork"].propertyName]?.Value;

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
                BuildStep(node.ChildNodes[i], ref metric);
        }
    }
}
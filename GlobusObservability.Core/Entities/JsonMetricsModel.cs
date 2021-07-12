using System;
using System.Collections.Generic;

namespace GlobusObservability.Core.Entities
{
    public class JsonMetricsModel
    {
        public string Name { get; set; }
        
        public DateTime Date { get; set; }
        
        public string SubNetworks { get; set; }
        
        public string NodeName { get; set; }

        public List<MetricModel> Metrics { get; set; } = new List<MetricModel>();
    }
}
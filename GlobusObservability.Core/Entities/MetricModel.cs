using System;
using System.Collections.Generic;

namespace GlobusObservability.Core.Entities
{
    public class MetricModel
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }

        public Dictionary<string, int[]> Value { get; set; }
    }
}
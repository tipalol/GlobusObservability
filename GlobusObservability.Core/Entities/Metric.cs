using System;
using System.Collections.Generic;

namespace GlobusObservability.Core.Entities
{
    public class Metric
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public string SubNetwork { get; set; }
        
        public Dictionary<string, string> Value { get; set; }
    }
}
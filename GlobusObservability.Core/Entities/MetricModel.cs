using System;
using System.Collections.Generic;

namespace GlobusObservability.Core.Entities
{
    public class MetricModel
    {
        public string Id { get; set; }
        
        public string Duration { get; set; }

        public List<Dictionary<string, Dictionary<string, long[]>>> Value { get; set; }
    }
}
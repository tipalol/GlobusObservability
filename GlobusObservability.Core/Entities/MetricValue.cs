using System.Collections.Generic;

namespace GlobusObservability.Core.Entities
{
    public class MetricValue
    {
        public string Name { get; set; }
        
        public List<Measure> Measures { get; set; }
    }
}
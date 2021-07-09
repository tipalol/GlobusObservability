using System;

namespace GlobusObservability.Core
{
    public class Metric
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public float[] Value { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobusObservability.Rest
{
    public class SimpleMetric
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string ValueType { get; set; }
        public int Value { get; set; }
    }
}

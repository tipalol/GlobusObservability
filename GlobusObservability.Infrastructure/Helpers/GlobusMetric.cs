using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobusObservability.Infrastructure
{
    public class GlobusMetric
    {
        public GlobusMetric(string name, DateTime time, string valueType, int value)
        {
            Name = name;
            Time = time;
            ValueType = valueType;
            Value = value;
        }

        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string ValueType { get; set; }
        public int Value { get; set; }
    }
}

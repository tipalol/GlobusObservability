using System;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Core.Services
{
    public class MetricConverterService : IMetricConverterService
    {
        public Metric ConvertToJson(XmlMetricDto xml)
        {
            // Convert code goes here

            //throw new NotImplementedException();

            return new() {Id = "Congrats!", Date = DateTime.Now, Value = new[]{1f, 2f}};
        }
    }
}
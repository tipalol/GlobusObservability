using System;
using System.Collections.Generic;
using GlobusObservability.Core;

namespace GlobusObservability.Infrastructure.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        public IEnumerable<Metric> GetAllMetrics()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Metric> GetMetricsInPeriod(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void AddMetric(Metric metric)
        {
            throw new NotImplementedException();
        }

        public void AddMetrics(IEnumerable<Metric> metrics)
        {
            throw new NotImplementedException();
        }
    }
}
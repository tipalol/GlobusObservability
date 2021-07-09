using System;
using System.Collections.Generic;
using GlobusObservability.Core;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Infrastructure.Repositories
{
    public interface IMetricRepository
    {
        /// <summary>
        /// Gets all metrics from metrics repository
        /// </summary>
        /// <returns>Enumerable of metrics</returns>
        public IEnumerable<Metric> GetAllMetrics();
        
        /// <summary>
        /// Gets metrics in specified period
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns>Enumerable of metrics</returns>
        public IEnumerable<Metric> GetMetricsInPeriod(DateTime from, DateTime to);

        /// <summary>
        /// Adds new metric to repository
        /// </summary>
        /// <param name="metric">Metric</param>
        public void AddMetric(Metric metric);
        
        /// <summary>
        /// Adds raw xml metric which will be converted to json metric
        /// </summary>
        /// <param name="xmlMetric"></param>
        public void AddRawXml(XmlMetricDto xmlMetric);
    }
}
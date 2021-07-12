using System;
using System.Collections.Generic;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Services;

namespace GlobusObservability.Infrastructure.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly Dictionary<string, Metric> _metrics;
        private readonly IMetricConverterService _metricConverter;

        public MetricRepository(IMetricConverterService metricConverter)
        {
            _metricConverter = metricConverter;
            _metrics = new Dictionary<string, Metric>();
        }
        
        public IEnumerable<Metric> GetAllMetrics()
            => _metrics.Values;

        public IEnumerable<Metric> GetMetricsInPeriod(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void AddMetric(Metric metric)
        {
            if (!_metrics.ContainsKey(metric.Date.ToString("G")))
                _metrics.Add(metric.Date.ToString("G"), metric);
        }

        public void AddRawXml(XmlMetricDto xmlMetric)
        {
            var metric = _metricConverter.ConvertToJson(xmlMetric);
            
            AddMetric(metric);
        }
    }
}
using System;
using System.Collections.Generic;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Services;

namespace GlobusObservability.Infrastructure.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly Dictionary<string, JsonMetricsModel> _metrics;
        private readonly IMetricConverterService _metricConverter;

        public MetricRepository(IMetricConverterService metricConverter)
        {
            _metricConverter = metricConverter;
            _metrics = new Dictionary<string, JsonMetricsModel>();
        }
        
        public IEnumerable<JsonMetricsModel> GetAllMetrics()
            => _metrics.Values;

        public IEnumerable<JsonMetricsModel> GetMetricsInPeriod(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void AddMetric(JsonMetricsModel metricModel)
        {
            if (!_metrics.ContainsKey(metricModel.Name))
                _metrics.Add(metricModel.Name, metricModel);
        }

        public void AddRawXml(XmlMetricDto xmlMetric)
        {
            var metric = _metricConverter.ConvertToJson(xmlMetric);
            
            AddMetric(metric);
        }
    }
}
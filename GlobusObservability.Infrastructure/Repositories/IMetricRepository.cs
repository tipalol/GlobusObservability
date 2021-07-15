using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public IEnumerable<JsonMetricsModel> GetAllMetrics();
        
        /// <summary>
        /// Gets metrics in specified period
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns>Enumerable of metrics</returns>
        public IEnumerable<JsonMetricsModel> GetMetricsInPeriod(DateTime from, DateTime to);

        /// <summary>
        /// Adds new metricModel to repository
        /// </summary>
        /// <param name="metricModel">MetricModel</param>
        public void AddMetric(JsonMetricsModel metricModel);

        /// <summary>
        /// Adds raw xml metricModel which will be converted to json metricModel
        /// </summary>
        /// <param name="xmlMetric"></param>
        public void AddRawXml(XmlMetricDto xmlMetric);

        /// <summary>
        /// Loads xml metrics from IMetricProvider, then convert them to json files
        /// </summary>
        /// <param name="onlyNew">Load only unparsed files</param>
        /// <returns>Collection of json metrics</returns>
        public IEnumerable<JsonMetricsModel> LoadAllMetrics(bool onlyNew);

        public void Clear();

        public IEnumerable<JsonMetricsModel> LoadParsed();

        public Task UploadMetric(JsonMetricsModel model);
    }
}
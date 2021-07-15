using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Services;
using GlobusObservability.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Dapper;
using GlobusObservability.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace GlobusObservability.Infrastructure.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly Dictionary<string, JsonMetricsModel> _metrics;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IMetricConverterService _metricConverter;

        private readonly string _connection = "";

        public MetricRepository(IMetricConverterService metricConverter, IConfiguration config, ILogger logger)
        {
            _metricConverter = metricConverter;
            _logger = logger;
            _config = config;
            _metrics = new Dictionary<string, JsonMetricsModel>();
            _connection = config.GetSection("Database")["ConnectionString"];
        }
        
        public IEnumerable<JsonMetricsModel> GetAllMetrics()
            => _metrics.Values;

        public IEnumerable<JsonMetricsModel> GetMetricsInPeriod(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void AddMetric(JsonMetricsModel metricModel)
        {
            if (!_metrics.ContainsKey(metricModel.Name))
                _metrics.Add(metricModel.Name, metricModel);
        }

        public void UploadMetric(JsonMetricsModel model)
        {
            using IDbConnection connection = new SqlConnection(_connection);
            const string query = @"insert to Globus.Globus (Name, Time, Value_TXT, Value_INT)
                              values (@name, @time, @valueType, @value)";

            var metrics = new MetricSimplifierHelper().MakeSimple(model);

            foreach (var metric in metrics)
            {
                var name = metric.name;
                var time = metric.time;
                var valueType = metric.valueType;
                var value = metric.value;

                connection.Execute(query);
                _logger.Debug($"Send to DB: {name} {time} {valueType} {value}");
            }
            
            

            connection.Execute(query);
        }

        public void AddRawXml(XmlMetricDto xmlMetric)
        {
            var metric = _metricConverter.ConvertToJson(xmlMetric);
            
            AddMetric(metric);
        }

        public void Clear()
        {
            _metrics.Clear();
        }

        public IEnumerable<JsonMetricsModel> LoadParsed()
        {
            var metricsFolder = _config.GetSection("Parsing")["MetricsJsonDestination"];
            
            _logger.Information($"Loading metrics from {metricsFolder}");
            
            var jsonMetrics = new FileMetricsProvider().GetParsed(metricsFolder);

            return jsonMetrics;
        }

        public IEnumerable<JsonMetricsModel> LoadAllMetrics(bool onlyNew)
        {
            var metricsFolder = _config.GetSection("Parsing")["MetricsRootFolder"];
            
            _logger.Information($"Loading metrics from {metricsFolder}");
            
            var xmlMetrics = new FileMetricsProvider().GetAll(onlyNew, metricsFolder);
            
            _logger.Information($"Xml loaded");

            _logger.Information($"Parsing started. Xml files to parse - {xmlMetrics.Count()}");

            var metrics = new List<JsonMetricsModel>();

            foreach (var xml in xmlMetrics)
            {
                metrics.Add(_metricConverter.ConvertToJson(xml));
                _logger.Information($"Xml parsed {xml.FileName}");
            }

            return metrics;
        }
    }
}
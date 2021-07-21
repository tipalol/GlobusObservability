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
using Dapper.Contrib;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using GlobusObservability.Core.Helpers;
using Newtonsoft.Json;

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
            _connection = "Data Source=172.21.224.36;Initial Catalog=SMP;Persist Security Info=True;User ID=Globus;Password=Globus";
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

        public async Task UploadMetric(JsonMetricsModel model)
        {
            using SqlConnection connection = new SqlConnection(_connection);

            var metrics = new MetricSimplifierHelper().MakeSimple(model);

            _logger.Debug($"Started to upload metrics");

            connection.Insert(metrics);
            /*
            foreach (var metric in metrics)
            {
                var name = metric.name;
                var time = model.Date;
                var valueType = metric.valueType.Replace(',', '_');
                var value = metric.value;

                var query = $@"insert into Globus.Globus (Name, Time, Value_TXT, Value_INT)
                              values (@name, @time, @valueType, @value)";

                _logger.Debug($"{query} {name} {valueType}");

                var parameters = new { name = name.ToCharArray(), time = time, valueType = valueType.ToCharArray(), value = value };

                connection.Execute(query, parameters);
                _logger.Debug($"Send to DB: {name} {time} {valueType} {value}");
                await System.Threading.Tasks.Task.Delay(1000);
            }*/
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

        public IEnumerable<string> CleanWrongMetrics()
        {
            var metricsFolder = _config.GetSection("Parsing")["MetricsJsonDestination"];

            var deleted = new FileMetricsProvider().CleanWrong(metricsFolder);
            
            _logger.Debug($"Clean Done. Cleaned files: {JsonConvert.SerializeObject(deleted)}");

            return deleted;
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
                try
                {
                    //metrics.Add(_metricConverter.ConvertToJson(xml));
                    var metric = _metricConverter.ConvertToJson(xml);
                    _logger.Information($"Xml parsed {xml.FileName}");
                    xml.Dispose();
                    MetricsPushToFileHelper.Push(new List<JsonMetricsModel>() { metric }, _config);
                    metric.Dispose();
                }
                catch (Exception e) { }
            }

            return metrics;
        }
    }
}
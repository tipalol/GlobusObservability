using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using GlobusObservability.Infrastructure.Repositories;
using GlobusObservability.Rest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using Newtonsoft.Json;

namespace GlobusObservability.Rest.Controllers
{
    [ApiController]
    [Route("api")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMetricRepository _metricRepository;

        public MetricsController(ILogger logger, IMetricRepository metricRepository, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
            _metricRepository = metricRepository;
        }

        [HttpGet("parseAndPush")]
        public async Task ParseAllAndPushAsync(bool onlyNew)
        {
            var metrics = _metricRepository.LoadAllMetrics(onlyNew);
            
            var paths = MetricsPushToFileHelper.Push(metrics, _configuration);

            await new PushGatewayHelper(_logger).PushMetrics(metrics);
        }

        [HttpGet("pushParsed")]
        public async void PushParsed()
        {
            var metrics = _metricRepository.LoadParsed();
            
            await new PushGatewayHelper(_logger).PushMetrics(metrics);
        }
        
        [HttpGet("uploadParsed")]
        public void UploadParsed()
        {
            var metrics = _metricRepository.LoadParsed();

            foreach (var metric in metrics)
            {
                _metricRepository.UploadMetric(metric);
            }
            
            
        }
        
        [HttpGet("parse")]
        public IEnumerable<string> ParseAll(bool onlyNew)
        {
            var metrics = _metricRepository.LoadAllMetrics(onlyNew);
            
            var paths = MetricsPushToFileHelper.Push(metrics, _configuration);

            return paths;
        }

        [HttpGet("metrics")]
        public IEnumerable<JsonMetricsModel> GetAllMetrics(bool printResults)
        {
            var metrics = _metricRepository.GetAllMetrics();
            
            MetricsPushToFileHelper.Push(metrics, _configuration);
            
            //_metricRepository.Clear();
            
            _logger.Information("GET Request: GetAllMetrics");

            return printResults ? metrics : new JsonMetricsModel[] {  };
        }
        
        

        [HttpGet("metricsInPeriod")]
        public IEnumerable<JsonMetricsModel> GetMetricsInPeriod(DateTime from, DateTime to)
        {
            var metrics = _metricRepository.GetMetricsInPeriod(from, to);
            
            MetricsPushToFileHelper.Push(metrics, _configuration);

            _logger.Information($"GET Request: GetMetricsInPeriod {from} {to}", from, to);

            return metrics;
        }
    }
}
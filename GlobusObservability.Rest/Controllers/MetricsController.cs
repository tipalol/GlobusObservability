using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using GlobusObservability.Core.Entities;
using GlobusObservability.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace GlobusObservability.Rest.Controllers
{
    [ApiController]
    [Route("api")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetricRepository _metricRepository;

        public MetricsController(ILogger logger, IMetricRepository metricRepository)
        {
            _logger = logger;
            _metricRepository = metricRepository;
        }

        [HttpGet("metrics")]
        public IEnumerable<JsonMetricsModel> GetAllMetrics()
        {
            var metrics = _metricRepository.GetAllMetrics();
            
            _logger.Information("GET Request: GetAllMetrics");
            
            System.IO.File.WriteAllText("metrics/debug.txt", JsonConvert.SerializeObject(metrics, Formatting.Indented));

            return metrics;
        }
        
        [HttpGet("metricsInPeriod")]
        public IEnumerable<JsonMetricsModel> GetMetricsInPeriod(DateTime from, DateTime to)
        {
            var metrics = _metricRepository.GetMetricsInPeriod(from, to);
            
            _logger.Information($"GET Request: GetMetricsInPeriod {from} {to}", from, to);

            return metrics;
        }
    }
}
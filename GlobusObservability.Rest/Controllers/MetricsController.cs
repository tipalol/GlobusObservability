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

        [HttpGet("uploadParsed")]
        public async Task UploadParsed()
        {
            var metrics = _metricRepository.LoadParsed();

            int counter = 0;
            foreach (var metric in metrics)
            {
                counter++;

                _logger.Information($"Uploading {counter} file of {metrics.Count()}");

                await _metricRepository.UploadMetric(metric);
                metric.Dispose();
            }
            
            
        }

        [HttpGet("uploadToVM")]
        public async Task UploadToVm()
        {
            var pushHelper = new VmPushHelper(_logger);
            
            var metrics = _metricRepository.LoadParsed();
            
            var counter = 0;
            var jsonMetricsModels = metrics.ToList();

            _logger.Debug("Started to upload metrics");
            foreach (var metric in jsonMetricsModels)
            {
                counter++;

                _logger.Information($"Uploading {counter} file of {jsonMetricsModels.Count()}");

                await pushHelper.Push(metric);
                metric.Dispose();
            }

            metrics = null;
        }

        [HttpGet("parseAndUpload")]
        public async Task ParseAndUpload(bool onlyNew)
        {
            _logger.Information("Parsing started");
            _logger.Information($"Parsed {ParseAll(onlyNew).Count()} files");

            _logger.Information("Starting upload to Sql Server");

            await UploadParsed();
        }
        
        [HttpGet("parse")]
        public IEnumerable<string> ParseAll(bool onlyNew)
        {
            var metrics = _metricRepository.LoadAllMetrics(onlyNew);
            
            var paths = MetricsPushToFileHelper.Push(metrics, _configuration);

            return paths;
        }

        [HttpGet("cleanWrong")]
        public IEnumerable<string> CleanWrongFormats()
        {
            var deleted = _metricRepository.CleanWrongMetrics();

            return deleted;
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
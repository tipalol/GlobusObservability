using System.Collections.Generic;
using GlobusObservability.Core.Entities;
using GlobusObservability.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GlobusObservability.Rest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetricRepository _metricRepository;

        public MetricsController(ILogger logger, IMetricRepository metricRepository)
        {
            _logger = logger;
            _metricRepository = metricRepository;
        }

        [HttpGet]
        public IEnumerable<Metric> GetAllMetrics()
        {
            var metrics = _metricRepository.GetAllMetrics();
            
            _logger.Information("GET Request: GetAllMetrics");

            return metrics;
        }
    }
}
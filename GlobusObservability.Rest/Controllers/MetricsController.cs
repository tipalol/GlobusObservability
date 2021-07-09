using System.Collections.Generic;
using GlobusObservability.Core;
using GlobusObservability.Infrastructure.Repositories;
using GlobusObservability.Rest.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GlobusObservability.Rest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMetricConverterService _metricConverter;
        private readonly IMetricRepository _metricRepository;

        public MetricsController(ILogger logger, IMetricConverterService metricConverter, IMetricRepository repository)
        {
            _logger = logger;
            _metricConverter = metricConverter;
            _metricRepository = repository;
        }

        [HttpGet]
        public IEnumerable<Metric> GetAllMetrics()
        {
            _logger.Debug("Get All Metrics Request");

            return null;
        }
    }
}
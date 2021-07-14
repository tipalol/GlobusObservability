using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using GlobusObservability.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GlobusObservability.Rest.Workers
{
    public class MetricsWatcher : BackgroundService
    {
        private readonly IMetricRepository _metricRepository;
        private readonly FileSystemWatcher _watcher;

        public MetricsWatcher(IMetricRepository repository, IConfiguration config)
        {
            var metricsPath = config.GetSection("Parsing")["MetricsRootFolder"];
            
            _metricRepository = repository;

            _watcher = new FileSystemWatcher() 
            { 
                Path = metricsPath, 
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |  
                               NotifyFilters.DirectoryName |  
                               NotifyFilters.FileName |  
                               NotifyFilters.LastAccess |  
                               NotifyFilters.LastWrite |  
                               NotifyFilters.Security |  
                               NotifyFilters.Size,
                Filter = "*.xml"
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileCreated;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _watcher.EnableRaisingEvents = true;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                //??????? do i really need this??????

                await Task.Delay(1000, stoppingToken);
            }
        }
        
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            var fileName = e.Name;
            var xml = File.ReadAllText(e.FullPath);

            var xmlMetric = new XmlMetricDto() {FileName = fileName, FileContent = xml};
            
            _metricRepository.AddRawXml(xmlMetric);

            var newPath = e.FullPath.Remove(e.FullPath.Length - 1 - 4, 4)
                          + "_parsed.xml";
            
            File.Move(e.FullPath, newPath, true);
        }
        
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            
        }
    }
}
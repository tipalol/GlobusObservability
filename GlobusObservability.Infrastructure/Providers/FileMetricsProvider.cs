using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Infrastructure.Providers
{
    public class FileMetricsProvider : IProvider<XmlMetricDto>
    {
        public IEnumerable<XmlMetricDto> GetAll(bool onlyNew, string metricsFolder)
        {
            var files = Directory.GetFiles(metricsFolder, "*.xml", SearchOption.AllDirectories).ToList();
            
            if (onlyNew)
            {
                var unParsed = new List<string>();
                unParsed.AddRange(files.Where(file => !file.Contains("parsed.xml")));
                files = unParsed;
            }
            
            var xmlMetrics = (
                from file in files 
                let info = new FileInfo(file) 
                let text = File.ReadAllText(file) 
                select new XmlMetricDto() {FileName = info.Name, FileContent = text}
            ).ToList();

            // Add _parsing label to file
            foreach (var file in files)
            {
                if (file.Contains("parsed.xml")) continue;
                
                var newPath = file.Remove(file.Length - 1 - 4, 4)
                              + "_parsed.xml";

                File.Move(file, newPath, true);
            }

            return xmlMetrics;
        }
    }
}
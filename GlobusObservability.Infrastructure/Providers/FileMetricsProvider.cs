using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;

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

        public IEnumerable<string> CleanWrong(string parsedFolder)
        {
            var files = Directory.GetFiles(parsedFolder, "*.json", SearchOption.AllDirectories).ToList();

            var wrongFiles = files.Where(file => new FileInfo(file).Length < 1000);

            var deletedFiles = new List<string>();

            foreach (var file in wrongFiles)
            {
                File.Delete(file);
                deletedFiles.Add(file);
            }

            return deletedFiles;
        }

        public IEnumerable<JsonMetricsModel> GetParsed(string parsedFolder)
        {
            var files = Directory.GetFiles(parsedFolder, "*.json", SearchOption.AllDirectories).ToList();

            try
            {
                var jsonMetrics = (
                    from file in files
                    select JsonConvert.DeserializeObject<JsonMetricsModel>(File.ReadAllText(file))
                ).ToList();

                return jsonMetrics;

            }
            catch (Exception e)
            {
                
            }

            return null;

            var json = new List<JsonMetricsModel>();
            foreach (var file in files)
            {
                try
                {
                    json.Add(JsonConvert.DeserializeObject<JsonMetricsModel>(file));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return null;
        }
    }
}
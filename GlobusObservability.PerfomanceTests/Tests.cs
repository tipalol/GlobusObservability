using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Parsing;
using GlobusObservability.Rest.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GlobusObservability.PerfomanceTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ConvertVmFormat()
        {
            var files = Directory.GetFiles("parsed/");
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var jsonMetricModel = JsonConvert.DeserializeObject<JsonMetricsModel>(json);
                
                Convert(jsonMetricModel);
            }
            
            stopwatch.Stop();
            
            Assert.Less(stopwatch.ElapsedMilliseconds, 100);
        }

        [Test]
        public void ConvertXmlToJson()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            var fileName =
                "A20210528.0900+0300-0915+0300_SubNetwork=ONRM_ROOT_MO,SubNetwork=eNodeB_PNZ,MeContext=GL587657_statsfile.xml";

            var xml = File.ReadAllText("testData/" + fileName);

            var xmlDto = new XmlMetricDto()
            {
                FileName = fileName,
                FileContent = xml
            };

            var metric = ParsingFacade.ParseMetricsTypeA(xmlDto);
            
            

            stopwatch.Stop();
            
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            Assert.Less(stopwatch.ElapsedMilliseconds, 100);
            
            Assert.Pass();
        }

        private void Convert(JsonMetricsModel model)
        {
            var client = new HttpClient();
            var counter = 0;
            var metricCounter = 0;
            var status = new List<Task>();
                foreach (var metric in model.Metrics)
                {
                    status.Add(Task.Run(() =>
                    {
                        counter++;
                    foreach (var values in metric.Value)
                    {
                        foreach (var value in values)
                        {
                            foreach (var measures in values)
                            {
                            var measureCounter = 0;
                                foreach (var measure in measures.Value)
                                {
                                measureCounter++;
                                    var metricId = measure.Key;

                                        metricCounter++;
                                    var vmModel = new VmModel()
                                    {
                                        metric = new Dictionary<string, string>()
                                        {
                                            {"__name__", measure.Key.Replace("statsfill", "")},
                                            {"instance", "GlobusObservability"},
                                            {"job", "GlobusMetrics"},
                                            //{"measureId", metric.Id},
                                            {"nodeName", model.NodeName.Replace("statsfill", "")}
                                            //{"nodeInfo", measures.Key}
                                        },
                                        values = measure.Value,
                                        timestamps = new [] {((DateTimeOffset) metric.Duration).ToUnixTimeMilliseconds()}
                                    };

                                    if (model.SubNetworks.Length >= 1)
                                        vmModel.metric["subNetwork1"] = model.SubNetworks[0];
                                    
                                    if (model.SubNetworks.Length >= 2)
                                        vmModel.metric["subNetwork2"] = model.SubNetworks[1];

                                    if (model.SubNetworks.Length >= 3)
                                        vmModel.metric["subNetwork3"] = model.SubNetworks[2];

                                    var dynamicProperties = measures.Key.Split(',');

                                    foreach (var propertyPair in dynamicProperties)
                                    {
                                        var pair = propertyPair.Split('=');

                                        vmModel.metric.Add(pair[0], pair[1]);
                                    }

                                    var json = JsonConvert.SerializeObject(vmModel);
                                    //File.WriteAllText($"parsed/{}");

                                    //var response = await client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(vmModel)));
                                    //_logger.Debug($"Metric {measureCounter}/{measures.Value.Count} from metric {counter}/{model.Metrics.Count} posted to VM. {JsonConvert.SerializeObject(vmModel)}");
                                    //_logger.Debug($"Response was {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
                                }
                            }
                        }
                    }
                    }));
                
                }

                Task.WaitAll(status.ToArray(), CancellationToken.None);
        }
    }
}
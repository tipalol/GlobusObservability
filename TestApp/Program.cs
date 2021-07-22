using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using GlobusObservability.Rest.Helpers;
using Newtonsoft.Json;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        private static void Test1()
        {
            var files = Directory.GetFiles("parsed/");

            Console.WriteLine("Async Method");
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonMetricModel = JsonConvert.DeserializeObject<JsonMetricsModel>(json);

                Console.WriteLine($"Converting {file}");
                ConvertAsync(jsonMetricModel);
            }
            
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            
            Console.WriteLine("Just A Method");
            Stopwatch stopwatch2 = Stopwatch.StartNew();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonMetricModel = JsonConvert.DeserializeObject<JsonMetricsModel>(json);

                Console.WriteLine($"Converting {file}");
                Convert(jsonMetricModel);
            }
            
            stopwatch2.Stop();

            Console.WriteLine(stopwatch2.ElapsedMilliseconds);
            
            Console.WriteLine("Just A Method 2");
            Stopwatch stopwatch3 = Stopwatch.StartNew();

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonMetricModel = JsonConvert.DeserializeObject<JsonMetricsModel>(json);

                Console.WriteLine($"Converting {file}");
                
                tasks.Add(Task.Run(() => Convert(jsonMetricModel)));
            }

            Task.WaitAll(tasks.ToArray(), CancellationToken.None);
            stopwatch3.Stop();

            Console.WriteLine(stopwatch3.ElapsedMilliseconds);
        }
        
        private static void ConvertAsync(JsonMetricsModel model)
        {
            var client = new HttpClient();
            var counter = 0;
            var metricCounter = 0;
            var status = new List<Task>();
                foreach (var metric in model.Metrics)
                {
                    status.Add(Task.Run(() =>
                    {
                        Interlocked.Increment(ref counter);
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
                Console.WriteLine($"Metrics: {metricCounter}");
        }
        
        private static void Convert(JsonMetricsModel model)
        {
            var client = new HttpClient();
            var counter = 0;
            var metricCounter = 0;
            foreach (var metric in model.Metrics)
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

                }
                Console.WriteLine($"Metrics: {metricCounter}");
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using GlobusObservability.Core.Entities;
using GlobusObservability.Core.Parsing;
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
    }
}
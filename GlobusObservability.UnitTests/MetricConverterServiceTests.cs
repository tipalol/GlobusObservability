using GlobusObservability.Core.Services;
using NUnit.Framework;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace GlobusObservability.UnitTests
{
    public class Tests
    {
        private readonly IMetricConverterService _metricsConverter;
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Ignore("Debug Method")]
        public async Task Test1()
        {
            var _connection = "Data Source=172.21.224.36;Initial Catalog=SMP;Persist Security Info=True;User ID=Globus;Password=Globus";

            using SqlConnection connection = new SqlConnection(_connection);

            var query = @"create table Globus.GlobusMetrics(
                            Id int identity(1,1) not null primary key,
                            Name varchar(255) not null,
                            Time datetime not null,
                            ValueType varchar(100) not null,
                            Value int not null)";

            await connection.ExecuteAsync(query);

            Assert.Pass();
        }
    }
}
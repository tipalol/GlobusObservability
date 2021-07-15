using System.Collections.Generic;
using GlobusObservability.Core.Entities;

namespace GlobusObservability.Infrastructure.Providers
{
    public interface IProvider<out T>
    {
        public IEnumerable<T> GetAll(bool onlyNew, string metricsFolder);

        public IEnumerable<JsonMetricsModel> GetParsed(string parsedFolder);
    }
}
using System.Collections.Generic;

namespace GlobusObservability.Infrastructure.Providers
{
    public interface IProvider<out T>
    {
        public IEnumerable<T> GetAll(bool onlyNew, string metricsFolder);
    }
}
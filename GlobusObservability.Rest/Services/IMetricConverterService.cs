namespace GlobusObservability.Rest.Services
{
    public interface IMetricConverterService
    {
        public string ConvertToJson(string xml);
    }
}
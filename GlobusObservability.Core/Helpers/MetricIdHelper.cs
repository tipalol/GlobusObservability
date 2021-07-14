using System.Text.RegularExpressions;
using System.Xml;

namespace GlobusObservability.Core.Helpers
{
    public static class MetricIdHelper
    {
        public static string GetMetricId(XmlNode node, string measureIdProperty)
        {
            var idNode = node.Attributes?[measureIdProperty]?.Value;

            var pattern = "PmGroup=[\\S]*";

            idNode = Regex.Match(idNode, pattern).Value;

            idNode = idNode?.Replace("PmGroup=", string.Empty);

            return idNode;
        }
    }
}
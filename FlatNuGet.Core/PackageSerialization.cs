using System.Xml.Serialization;

namespace FlatNuGet.Core
{
    public class PackageSerialization
    {
        public static NuGetPackageIndex Deserialize(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(NuGetPackageIndex));
            using (StringReader sr = new StringReader(xml))
            {
                return xmlSerializer.Deserialize(sr) as NuGetPackageIndex;
            }
        }
        public static string Serialize(NuGetPackageIndex package)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(NuGetPackageIndex));
            using (StringWriter sw = new StringWriter())
            {
                xmlSerializer.Serialize(sw, package);
                return sw.ToString();
            }

        }
    }
}
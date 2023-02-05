using System.Xml.Serialization;

namespace FlatNuGet.Core
{
    [Serializable]
    public class NuGetPackageIndex
    {
        public string DownloadCache = "./Caches/NuGet/";
        public bool KeepCache = true;
        //public bool TryResolveDependency= true;
        public List<string> TrackingExtensions = new List<string> { };
        public List<string> Filters = new List<string> { };
        public string TargetDirectory = "./Assets/Plug-in";
        public List<PackageReference> ItemGroup = new List<PackageReference>();
    }
    [Serializable]
    public class PackageReference
    {
        [XmlAttribute]
        public string Include;
        [XmlAttribute]
        public string Version;
    }
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
            string s = "";
            using (StringWriter sw = new StringWriter())
            {
                xmlSerializer.Serialize(sw, package);
                return sw.ToString();
            }

        }
    }
}
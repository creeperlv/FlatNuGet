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
        public NuGetPackageIndex() { }
	}
    [Serializable]
    public class PackageReference
    {
        [XmlAttribute]
        public string Include;
        [XmlAttribute]
        public string Version;
    }
}
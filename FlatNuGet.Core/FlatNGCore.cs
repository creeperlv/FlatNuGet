using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Text.RegularExpressions;

namespace FlatNuGet.Core
{
    public class FlatNGCore
    {
        NuGetPackageIndex _packages;
        FileInfo IndexFile;
        string CacheFolder;
        string TargetFolder;
        public FlatNGCore(FileInfo fileInfo)
        {
            IndexFile = fileInfo;
            if (fileInfo.Exists)
            {
                _packages = PackageSerialization.Deserialize(File.ReadAllText(fileInfo.FullName));
                CacheFolder = Path.Combine(IndexFile.Directory.FullName, _packages.DownloadCache);
                TargetFolder = Path.Combine(IndexFile.Directory.FullName, _packages.TargetDirectory);
            }
        }
        public void InitIndexFile()
        {

            NuGetPackageIndex p = new NuGetPackageIndex();
            p.ItemGroup.Add(new PackageReference { Include = "Newtonsoft.Json", Version = "13.0.2" });
            p.ItemGroup.Add(new PackageReference { Include = "LibCLCC.NET", Version = "1.0.0" });
            File.WriteAllText(IndexFile.FullName, PackageSerialization.Serialize(p));
            _packages = p;
            CacheFolder = Path.Combine(IndexFile.Directory.FullName, _packages.DownloadCache);
        }
        async Task InstallSinglePackage(PackageReference pkg, bool ForceDownload = false)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;
            string packageId = pkg.Include;
            NuGetVersion packageVersion = new NuGetVersion(pkg.Version);
            var NuGetFile = Path.Combine(CacheFolder, $"{pkg.Include}-{pkg.Version}.nupkg");

            if (!File.Exists(NuGetFile) || ForceDownload == true)
            {
                if (File.Exists(NuGetFile)) File.Delete(NuGetFile);
                SourceCacheContext cache = new SourceCacheContext();
                SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
                FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                var d = await resource.GetPackageDownloaderAsync(new PackageIdentity(packageId, packageVersion), cache, logger, cancellationToken);
                await d.CopyNupkgFileToAsync(NuGetFile, cancellationToken);
                OperationRouter.BroadcastOperation(new() { OperationType = OperationType.Download, Message = pkg });
            }
            else
            {
                OperationRouter.BroadcastOperation(new() { OperationType = OperationType.CacheHit, Message = pkg });
            }
            using PackageArchiveReader packageReader = new PackageArchiveReader(NuGetFile);
            var FS = packageReader.GetFiles();
            foreach (var item in FS)
            {
                if (_packages.TrackingExtensions.Count > 0)
                {
                    foreach (var ext in _packages.TrackingExtensions)
                    {

                        if (item.EndsWith(ext))
                        {
                            ExtractFilter(item);
                            break;
                        }
                    }
                }
                else
                {
                    ExtractFilter(item);
                }
            }
            void ExtractFilter(string item)
            {
                if (_packages.Filters.Count > 0)
                {
                    foreach (var f in _packages.Filters)
                    {
                        if (Regex.IsMatch(item, f))
                        {
                            Extract(item);
                            break;
                        }
                    }
                }
                else
                    Extract(item);
            }
            void Extract(string item)
            {
                var target = Path.Combine(TargetFolder, item);
                packageReader.ExtractFile(item, target, logger);
                OperationRouter.BroadcastOperation(new Operation
                {
                    OperationType = OperationType.Extract,
                    Message = new CombinedData { L = pkg, R = item }
                });
            }
        }
        public void ForceUpdate()
        {

            List<Task> tasks = new List<Task>();
            if (!Directory.Exists(CacheFolder))
            {
                Directory.CreateDirectory(CacheFolder);
            }
            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }
            foreach (var item in _packages.ItemGroup)
            {
                tasks.Add(InstallSinglePackage(item, true));

                //InstallSinglePackage(item,true).Wait();
            }
            foreach (var item in tasks)
            {
                item.Wait();
            }
            if (!_packages.KeepCache)
            {
                Directory.Delete(CacheFolder, true);
            }
        }
        public void Remove(string name)
        {
            for (int i = 0; i < _packages.ItemGroup.Count; i++)
            {
                var item = _packages.ItemGroup[i];
                if (item.Include == name)
                {
                    _packages.ItemGroup.Remove(item);
                    break;
                }
            }
            if (File.Exists(IndexFile.FullName)) File.Delete(IndexFile.FullName);
            File.WriteAllText(IndexFile.FullName, PackageSerialization.Serialize(_packages));
            CacheFolder = Path.Combine(IndexFile.Directory.FullName, _packages.DownloadCache);
        }
        public void Track(string Extension)
        {
            if (_packages.TrackingExtensions.Contains(Extension)) return;
            _packages.TrackingExtensions.Add(Extension);
            if (File.Exists(IndexFile.FullName)) File.Delete(IndexFile.FullName);
            File.WriteAllText(IndexFile.FullName, PackageSerialization.Serialize(_packages));
        }
        public void Filter(string filter)
        {
            if (_packages.Filters.Contains(filter)) return;
            _packages.Filters.Add(filter);
            if (File.Exists(IndexFile.FullName)) File.Delete(IndexFile.FullName);
            File.WriteAllText(IndexFile.FullName, PackageSerialization.Serialize(_packages));
        }
        public void Add(string desc)
        {
            var G = desc.Split('/');
            bool Hit = false;
            foreach (var item in _packages.ItemGroup)
            {
                if (item.Include == G[0])
                {
                    item.Version = G[1];
                    Hit = true;
                    break;
                }
            }
            if (!Hit)
            {
                PackageReference packageReference = new PackageReference { Include = G[0], Version = G[1] };
                _packages.ItemGroup.Add(packageReference);
            }
            if (File.Exists(IndexFile.FullName))
                File.Delete(IndexFile.FullName);
            File.WriteAllText(IndexFile.FullName, PackageSerialization.Serialize(_packages));
            CacheFolder = Path.Combine(IndexFile.Directory.FullName, _packages.DownloadCache);

        }
        public void Update()
        {
            List<Task> tasks = new List<Task>();
            if (!Directory.Exists(CacheFolder))
            {
                Directory.CreateDirectory(CacheFolder);
            }
            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }
            foreach (var item in _packages.ItemGroup)
            {
                tasks.Add(InstallSinglePackage(item));
                //InstallSinglePackage(item).Wait();
            }
            foreach (var item in tasks)
            {
                item.Wait();
            }
            if (!_packages.KeepCache)
            {
                Directory.Delete(CacheFolder, true);
            }
        }
    }

}
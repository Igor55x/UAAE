using System;
using System.Collections.Generic;
using System.IO;

namespace UnityTools
{
    public class AssetsFileInstance
    {
        public Stream AssetsStream => file.readerPar;
        public string path;
        public string name;
        public AssetsFile file;
        public AssetsFileTable table;
        public List<AssetsFileInstance> dependencies;
        public BundleFileInstance parentBundle = null;
        //for monobehaviours
        public Dictionary<uint, string> monoIdToName;

        public AssetsFileInstance(Stream stream, string filePath, string root)
        {
            path = Path.GetFullPath(filePath);
            name = Path.Combine(root, Path.GetFileName(path));
            file = new AssetsFile(new AssetsFileReader(stream));
            table = new AssetsFileTable(file);
            dependencies = new List<AssetsFileInstance>(new AssetsFileInstance[file.dependencies.dependencyCount]);
        }

        public AssetsFileInstance(FileStream stream, string root)
        {
            path = stream.Name;
            name = Path.Combine(root, Path.GetFileName(path));
            file = new AssetsFile(new AssetsFileReader(stream));
            table = new AssetsFileTable(file);
            dependencies = new List<AssetsFileInstance>(new AssetsFileInstance[file.dependencies.dependencyCount]);
        }

        public AssetsFileInstance GetDependency(AssetsManager am, int depIdx)
        {
            if (dependencies[depIdx] != null)
                return dependencies[depIdx];

            var depPath = file.dependencies.dependencies[depIdx].assetPath;

            if (depPath == string.Empty)
            {
                return null;
            }

            var instIndex = am.files.FindIndex(f => string.Equals(Path.GetFileName(f.path), Path.GetFileName(depPath), StringComparison.CurrentCultureIgnoreCase));
            if (instIndex == -1)
            {
                var pathDir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(pathDir))
                {
                    pathDir = Environment.CurrentDirectory;
                }
                var absPath = Path.Combine(pathDir, depPath);
                var localAbsPath = Path.Combine(pathDir, Path.GetFileName(depPath));
                if (File.Exists(absPath))
                {
                    dependencies[depIdx] = am.LoadAssetsFile(File.OpenRead(absPath), true);
                }
                else if (File.Exists(localAbsPath))
                {
                    dependencies[depIdx] = am.LoadAssetsFile(File.OpenRead(localAbsPath), true);
                }
                else if (parentBundle != null)
                {
                    dependencies[depIdx] = am.LoadAssetsFileFromBundle(parentBundle, depPath, true);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                dependencies[depIdx] = am.files[instIndex];
            }
            return dependencies[depIdx];
        }
    }
}

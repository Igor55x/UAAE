using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
using UnityTools;
using Mono.Cecil;

namespace AssetsAdvancedEditor.Assets
{
    public class AssetsWorkspace
    {
        public AssetsManager Am { get; }
        public PluginManager Pm { get; }
        public AssetsFileInstance MainInstance { get; }
        public bool FromBundle { get; }

        public List<AssetsFileInstance> LoadedFiles { get; }
        public List<AssetItem> LoadedAssets { get; }

        public Dictionary<string, AssetsFileInstance> LoadedFileLookup { get; }
        public Dictionary<string, AssemblyDefinition> LoadedAssemblies { get; }

        public AssetImporter Importer { get; }
        public AssetExporter Exporter { get; }

        public Dictionary<int, List<AssetsReplacer>> NewReplacers { get; }
        public Dictionary<AssetID, AssetsReplacer> NewAssets { get; }
        public Dictionary<AssetID, Stream> NewAssetDatas { get; }

        public bool Modified { get; set; }
        public string AssetsFileName { get; }
        public string AssetsRootDir { get; }
        public string UnityVersion { get; }

        public AssetsWorkspace(AssetsManager am, AssetsFileInstance file, bool fromBundle = false)
        {
            Am = am;
            Pm = new PluginManager(am);
            Pm.LoadPluginsLibrary(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins.dll"));
            MainInstance = file;
            FromBundle = fromBundle;

            LoadedFiles = new List<AssetsFileInstance>();
            LoadedAssets = new List<AssetItem>();

            LoadedFileLookup = new Dictionary<string, AssetsFileInstance>();
            LoadedAssemblies = new Dictionary<string, AssemblyDefinition>();

            Importer = new AssetImporter(this);
            Exporter = new AssetExporter(this);

            NewReplacers = new Dictionary<int, List<AssetsReplacer>>();
            NewAssets = new Dictionary<AssetID, AssetsReplacer>();
            NewAssetDatas = new Dictionary<AssetID, Stream>();

            Modified = false;

            AssetsFileName = file.name;
            AssetsRootDir = Path.GetDirectoryName(AssetsFileName);
            UnityVersion = MainInstance.file.typeTree.unityVersion;
        }

        public void AddReplacer(ref AssetItem item, AssetsReplacer replacer, MemoryStream previewStream = null)
        {
            if (item == null || replacer == null) return;
            var fileId = replacer.GetFileID();
            var forInstance = LoadedFiles[fileId];
            var assetId = new AssetID(forInstance.path, replacer.GetPathID());

            if (NewAssets.ContainsKey(assetId))
                RemoveReplacer(replacer);

            NewAssets[assetId] = replacer;
            if (NewReplacers.ContainsKey(fileId))
            {
                NewReplacers[fileId].Add(replacer);
            }
            else
            {
                NewReplacers.Add(fileId, new List<AssetsReplacer> { replacer });
            }

            if (previewStream == null)
            {
                var newStream = new MemoryStream();
                var newWriter = new AssetsFileWriter(newStream);
                replacer.Write(newWriter);
                newStream.Position = 0;
                previewStream = newStream;
            }

            NewAssetDatas[assetId] = previewStream;

            if (replacer is AssetsRemover)
            {
                LoadedAssets.Remove(item);
            }
            else
            {
                var reader = new AssetsFileReader(previewStream)
                {
                    BigEndian = false
                };
                item.Position = 0L;
                item.Cont = new AssetContainer(reader, forInstance);
                UpdateAssetInfo(ref item, replacer);
            }

            Modified = true;
        }

        public void RemoveReplacer(AssetsReplacer replacer, bool closePreviewStream = true)
        {
            if (replacer == null) return;
            var fileId = replacer.GetFileID();
            var forInstance = LoadedFiles[fileId];
            var assetId = new AssetID(forInstance.path, replacer.GetPathID());

            NewAssets.Remove(assetId);
            NewReplacers[fileId].Remove(replacer);
            if (NewAssetDatas.ContainsKey(assetId))
            {
                if (closePreviewStream)
                    NewAssetDatas[assetId].Close();
                NewAssetDatas.Remove(assetId);
            }

            Modified = NewAssets.Count != 0;
        }

        public void UpdateAssetInfo(ref AssetItem item, AssetsReplacer replacer)
        {
            if (item == null || replacer == null) return;
            Extensions.GetAssetNameFast(Am.classFile, item, out _, out var listName, out var name);

            item.Name = name;
            item.ListName = listName;
            item.Size = replacer.GetSize();
            item.Modified = "*";
        }

        public void MakeAssetContainer(ref AssetItem item, bool onlyInfo = false, bool forceFromCldb = false)
        {
            if (item == null) return;
            var cont = item.Cont;
            if (!onlyInfo && !cont.HasInstance)
            {
                var templateField = GetTemplateField(item, forceFromCldb);
                var typeInst = new AssetTypeInstance(templateField, cont.FileReader, item.Position);
                cont = new AssetContainer(cont, typeInst);
            }
            item.Cont = cont;
        }

        public AssetContainer GetAssetContainer(int fileId, long pathId)
        {
            var item = LoadedAssets.FirstOrDefault(i => i.FileID == fileId && i.PathID == pathId);
            return item?.Cont;
        }

        public AssetContainer GetAssetContainer(AssetTypeValueField pptrField)
        {
            var fileId = pptrField.Get("m_FileID").GetValue().AsInt();
            var pathId = pptrField.Get("m_PathID").GetValue().AsInt64();
            return GetAssetContainer(fileId, pathId);
        }

        public AssetTypeTemplateField GetTemplateField(AssetItem item, bool forceFromCldb = false)
        {
            var file = LoadedFiles[item.FileID].file;
            var hasTypeTree = file.typeTree.hasTypeTree;
            var baseField = new AssetTypeTemplateField();
            var scriptIndex = item.MonoID;
            var fixedId = AssetHelper.FixAudioID(item.TypeID);

            if (hasTypeTree && !forceFromCldb)
            {
                var type0d = AssetHelper.FindTypeTreeTypeByID(file.typeTree, fixedId, scriptIndex);
                if (type0d is {ChildrenCount: > 0})
                {
                    baseField.From0D(type0d);
                    return baseField;
                }
            }
            baseField.FromClassDatabase(Am.classFile, AssetHelper.FindAssetClassByID(Am.classFile, fixedId), 0);
            return baseField;
        }

        public AssetTypeValueField GetBaseField(AssetItem item)
        {
            var cont = item.Cont;
            var fileInst = cont.FileInstance;
            if (item.TypeID is AssetClassID.MonoBehaviour)
            {
                var tt = fileInst.file.typeTree;
                //check if typetree data exists already
                if (!tt.hasTypeTree || AssetHelper.FindTypeTreeTypeByScriptIndex(tt, item.MonoID) == null)
                {
                    var filePath = Path.GetDirectoryName(fileInst.parentBundle != null ? fileInst.parentBundle.path : fileInst.path);
                    var managedPath = Path.Combine(filePath ?? Environment.CurrentDirectory, "Managed");
                    if (!Directory.Exists(managedPath))
                    {
                        var ofd = new OpenFolderDialog
                        {
                            Title = @"Select a folder for assemblies"
                        };
                        if (ofd.ShowDialog() != DialogResult.OK)
                            return cont.TypeInstance?.GetBaseField();

                        managedPath = ofd.Folder;
                    }
                    var monoField = GetMonoBaseField(item, managedPath);
                    if (monoField != null)
                        return monoField;
                }
            }
            return cont.TypeInstance?.GetBaseField();
        }

        public AssetTypeValueField GetMonoBaseField(AssetItem item, string managedPath)
        {
            var file = item.Cont.FileInstance.file;
            var reader = item.Cont.FileReader;
            var baseTemp = new AssetTypeTemplateField();
            baseTemp.FromClassDatabase(Am.classFile, AssetHelper.FindAssetClassByID(Am.classFile, item.TypeID), 0);
            var mainAti = new AssetTypeInstance(baseTemp, reader, item.Position);
            var scriptIndex = item.MonoID;
            if (scriptIndex != 0xFFFF)
            {
                var monoScriptCont = GetAssetContainer(mainAti.GetBaseField().Get("m_Script"));
                if (monoScriptCont == null)
                    return null;

                var scriptBaseField = monoScriptCont.TypeInstance.GetBaseField();
                var scriptName = scriptBaseField.Get("m_Name").GetValue().AsString();
                var scriptNamespace = scriptBaseField.Get("m_Namespace").GetValue().AsString();
                var assemblyName = scriptBaseField.Get("m_AssemblyName").GetValue().AsString();
                var assemblyPath = Path.Combine(managedPath, assemblyName);

                if (scriptNamespace != string.Empty)
                    scriptName = scriptNamespace + "." + scriptName;

                if (File.Exists(assemblyPath))
                {
                    if (!LoadedAssemblies.ContainsKey(assemblyName))
                    {
                        LoadedAssemblies.Add(assemblyName, MonoDeserializer.GetAssemblyWithDependencies(assemblyPath));
                    }
                    var asmDef = LoadedAssemblies[assemblyName];

                    var mc = new MonoDeserializer();
                    mc.Read(scriptName, asmDef, file.header.Version);
                    var monoTemplateFields = mc.children;

                    var templateField = baseTemp.children.Concat(monoTemplateFields).ToList();
                    baseTemp.children = templateField;
                    baseTemp.childrenCount = baseTemp.children.Count;

                    mainAti = new AssetTypeInstance(baseTemp, reader, item.Position);
                }
            }
            return mainAti.GetBaseField();
        }

        public void GenerateAssetsFileLookup()
        {
            foreach (var fileInst in LoadedFiles)
            {
                LoadedFileLookup[fileInst.path.ToLower()] = fileInst;
            }
        }

        public void ClearModified()
        {
            foreach (var item in LoadedAssets)
            {
                item.Modified = "";
            }
        }
    }
}

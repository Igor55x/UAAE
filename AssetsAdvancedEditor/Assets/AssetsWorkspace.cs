using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AssetsAdvancedEditor.Plugins;
using AssetsAdvancedEditor.Utils;
using UnityTools;
using Mono.Cecil;
using System.Linq;

namespace AssetsAdvancedEditor.Assets
{
    public class AssetsWorkspace
    {
        public AssetsManager Am { get; }
        public PluginManager Pm { get; }
        public AssetsFileInstance MainInstance { get; }
        public bool FromBundle { get; }

        public List<AssetsFileInstance> LoadedFiles { get; }
        public Dictionary<AssetID, AssetItem> LoadedAssets { get; }

        public Dictionary<string, AssetsFileInstance> LoadedFileLookup { get; }
        public Dictionary<string, AssemblyDefinition> LoadedAssemblies { get; }

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

            LoadedFiles = new List<AssetsFileInstance>
            {
                file
            };
            LoadedAssets = new Dictionary<AssetID, AssetItem>();

            LoadedFileLookup = new Dictionary<string, AssetsFileInstance>();
            LoadedAssemblies = new Dictionary<string, AssemblyDefinition>();

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

            // Make stream to use as a replacement to the one from file
            if (previewStream == null)
            {
                var newStream = new MemoryStream();
                var newWriter = new AssetsFileWriter(newStream);
                replacer.Write(newWriter);
                newStream.Position = 0;
                previewStream = newStream;
            }

            if (NewReplacers.ContainsKey(fileId))
            {
                for (var i = 0; i < NewReplacers[fileId].Count; i++)
                {
                    if (NewReplacers[fileId][i].GetPathID() == replacer.GetPathID())
                    {
                        NewReplacers[fileId][i] = replacer;
                    }
                    else
                    {
                        NewReplacers[fileId].Add(replacer);
                    }
                }
            }
            else
            {
                NewReplacers.Add(fileId, new List<AssetsReplacer> { replacer });
            }

            NewAssetDatas[assetId] = previewStream;

            if (replacer is AssetsRemover)
            {
                LoadedAssets.Remove(assetId);
            }
            else
            {
                var reader = new AssetsFileReader(previewStream)
                {
                    BigEndian = false
                };
                var cont = new AssetContainer(reader, forInstance);
                Extensions.GetAssetItemFast(Am.classFile, cont, replacer, out var newItem);
                MakeAssetContainer(ref newItem);
                item.SetSubItems(newItem);
                LoadedAssets[assetId] = item;
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

        public void MakeAssetContainer(ref AssetItem item, bool onlyInfo = false)
        {
            var cont = item.Cont;
            if (!onlyInfo && !cont.HasInstance)
            {
                var templateField = GetTemplateField(item);
                var typeInst = new AssetTypeInstance(templateField, cont.FileReader, item.Position);
                cont = new AssetContainer(cont, typeInst);
                item.Cont = cont;
            }
        }

        public AssetItem GetAssetItem(AssetID assetId, bool onlyInfo = false)
        {
            if (LoadedAssets.TryGetValue(assetId, out var item))
            {
                if (!item.Cont.HasInstance)
                    MakeAssetContainer(ref item, onlyInfo);
                return item;
            }
            return null;
        }

        public AssetItem GetAssetItem(int fileId, long pathId, bool onlyInfo = false)
        {
            if (fileId < 0 || fileId >= LoadedFiles.Count)
                return null;

            var fileInst = LoadedFiles[fileId];
            var assetId = new AssetID(fileInst.path, pathId);
            return GetAssetItem(assetId, onlyInfo);
        }

        public AssetItem GetAssetItem(AssetTypeValueField pptrField)
        {
            var fileId = pptrField.Get("m_FileID").GetValue().AsInt();
            var pathId = pptrField.Get("m_PathID").GetValue().AsInt64();
            return GetAssetItem(fileId, pathId);
        }

        public AssetTypeTemplateField GetTemplateField(AssetItem item, bool deserializeMono = true)
        {
            var cont = item.Cont;
            var fileInst = cont.FileInstance;
            var typeTree = fileInst.file.typeTree;
            var hasTypeTree = typeTree.hasTypeTree;
            var fixedId = AssetHelper.FixAudioID(item.TypeID);
            var scriptIndex = item.MonoID;

            var baseField = new AssetTypeTemplateField();
            if (hasTypeTree)
            {
                var type0d = AssetHelper.FindTypeTreeTypeByID(typeTree, fixedId, scriptIndex);

                if (type0d != null && type0d.ChildrenCount > 0)
                    baseField.From0D(type0d);
                else // Fallback to cldb
                    baseField.FromClassDatabase(Am.classFile, AssetHelper.FindAssetClassByID(Am.classFile, fixedId));
            }
            else
            {
                if (fixedId is AssetClassID.MonoBehaviour && deserializeMono)
                {
                    // Check if typetree data exists already
                    if (!hasTypeTree || AssetHelper.FindTypeTreeTypeByScriptIndex(typeTree, scriptIndex) == null)
                    {
                        var filePath = Path.GetDirectoryName(fileInst.parentBundle != null ? fileInst.parentBundle.path : fileInst.path);
                        var managedPath = Path.Combine(filePath ?? Environment.CurrentDirectory, "Managed");
                        if (Directory.Exists(managedPath))
                        {
                            return GetMonoTemplateField(item, managedPath);
                        }
                        else
                        {
                            var ofd = new OpenFolderDialog
                            {
                                Title = @"Select a folder for assemblies"
                            };
                            if (ofd.ShowDialog() is DialogResult.OK)
                            {
                                managedPath = ofd.Folder;
                                return GetMonoTemplateField(item, managedPath);
                            }
                        }
                    }
                }
                baseField.FromClassDatabase(Am.classFile, AssetHelper.FindAssetClassByID(Am.classFile, fixedId));
            }
            return baseField;
        }

        public AssetTypeValueField GetBaseField(AssetItem item)
        {
            if (!item.Cont.HasInstance)
                MakeAssetContainer(ref item);
            return item.Cont.TypeInstance?.GetBaseField();
        }

        public AssetTypeValueField GetBaseField(int fileId, long pathId)
        {
            var item = GetAssetItem(fileId, pathId);
            if (item != null)
                return GetBaseField(item);
            else
                return null;
        }

        public AssetTypeValueField GetBaseField(AssetTypeValueField pptrField)
        {
            var item = GetAssetItem(pptrField);
            if (item != null)
                return GetBaseField(item);
            else
                return null;
        }

        public AssetTypeValueField GetMonoBaseField(AssetItem item, string managedPath)
        {
            var baseTemp = GetMonoTemplateField(item, managedPath);
            return new AssetTypeInstance(baseTemp, item.Cont.FileReader, item.Position).GetBaseField();
        }

        public AssetTypeTemplateField GetMonoTemplateField(AssetItem item, string managedPath)
        {
            var cont = item.Cont;
            var file = cont.FileInstance.file;
            var baseTemp = GetTemplateField(item, false);

            var scriptIndex = item.MonoID;
            if (scriptIndex != 0xFFFF)
            {
                var baseField = new AssetTypeInstance(baseTemp, cont.FileReader, item.Position).GetBaseField();

                var monoScriptItem = GetAssetItem(baseField.Get("m_Script"));
                if (monoScriptItem == null)
                    return baseTemp;

                var scriptBaseField = monoScriptItem.Cont.TypeInstance.GetBaseField();
                var scriptClassName = scriptBaseField.Get("m_ClassName").GetValue().AsString();
                var scriptNamespace = scriptBaseField.Get("m_Namespace").GetValue().AsString();
                var assemblyName = scriptBaseField.Get("m_AssemblyName").GetValue().AsString();
                var assemblyPath = Path.Combine(managedPath, assemblyName);

                if (scriptNamespace != string.Empty)
                    scriptClassName = scriptNamespace + "." + scriptClassName;

                if (!File.Exists(assemblyPath))
                    return baseTemp;

                if (!LoadedAssemblies.ContainsKey(assemblyName))
                {
                    LoadedAssemblies.Add(assemblyName, MonoDeserializer.GetAssemblyWithDependencies(assemblyPath));
                }
                var asmDef = LoadedAssemblies[assemblyName];

                var mc = new MonoDeserializer();
                mc.Read(scriptClassName, asmDef, new UnityVersion(file.typeTree.unityVersion));
                var monoTemplateFields = mc.children;

                baseTemp.AddChildren(monoTemplateFields);
            }
            return baseTemp;
        }

        public void GenerateAssetsFileLookup()
        {
            for (var i = 0; i < LoadedFiles.Count; i++)
            {
                var fileInst = LoadedFiles[i];
                LoadedFileLookup[fileInst.path.ToLower()] = fileInst;
            }
        }

        public void ClearModified()
        {
            Modified = false;
            foreach (var (_, item) in LoadedAssets)
            {
                item.ClearModified();
            }
        }
    }
}

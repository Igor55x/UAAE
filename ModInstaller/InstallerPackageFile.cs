using System;
using System.Collections.Generic;
using System.Text;
using UnityTools;
using UnityTools.EndianIO;

namespace ModInstaller
{
    public class InstallerPackageFile
    {
        public string magic;
        public bool includesCldb;
        public string modName;
        public string modCreators;
        public string modDescription;
        public ClassDatabaseFile addedTypes;
        public List<InstallerPackageAssetsDescription> affectedFiles;

        public bool Read(EndianReader reader, bool prefReplacersInMemory = false)
        {
            reader.BigEndian = false;

            magic = reader.ReadStringLength(4);
            if (magic != "EMIP")
                return false;

            includesCldb = reader.ReadBoolean();

            modName = reader.ReadCountStringInt16();
            modCreators = reader.ReadCountStringInt16();
            modDescription = reader.ReadCountStringInt16();

            if (includesCldb)
            {
                addedTypes = new ClassDatabaseFile();
                addedTypes.Read(reader);
                //get past the string table since the reader goes back to the beginning
                reader.Position = addedTypes.header.stringTablePos + addedTypes.header.stringTableLen;
            }
            else
            {
                addedTypes = null;
            }

            var affectedFilesCount = reader.ReadInt32();
            affectedFiles = new List<InstallerPackageAssetsDescription>();
            for (var i = 0; i < affectedFilesCount; i++)
            {
                var replacers = new List<object>();
                var desc = new InstallerPackageAssetsDescription
                {
                    isBundle = reader.ReadBoolean(),
                    path = reader.ReadCountStringInt16()
                };
                var replacerCount = reader.ReadInt32();
                for (var j = 0; j < replacerCount; j++)
                {
                    var repObj = ParseReplacer(reader, prefReplacersInMemory);
                    if (repObj is AssetsReplacer repAsset)
                    {
                        replacers.Add(repAsset);
                    }
                    else if (repObj is BundleReplacer repBundle)
                    {
                        replacers.Add(repBundle);
                    }
                }
                desc.replacers = replacers;
                affectedFiles.Add(desc);
            }

            return true;
        }

        public void Write(EndianWriter writer)
        {
            writer.BigEndian = false;

            writer.Write(Encoding.ASCII.GetBytes(magic));

            writer.Write(includesCldb);

            writer.WriteCountStringInt16(modName);
            writer.WriteCountStringInt16(modCreators);
            writer.WriteCountStringInt16(modDescription);

            if (includesCldb)
            {
                addedTypes.Write(writer);
                writer.Position = addedTypes.header.stringTablePos + addedTypes.header.stringTableLen;
            }

            writer.Write(affectedFiles.Count);
            foreach (var desc in affectedFiles)
            {
                writer.Write(desc.isBundle);
                writer.WriteCountStringInt16(desc.path);

                writer.Write(desc.replacers.Count);
                for (var j = 0; j < desc.replacers.Count; j++)
                {
                    var repObj = desc.replacers[j];
                    if (repObj is AssetsReplacer repAsset)
                    {
                        repAsset.WriteReplacer(writer);
                    }
                    else if (repObj is BundleReplacer repBundle)
                    {
                        repBundle.WriteReplacer(writer);
                    }
                }
            }
        }

        private static object ParseReplacer(EndianReader reader, bool prefReplacersInMemory)
        {
            var replacerType = reader.ReadInt16();
            var fileType = reader.ReadByte();
            switch (fileType)
            {
                //BundleReplacer
                case 0:
                {
                    var oldName = reader.ReadCountStringInt16();
                    var newName = reader.ReadCountStringInt16();
                    var hasSerializedData = reader.ReadBoolean();
                    var replacerCount = reader.ReadInt64();
                    var replacers = new List<AssetsReplacer>();
                    for (var i = 0; i < replacerCount; i++)
                    {
                        var assetReplacer = (AssetsReplacer)ParseReplacer(reader, prefReplacersInMemory);
                        replacers.Add(assetReplacer);
                    }

                    if (replacerType == 4) //BundleReplacerFromAssets
                    {
                        //we have to null the assetsfile here and call init later
                        var replacer = new BundleReplacerFromAssets(oldName, newName, null, replacers, 0);
                        return replacer;
                    }

                    break;
                }
                //AssetsReplacer
                case 1:
                    {
                        var hasSerializedData = reader.ReadBoolean();
                        var fileId = reader.ReadInt32();
                        var pathId = reader.ReadInt64();
                        var classId = (AssetClassID)reader.ReadInt32();
                        var monoScriptIndex = reader.ReadUInt16();

                        var preloadDependencies = new List<AssetPPtr>();
                        var preloadDependencyCount = reader.ReadInt32();
                        for (var i = 0; i < preloadDependencyCount; i++)
                        {
                            var pptr = new AssetPPtr()
                            {
                                fileID = reader.ReadInt32(),
                                pathID = reader.ReadInt64()
                            };
                            preloadDependencies.Add(pptr);
                        }

                        switch (replacerType)
                        {
                            //remover
                            case 0:
                            {
                                var replacer = new AssetsRemover(fileId, pathId, classId, monoScriptIndex);
                                if (preloadDependencyCount != 0)
                                    replacer.SetPreloadDependencies(preloadDependencies);

                                return replacer;
                            }
                            //adder/replacer?
                            case 2:
                            {
                                var propertiesHash = new Hash128(new byte[16]);
                                var scriptHash = new Hash128(new byte[16]);
                                ClassDatabaseFile classData = null;
                                AssetsReplacer replacer;

                                var flag1 = reader.ReadBoolean(); //no idea, couldn't get it to be 1
                                if (flag1)
                                {
                                    throw new NotSupportedException("You just found a file with the mysterious flag1 set, send the file to Igor55x");
                                }

                                var hasPropertiesHash = reader.ReadBoolean();
                                if (hasPropertiesHash)
                                {
                                    propertiesHash = new Hash128(reader);
                                }

                                var hasScriptHash = reader.ReadBoolean();
                                if (hasScriptHash)
                                {
                                    scriptHash = new Hash128(reader);
                                }

                                var hasCldb = reader.ReadBoolean();
                                if (hasCldb)
                                {
                                    classData = new ClassDatabaseFile();
                                    classData.Read(reader);
                                }

                                var size = reader.ReadInt64();
                                if (prefReplacersInMemory)
                                {
                                    var data = reader.ReadBytes((int)size);
                                    replacer = new AssetsReplacerFromMemory(fileId, pathId, classId, monoScriptIndex, data);
                                }
                                else
                                { 
                                    replacer = new AssetsReplacerFromStream(fileId, pathId, classId, monoScriptIndex, reader.BaseStream, reader.Position, size);
                                    reader.Position += size;
                                }

                                if (hasPropertiesHash)
                                {
                                    replacer.SetPropertiesHash(propertiesHash);
                                }
                                if (hasScriptHash)
                                {
                                    replacer.SetScriptIDHash(scriptHash);
                                    replacer.SetTypeInfo(classData, null, false); //idk what the last two are supposed to do
                                }
                                if (preloadDependencyCount != 0)
                                {
                                    replacer.SetPreloadDependencies(preloadDependencies);
                                }
                                return replacer;
                            }
                        }

                        break;
                    }
            }
            return null;
        }
    }
}

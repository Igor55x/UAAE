using System;
using System.Collections.Generic;
using System.IO;
using UnityTools;

namespace AssetsAdvancedEditor.Assets
{
    public static class CommandLineParser
    {
        public static void Parse(string[] args)
        {
            if (args.Length < 2 || args[0].ToLower() != "uaae")
            {
                PrintError();
                return;
            }

            switch (args[1])
            {
                case "/help":
                case "/?":
                    PrintHelp();
                    break;
                case "batchimport":
                    BatchImport(args);
                    break;
                case "batchexport":
                    BatchExport(args);
                    break;
                case "compress":
                    Compress(args);
                    break;
                case "decompress":
                    Decompress(args);
                    break;
                default:
                    PrintError();
                    break;
            }
        }

        private static string GetMainFileName(IReadOnlyList<string> args)
        {
            for (var i = 1; i < args.Count; i++)
            {
                if (!args[i].StartsWith("-"))
                    return args[i];
            }
            return string.Empty;
        }
        
        private static string GetDestFileName(IReadOnlyList<string> args)
        {
            for (var i = 2; i < args.Count; i++)
            {
                if (!args[i].StartsWith("-"))
                    return args[i];
            }
            return string.Empty;
        }

        private static HashSet<string> GetFlags(IReadOnlyList<string> args)
        {
            var flags = new HashSet<string>();
            for (var i = 1; i < args.Count; i++)
            {
                if (args[i].StartsWith("-"))
                    flags.Add(args[i]);
            }
            return flags;
        }
        
        private static HashSet<string> GetAllFilesFromBatchFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var allPaths = new HashSet<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith(" +DIR "))
                {
                    var dirName = line[6..].Trim();
                    var files = Directory.GetFiles(dirName, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                        allPaths.Add(file);
                }
                else if (line.StartsWith(" -DIR "))
                {
                    var dirName = line[6..].Trim();
                    var files = Directory.GetFiles(dirName, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                        allPaths.Remove(file);
                }
                else if (line.StartsWith(" +FILE "))
                {
                    var fileName = line[7..].Trim();
                    allPaths.Add(fileName);
                }
                else if (line.StartsWith(" -FILE "))
                {
                    var fileName = line[7..].Trim();
                    allPaths.Remove(fileName);
                }
            }
            return allPaths;
        }

        public static void PrintError()
        {
            Console.WriteLine(@"Command syntax error.");
            Console.WriteLine(@"Type /help or /? to see list of supported commands.");
        }

        public static void PrintHelp()
        {
            Console.WriteLine(@"Unity Assets Advanced Editor");
            Console.Beep();
            Console.WriteLine(@"[WARNING] Command line support is under development!");
            Console.WriteLine(@"[WARNING] There is a high chance that something could go wrong");
            Console.WriteLine(@"[WARNING] Use at your own risk");
            Console.WriteLine(@"  UAAE batchexport <batch file>");
            Console.WriteLine(@"  UAAE batchimport <batch file>");
            Console.WriteLine(@"  UAAE applyemip <aemip file> <directory>");
            Console.WriteLine(@"  UAAE compress <compression type> <bundle file> <output path>");
            Console.WriteLine(@"  UAAE decompress <bundle file> <output path>");
            Console.WriteLine(@"Import/Export arguments:");
            Console.WriteLine(@"  -keepnames writes out to the exact file name in the bundle.");
            Console.WriteLine(@"      Normally, file names are prepended with the bundle's name.");
            Console.WriteLine(@"      Note: these names are not compatible with batchimport.");
            Console.WriteLine(@"  -kd keep .decomp files. When UAAE opens compressed bundles,");
            Console.WriteLine(@"      they are decompressed into .decomp files. If you want to");
            Console.WriteLine(@"      decompress bundles, you can use this flag to keep them");
            Console.WriteLine(@"      without deleting them.");
            Console.WriteLine(@"  -fd overwrite old .decomp files.");
            Console.WriteLine(@"  -md decompress into memory. Doesn't write .decomp files.");
            Console.WriteLine(@"      -kd and -fd won't do anything with this flag set.");
            Console.WriteLine(@"Batch file example:");
            Console.WriteLine(@"  (+ to include file/folder, - to exclude file/folder)");
            Console.WriteLine(@"  -DIR C:/somefolder/bundles/");
            Console.WriteLine(@"  +FILE C:/somefolder/bundles/resources.assets");
        }

        private static void BatchExport(IReadOnlyList<string> args)
        {
            var mainFileName = GetMainFileName(args);
            var flags = GetFlags(args);
            var files = GetAllFilesFromBatchFile(mainFileName);
            foreach (var file in files)
            {
                var decompFile = $"{file}.decomp";
                var filePath = Path.GetDirectoryName(file);

                if (flags.Contains("-md"))
                    decompFile = null;

                if (!File.Exists(file))
                {
                    Console.WriteLine($@"File {file} does not exist!");
                    return;
                }

                Console.WriteLine($@"Decompressing {file}...");
                var bun = DecompressBundle(file, decompFile);

                var entryCount = bun.Metadata.DirectoryCount;
                for (var i = 0; i < entryCount; i++)
                {
                    var name = bun.Metadata.DirectoryInfo[i].Name;
                    var data = BundleHelper.LoadAssetDataFromBundle(bun, i);
                    var outName = Path.Combine(filePath ?? Environment.CurrentDirectory, flags.Contains("-keepnames") ? $"{name}.assets" : $"{Path.GetFileName(file)}_{name}.assets");
                    Console.WriteLine($@"Exporting {outName}...");
                    File.WriteAllBytes(outName, data);
                }

                bun.Close();

                if (!flags.Contains("-kd") && !flags.Contains("-md") && File.Exists(decompFile) && decompFile != null)
                    File.Delete(decompFile);

                Console.WriteLine(@"Done.");
            }
        }

        private static void BatchImport(IReadOnlyList<string> args)
        {
            var mainFileName = GetMainFileName(args);
            var flags = GetFlags(args);
            var files = GetAllFilesFromBatchFile(mainFileName);
            foreach (var file in files)
            {
                var decompFile = $"{file}.decomp";

                if (flags.Contains("-md"))
                    decompFile = null;

                if (!File.Exists(file))
                {
                    Console.WriteLine($@"File {file} does not exist!");
                    return;
                }

                Console.WriteLine($@"Decompressing {file} to {decompFile}...");
                var bun = DecompressBundle(file, decompFile);

                var reps = new List<BundleReplacer>();
                var streams = new List<Stream>();

                var entryCount = bun.Metadata.DirectoryCount;
                for (var i = 0; i < entryCount; i++)
                {
                    var name = bun.Metadata.DirectoryInfo[i].Name;
                    var matchName = Path.Combine(file, $"{Path.GetFileName(file)}_{name}.assets");

                    if (File.Exists(matchName))
                    {
                        var fs = File.OpenRead(matchName);
                        var length = fs.Length;
                        reps.Add(new BundleReplacerFromStream(matchName, matchName, true, fs, 0, length));
                        streams.Add(fs);
                        Console.WriteLine($@"Importing {matchName}...");
                    }
                }

                //I guess uabe always writes to .decomp even if
                //the bundle is already decompressed, that way
                //here it can be used as a temporary file. for
                //now I'll write to memory since having a .decomp
                //file isn't guaranteed here
                byte[] data;
                using (var ms = new MemoryStream())
                using (var writer = new EndianWriter(ms, true))
                {
                    bun.Write(writer, reps);
                    data = ms.ToArray();
                }

                Console.WriteLine($@"Writing changes to {file}...");

                //uabe doesn't seem to compress here

                foreach (var stream in streams)
                    stream.Close();

                bun.Close();
                bun.Reader.Close();

                File.WriteAllBytes(file, data);

                if (!flags.Contains("-kd") && !flags.Contains("-md") && File.Exists(decompFile) && decompFile != null)
                    File.Delete(decompFile);

                Console.WriteLine(@"Done.");
            }
        }

        private static void Compress(IReadOnlyList<string> args)
        {
            var mainFileName = GetMainFileName(args);
            var flags = GetFlags(args);
            var compType = AssetBundleCompressionType.Lz4;
            if (flags.Contains("-lz4"))
                compType = AssetBundleCompressionType.Lz4;
            else if (flags.Contains("-lzma"))
                compType = AssetBundleCompressionType.Lzma;
            
            var destFileName = GetDestFileName(args);
            if (destFileName == string.Empty)
                destFileName = mainFileName + ".packed";

            CompressBundle(mainFileName, destFileName, compType);
            Console.WriteLine(@"Done.");
        }

        private static void CompressBundle(string file, string compFile, AssetBundleCompressionType compType)
        {
            var bun = DecompressBundle(file, null);
            var fs = File.OpenWrite(compFile);
            using var writer = new EndianWriter(fs, true);
            bun.Pack(bun.Reader, writer, compType);
        }

        private static void Decompress(IReadOnlyList<string> args)
        {
            var mainFileName = GetMainFileName(args);
            var destFileName = GetDestFileName(args);
            if (destFileName == string.Empty)
                destFileName = mainFileName + ".unpacked";

            DecompressBundle(mainFileName, destFileName);
            Console.WriteLine(@"Done.");
        }

        private static AssetBundleFile DecompressBundle(string file, string decompFile)
        {
            var bun = new AssetBundleFile();

            var fs = (Stream)File.OpenRead(file);
            var reader = new EndianReader(fs, true);

            bun.Read(reader, true);
            if (bun.Header.GetCompressionType() != 0)
            {
                Stream nfs = decompFile switch
                {
                    null => new MemoryStream(),
                    _ => File.Open(decompFile, FileMode.Create, FileAccess.ReadWrite)
                };
                var writer = new EndianWriter(nfs, true);
                bun.Unpack(reader, writer);

                nfs.Position = 0;
                fs.Close();

                fs = nfs;
                reader = new EndianReader(fs, true);

                bun = new AssetBundleFile();
                bun.Read(reader);
            }
            return bun;
        }
    }
}
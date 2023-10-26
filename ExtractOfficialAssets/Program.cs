// See https://aka.ms/new-console-template for more information
using ExtractOfficialAssets;
using ExtractOfficialAssets.Process;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ExtractOfficialAssets.Gen.Enums;
using ExtractOfficialAssets.Gen.Class;
using NPOI.SS.Formula.Functions;
using Match = System.Text.RegularExpressions.Match;

const string ResourcePrePath = "Assets/artres/Resources/";
uint _PrefixHash = GetLowerHashRelpaceDot(0, ResourcePrePath);


if (args.Length > 0)
{
    switch (args[0].ToLower())
    {
        case "table":
            TableProcess.DumpTable(args[1]);
            break;
        case "animinfo":
            TextFileProcess.DumpAnimationRes();
            break;
        case "dump":
            new UnityResProcess().SaveAllRes(uint.Parse(args[1]));
            break;
        case "lang":
            LanguageProcess.Dump((MGameLanguage)(int.Parse(args[1])));
            break;
        case "lua":
            LuaScriptProcess.Dump(args[1]);
            break;
        case "dumpblockbytes":
            ReadBlockFileInfo();
            break;
        case "testtable":
            TableDataDumper.DumpTable($"TableBytes/{args[1]}.bytes");
            break;
        case "decodeluabyte":

            LuaByteProcess.Dump();
            break;
        case "movetablebyte":
            TableProcess.MoveTableByte();
            break;
        case "dumplua":
            LuaScriptProcess.Dump("Main");
            goto case "uiscript";
        case "uiscript":
            // dump UI
        {
            var str = File.ReadAllText("Lua\\Scripts\\UI\\UIConst.lua");
            var content = Regex.Match(str, @"LuaScriptPath = [\{\r\n\s]+([a-zA-Z\=\s0-9\/\""\,_]+)[\}\n\n\s]+");
            var matchs = Regex.Matches(content.Groups[1].Value, @"""([a-zA-Z0-9\/_]+)""");
            foreach (Match m in matchs)
            {
                var path = m.Groups[1].Value;
                LuaScriptProcess.Dump(path);
                }
        }
            goto case "uimgr";
        case "uimgr":
        {
            {
                foreach (string s in new[]{ "Lua\\Scripts\\Event\\MUIEvent.lua", "Lua\\Scripts\\Network\\Network_Init.lua" })
                {
                    var str = File.ReadAllText(s);
                    var matchs = Regex.Matches(str, @"(file = ""([a-zA-Z0-9\/_\.]+)"")");
                    foreach (Match m in matchs)
                    {
                        var path = m.Groups[2].Value;
                        LuaScriptProcess.Dump(path);
                    }
                    }
                
            }
            }
            break;
        case "genenum":
            GenEnumLuaWarp.Gen<UILayer>();
            break;
        case "genclass":
            GenEnumLuaWarp.GenClass<SDKPublicDefine>();
            break;
        case "dumpglobaloldtable":
            var globalTable=TableDataDumper.DumpTable("New Folder/GlobalTable_old.bytes", false);
            var header = "";
            foreach (var f in globalTable.FieldInfos)
            {
                if (!string.IsNullOrEmpty(header))
                    header += ",";
                header += $"{f.Hash}";

            }

            header += "\r\n";
            for (var i = 0; i < globalTable.RowNumber; i++)
            {
                var row = "";
                foreach (var f in globalTable.FieldInfos)
                {
                    if (!string.IsNullOrEmpty(row))
                        row += ",";
                    row += $"{globalTable.Body[i, f.Hash]}";
                }

                row += "\r\n";
                header += row;
            }
            File.WriteAllText("New Folder\\GlobalTableOld.csv",header,Encoding.UTF8);
            break;
    }
    return 0;
}
// TableProcess.MoveTableByte();
//TableDataDumper.DumpTable($"New Folder/GlobalTable.bytes");
//TableProcess.DumpTable("GlobalTable");

GenEnumLuaWarp.Gen<ESDKBridge>();
return 0;
var targetLang = MGameLanguage.English;
const string LocalizationPath = "Localization";
string RuntimeStringPoolName =
    $"{targetLang.ToString().ToUpper()}_RUNTIME_STRING_POOL.bytes";
var RuntimeStringPoolExtendsName =
    $"{targetLang.ToString().ToUpper()}_RUNTIME_STRING_POOL_EXTENDS.bytes";
//string location = string.Format("UI/Atlas/{0}", "Common");
//string location = string.Format("UI/Texture/Painting/{0}", "Minstrel_M");
string filePath = Combine(LocalizationPath, targetLang.ToString());
Console.WriteLine(filePath);
string location = $"Assets/Resources/{filePath}.bytes";
Console.WriteLine(location);
Console.WriteLine(GetHash(location));
//new UnityResProcess().SaveJson();

string Combine(params string[] paths)
{
    string result = Path.Combine(paths);
    result = MakePathStandard(result);
    return result;
}
string MakePathStandard(string path)
{
    return path.Trim().Replace("\\", "/");
}
void DumpTable()
{

    const string INFO_PATH =
        "bytesblock/3135063240.bytes";
    var tab=TableDataDumper.DumpTable(INFO_PATH);
    var str = "";
    for (int i = 0; i < tab.RowNumber; i++)
    {
        var row = "";
        foreach (var t in tab.FieldInfos)
        {
            if(row!="")
                row += "|";
            row += tab.Body[i, t.Hash].ToString();
        }

        str += row + "\r\n";

        // var row = new ProfessionTable();
        // row.Parse(i,tab.Body);
    }

    File.AppendAllText("ProfessTable.csv", str);
}


void ReadBlockFileInfo()
{

    var luaScriptCount = 0;
    const string INFO_PATH =
        "F:\\Downloads\\GameLauncher\\ROO_PC\\ro_Data\\StreamingAssets\\Unzips\\BYTES_BLOCK";

    var allFiles = Directory.GetFiles(INFO_PATH, "*.bytesblock");
    var sFiles = new Dictionary<int, string>();
    foreach (var file in allFiles)
    {
        var eFile = file.Replace(INFO_PATH, "").Replace("\\", "").Replace("BYTES_BLOCK", "")
            .Replace(".bytesblock", "");
        if (eFile == "")
            eFile = "0";
        sFiles.Add(int.Parse(eFile), file);

    }

    var dicStream = new Dictionary<int, BinaryReader>();
    foreach (var file in sFiles.OrderBy(m => m.Key))
    {
        var stream=File.OpenRead(file.Value);
        var read=new BinaryReader(stream);
        dicStream[file.Key] = read;
    }


    Dictionary<uint, BlockAssetBundleInfo> BlockAssetBundleInfos = new Dictionary<uint, BlockAssetBundleInfo>();
    using var fStrem = File.OpenRead($"{INFO_PATH}//BYTES_BLOCK.blockinfo");
    using var r = new BinaryReader(fStrem);
    BlockAssetBundleInfo currentBlockAbInfo = new BlockAssetBundleInfo();
    try
    {
        if (r.ReadChar() != 'A' || r.ReadChar() != 'B' || r.ReadChar() != 'B' || r.ReadChar() != 'B')
        {
            Console.WriteLine("ERROR BLOCK");
            throw new InvalidDataException("ERROR BLOCK HEADER");
        }
        else
        {
            int fileCount = r.ReadInt32();
            for (int i = 0; i < fileCount; ++i)
            {
                currentBlockAbInfo = new BlockAssetBundleInfo();
                currentBlockAbInfo.fullName = r.ReadUInt32();
                currentBlockAbInfo.fileId = r.ReadUInt16();
                currentBlockAbInfo.offset = r.ReadUInt32();
                currentBlockAbInfo.length = r.ReadUInt32();
                //if(DepFile.TryGetValue(currentBlockAbInfo.fileId, out var inf))
                //{
                //    currentBlockAbInfo.FileName = inf.assetPath;
                //}
                BlockAssetBundleInfos[currentBlockAbInfo.fullName] = currentBlockAbInfo;
            }

            if (!Directory.Exists("bytesblock"))
                Directory.CreateDirectory("bytesblock");
            foreach (var b in BlockAssetBundleInfos.Values)
            {
                dicStream[b.fileId].BaseStream.Seek(b.offset, SeekOrigin.Begin);
                var fileData = dicStream[b.fileId].ReadBytes((int)b.length);
                if (fileData.Length >4&&fileData[0] == 0x1B &&
                    fileData[1] == 0x4C &&
                    fileData[2] == 0x4A &&
                    fileData[3] == 0x2)
                {
                    luaScriptCount++;
                    if (!Directory.Exists("luabytes"))
                    {
                        Directory.CreateDirectory("luabytes");
                    }
                    File.WriteAllBytes($"luabytes\\{b.fullName}.bytes", fileData);
                }
                File.WriteAllBytes($"bytesblock\\{b.fullName}.bytes", fileData);
            }
        }

        foreach (var binaryReader in dicStream)
        {
            binaryReader.Value.Dispose();
        }

        Console.WriteLine($"Lua script count : {luaScriptCount}");
    }
    catch
    {
        throw;
    }
}

void ReadBlockUnityFileInfo()
{


    const string INFO_PATH =
        "F:\\Downloads\\GameLauncher\\ROO_PC\\ro_Data\\StreamingAssets\\AssetBundles\\BLOCK";

    var allFiles = Directory.GetFiles(INFO_PATH, "*.block");
    var sFiles = new Dictionary<int, string>();
    foreach (var file in allFiles)
    {
        var eFile = file.Replace(INFO_PATH, "").Replace("\\", "").Replace("BLOCK", "")
            .Replace(".block", "");
        if (eFile == "")
            eFile = "0";
        sFiles.Add(int.Parse(eFile), file);

    }

    var dicStream = new Dictionary<int, BinaryReader>();
    foreach (var file in sFiles.OrderBy(m => m.Key))
    {
        var stream = File.OpenRead(file.Value);
        var read = new BinaryReader(stream);
        dicStream[file.Key] = read;
    }


    Dictionary<uint, BlockAssetBundleInfo> BlockAssetBundleInfos = new Dictionary<uint, BlockAssetBundleInfo>();
    using var fStrem = File.OpenRead($"{INFO_PATH}//BLOCK.blockinfo");
    using var r = new BinaryReader(fStrem);
    BlockAssetBundleInfo currentBlockAbInfo = new BlockAssetBundleInfo();
    try
    {
        if (r.ReadChar() != 'A' || r.ReadChar() != 'B' || r.ReadChar() != 'B' || r.ReadChar() != 'B')
        {
            Console.WriteLine("ERROR BLOCK");
            throw new InvalidDataException("ERROR BLOCK HEADER");
        }
        else
        {
            int fileCount = r.ReadInt32();
            for (int i = 0; i < fileCount; ++i)
            {
                currentBlockAbInfo = new BlockAssetBundleInfo();
                currentBlockAbInfo.fullName = r.ReadUInt32();
                currentBlockAbInfo.fileId = r.ReadUInt16();
                currentBlockAbInfo.offset = r.ReadUInt32();
                currentBlockAbInfo.length = r.ReadUInt32();
                //if (DepFile.TryGetValue(currentBlockAbInfo.fileId, out var inf))
                //{
                //    currentBlockAbInfo.FileName = inf.assetPath;
                //}
                BlockAssetBundleInfos[currentBlockAbInfo.fullName] = currentBlockAbInfo;
            }

            if (!Directory.Exists("block"))
                Directory.CreateDirectory("block");
            foreach (var b in BlockAssetBundleInfos.Values)
            {
                dicStream[b.fileId].BaseStream.Seek(b.offset, SeekOrigin.Begin);
                File.WriteAllBytes($"block\\{b.fullName}.unity3d", dicStream[b.fileId].ReadBytes((int)b.length));
            }
        }

        foreach (var binaryReader in dicStream)
        {
            binaryReader.Value.Dispose();
        }
    }
    catch
    {
        throw;
    }
}

uint GetHash(string str)
{
    if (str == null)
        return 0;
    uint hash = 0;
    for (int index = 0; index < str.Length; ++index)
        hash = (hash << 5) + hash + (uint)str[index];
    return hash;
}

uint GetLowerHashRelpaceDot(uint hash, string str)
{
    if (string.IsNullOrEmpty(str)) return hash;

    for (int i = 0; i < str.Length; i++)
    {
        char c = char.ToLower(str[i]);
        if (c == '/' || c == '\\')
        {
            c = '.';
        }
        else if (c == ' ')
        {
            c = '_';
        }
        hash = (hash << 5) + hash + c;
    }

    return hash;
}

uint Hash(string location, string ext)
{
    uint hash = GetLowerHashRelpaceDot(_PrefixHash, location);
    return GetLowerHashRelpaceDot(hash, ext);
}

uint GetLocationHash(ref string location, string suffix)
{
   return Hash(location, suffix);
}

return 0;
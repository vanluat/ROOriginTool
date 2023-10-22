// See https://aka.ms/new-console-template for more information
using ExtractOfficialAssets;
using ExtractOfficialAssets.Process;
using System.IO;

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
    }
    return 0;
}

Console.Read();
LuaScriptProcess.Dump("Main");
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
                File.WriteAllBytes($"bytesblock\\{b.fullName}.bytes", dicStream[b.fileId].ReadBytes((int)b.length));
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
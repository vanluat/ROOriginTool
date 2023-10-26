using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace ExtractOfficialAssets.Process;

public class LuaScriptProcess
{
    private static readonly string[] FolderSearch =
    {
        "LuaGame",
        "Scripts",
        "ToLua",
        "Scripts/Protobuf"
    };
    public static unsafe void Dump(string fileName)
    {
        foreach (var f in FolderSearch)
        {
            fileName = fileName.Replace(".lua", "");

            var srcFilePath = $"Assets/Resources/LuaSource/Lua64/{f}/{fileName}.lua";
            var id = TableProcess.GetHash(srcFilePath.Replace("\\", "/"));

            string INFO_PATH =
                $"bytesblock/{id}.bytes";

            if (!File.Exists(INFO_PATH))
            {
                continue;
            }
            if (!Directory.Exists("Lua"))
                Directory.CreateDirectory("Lua");
            var dstPath = $"Lua/{f}/{fileName}.lua";
            if (File.Exists(dstPath))
            {
                Console.WriteLine($"Already : {dstPath}");
                File.Delete(INFO_PATH);
                continue;
            }
            var dstFolder = dstPath.Remove(dstPath.LastIndexOf("/"));
            if (!Directory.Exists(dstFolder))
                Directory.CreateDirectory(dstFolder);
            var data=File.ReadAllBytes(INFO_PATH);
            try
            {

                if (id == 3346012256)
                {
                    CopyToUnDecode(dstPath, data);
                    File.Delete(INFO_PATH);
                    continue;
                }
                fixed (byte* fileData = data)
                {
                    int outLen = 0;
                    var ret = LuaJitDecode.Decode(fileData, data.Length, &outLen);
                    data = new byte[outLen];
                    for (int i = 0; i < outLen; i++)
                    {
                        data[i] = ret[i];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CopyToUnDecode(dstPath, data);
                File.Delete(INFO_PATH);
                continue;
            }
           
           
            //File.Copy(INFO_PATH, dstPath, true);
            File.WriteAllBytes(dstPath, data);
            // read all required
            DumpDepend(data);
            Console.WriteLine($"{dstPath} => OK!");
            File.Delete(INFO_PATH);
            break;
        }
    }

    private static void CopyToUnDecode(string dstPath, byte[] data)
    {
        var newPath = dstPath.Replace("Lua/", "UnDecodeLua/");
        var newPathDir  = newPath.Remove(newPath.LastIndexOf("/"));
        if (!Directory.Exists(newPathDir))
            Directory.CreateDirectory(newPathDir);
        File.WriteAllBytes(newPath, data);
    }

    private static void DumpDepend(byte[] data)
    {
        var str=Encoding.UTF8.GetString(data);
        var match=Regex.Matches(str, @"(require\(\""([a-zA-Z0-9\/_\-\.]+)\""\))");
        Console.WriteLine($"require: {match.Count}");
        foreach (Match m in match)
        {
            var dependFile = m.Groups[2].Value;
            Dump(dependFile);
        }
        match = Regex.Matches(str, @"(DataMgr:GetData\(\""([a-zA-Z0-9_\/\-\.]+)\""\))");
        foreach (Match m in match)
        {
            var dependFile = m.Groups[2].Value;
            Dump($"ModuleData/{dependFile}");
        }
    }
}
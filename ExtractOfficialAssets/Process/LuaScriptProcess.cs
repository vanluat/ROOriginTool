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

            Console.WriteLine($"BEGIN DUMP : {fileName} ({id})");
            if (!File.Exists(INFO_PATH))
            {
                continue;
            }
            if (!Directory.Exists("Lua"))
                Directory.CreateDirectory("Lua");
            var dstPath = $"Lua/{f}/{fileName}.lua";
            var dstFolder = dstPath.Remove(dstPath.LastIndexOf("/"));
            if (!Directory.Exists(dstFolder))
                Directory.CreateDirectory(dstFolder);
            var data=File.ReadAllBytes(INFO_PATH);
            try
            {
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
                continue;
            }
           
           
            //File.Copy(INFO_PATH, dstPath, true);
            File.WriteAllBytes(dstPath, data);
            // read all required
            DumpDepend(data);
            Console.WriteLine($"{dstPath} => OK!");
            break;
        }
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
    }
}
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractOfficialAssets.Process;

public class LuaByteProcess
{
    private const string Root = "luabytes";
    public unsafe static void Dump()
    {
        var files = Directory.GetFiles(Root);
        var idx = 0;
        var count=files.Length;
        foreach (var file in files)
        {

            Console.WriteLine($"{idx++}/{count} : {file}");
            if (file.EndsWith("3346012256.bytes")||
                file.EndsWith("4257882427.bytes"))
                continue;
            var data=File.ReadAllBytes(file);
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

                    RecoveryFolder(data);
                    File.Delete(file);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                continue;
            }
        }
    }

    private static void RecoveryFolder(byte[] data)
    {
        var str=Encoding.UTF8.GetString(data);

        var match = Regex.Match(str, @"-- chunkname: @[a-zA-Z0-9\/\.\:_\-]+\\\\([a-zA-Z0-9\/\\\._\-]+)");

        var path = Root+"/"+match.Groups[1].Value;
        var newPathDir = path.Remove(path.LastIndexOf("\\"));
        if (!Directory.Exists(newPathDir))
            Directory.CreateDirectory(newPathDir);
        File.WriteAllBytes(path, data);
        Console.WriteLine($"save file : {match.Groups[1].Value}");
    }
}
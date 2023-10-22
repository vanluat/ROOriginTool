using Newtonsoft.Json;

namespace ExtractOfficialAssets.Process;

public class LanguageProcess
{
    const string LocalizationPath = "Localization";
    public static void Dump(MGameLanguage lang)
    {
        string filePath = Combine(LocalizationPath, lang.ToString());
        Console.WriteLine(filePath);
        string location = $"Assets/Resources/{filePath}.bytes";
        var id = TableProcess.GetHash(location);
        using var fStream = File.OpenRead($"bytesblock/{id}.bytes");
        using var read = new BinaryReader(fStream);
        var count=read.ReadUInt32();
        var dic = new List<LangDetail>();
        for (int i = 0; i < count; i++)
        {
            var kId=read.ReadUInt32();
            var val = read.ReadString();
            dic.Add(new LangDetail()
            {
                id = kId,
                val = val
            });
        }

        File.WriteAllText($"{lang}_lang.json", JsonConvert.SerializeObject(new LangWrap
        {
            data = dic.ToArray()
        }, Formatting.Indented));
    }

    static string Combine(params string[] paths)
    {
        string result = Path.Combine(paths);
        result = MakePathStandard(result);
        return result;
    }
    static string MakePathStandard(string path)
    {
        return path.Trim().Replace("\\", "/");
    }
}

public class LangDetail
{
    public uint id;
    public string val;
}

public class LangWrap
{
    public LangDetail[] data;
}
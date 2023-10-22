namespace ExtractOfficialAssets.Process;

public class TableProcess
{
    private const string ROOT = "Assets/Resources/Table/";
    public static string GetAssetPath(string fullPath)
    {
        fullPath = fullPath.Replace("\\", "/");
        int index = fullPath.IndexOf("Assets");
        if (index < 0)
        {
            return null;
        }

        return fullPath.Substring(index);
    }

    public static uint GetHash(string str)
    {
        if (str == null)
            return 0;
        uint hash = 0;
        for (int index = 0; index < str.Length; ++index)
            hash = (hash << 5) + hash + (uint)str[index];
        return hash;
    }
    public static void DumpTable(string tabname)
    {
        var path = $"{ROOT}{tabname}.bytes";
        Console.WriteLine(path);
        var id = GetHash(path);

        DumpTable(id, tabname);

    }

    static void DumpTable(uint id, string name)
    {
        string INFO_PATH =
            $"bytesblock/{id}.bytes";
        var tab = TableDataDumper.DumpTable(INFO_PATH);
        var str = "";
        for (int i = 0; i < tab.RowNumber; i++)
        {
            var row = "";
            foreach (var t in tab.FieldInfos)
            {
                if (row != "")
                    row += "|";
                row += tab.Body[i, t.Hash].ToString();
            }
            str += row + "\r\n";
        }

        if (!Directory.Exists("TabExport"))
            Directory.CreateDirectory("TabExport");
        File.WriteAllText($"TabExport\\{name}.csv", str);
        Console.WriteLine("DONE!");
    }
}
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Serializer;

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
    public static TableDumpData? DumpTable(string tabname)
    {
        var id = GetIdTableByte(tabname);

        return DumpTable(id, tabname);

    }

    static uint GetIdTableByte(string tableName)
    {
        var path = $"{ROOT}{tableName}.bytes";
        Console.WriteLine(path);
        var id = GetHash(path);
        return id;
    }

    public static void MoveTableByte()
    {
        if (!Directory.Exists("TableBytes"))
        {
            Directory.CreateDirectory("TableBytes");
        }
        var str = File.ReadAllText("Luabytes\\LuaClient\\Misc\\ClientLoadTables.lua");
        var matchs = Regex.Matches(str, @"\""([a-zA-Z]+)\""");
        Console.WriteLine(matchs.Count);
        foreach (Match m in matchs)
        {
            var tableName = m.Groups[1].Value;
            Console.WriteLine(tableName);
            var id = GetIdTableByte(tableName);
            string INFO_PATH =
                $"bytesblock/{id}.bytes";
            var tab = TableDataDumper.DumpTable(INFO_PATH);
            if (tab != null)
            {
                SaveTableBytes(tab, INFO_PATH, $"TableBytes/{tableName}.bytes");
            }
            else
            {
                File.Copy($"bytesblock/{id}.bytes", $"TableBytes/{tableName}.bytes", true);
            }
            //break;
        }

        Console.WriteLine("DONE!");
    }

    private static void SaveTableBytes(TableDumpData data,string origin, string path)
    {
        var originData=File.ReadAllBytes(origin);
        using var s=new MemoryStream();
        using var w=new BinaryWriter(s);
        w.Write((long)(data.EndOffset - 8));
        w.Write(originData, 8, data.RemoveOffset-8);
        w.Write(originData, data.RemoveOffset + 8, (int)(data.EndOffset - 8 - data.RemoveOffset));
        File.WriteAllBytes(path, s.ToArray());
        //var tmp = new Dictionary<string, uint>();
        //for (int i = 0; i < data.dictHashToString.Length; i++)
        //{
        //    tmp[data.dictHashToString[i]] =(uint) i;
        //}
        //using var m=new MemoryStream();
        //using var w=new BinaryWriter(m);
        //w.Write(data.Size);
        //w.Write(data.hashMD5);
        //w.Write(data.hashTime);
        //w.Write(data.totalStringNum);
        //foreach (var s in data.dictHashToString)
        //{
        //    w.Write(s);
        //}
        //w.Write(data.HeadSize);
        //w.Write(data.HeadBinID);
        //for (int index = 0; (long)index < (long)data.HeadSize; ++index)
        //{
        //    var f = data.FieldInfos[index];
        //    w.Write(f.NeedLocal);
        //    w.Write(f.Hash);
        //    w.Write((short)f.FieldType);
        //    w.Write(f.SeqLength);
        //    ParserUtil.GetParser(f.FieldType, f.SeqLength).Write(false, ref tmp,w,f.DefaultValueOrigin);

        //}

        //w.Write(data.RowNumber);

        //for (int index1 = 0; index1 < data.RowNumber; ++index1)
        //{
        //    for (int index2 = 0; index2 < data.HeadSize; ++index2)
        //    {
        //        TableDumpData.FieldInfo fieldInfo = data.FieldInfos[index2];
        //        ParserUtil.GetParser(fieldInfo.FieldType, fieldInfo.SeqLength).Write(false,ref tmp, w, data.BodyOrigin[index1, fieldInfo.Hash]);

        //    }
        //}

        //File.WriteAllBytes(path, m.ToArray());
    }

    static TableDumpData? DumpTable(uint id, string name)
    {
        if (LanguageDic == null)
            LanguageDic = JsonConvert.DeserializeObject<LanguageWrap>(
                File.ReadAllText($"Chinese_lang.json")).data.ToDictionary(m => m.id, m => m.val);
       
        // load table prototype
        var prototypePath = $"luabytes\\LuaClient\\Table\\{name}.lua";
        if(!File.Exists(prototypePath))
            return null;
        var tableStr = File.ReadAllText(prototypePath);


        var propMatches = Regex.Matches(tableStr,
            @"row.([a-zA-Z_0-9]+) = ROGameLibs.RowDataLuaProxy:[a-zA-Z_]+\(row._rp, ([0-9]+)\)");

        var translateMatches = Regex.Matches(tableStr, @"StringPoolManager:GetString\(row\.([a-zA-Z0-9_]+)\)");
        var listProTranslate= new List<string>();
        foreach (Match m in translateMatches)
        {
            listProTranslate.Add(m.Groups[1].Value);
        }
        var props=new Dictionary<int,string>();
        foreach (Match m in propMatches)
        {
            props[int.Parse(m.Groups[2].Value)] = m.Groups[1].Value;
        }
        string INFO_PATH =
            $"bytesblock/{id}.bytes";
        var tab = TableDataDumper.DumpTable(INFO_PATH);
        if (tab == null)
        {
            return null;
        }
        XSSFWorkbook hssfwb =new XSSFWorkbook();
        var sheet=hssfwb.CreateSheet("Sheet1");
        var header=sheet.CreateRow(0);
        foreach (var p in props)
        {
            var cell=header.CreateCell(p.Key);
            cell.SetCellValue(p.Value);
        }

        var type = sheet.CreateRow(1);
        foreach (var p in tab.FieldInfos)
        {
            var cell = type.CreateCell(p.Hash);
            cell.SetCellValue($"{p.FieldType}|{p.SeqLength}");
        }

        var fieldDic = tab.FieldInfos.ToDictionary(m => (int)m.Hash);
        for (int i = 0; i < tab.RowNumber; i++)
        {
            var row=sheet.CreateRow(2+i);
            foreach (var f in tab.FieldInfos)
            {
                var cell = row.CreateCell(f.Hash);
                var val = tab.Body[i, f.Hash].ToString();
                if (props.TryGetValue((int)f.Hash, out var prop))
                {
                    if (listProTranslate.Contains(prop))
                    {
                        val = LanguageDic[uint.Parse(val)];
                    }
                }
               

                cell.SetCellValue(val);
            }
            foreach (var p in props)
            {
                
            }
        }

        if (!Directory.Exists("TabExport"))
            Directory.CreateDirectory("TabExport");
        using MemoryStream sw=new MemoryStream();
        hssfwb.Write(sw);
        File.WriteAllBytes($"TabExport\\{name}.xlsx", sw.ToArray());
        Console.WriteLine("DONE!");
        return tab;
    }

    public static Dictionary<uint, string> LanguageDic { get; set; }
}
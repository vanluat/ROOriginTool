using System.Diagnostics;
using System.Text;
using Microsoft.VisualBasic;
using Serializer;

namespace ExtractOfficialAssets;

public class TableDataDumper
{
    private static TableDumpData data;

    public static TableDumpData? DumpTable(string dataPath,bool isNew=true)
    {
        try
        {
            using FileStream input = new FileStream(dataPath, FileMode.Open);
            using BinaryReader reader = new BinaryReader((Stream)input);
            return DumpTable(reader,isNew);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
      
    }

    private static int ReadHeadInfo(TableDumpData dumpData, BinaryReader reader)
    {
        data.HeadSize = reader.ReadUInt32();
        data.HeadBinID = reader.ReadInt16();
        data.FieldInfos = new TableDumpData.FieldInfo[(int)data.HeadSize];
        int num = -1;
        for (int index = 0; (long)index < (long)data.HeadSize; ++index)
        {
            data.FieldInfos[index] = new TableDumpData.FieldInfo();
            data.FieldInfos[index].NeedLocal = reader.ReadBoolean();
            data.FieldInfos[index].Hash = reader.ReadSByte();
            if (num < (int)data.FieldInfos[index].Hash)
                num = (int)data.FieldInfos[index].Hash;
            TableTypeEnum typeEnum = (TableTypeEnum)reader.ReadInt16();
            data.FieldInfos[index].FieldType = typeEnum;
            sbyte seqLength = reader.ReadSByte();
            data.FieldInfos[index].SeqLength = seqLength;
            Console.WriteLine($"{index} : {typeEnum} ({seqLength}) hash : {data.FieldInfos[index].Hash} ");
            object obj;
            ParserUtil.GetParser(typeEnum, seqLength).Read(ref dumpData.dictHashToString, reader, out obj);
            data.FieldInfos[index].DefaultValue = obj.ToString();
            data.FieldInfos[index].DefaultValueOrigin = obj;
        }
        return num;
    }

    private static void ReadBodyInfo(TableDumpData dumpData, BinaryReader reader, int maxHash)
    {
        data.RowNumber = reader.ReadInt32();
        data.Body = new string?[data.RowNumber, maxHash + 1];
        data.BodyOrigin = new object[data.RowNumber, maxHash + 1];
        TableDumpData.FieldInfo[] fieldInfos = data.FieldInfos;
        data.FieldInfos = fieldInfos.ToArray();
        for (int index1 = 0; index1 < data.RowNumber; ++index1)
        {
            for (int index2 = 0; index2 < data.HeadSize; ++index2)
            {
                TableDumpData.FieldInfo fieldInfo = data.FieldInfos[index2];
                object obj;
                ParserUtil.GetParser(fieldInfo.FieldType, fieldInfo.SeqLength).Read(ref dumpData.dictHashToString, reader, out obj);
                int fieldType = (int)fieldInfo.FieldType;
                data.Body[index1, fieldInfo.Hash] = DumpItem(obj, (TableTypeEnum)fieldType);
                data.BodyOrigin[index1, fieldInfo.Hash] = obj;
            }
        }
    }

    public static TableDumpData DumpTable(BinaryReader reader,bool isNew= true)
    {
        data = new TableDumpData();
        data.Size = reader.ReadInt64();
        data.hashMD5 = reader.ReadUInt64();
        data.hashTime = reader.ReadUInt64();
        TableUtil.ReadStringInfo(reader, out data.totalStringNum, out data.dictHashToString);
        int maxHash = ReadHeadInfo(data, reader);
        if (isNew)
        {
            data.RemoveOffset = (int)reader.BaseStream.Position;
            data.EndOffset = reader.ReadInt64();
        }
        ReadBodyInfo(data, reader, maxHash);
        return data;
    }

    private static void appendFirstOrEndCharByDepth(ref StringBuilder sb, int depth, bool first)
    {
        if (depth == 2)
            sb.Append(first ? "(" : ")");
        if (depth != 1)
            return;
        sb.Append(first ? "[" : "]");
    }

    private static string? DumpItem(object? obj, TableTypeEnum type,string splitChar="|")
    {
        var sp = new StringBuilder();
        if (obj == null)
            return sp.ToString();
        Array arr = null;
        TableTypeEnum secondType = TableTypeEnum.NONE;
        bool isArray = false;
        switch (type)
        {
            case TableTypeEnum.CHAR:
            case TableTypeEnum.BOOL:
            case TableTypeEnum.INT:
            case TableTypeEnum.UINT:
            case TableTypeEnum.FLOAT:
            case TableTypeEnum.DOUBLE:
            case TableTypeEnum.LONGLONG:
            case TableTypeEnum.STRING:
                sp.Append($"{obj}");
                return sp.ToString();
            case TableTypeEnum.VECTOR_CHAR:
            case TableTypeEnum.VECTOR_BOOL:
            case TableTypeEnum.VECTOR_INT:
            case TableTypeEnum.VECTOR_UINT:
            case TableTypeEnum.VECTOR_FLOAT:
            case TableTypeEnum.VECTOR_DOUBLE:
            case TableTypeEnum.VECTOR_LONGLONG:
            case TableTypeEnum.VECTOR_STRING:
                arr = obj as Array;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0)
                        sp.Append(splitChar);
                    sp.Append(arr.GetValue(i));

                }
                return sp.ToString();
            case TableTypeEnum.SEQUENCE_CHAR:
            case TableTypeEnum.SEQUENCE_BOOL:
            case TableTypeEnum.SEQUENCE_INT:
            case TableTypeEnum.SEQUENCE_UINT:
            case TableTypeEnum.SEQUENCE_FLOAT:
            case TableTypeEnum.SEQUENCE_DOUBLE:
            case TableTypeEnum.SEQUENCE_LONGLONG:
            case TableTypeEnum.SEQUENCE_STRING:
                return obj.ToString();
            case TableTypeEnum.VECTOR_SEQUENCE_CHAR:
                secondType = TableTypeEnum.SEQUENCE_CHAR;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_BOOL:
                secondType = TableTypeEnum.SEQUENCE_BOOL;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_INT:
                secondType = TableTypeEnum.SEQUENCE_INT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_UINT:
                secondType = TableTypeEnum.SEQUENCE_UINT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_FLOAT:
                secondType = TableTypeEnum.SEQUENCE_FLOAT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_DOUBLE:
                secondType = TableTypeEnum.SEQUENCE_DOUBLE;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_LONGLONG:
                secondType = TableTypeEnum.SEQUENCE_LONGLONG;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_SEQUENCE_STRING:
                secondType = TableTypeEnum.SEQUENCE_STRING;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_CHAR:
                secondType = TableTypeEnum.VECTOR_CHAR;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_BOOL:
                secondType = TableTypeEnum.VECTOR_BOOL;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_INT:
                secondType = TableTypeEnum.VECTOR_INT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_UINT:
                secondType = TableTypeEnum.VECTOR_UINT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_FLOAT:
                secondType = TableTypeEnum.VECTOR_FLOAT;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_DOUBLE:
                secondType = TableTypeEnum.VECTOR_DOUBLE;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_LONGLONG:
                secondType = TableTypeEnum.VECTOR_LONGLONG;
                isArray = true;
                break;
            case TableTypeEnum.VECTOR_VECTOR_STRING:
                secondType = TableTypeEnum.VECTOR_STRING;
                isArray = true;
                break;
            default:
                throw new("Not Support");
        }

        if (isArray)
        {
            arr=obj as Array;

            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                    sp.Append("|");
                sp.Append(DumpItem(arr.GetValue(i), secondType,"="));

            }
        }
        return sp.ToString();
    }

    
}
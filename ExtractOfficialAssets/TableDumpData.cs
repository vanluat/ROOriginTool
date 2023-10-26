namespace ExtractOfficialAssets;

public class TableDumpData
{
    public long Size;
    public ulong hashMD5 = 0;
    public ulong hashTime = 0;
    public uint totalStringNum;
    public string[] dictHashToString;
    public uint HeadSize;
    public short HeadBinID;
    public TableDumpData.FieldInfo[] FieldInfos;
    public int RowNumber;
    public object?[,] Body;
    public object?[,] BodyOrigin;
    public int RemoveOffset;
    public long EndOffset;
    public class FieldInfo
    {
        public bool NeedLocal;
        public sbyte Hash;
        public TableTypeEnum FieldType;
        public sbyte SeqLength;
        public string DefaultValue;
        public object DefaultValueOrigin;
    }
}
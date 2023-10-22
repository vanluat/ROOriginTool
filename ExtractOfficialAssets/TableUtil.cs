namespace ExtractOfficialAssets;

internal class TableUtil
{
    public static bool ReadStringInfo(
        BinaryReader reader,
        out uint totalStringNum,
        out string[] dictHashToString)
    {
        totalStringNum = reader.ReadUInt32();
        dictHashToString = new string[(int)totalStringNum];
        for (uint index = 0; index < totalStringNum; ++index)
        {
            try
            {
                string str = reader.ReadString();
                dictHashToString[(int)index] = str;
            }
            catch (Exception ex)
            {
                //Context.Logger.Error(string.Format("总计{0}行第{1}行\n{2}", (object)totalStringNum, (object)index, (object)ex.ToString()));
                throw ex;
            }
        }
        return true;
    }
}
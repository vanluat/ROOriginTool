using System;
using System.IO;

namespace ToolLib
{
    internal class TableUtil
    {
        public static bool ReadStringInfo(BinaryReader reader, out uint totalStringNum, out string[] dictHashToString)
        {
            totalStringNum = reader.ReadUInt32();

            dictHashToString = new string[totalStringNum];
            for (uint i = 0; i < totalStringNum; i++)
            {
                try
                {
                    var strValue = reader.ReadString();
                    dictHashToString[i] = strValue;
                }
                catch (Exception ex)
                {
                    throw new($"总计{totalStringNum}行第{i}行\n{ex.ToString()}");
                }
            }
            return true;
        }

    }
}

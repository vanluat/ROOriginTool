using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class IntParser : Parser<int>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, int value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out int value)
        {
            value = reader.ReadInt32();
            return true;
        }

        public override int Compare(int value0, int value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out int value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (int)DefaultValue;
                return true;
            }
            return int.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class LongParser : Parser<long>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, long value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out long value)
        {
            value = reader.ReadInt64();
            return true;
        }

        public override int Compare(long value0, long value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out long value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (long)DefaultValue;
                return true;
            }
            return long.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
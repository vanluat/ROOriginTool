using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class UIntParser : Parser<uint>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, uint value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out uint value)
        {
            value = reader.ReadUInt32();
            return true;
        }

        public override int Compare(uint value0, uint value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out uint value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (uint)DefaultValue;
                return true;
            }
            return uint.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
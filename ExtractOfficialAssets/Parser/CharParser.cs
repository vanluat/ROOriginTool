using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class CharParser : Parser<char>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, char value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out char value)
        {
            value = reader.ReadChar();
            return true;
        }

        public override int Compare(char value0, char value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out char value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (char)DefaultValue;
                return true;
            }
            return char.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }

    }
}
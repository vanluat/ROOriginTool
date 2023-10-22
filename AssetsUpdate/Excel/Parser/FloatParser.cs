using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class FloatParser : Parser<float>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, float value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out float value)
        {
            value = reader.ReadSingle();
            return true;
        }

        public override int Compare(float value0, float value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out float value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (float)DefaultValue;
                return true;
            }
            return float.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
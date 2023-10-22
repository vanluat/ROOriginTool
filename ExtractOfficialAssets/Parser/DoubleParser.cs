using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class DoubleParser : Parser<double>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, double value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out double value)
        {
            value = reader.ReadDouble();
            return true;
        }

        public override int Compare(double value0, double value1)
        {
            return value0.CompareTo(value1);
        }

        public override bool Parse(string str, out double value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (double)DefaultValue;
                return true;
            }
            return double.TryParse(str, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class BoolParser : Parser<bool>
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, bool value)
        {
            if (!isColloctString)
            {
                writer.Write(value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out bool value)
        {
            value = reader.ReadBoolean();
            return true;
        }

        public override int Compare(bool value0, bool value1)
        {
            return 0;
        }

        public override bool Parse(string str, out bool value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (bool) DefaultValue;
                return true;
            }
            value = false;
            if (string.IsNullOrEmpty(str))
            {
                value = false;
                return true;
            }

            if (str.ToLower() == "true" || str == "1")
            {
                value = true;
                return true;
            }

            if (str.ToLower() == "false" || str == "0")
            {
                value = false;
                return true;
            }

            return false;
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }


    }
}
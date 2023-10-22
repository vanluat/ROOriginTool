using System;
using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class EnumParser<TE> : Parser<TE> where TE : struct
    {
        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, TE value)
        {
            if (!isColloctString)
            {
                writer.Write((int)(object)value);
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out TE value)
        {
            var enumInt = reader.ReadInt32();
            value = (TE) Enum.ToObject(typeof(TE), enumInt);
            return true;
        }

        public override int Compare(TE value0, TE value1)
        {
            return ((int)(object)value0).CompareTo((int)(object)value1);
        }

        public override bool Parse(string str, out TE value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = (TE)DefaultValue;
                return true;
            }
            return Enum.TryParse(str, false, out value);
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }
    }
}
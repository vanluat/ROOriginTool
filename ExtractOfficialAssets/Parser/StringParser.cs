using System;
using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class StringParser : Parser<string>
    {
        public override object DefaultValue => string.Empty;

        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, string value)
        {
            if(!isColloctString)
            {
                if (value == null)
                {
                   // ToolLib.Context.Logger.Error($"value is null!!!");
                }
                else
                {
                    writer.Write(value);
                }
            }
        }

        public override bool WriteLocal(BinaryWriter writer, object obj)
        {
            string value;
            if (obj == null)
            {
                value = DefaultValue as string;
            }
            else
            {
                value = (string)obj;
            }

            throw new Exception();
            //uint key = StringPoolManager.singleton.GetKey(value);
            //writer.Write(key);
            return true;
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out string value)
        {
            var hash = reader.ReadUInt32();
            value = ParserUtil.GetStringByHash(ref dictHashToString, hash);
            return true;
        }

        public override int Compare(string value0, string value1)
        {
            return String.Compare(value0, value1, StringComparison.Ordinal);
        }

        public override bool Parse(string str, out string value)
        {
            if (str == null)
            {
                value = (string)DefaultValue;
                return true;
            }
            value = str;
            return true;
        }

        public override string SerializeExcel(object obj)
        {
            return obj.ToString();
        }

        public override List<string> GetStringValue(object value)
        {
            List<string> res = new List<string>();
            string str = (string)value;
            if (str == null)
            {
                res.Add((string)DefaultValue);
            }
            else
            {
                res.Add(str);
            }
            return res;
        }

    }
}
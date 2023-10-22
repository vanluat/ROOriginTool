using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public class VectorParser<TV> : Parser<TV[]>
    {
        
        public override object DefaultValue => new TV[0];

        private readonly Parser<TV> _parser;
        public VectorParser() : this(ParserUtil.GetParser<TV>() as Parser<TV>)
        {
            
        }

        public VectorParser(Parser<TV> parser)
        {
            _parser = parser;
            _parser?.SetParent(this);
        }

        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, TV[] value)
        {
            if (value == null)
            {
                value = DefaultValue as TV[];
            }
            if (!isColloctString)
            {
                writer.Write((ushort)value.Length);
            }
            for (var i = 0; i < value.Length; i++)
            {
                _parser.Write(isColloctString, ref dictStringToHash, writer, value[i]);
            }
        }

        public override bool WriteLocal(BinaryWriter writer, object obj)
        {
            TV[] value;
            if (obj == null)
            {
                value = DefaultValue as TV[];
            }
            else
            {
                value = (TV[])obj;
            }
            writer.Write((ushort)value.Length);
            if (typeof(TV) == typeof(string))
            {
                for (var i = 0; i < value.Length; i++)
                {
                    string s = value[i] as string;
                    string wStr = s;
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        wStr = (string)_parser.DefaultValue;
                    }
                    throw new Exception();
                    //uint key = StringPoolManager.singleton.GetKey(wStr);
                    //writer.Write(key);
                }
            }
            else
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (!_parser.WriteLocal(writer, value[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out TV[] value)
        {
            var length = reader.ReadUInt16();
            value = new TV[length];
            for (int i = 0; i < length; i++)
            {
                if(typeof(TV)==typeof(string))
                {
                    value[i]=(TV)(object)reader.ReadString();
                }
                else
                {
                    if (!_parser.Read(ref dictHashToString, reader, out TV res))
                    {
                        return false;
                    }
                    else
                    {
                        value[i] = res;
                    }
                }
            }
            return true;
        }

        public override int Compare(TV[] value0, TV[] value1)
        {
            if (value0.Length != value1.Length)
                return value0.Length.CompareTo(value1.Length);

            for (int i = 0; i < value0.Length; i++)
            {
                var result = _parser.Compare(value0[i], value1[i]);
                if (result != 0)
                    return result;
            }
            return 0;
        }

        public override bool Parse(string str, out TV[] value)
        {
            str = str?.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                value = new TV[0];
                return true;
            }
            var values = str.Split(ParserUtil.ListSeparator[ChildLevel]);
            value = new TV[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                TV res;
                if (!_parser.Parse(values[i], out res))
                {
                    res = (TV)_parser.DefaultValue;
                    return false;
                }
                value[i] = res;
            }
            return true;
        }

        public override string SerializeExcel(object obj)
        {
            TV[] value = (TV[])obj;
            string res = "";
            var splitStr = ParserUtil.ListSeparator[ChildLevel];
            for(int i=0;i<value.Length;i++)
            {
                res += _parser.SerializeExcel(value[i]);
                if(i!=value.Length-1)
                {
                    res += splitStr;
                }
            }
            return res;
        }

        public override List<string> GetStringValue(object value)
        {
            List<string> res = new List<string>();

            TV[] v = (TV[])value;
            if (typeof(TV) == typeof(string))
            {
                for (var i = 0; i < v.Length; i++)
                {
                    object obj = v[i];
                    string s = (string)obj;
                    string wStr = s;
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        wStr = (string)_parser.DefaultValue;
                    }
                    res.Add(wStr);
                }
            }
            else
            {
                for (var i = 0; i < v.Length; i++)
                {
                    List<string> tmpLs;
                    tmpLs = _parser.GetStringValue(v[i]);
                    res.AddRange(tmpLs);
                }
            }
            
            return res;
        }

    }
}
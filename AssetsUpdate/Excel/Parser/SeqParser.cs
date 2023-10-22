using System;
using System.IO;
using System.Collections.Generic;
using AssetsUpdate;
using ToolLib;

namespace Serializer
{
    public class SeqParser<TV> : Parser<MSeq<TV>>
    {

        private readonly byte _length;
        private readonly Parser<TV> _parser;

        public override object DefaultValue => new MSeq<TV>(_length);

        public SeqParser(byte length) : this(length, ParserUtil.GetParser<TV>() as Parser<TV>)
        {
            
        }

        public SeqParser(byte length, Parser<TV> parser)
        {
            if (length > ParserUtil.Seq_Max_Len)
                throw new Exception($"Seq数量不能超过{ParserUtil.Seq_Max_Len}个！！");
            _length = length;
            _parser = parser;
            _parser?.SetParent(this);
        }

        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, MSeq<TV> value)
        {
            if (value == null)
            {
                value = DefaultValue as MSeq<TV>;
            }
            if(typeof(TV)==typeof(string))
            {
                for (var i = 0; i < _length; i++)
                {
                    object obj = value[i];
                    string s = (string)obj;
                    string wStr = s;
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        wStr = (string)_parser.DefaultValue;
                    }
                    ParserUtil.AddStr(isColloctString, ref dictStringToHash, writer, wStr);
                }
            }
            else
            {
                for (var i = 0; i < _length; i++)
                {
                    if (value.Capacity <= i)
                    {
                        _parser.Write(isColloctString, ref dictStringToHash, writer, _parser.DefaultValue);
                    }
                    else
                    {
                        _parser.Write(isColloctString, ref dictStringToHash, writer, value[i]);
                    }
                }
            }
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out MSeq<TV> value)
        {
            value = new MSeq<TV>(_length);
            for (int i = 0; i < _length; i++)
            {
                if (!_parser.Read(ref dictHashToString, reader, out TV res))
                {
                    return false;
                }
                value[i] = res;
            }
            return true;
        }

        public override int Compare(MSeq<TV> value0, MSeq<TV> value1)
        {
            if (value0.Capacity != value1.Capacity)
                return value0.Capacity.CompareTo(value1.Capacity);

            for (int i = 0; i < value0.Capacity; i++)
            {
                var result = _parser.Compare(value0[i], value1[i]);
                if (result != 0)
                    return result;
            }
            return 0;
        }

        public override bool Parse(string str, out MSeq<TV> value)
        {
            str = str?.Trim();
            var values = str.Split(ParserUtil.SeqSeparator[ChildLevel]);
            if(values.Length > ParserUtil.Seq_Max_Len)
            {
                throw new Exception($"Seq数量不能超过{ParserUtil.Seq_Max_Len}个！！value:{str}");
            }
	        bool succ = true;
            value = new MSeq<TV>(_length);
            for (var i = 0; i < _length; i++)
            {
                if (values.Length <= i)
                {
                    value[i] = (TV)_parser.DefaultValue;
                }
                else
                {
                    TV res;
                    if (!_parser.Parse(values[i], out res))
                    {
                        res = (TV)_parser.DefaultValue;
	                    return false;
                    }
                    value[i] = res;
                }
                
            }
            return succ;
        }

        public override string SerializeExcel(object obj)
        {
            string res = "";
            MSeq<TV> value = (MSeq<TV>)obj;
            char splitStr = ParserUtil.SeqSeparator[ChildLevel];
            int cap = Math.Min(value.Capacity, _length);
            for (int i=0;i< cap; i++)
            {
                res += _parser.SerializeExcel(value[i]);
                if(i!=cap-1)
                {
                    res += splitStr;
                }
            }
            return res;
        }

        public override List<string> GetStringValue(object value)
        {
            List<string> res = new List<string>();

            MSeq<TV> v = (MSeq<TV>)value;

            if (typeof(TV) == typeof(string))
            {
                for (var i = 0; i < _length; i++)
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
                for (var i = 0; i < _length; i++)
                {
                    List<string> tmpLs;
                    if (v.Capacity <= i)
                    {
                        tmpLs = _parser.GetStringValue(_parser.DefaultValue);
                    }
                    else
                    {
                        tmpLs = _parser.GetStringValue(v[i]);
                    }
                    res.AddRange(tmpLs);
                }
            }

            return res;
        }

        public override bool WriteLocal(BinaryWriter writer, object obj)
        {
            MSeq<TV> value;
            if (obj == null)
            {
                value = DefaultValue as MSeq<TV>;
            }
            else
            {
                value = (MSeq<TV>)obj;
            }
            if (typeof(TV) == typeof(string))
            {
                for(var i = 0; i < _length; i++)
                {
                    string s;
                    if (value.Capacity <= i)
                    {
                        s = (string)_parser.DefaultValue;
                    }
                    else
                    {
                        s = value[i] as string;
                    }
                    uint key = Program.GetKey(s);
                    writer.Write(key);
                }
            }
            else
            {
                for (var i = 0; i < _length; i++)
                {
                    if (value.Capacity <= i)
                    {
                        if(!_parser.WriteLocal(writer, _parser.DefaultValue))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if(!_parser.WriteLocal(writer, value[i]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
using System;
using System.IO;
using System.Text;

namespace Serializer
{
    public class MSeq<T>
    {
        private T[] _values = null;
        private byte _capacity = 0;

        public MSeq(byte capacity)
        {
            _capacity = capacity;
            _values = new T[capacity];
        }

        public int Capacity { get { return _capacity; } }

        public T this[int key]
        {
            get
            {
                if (key >= _capacity)
                {
                    //MDebug.singleton.AddErrorLogF("Set MSeq out of range! current:{0}, input:{1}", _capacity, key);
                    return default(T);
                }

                return _values[key];
            }
            set
            {
                if (key >= _capacity)
                {
                    //MDebug.singleton.AddErrorLogF("Get MSeq out of range! current:{0}, input:{1}", _capacity, key);
                    return;
                }

                _values[key] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (var i = 0; i < _values.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append("=");
                }
                builder.Append(_values[i].ToString());
            }
            return builder.ToString();
        }
        
    }
    public class MSeqList<T>
    {
        private T[,] _values = null;
        private ushort _count = 0;
        private byte _capacity = 0;

        public ushort Count { get { return _count; } }
        public byte Capacity { get { return _capacity; } }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="count">多少个seq</param>
        /// <param name="capacity">每个seq的元素个数</param>
        public MSeqList(ushort count, byte capacity)
        {
            if (capacity < 0 || capacity > 9)
            {
                //MDebug.singleton.AddErrorLogF("MSeqList capacity:{0}", capacity);
                return;
            }
            _count = count;
            _capacity = capacity;
            if (_count > 0 && _capacity > 0)
            {
                _values = new T[_count, _capacity];
            }
            else
            {
                _values = null;
            }
        }
        public T this[int k1, int k2]
        {
            get
            {
                if (k1 >= _count || k2 >= _capacity)
                {
                   // MDebug.singleton.AddErrorLogF("Get MSeqList out of range! current:{0},{1}, input:{2},{3}", _count, _capacity, k1, k2);
                    return default(T);
                }

                return _values[k1, k2];
            }
            set
            {
                if (k1 >= _count || k2 >= _capacity)
                {
                    //MDebug.singleton.AddErrorLogF("Set MSeqList out of range! current:{0},{1}, input:{2},{3}", _count, _capacity, k1, k2);
                    return;
                }

                _values[k1, k2] = value;
            }
        }
    }
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

                    throw new Exception();
                    //uint key = StringPoolManager.singleton.GetKey(s);
                    //writer.Write(key);
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
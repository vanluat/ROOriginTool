using System;
using System.IO;
using System.Collections.Generic;

namespace Serializer
{
    public abstract class Parser
    {
        public int ChildLevel { get; private set; }//作为子层级的层级（最外层为0）
        public Parser ParentParser { get; private set; }
        public void SetParent(Parser parser)
        {
            ParentParser = parser;
            ChildLevel = parser.ChildLevel + 1;
        }

        public abstract object DefaultValue { get; }
        
        private bool WriteFromString(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                Write(isColloctString, ref dictStringToHash, writer, DefaultValue);
                return true;
            }
            if (Parse(str, out var value))
            {
                Write(isColloctString, ref dictStringToHash, writer, value);
                return true;
            }
            else
            {
                Write(isColloctString, ref dictStringToHash, writer, value);
                return false;
            }
        }

        public virtual bool WriteLocalFromString(BinaryWriter writer, string str)
        {
            bool res = true;
            if (string.IsNullOrWhiteSpace(str))
            {
                res = WriteLocal(writer,DefaultValue);
            }
            else if (Parse(str, out var value))
            {
                res = WriteLocal(writer, value);
            }
            else
            {
                res = WriteLocal(writer, value);
            }
            return res;
        }

        public bool WriteHashFromString(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, string str,string typeName)
        {
            if (ParserUtil.IsStringType(typeName))
            {
                string wStr = str;
                if (string.IsNullOrWhiteSpace(str))
                {
                    wStr = (string)DefaultValue;
                }
                ParserUtil.AddStr(isColloctString, ref dictStringToHash, writer, wStr);
                return true;
            }
            else
            {
                return WriteFromString(isColloctString, ref dictStringToHash, writer, str);
            }
        }
        
        public abstract void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, object value);

        public virtual bool WriteLocal(BinaryWriter writer, object value)
        {
           // Context.Logger.Error("暂未支持的本地化类型:"+value.GetType().Name);
            return false;
        }
        
        public abstract bool Read(ref string[] dictHashToString, BinaryReader reader, out object value);

        public abstract int Compare(object value0, object value1);

        public int CompareFromString(string str1, string str2)
        {
            if (!Parse(str1, out object obj1) || !Parse(str2, out object obj2))
            {
                return 0;
            }
            return Compare(obj1, obj2);
        }

        public abstract bool Parse(string str, out object value);

        public abstract string SerializeExcel(object obj);

        public virtual List<string> GetStringValueFromString(string str)
        {
            List<string> res = new List<string>();
            if (!Parse(str, out var value))
            {
                value = DefaultValue;
            }
            List<string> tmpLs = GetStringValue(value);
            res.AddRange(tmpLs);
            return res;
        }

        public virtual List<string> GetStringValue(object value)
        {
            List<string> res = new List<string>();
            return res;
        }

    }

    public abstract class Parser<T> : Parser
    {
        public override object DefaultValue => default(T);
        
        public abstract void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, T value);
        
        public abstract bool Read(ref string[] dictHashToString, BinaryReader reader, out T value);

        public abstract int Compare(T value0, T value1);

        public abstract bool Parse(string str, out T value);

        public override void Write(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, BinaryWriter writer, object value)
        {
            Write(isColloctString, ref dictStringToHash, writer, (T)value);
        }

        public override bool Read(ref string[] dictHashToString, BinaryReader reader, out object value)
        {
            if (Read(ref dictHashToString, reader, out T result))
            {
                value = result;
                return true;
            }
            value = result;
            return false;
        }

        public override int Compare(object value0, object value1)
        {
            return Compare((T) value0, (T)value1);
        }

        public override bool Parse(string str, out object value)
        {
            if (Parse(str, out T result))
            {
                value = result;
                return true;
            }
            value = result;
            return false;
        }

    }
}
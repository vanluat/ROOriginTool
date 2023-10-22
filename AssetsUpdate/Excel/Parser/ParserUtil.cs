using System;
using System.Collections.Generic;
using System.Reflection;
using ExtractOfficialAssets;
using ToolLib;
using ToolLib.Excel;

namespace Serializer
{
    public enum FieldCollectionType
    {
        None,
        Vector,
        Sequence,
        VectorSequence,
        VectorVector
    }

    public class ParserUtil
    {
        public static readonly char[] ListSeparator = { '|', '=' };
        public static readonly char[] SeqSeparator = { '=' , '='};
        public static readonly int Seq_Max_Len = 9;
        public static readonly char PLACE_HOLDER = '#';
        
        public static bool IsStringType(string typeName)
        {
            return (typeName == "string");
        }
        public static bool IsStringType(Type type)
        {
            return (type == typeof(string));
        }

        internal static void AddStr(bool isColloctString, ref Dictionary<string, uint> dictStringToHash, System.IO. BinaryWriter writer, string strValue)
        {
            if (isColloctString)
            {
                ParserUtil.AddStrToDict(ref dictStringToHash, strValue);
            }
            else
            {
                uint hash = ParserUtil.GetStrHashFromDict(ref dictStringToHash, strValue);
                writer.Write(hash);
            }
        }

        private static void AddStrToDict(ref Dictionary<string, uint> dictStringToPos, string strValue)
        {
            if(!dictStringToPos.ContainsKey(strValue))
            {
                uint cnt = (uint)(dictStringToPos.Count);
                dictStringToPos.Add(strValue, cnt);
            }
        }
        private static uint GetStrHashFromDict(ref Dictionary<string, uint> dictStringToHash, string strValue)
        {
            if(dictStringToHash.TryGetValue(strValue, out var hash))
            {
                return hash;
            }
            else
            {
               throw new($"{strValue} 不存在hash!!!");
                return 0;
            }
        }
        internal static string GetStringByHash(ref string[] dictHashToString, uint hash)
        {
            if (hash < dictHashToString.Length)
            {
                if(dictHashToString[hash] != null)
                {
                    return dictHashToString[hash];
                }
                else
                {
                   throw new($"总计：{dictHashToString.Length},第{hash}个为null!!!");
                    return "null string!!!";
                }
            }
            else
            {
               throw new($"总计：{dictHashToString.Length},第{hash}个不存在!!!");
                return "不存在string!!!";
            }
        }

        public static bool IsSupportVarType(string typeName)
        {
            if(-1 != typeName.IndexOf("vector<bool>", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool IsSupportBinType(string typeName)
        {
            switch(typeName)
            {
                case BASIC_TYPE_CHAR:
                case BASIC_TYPE_BOOL:
                case BASIC_TYPE_FLOAT:
                case BASIC_TYPE_DOUBLE:
                    {
                        return false;
                    }
                case BASIC_TYPE_INT:
                case BASIC_TYPE_UINT:
                case BASIC_TYPE_LONG_LONG:
                case BASIC_TYPE_STRING:
                    {
                        return true;
                    }
                default:
                    {
                       throw new($"typeName【{typeName}】 dont handle");
                        return false;
                    }
            }
        }

        private const string BASIC_TYPE_CHAR = "char";
        private const string BASIC_TYPE_BOOL = "bool";
        private const string BASIC_TYPE_INT = "int";
        private const string BASIC_TYPE_UINT = "uint";
        private const string BASIC_TYPE_FLOAT = "float";
        private const string BASIC_TYPE_DOUBLE = "double";
        private const string BASIC_TYPE_LONG_LONG = "long long";
        private const string BASIC_TYPE_STRING = "string";

        public static string[] basicType = {
            BASIC_TYPE_CHAR
            , BASIC_TYPE_BOOL
            , BASIC_TYPE_INT
            , BASIC_TYPE_UINT
            , BASIC_TYPE_FLOAT
            , BASIC_TYPE_DOUBLE
            , BASIC_TYPE_LONG_LONG
            , BASIC_TYPE_STRING
        };

        public static Dictionary<string, string> typeStrMap = new Dictionary<string, string>()
        {
            ["Char"] = "Char",
            ["Boolean"] = "Bool",
            ["Double"] = "Double",
            ["Single"] = "Float",
            ["Int32"] = "Int",
            ["Int64"] = "Long",
            ["String"] = "String",
            ["UInt32"] = "UInt",
        };

        public static Dictionary<string, string> basicTypeStrMap = new Dictionary<string, string>()
        {
            ["char"] = "Char",
            ["bool"] = "Bool",
            ["double"] = "Double",
            ["float"] = "Float",
            ["int"] = "Int",
            ["long long"] = "Long",
            ["string"] = "String",
            ["uint"] = "UInt",
        };

        public static Dictionary<string, Type> basicTypeMap = new Dictionary<string, Type>()
        {
            ["char"] = typeof(char),
            ["bool"] = typeof(bool),
            ["double"] = typeof(double),
            ["float"] = typeof(float),
            ["int"] = typeof(int),
            ["long long"] = typeof(long),
            ["string"] = typeof(string),
            ["uint"] = typeof(uint),
        };

        /// <summary>
        /// 获取基本类型的Parser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser GetParser<T>()
        {
            if (typeStrMap.TryGetValue(typeof(T).Name, out string value))
            {
                var type = typeof(BoolParser).Assembly.GetTypes().First(m => m.FullName == $"Serializer.{value}Parser");
                var argTypes = new Type[0];
                var args = new object[0];
                return type.GetConstructor(argTypes)?.Invoke(args) as Parser<T>;
            }
            return null;
        }

        /// <summary>
        /// 获取解析器
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Parser GetParser(string typeName)
        {
            
            if (basicTypeStrMap.ContainsKey(typeName))
            {
                var type = typeof(BoolParser).Assembly.GetTypes().First(m => m.FullName == $"Serializer.{basicTypeStrMap[typeName]}Parser");
                var argTypes = new Type[0];
                var args = new object[0];
                return type.GetConstructor(argTypes)?.Invoke(args) as Parser;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                typeName = typeName.Trim();
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                var subTypeFullName = GetTypeFullName(subType);
                var seqNum = byte.Parse(typeName.Substring(typeName.Length - 2, 1));
                var parserTypeName =
                    $"Serializer.SeqParser`1[[{subTypeFullName}]], ToolLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                var type = Type.GetType(parserTypeName);
                var subParser = GetParser(subType);
                var argTypes = new Type[2] {typeof(byte), subParser.GetType() };
                var args = new object[2] { seqNum, subParser };
                return type.GetConstructor(argTypes)?.Invoke(args) as Parser;
            }
            else if (typeName.StartsWith("vector"))
            {
                typeName = typeName.Trim();
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                var subTypeFullName = GetTypeFullName(subTypeName);
                var parserTypeName =
                    $"Serializer.VectorParser`1[[{subTypeFullName}]], ToolLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                var type = Type.GetType(parserTypeName);
                var subParser = GetParser(subTypeName);
                var argTypes = new Type[1]{ subParser.GetType() };
                var args = new object[1]{ subParser };
                return type.GetConstructor(argTypes)?.Invoke(args) as Parser;
            }
            return null;
        }

        public static Parser GetParser(TableTypeEnum typeEnum, sbyte seqLength)
        {
            int type = (int) typeEnum;
            if (type < 0)
            {
               throw new($"BaseType enum is error type is {type}");
                return null;
            }
            if (type % 100 >= basicType.Length)
            {
               throw new($"BaseType enum is error type is {type}");
                return null;
            }
            string baseType = basicType[type % 100];
            if (type / 100 == 0)
            {
                return GetParser(baseType);
            }
            else if (type / 100 == 1)
            {
                return GetParser($"vector<{baseType}>");
            }
            else if (type / 100 == 2)
            {
                return GetParser($"Sequence<{baseType}, {seqLength}>");
            }
            else if (type / 100 == 3)
            {
                return GetParser($"vector<Sequence<{baseType}, {seqLength}>>");
            }
            else if (type / 100 == 4)
            {
                return GetParser($"vector<vector<{baseType}>>");
            }
            return null;
        }

        /// <summary>
        /// 获取表中所有类型在代码中的真实类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetTypeFullName(string typeName)
        {
            if (basicTypeMap.ContainsKey(typeName))
            {
                var type = basicTypeMap[typeName];
                return type.AssemblyQualifiedName;
            }else if (typeName.StartsWith("Sequence"))
            {
                typeName = typeName.Trim();
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                var subTypeFullName = GetTypeFullName(subType);
                var type = Type.GetType($"MoonCommonLib.MSeq`1[[{subTypeFullName}]], MoonCommonLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                return type.AssemblyQualifiedName;
            }
            else if (typeName.StartsWith("vector"))
            {
                typeName = typeName.Trim();
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                var subTypeFullName = GetTypeFullName(subTypeName);
                var subType = Type.GetType($"{subTypeFullName}");
                var type = subType.MakeArrayType();
                return type.AssemblyQualifiedName;
            }
            return null;
        }

        public static string GetCppDefaultValue(string typename) {
            if (typename.StartsWith("Sequence"))
            {
                return ";";
            } else if (typename.StartsWith("vector")) {
                return "={};";
            }
            if (typename == "string")
                return ";";
            return "=0;";
        }

        public static string GetCppTypeName(string typeName,bool needLocal=false)
        {
            if (basicType.Contains(typeName))
            {
                string typeStr = typeName;
                switch (typeStr)
                {
                    case "int":
                        typeStr = "int32_t";
                        break;
                    case "uint":
                        typeStr = "uint32_t";
                        break;
                    case "long long":
                        typeStr = "int64_t";
                        break;
                }
                if(needLocal && typeStr == BASIC_TYPE_STRING)
                {
                    typeStr = "uint32_t";
                }
                return typeStr;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                return $"Sequence<{GetCppTypeName(subType,needLocal)}>";
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                return $"vector<{GetCppTypeName(subTypeName,needLocal)}>";
            }
            return null;
        }

        public static string GetCppOrginTypeName(string typeName)
        {
            if (basicType.Contains(typeName))
            {
                string typeStr = typeName;
                switch (typeStr)
                {
                    case "uint":
                        typeStr = "unsigned int";
                        break;
                    case "string":
                        typeStr = "std::string";
                        break;
                }
                return typeStr;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                return $"Sequence<{GetCppTypeName(subType)}>";
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                return $"vector<{GetCppTypeName(subTypeName)}>";
            }
            return null;
        }

        public static string GetCSTypeName(string typeName)
        {

            if (basicType.Contains(typeName))
            {
                string typeStr = typeName;
                switch (typeStr)
                {
                    case "long long":
                        typeStr = "long";
                        break;
                }
                return typeStr;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                return $"MSeq<{GetCSTypeName(subType)}>";
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                if (subTypeName.StartsWith("Sequence"))
                {
                    var subsubTypeName = subTypeName.Substring("Sequence<".Length, subTypeName.Length - "Sequence<, 3>".Length);
                    return $"MSeqList<{GetCSTypeName(subsubTypeName)}>";
                }
                else if (subTypeName.StartsWith("vector"))
                {
                    var subsubTypeName = subTypeName.Substring("vector<".Length, subTypeName.Length - "vector<>".Length);
                    return $"{GetCSTypeName(subsubTypeName)}[][]";
                }
                else
                {
                    return $"{GetCSTypeName(subTypeName)}[]";
                }
            }
            return null;
        }

        public static bool GetTypeEnum(string typeName, out TableTypeEnum result,bool needLocal = false)
        {
            var enumName = GetTypeEnumStr(typeName, needLocal);
            return Enum.TryParse(enumName, true, out result);
        }

        public static string GetTypeEnumStr(string typeName,bool needLocal=false)
        {
            if (basicType.Contains(typeName))
            {
                if(needLocal && typeName == "string")
                {
                    typeName = "uint";
                }
                return typeName.ToUpper().Replace(" ", "");
            }
            else if (typeName.StartsWith("Sequence"))
            {
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                if(needLocal && subType=="string")
                {
                    subType = "uint";
                }
                return $"Sequence_{subType}".ToUpper().Replace(" ", "");
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                if (subTypeName.StartsWith("Sequence"))
                {
                    var subsubTypeName = subTypeName.Substring("Sequence<".Length, subTypeName.Length - "Sequence<, 3>".Length);
                    if (needLocal && subsubTypeName == "string")
                    {
                        subsubTypeName = "uint";
                    }
                    return $"vector_Sequence_{subsubTypeName}".ToUpper().Replace(" ", "");
                }
                else if (subTypeName.StartsWith("vector"))
                {
                    var subsubTypeName = subTypeName.Substring("vector<".Length, subTypeName.Length - "vector<>".Length);
                    if (needLocal && subsubTypeName == "string")
                    {
                        subsubTypeName = "uint";
                    }
                    return $"vector_vector_{subsubTypeName}".ToUpper().Replace(" ", "");
                }
                else
                {
                    if (needLocal && subTypeName == "string")
                    {
                        subTypeName = "uint";
                    }
                    return $"vector_{subTypeName}".ToUpper().Replace(" ", "");
                }
            }
            return "NONE";
        }

        public static sbyte GetSeqLength(string typeName)
        {
            if (typeName.StartsWith("Sequence"))
            {
                var seqNum = sbyte.Parse(typeName.Substring(typeName.Length - 2, 1));
                return seqNum;
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                if (subTypeName.StartsWith("Sequence"))
                {
                    var seqNum = sbyte.Parse(subTypeName.Substring(subTypeName.Length - 2, 1));
                    return seqNum;
                }
            }
            return 0;
        }

        public static string GetBasicTypeName(string typeName)
        {
            if (basicType.Contains(typeName))
            {
                return typeName;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                var subType = typeName.Substring("Sequence<".Length, typeName.Length - "Sequence<, 3>".Length);
                return subType;
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                if (subTypeName.StartsWith("Sequence"))
                {
                    var subsubTypeName = subTypeName.Substring("Sequence<".Length, subTypeName.Length - "Sequence<, 3>".Length);
                    return subsubTypeName;
                }
                else if (subTypeName.StartsWith("vector"))
                {
                    var subsubTypeName = subTypeName.Substring("vector<".Length, subTypeName.Length - "vector<>".Length);
                    return subsubTypeName;
                }
                else
                {
                    return subTypeName;
                }
            }
            return null;
        }

        public static FieldCollectionType GetCollectionType(string typeName)
        {
            if (basicType.Contains(typeName))
            {
                return FieldCollectionType.None;
            }
            else if (typeName.StartsWith("Sequence"))
            {
                return FieldCollectionType.Sequence;
            }
            else if (typeName.StartsWith("vector"))
            {
                var subTypeName = typeName.Substring("vector<".Length, typeName.Length - "vector<>".Length);
                if (subTypeName.StartsWith("Sequence"))
                {
                    return FieldCollectionType.VectorSequence;
                }
                else if (subTypeName.StartsWith("vector"))
                {
                    return FieldCollectionType.VectorVector;
                }
                else
                {
                    return FieldCollectionType.Vector;
                }
            }
            return FieldCollectionType.None;
        }

        public static string GetAPINameByFieldTypeName(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
               throw new("FieldTypeName IsNullOrEmpty!!!");
                return null;
            }
            var lowerName = Name.ToLower() + ">";
            List<string> typeList = new List<string>();
            int startPos = -1;
            bool findLetter = true;
            for (int i = 0; i < lowerName.Length; i++)
            {
                var curChar = lowerName[i];
                if (findLetter)
                {
                    if (IsLetter(curChar))
                    {
                        startPos = i;
                        findLetter = !findLetter;
                    }
                }
                else
                {
                    if (!IsLetter(curChar))
                    {
                        var type = lowerName.Substring(startPos, i - startPos);
                        typeList.Add(type);
                        findLetter = !findLetter;
                    }
                }
            }

            string strAPIName = "";
            int size = typeList.Count;
            for (int i = 0; i < size; i++)
            {
                var curType = typeList[i];
                if (curType == "long")
                {
                    if (i + 1 < size)
                    {
                        curType = curType + " " + typeList[i + 1];
                        i++;
                    }
                }
                if (ParserUtil.basicTypeStrMap.ContainsKey(curType))
                {
                    strAPIName += ParserUtil.basicTypeStrMap[curType];
                }
                else
                {
                    strAPIName += curType.Substring(0, 1).ToUpper() + curType.Substring(1);
                }
            }
            return strAPIName;
        }

        public static bool IsLetter(char ch)
        {
            return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'));
        }

    }

}
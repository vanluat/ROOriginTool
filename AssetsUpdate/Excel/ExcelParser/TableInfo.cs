using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ToolLib.Excel
{
   // [System.Flags]
    public enum TableCodeTarget
    {
        Cpp = 1,
        CSharp = 1 << 1,
        Lua = 1 << 2,
    }

  //  [System.Flags]
    public enum TableBytesTarget
    {
        Server = 1,
        Client = 2,
        Editor = 3,
    }


    public class TableFieldInfo
    {
        public enum EIndexType
        {
            None,//无
            Bin,//主键使用二分搜索（唯一）
            Search//副键使用字典搜索
        }

        public string FieldName;
        public string FieldTypeName;
        public string DefaultValue;
        public bool ForClient;
        public bool ForServer;
        internal bool ForEditor
        {
            get { return (ForClient || ForServer); }
        }
        public bool IsForEditor()
        {
            return ForEditor;
        }
        internal bool ForAny
        {
            get { return (ForClient || ForServer); }
        }
        public int ClientPosID = -1;
        public int ServerPosID = -1;
        public int EditorPosID = -1;
        public long ClientPosTimeStamp = 0;
        public long ServerPosTimeStamp = 0;
        public long EditorPosTimeStamp = 0;
        public EIndexType IndexType;
        public CheckerInfo[] CheckInfos;
        public bool NeedLocal = false;
        
        public void ResetClientPosID()
        {
            SetClientPosID(-1);
        }
        public void ResetServerPosID()
        {
            SetServerPosID(-1);
        }

        public void ResetEditorPosID()
        {
            SetEditorPosID(-1);
        }
        

        public void SetClientPosID(int pos)
        {
            if(pos < -1 || pos > 500)
            {
               throw new($"error Client pos:{pos}|FieldName:{FieldName}");
                return;
            }
            if(ClientPosID != pos || ClientPosTimeStamp <= 0)
            {
                ClientPosID = pos;
                ClientPosTimeStamp = GetNowTimeStamp();
            }
        }

        public void SetServerPosID(int pos)
        {
            if (pos < -1 || pos > 500)
            {
               throw new($"error Client pos:{pos}|FieldName:{FieldName}");
                return;
            }
            if(ServerPosID != pos || ServerPosTimeStamp <= 0)
            {
                ServerPosID = pos;
                ServerPosTimeStamp = GetNowTimeStamp();
            }
        }
        public void SetEditorPosID(int pos)
        {
            if (pos < -1 || pos > 500)
            {
               throw new($"error Client pos:{pos}|FieldName:{FieldName}");
                return;
            }
            if (EditorPosID != pos || EditorPosTimeStamp <= 0)
            {
                EditorPosID = pos;
                EditorPosTimeStamp = GetNowTimeStamp();
            }
        }
        
        public string GetAPIFieldTypeName()
        {
            return FieldTypeName.Replace(" ", "").ToUpper();
        }

        public static long GetNowTimeStamp()
        {
            var now = DateTime.Now;
            var timeArr = new int[] { now.Month, now.Day, now.Hour, now.Minute, now.Second };
            long timeStamp = now.Year;
            for(int i = 0; i < timeArr.Length; i++)
            {
                timeStamp = timeStamp * 100L + timeArr[i];
            }
            return timeStamp;
        }

       
    }

    public class TableInfo
    {
        
       

        /// <summary>
        /// 生成Bytes时是否需要预处理
        /// </summary>
        public bool isNeedPre;

        /// <summary>
        /// 生成Bytes时是否需要后处理
        /// </summary>
        public bool isNeedPost;

        /// <summary>
        /// 表代码目标
        /// </summary>
        public uint TableCodeTarget;

        /// <summary>
        /// 表Bytes目标
        /// </summary>
        public uint TableBytesTarget;

        /// <summary>
        /// 主表名
        /// </summary>
        public string MainTableName;

        /// <summary>
        /// 各个分表名
        /// </summary>
        public TableLocation[] TableLocations;

        /// <summary>
        /// 表字段信息
        /// </summary>
        public TableFieldInfo[] Fields;

        /// <summary>
        /// 子表
        /// </summary>
        public TableInfo[] Children;

        /// <summary>
        /// 表检查器
        /// </summary>
        public CheckerInfo[] CheckInfos;
        public void ClearUnusedPosID()
        {
            if (null == Fields) return;

            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var field = Fields[i];
                if (!field.ForClient)
                {
                    field.ResetClientPosID();
                }
                if (!field.ForServer)
                {
                    field.ResetServerPosID();
                }
                if(!field.ForEditor)
                {
                    field.ResetEditorPosID();
                }
            }
        }

        public int CheckNoValidPosID()
        {
            if (null == Fields) return -1;

            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                if(curField.ForClient && curField.ClientPosID < 0)
                {
                    return i;
                }
                if(curField.ForServer && curField.ServerPosID < 0)
                {
                    return i;
                }
                if((curField.ForEditor) && curField.EditorPosID < 0)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public void BuildPosID(bool isForceRebuild)
        {
            if (null == Fields) return;
            int iLen = Fields.Length;
            ClearUnusedPosID();
            if (isForceRebuild)
            {
                for (int i = 0; i < iLen; i++)
                {
                    var field = Fields[i];
                    field.ResetClientPosID();
                    field.ResetServerPosID();
                    field.ResetEditorPosID();
                }
            }
            for (int i = 0; i < iLen; i++)
            {
                var field = Fields[i];
                if (field.ForClient && (field.ClientPosID < 0))
                {
                    field.SetClientPosID(GetClientMinPosID());
                }
                if (field.ForServer && (field.ServerPosID < 0))
                {
                    field.SetServerPosID(GetServerMinPosID());
                }
                if (field.ForEditor && (field.EditorPosID < 0))
                {
                    field.SetEditorPosID(GetEditorMinPosID());
                }
            }
        }

        public bool IsConflictPosID()
        {
            if (null == Fields) return true;

            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                for(int j = i + 1; j < iLen; j++)
                {
                    var nxtField = Fields[j];
                    if (curField.ForClient && nxtField.ForClient && curField.ClientPosID == nxtField.ClientPosID)
                    {
                        return true;
                    }
                    if (curField.ForServer && nxtField.ForServer && curField.ServerPosID == nxtField.ServerPosID)
                    {
                        return true;
                    }
                    if(curField.ForEditor && nxtField.ForEditor && curField.EditorPosID == nxtField.EditorPosID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetClassComment()
        {
            StringBuilder result = new StringBuilder();

            int iLen = Fields.Length;

            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                result.AppendLine($"---@field {curField.FieldName} {curField.GetEmmyLuaType()}");
            }

            return result.ToString();
        }

        public int GetServerMinPosID()
        {
            if (null == Fields)
            {
                throw new(string.Format("Fields is null by MainTableName:{0}", MainTableName));
                return -1;
            }

            HashSet<int> hashPosID = new HashSet<int>();
            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                if (curField.ForServer)
                {
                    hashPosID.Add(curField.ServerPosID);
                }
            }
            return GetMinPosID(hashPosID);
        }

        public int GetEditorMinPosID()
        {
            if (null == Fields)
            {
                throw new(string.Format("Fields is null by MainTableName:{0}", MainTableName));
                return -1;
            }

            HashSet<int> hashPosID = new HashSet<int>();
            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                if (curField.ForEditor)
                {
                    hashPosID.Add(curField.EditorPosID);
                }
            }
            return GetMinPosID(hashPosID);
        }
        
        public int GetClientMinPosID()
        {
            if (null == Fields)
            {
                throw new($"Fields is null by MainTableName:{MainTableName}");
                return -1;
            }

            HashSet<int> hashPosID = new HashSet<int>();
            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                if(curField.ForClient)
                {
                    hashPosID.Add(curField.ClientPosID);
                }
            }
            return GetMinPosID(hashPosID);
        }

        private static int GetMinPosID(HashSet<int> hashPosID)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (!hashPosID.Contains(i))
                {
                    return i;
                }
                else
                {
                    continue;
                }
            }
           throw new($"hash is fully:{hashPosID.Count}");
            throw new(string.Format("hash is fully:{0}", hashPosID.Count));
            return -1;
        }

        public void SolvePosIDConflict()
        {
            if (null == Fields) return ;

            int iLen = Fields.Length;
            for (int i = 0; i < iLen; i++)
            {
                var curField = Fields[i];
                for (int j = i + 1; j < iLen; j++)
                {
                    var nxtField = Fields[j];
                    if (curField.ForClient && nxtField.ForClient && curField.ClientPosID == nxtField.ClientPosID)
                    {
                        if(curField.ClientPosTimeStamp > nxtField.ClientPosTimeStamp)
                        {
                            curField.SetClientPosID(GetClientMinPosID());
                        }
                        else
                        {
                            nxtField.SetClientPosID(GetClientMinPosID());
                        }
                    }
                    if (curField.ForServer && nxtField.ForServer && curField.ServerPosID == nxtField.ServerPosID)
                    {
                        if (curField.ServerPosTimeStamp > nxtField.ServerPosTimeStamp)
                        {
                            curField.SetServerPosID(GetServerMinPosID());
                        }
                        else
                        {
                            nxtField.SetServerPosID(GetServerMinPosID());
                        }
                    }
                    if (curField.ForEditor && nxtField.ForEditor && curField.EditorPosID == nxtField.EditorPosID)
                    {
                        if (curField.EditorPosTimeStamp > nxtField.EditorPosTimeStamp)
                        {
                            curField.SetEditorPosID(GetEditorMinPosID());
                        }
                        else
                        {
                            nxtField.SetEditorPosID(GetEditorMinPosID());
                        }
                    }
                }
            }
        }
    }

    public class TableLocation
    {
        public string ExcelPath;
        public string[] SheetName;
    }

    public class CheckerInfo
    {
        public bool Enable;
        public string CheckerType;
        public string[] CheckArgs;
    }

    public static class TableInfoUtil
    {
        public static bool IsBoolean(this TableFieldInfo fieldInfo)
        {
            return fieldInfo.FieldTypeName == "bool";
        }

        public static bool IsNumber(this TableFieldInfo fieldInfo)
        {
            return fieldInfo.FieldTypeName == "int" ||
                   fieldInfo.FieldTypeName == "uint" ||
                   fieldInfo.FieldTypeName == "float" ||
                   fieldInfo.FieldTypeName == "double" ||
                   fieldInfo.FieldTypeName == "long long";
        }

        private static string MatchVectorObject(this TableFieldInfo fieldInfo)
        {
            string text = fieldInfo.FieldTypeName;
            Regex reg = new Regex(@"(?<=\<).+(?=\>)");
            string arrayStr = "";

            var mc = reg.Match(text);
            bool result = mc.Success;
            while (mc.Success)
            {
                text = mc.Value;
                mc = reg.Match(text);
                arrayStr = arrayStr + "[]";
            }

            string[] list = text.Split(',');
            if (list.Length > 0)
            {
                text = list[0].Trim();
            }

            if (text == "int" ||
                   text == "uint" ||
                   text == "float" ||
                   text == "double" ||
                   text == "long long")
            {
                text = "number";
            }
            if (text == "bool")
            {
                text = "boolean";
            }

            return text + arrayStr;
        }

        public static string GetEmmyLuaType(this TableFieldInfo fieldInfo)
        {
            if (fieldInfo.IsNumber())
            {
                return "number";
            }
            if (fieldInfo.IsBoolean())
            {
                return "boolean";
            }
            return fieldInfo.MatchVectorObject();
        }

        public static bool IsString(this TableFieldInfo fieldInfo)
        {
            return fieldInfo.FieldTypeName == "string";
        }

        public static bool IsVector(this TableFieldInfo fieldInfo)
        {
            return fieldInfo.FieldTypeName.StartsWith("vector");
        }

        public static bool IsSequence(this TableFieldInfo fieldInfo)
        {
            return fieldInfo.FieldTypeName.StartsWith("Sequence");
        }

       

        //public static void Save(this TableInfo tableInfo)
        //{
        //    JsonExtesion.SaveObject(Path.Combine(ExcelUtil.ExcelConfigPath, $"{tableInfo.MainTableName}.json"), tableInfo);
        //}

        //public static string GetFullPath(this TableLocation location)
        //{
        //    return Path.Combine(CSVUtil.CSVPath, location.ExcelPath);
        //}

        /// <summary>
        /// 是否满足表Code条件
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="codeTarget"></param>
        /// <returns></returns>
        public static bool MatchTableCodeTarget(this TableInfo tableInfo, TableCodeTarget codeTarget)
        {
            var targetMask = tableInfo.TableCodeTarget;
            return (Convert.ToUInt32(codeTarget) & targetMask) != 0;
        }

      
    }
}
namespace ExtractOfficialAssets.Process;

public class GenEnumLuaWarp
{
    #region Template

    private static readonly string EnumTemplate = @"using System;
using LuaInterface;
using SDKLib.SDKInterface;

public class MoonClient_{0}
{{
    public static void Register(LuaState L)
    {{
        L.BeginEnum(typeof({0}));

        //L.RegVar(""Announce"", get_Announce, null);
        {1}
        L.RegFunction(""IntToEnum"", IntToEnum);
        L.EndEnum();
        TypeTraits<{0}>.Check = CheckType;
        StackTraits<{0}>.Push = Push;
    }}

    static void Push(IntPtr L, {0} arg)
    {{
        ToLua.Push(L, arg);
    }}

    static bool CheckType(IntPtr L, int pos)
    {{
        return TypeChecker.CheckEnumType(typeof({0}), L, pos);
    }}

    {2}

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    static int IntToEnum(IntPtr L)
    {{
        int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
        var o = ({0})arg0;
        ToLua.Push(L, o);
        return 1;
    }}
}}";

    private static readonly string ClassTemplate = @"using System;
using LuaInterface;
using SDKLib.SDKInterface;

public class MoonClient_{0}
{{
    public static void Register(LuaState L)
    {{
        L.BeginClass(typeof({0}), typeof(System.Object));

        //L.RegVar(""Announce"", get_Announce, null);
        {1}
        L.EndClass();
    }}

    {2}
}}"; 
    #endregion
    public static void Gen<T>() where T : Enum
    {
        var names = Enum.GetNames(typeof(T));
        var initEnum = "";
        var methods = "";
        var enumName = typeof(T).Name.Replace(".", "_");
        foreach (var name in names)
        {
            initEnum += $"\tL.RegVar(\"{name}\", get_{name}, null);\r\n";
            methods += $@"  [MonoPInvokeCallback(typeof(LuaCSFunction))]
    static int get_{name}(IntPtr L)
    {{
        ToLua.Push(L, {typeof(T).Name}.{name});
        return 1;
    }}
";
        }

        var script = string.Format(EnumTemplate, enumName, initEnum, methods);
        Console.WriteLine(script);
    }

    public static void GenClass<T>() where T : class
    {
        var initFields = "";
        var methods = "";
        var className = typeof(T).Name.Replace(".", "_");
        foreach (var f in typeof(T).GetFields())
        {
            initFields += $"\tL.RegVar(\"{f.Name}\", get_{f.Name}, null);\r\n";
            methods  += $@"  [MonoPInvokeCallback(typeof(LuaCSFunction))]
    static int get_{f.Name}(IntPtr L)
    {{
        ToLua.Push(L, {typeof(T).Name}.{f.Name});
        return 1;
    }}
";
            //            methods += $@"  [MonoPInvokeCallback(typeof(LuaCSFunction))]
            //    static int get_{f.Name}(IntPtr L)
            //    {{
            //       object o = null;

            //		try
            //		{{
            //			o = ToLua.ToObject(L, 1);
            //			{typeof(T).Name} obj = ({typeof(T).Name})o;
            //			var ret = obj.{f.Name};
            //			ToLua.PushValue(L, ret);
            //			return 1;
            //		}}
            //		catch(Exception e)
            //		{{
            //			return LuaDLL.toluaL_exception(L, e, o, ""attempt to index instance on a nil value"");
            //		}}
            //    }}
            //";
        }

        var script = string.Format(ClassTemplate, className, initFields, methods);
        Console.WriteLine(script);
    }
}
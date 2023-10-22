using System.Reflection;

namespace ExtractOfficialAssets;

public class AssemblyManager
{
    private static Assembly defaultCSharpAssembly;
    private static Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();

    public static Assembly DefaultCSharpAssembly
    {
        get
        {
            if (AssemblyManager.defaultCSharpAssembly != (Assembly)null)
                return AssemblyManager.defaultCSharpAssembly;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "ToolLib")
                {
                    AssemblyManager.defaultCSharpAssembly = assembly;
                    break;
                }
            }
            return AssemblyManager.defaultCSharpAssembly;
        }
    }

    public static Assembly GetAssembly(string name)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == name)
                return assembly;
        }
        return (Assembly)null;
    }

    public static Type GetAssemblyType(string assembly, string typeName) => AssemblyManager.GetAssembly(assembly)?.GetType(typeName);

    public static Type GetAssemblyType(string typeFullName)
    {
        int num = typeFullName.LastIndexOf(".", StringComparison.Ordinal);
        string assembly = (string)null;
        string typeName;
        if (num > 0)
        {
            assembly = typeFullName.Substring(0, num);
            typeName = typeFullName.Substring(num);
        }
        else
            typeName = typeFullName;
        return assembly == null ? AssemblyManager.GetDefaultAssemblyType(typeName) : AssemblyManager.GetAssemblyType(assembly, typeName);
    }

    public static Type GetDefaultAssemblyType(string typeName) => Assembly.GetAssembly(typeof(AssemblyManager))?.GetType(typeName);

    public static Type[] GetTypeList(string assemblyName) => AssemblyManager.GetAssembly(assemblyName).GetTypes();
}
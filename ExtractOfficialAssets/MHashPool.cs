using System.Diagnostics;

namespace ExtractOfficialAssets;

public class MHashPool<T>
{
    private static readonly MObjectPool<HashSet<T>> s_pool = new MObjectPool<HashSet<T>>(new MObjectPool<HashSet<T>>.CreateObj(MHashPool<T>.Create), (Action<HashSet<T>>)(l => l.Clear()), (Action<HashSet<T>>)(l => l.Clear()));

    private static HashSet<T> Create() => new HashSet<T>();

    public static HashSet<T> Get() => MHashPool<T>.s_pool.Get();

    public static void Release(HashSet<T> toRelease)
    {
        if (toRelease == null)
            throw new Exception("toRelease is null");
        MHashPool<T>.s_pool.Release(toRelease);
    }

    public static void Clear() => MHashPool<T>.s_pool.Clear();
}
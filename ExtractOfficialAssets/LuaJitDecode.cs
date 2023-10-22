using System.Runtime.InteropServices;

namespace ExtractOfficialAssets;

public unsafe class LuaJitDecode
{
    [DllImport("LuaJitDecode")]
    public static extern byte* Decode(byte* fileData, int len, int* outLen);
}
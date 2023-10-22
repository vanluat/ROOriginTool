using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace ExtractOfficialAssets.Process;

public class TextFileProcess
{
    
    private const string ROOT = "Assets/Resources/ResInfo/";

    public static void DumpAnimationRes()
    {
        var respath = $"{ROOT}animInfo.bytes";
        Console.WriteLine(respath);
        var id = TableProcess.GetHash(respath);
        using var stream = File.OpenRead($"bytesblock/{id}.bytes");
        if (stream != null)
        {
            using (BinaryReader sr = new BinaryReader(stream, Encoding.UTF8))
            {
                try
                {
                    IList<ClipInfo> _clipInfoDict=new List<ClipInfo>();
                    int size = sr.ReadInt32();
                    for (int i = 0; i < size; i++)
                    {
                        string path = sr.ReadString();
                        float length = sr.ReadSingle();
                        bool isLoop = sr.ReadBoolean();

                        _clipInfoDict.Add( new ClipInfo(path, length, isLoop));
                    }

                    File.WriteAllText("animInfo.json", JsonConvert.SerializeObject(_clipInfoDict, Formatting.Indented));
                    var uniEx = new UnityResProcess();
                    uniEx.DumpAnimationClip(_clipInfoDict);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        Console.WriteLine("DONE");
    }

}

public class ClipInfo
{
    public ClipInfo(string path, float length, bool isLoop)
    {
        ClipPath=path;
        this.Length = length;
        this.IsLooping = isLoop;
    }

    public string ClipPath { get; private set; }
    public float Length { get; private set; }
    public bool IsLooping { get; private set; }
}
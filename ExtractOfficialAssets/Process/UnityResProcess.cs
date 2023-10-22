using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ExtractOfficialAssets.Process;

public class UnityResProcess
{
    Dictionary<uint, ABData> DepFile = new Dictionary<uint, ABData>();
    void ReadDepFile()
    {
        const string INFO_PATH =
            "F:\\Downloads\\GameLauncher\\ROO_PC\\ro_Data\\StreamingAssets\\AssetBundles\\dep.all";
        using var fstream = File.OpenRead(INFO_PATH);
        using var br = new BinaryReader(fstream);

        ABDataHelper depInfo = new ABDataHelper();

        try
        {
            if (br.ReadChar() == 'A' && br.ReadChar() == 'B' && br.ReadChar() == 'D')
            {
                if (br.ReadChar() == 'T')
                {
                    fstream.Position = 0;
                    depInfo.Read(fstream, false, false);
                }
                else
                {
                    fstream.Position = 0;
                    depInfo.Read(fstream, true, false);
                }
            }

            br.Close();
            DepFile = depInfo.InfoList.ToDictionary(m => m.fullName);
            // extract all data

        }
        catch (Exception e)
        {
            throw;
        }

    }

    public UnityResProcess()
    {
        ReadDepFile();
    }

    public void SaveJson()
    {
        var list = DepFile.ToDictionary(m => m.Key, m => m.Value);
        foreach (var v in DepFile.Values)
        {
            if (v.dependencies != null && v.dependencies.Length > 0)
            {
                v.strDependencies = new string[v.dependencies.Length];
                for (int i = 0; i < v.dependencies.Length; i++)
                {
                    if (list.TryGetValue(v.dependencies[i], out var d))
                    {
                        v.strDependencies[i] = d.assetPath;
                    }
                }
            }
        }
        File.WriteAllText("unityRes.json", JsonConvert.SerializeObject(DepFile, Formatting.Indented));
    }

    public void DumpAnimationClip(IList<ClipInfo> request)
    {
        const string dstFolder = "AnimationExport";
        if (!Directory.Exists(dstFolder))
            Directory.CreateDirectory(dstFolder);
        var processIdx = 0;
        foreach (var clip in request)
        {
            var data = DepFile.Values.FirstOrDefault(m => m.assetPath == $"Resources/{clip.ClipPath}.anim");
            if (data == null)
            {
                Console.WriteLine($"File not found {clip.ClipPath}");
                continue;
            }
            var dstPath = clip.ClipPath.Remove(clip.ClipPath.LastIndexOf("/"));
            if (!Directory.Exists($"{dstFolder}//{dstPath}"))
                Directory.CreateDirectory($"{dstFolder}//{dstPath}");
            File.Copy($"block/{data.fullName}.unity3d", $"{dstFolder}/{clip.ClipPath}.unity3d",true);
            Console.WriteLine($"Exported  {++processIdx}/{request.Count}: {clip.ClipPath}");
        }
    }

    public void SaveAllRes(uint id)
    {
        const string dstFolder = "TmpUnity";
        if (!Directory.Exists(dstFolder))
            Directory.CreateDirectory(dstFolder);
        var idPath = $"{dstFolder}/{id}";
        if (!Directory.Exists(idPath))
            Directory.CreateDirectory(idPath);

        if (DepFile.TryGetValue(id, out var res))
        {
            CopyFile(idPath, res);
        }
    }

    private void CopyFile(string dstPath, ABData res)
    {
        File.Copy($"block/{res.fullName}.unity3d", $"{dstPath}/{res.fullName}.unity3d",true);
        foreach (var dependency in res.dependencies)
        {
            if (DepFile.TryGetValue(dependency, out var dep))
            {
                CopyFile(dstPath, dep);
            }
        }
    }
}
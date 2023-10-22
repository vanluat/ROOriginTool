using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
namespace ExtractOfficialAssets;
public class ABDataHelper
{
    protected List<ABData> _infoList = new List<ABData>(8192);

    public List<ABData> InfoList => this._infoList;

    public ABData GetABData(uint fullName)
    {
        if (fullName == 0U)
            return (ABData)null;
        int num1 = 0;
        int num2 = this._infoList.Count - 1;
        while (num1 <= num2)
        {
            int index = num1 + num2 >> 1;
            uint fullName1 = this._infoList[index].fullName;
            if (fullName > fullName1)
            {
                num1 = index + 1;
            }
            else
            {
                if (fullName >= fullName1)
                    return this._infoList[index];
                num2 = index - 1;
            }
        }
        return (ABData)null;
    }

    private static int infoCompairer(ABData a, ABData b)
    {
        if (a.fullName < b.fullName)
            return -1;
        return a.fullName > b.fullName ? 1 : 0;
    }

    public void Read(Stream fs, bool useBin, bool append)
    {
        if (useBin)
            this.readBin(fs, append);
        else
            this.readText(fs, append);
        this._infoList.Sort(new Comparison<ABData>(ABDataHelper.infoCompairer));
    }

    private void readBin(Stream fs, bool append)
    {
        if (fs.Length < 4L)
            return;
        using (BinaryReader binaryReader = new BinaryReader(fs, Encoding.UTF8))
        {
            char[] chArray = binaryReader.ReadChars(4);
            if (chArray.Length < 4 || chArray[0] != 'A' || chArray[1] != 'B' || chArray[2] != 'D' || chArray[3] != 'B')
                return;
            int num1 = binaryReader.ReadInt32();
            if (!append)
                this._infoList.Clear();
            for (int index1 = 0; index1 < num1; ++index1)
            {
                string str = binaryReader.ReadString();
                uint num2 = binaryReader.ReadUInt32();
                byte num3 = binaryReader.ReadByte();
                int length = binaryReader.ReadInt32();
                uint[] numArray = new uint[length];
                for (int index2 = 0; index2 < length; ++index2)
                    numArray[index2] = binaryReader.ReadUInt32();
                this.addAbData(new ABData()
                {
                    fullName = num2,
                    assetPath = str,
                    dependencies = numArray,
                    compositeType = (ABExportType)num3
                }, append);
            }
            binaryReader.Close();
        }
    }

    private void readText(Stream fs, bool append)
    {
        using (StreamReader streamReader = new StreamReader(fs, Encoding.UTF8, true))
        {
            char[] buffer = new char[6];
            streamReader.Read(buffer, 0, buffer.Length);
            if (buffer.Length < 4 || buffer[0] != 'A' || buffer[1] != 'B' || buffer[2] != 'D' || buffer[3] != 'T')
                return;
            int int32_1 = Convert.ToInt32(streamReader.ReadLine());
            if (!append)
                this._infoList.Clear();
            for (int index1 = 0; index1 < int32_1; ++index1)
            {
                string str = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(str))
                {
                    uint num = uint.Parse(streamReader.ReadLine().Replace(".ab", string.Empty));
                    ABExportType result;
                    Enum.TryParse<ABExportType>(streamReader.ReadLine(), out result);
                    int int32_2 = Convert.ToInt32(streamReader.ReadLine());
                    uint[] numArray = new uint[int32_2];
                    for (int index2 = 0; index2 < int32_2; ++index2)
                        numArray[index2] = uint.Parse(streamReader.ReadLine().Replace(".ab", string.Empty));
                    streamReader.ReadLine();
                    this.addAbData(new ABData()
                    {
                        assetPath = str,
                        fullName = num,
                        dependencies = numArray,
                        compositeType = result
                    }, append);
                }
                else
                    break;
            }
            streamReader.Close();
        }
    }

    private void addAbData(ABData info, bool append)
    {
        if (append)
        {
            ABData abData = this.GetABData(info.fullName);
            if (abData != null)
            {
                abData.compositeType = info.compositeType;
                HashSet<uint> uintSet = MHashPool<uint>.Get();
                for (int index = 0; index < info.dependencies.Length; ++index)
                    uintSet.Add(info.dependencies[index]);
                for (int index = 0; index < abData.dependencies.Length; ++index)
                    uintSet.Add(abData.dependencies[index]);
                abData.dependencies = new uint[uintSet.Count];
                int index1 = 0;
                foreach (uint num in uintSet)
                {
                    abData.dependencies[index1] = num;
                    ++index1;
                }
            }
            else
                this._infoList.Add(info);
        }
        else
            this._infoList.Add(info);
    }

    public void Write(Stream fs, bool useBin)
    {
        if (useBin)
            this.writeBin(fs);
        else
            this.writeText(fs);
    }

    private void writeBin(Stream fs)
    {
        using (BinaryWriter binaryWriter = new BinaryWriter(fs))
        {
            binaryWriter.Write(new char[4] { 'A', 'B', 'D', 'B' });
            binaryWriter.Write(this._infoList.Count);
            for (int index = 0; index < this._infoList.Count; ++index)
            {
                ABData info = this._infoList[index];
                binaryWriter.Write(info.assetPath);
                binaryWriter.Write(info.fullName);
                binaryWriter.Write((byte)info.compositeType);
                binaryWriter.Write(info.dependencies.Length);
                foreach (uint dependency in info.dependencies)
                    binaryWriter.Write(dependency);
            }
            binaryWriter.Close();
        }
    }

    private void writeText(Stream fs)
    {
        using (StreamWriter streamWriter = new StreamWriter(fs))
        {
            streamWriter.WriteLine("ABDT");
            streamWriter.WriteLine(this._infoList.Count.ToString());
            int index = 0;
            for (int count = this._infoList.Count; index < count; ++index)
            {
                ABData info = this._infoList[index];
                if (info != null)
                {
                    streamWriter.WriteLine(info.assetPath);
                    streamWriter.WriteLine(info.fullName.ToString() + ".ab");
                    streamWriter.WriteLine(info.compositeType.ToString());
                    if (info.dependencies == null)
                    {
                        throw new Exception("##### " + info.assetPath);
                    }
                    else
                    {
                        streamWriter.WriteLine(info.dependencies.Length.ToString());
                        foreach (uint dependency in info.dependencies)
                            streamWriter.WriteLine(dependency.ToString() + ".ab");
                        streamWriter.WriteLine("<------------->");
                    }
                }
            }
            streamWriter.Close();
        }
    }
}
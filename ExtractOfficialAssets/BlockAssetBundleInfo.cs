using System.Text;

namespace ExtractOfficialAssets;
public struct BlockAssetBundleInfo : IEquatable<BlockAssetBundleInfo>
{
    public uint fullName;
    public ushort fileId;
    public uint offset;
    public uint length;
    public string FileName;

    public bool Equals(BlockAssetBundleInfo other) => (int)this.fullName == (int)other.fullName && (int)this.fileId == (int)other.fileId && (int)this.offset == (int)other.offset && (int)this.length == (int)other.length;

    public override string ToString()
    {
        StringBuilder stringBuilder = SharedStringBuilder.Get();
        stringBuilder.Append("fullName:").Append(this.fullName).Append(" fileId:").Append(this.fileId).Append(" offset:").Append(this.offset);
        return stringBuilder.ToString();
    }
}
using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace ExtractOfficialAssets;

public class ABData
{
    public uint fullName;
    public string assetPath;
    public ABExportType compositeType;
    public uint[] dependencies;
    public string[] strDependencies;
    public ABData[] dependList;

    public override string ToString() => string.Format("fullName:{0} assetPath:{1} compositeType:{2} dependencies:{3}", (object)this.fullName, (object)this.assetPath, (object)this.compositeType,/* (object)this.dependencies.ConverToString()*/ null);

}
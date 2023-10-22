namespace AssetsUpdate.Features.TableUpdate;

public class FieldTranslateDefine
{
    public string FieldName { get; set; }
    public FileTranslateType FieldType { get; set; }
    public int Index { get; set; }
}

public enum FileTranslateType
{
    DontCare=-2,
    None=-1,
    SingleId=0,
    ArrayId=1
}
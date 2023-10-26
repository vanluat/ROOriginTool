namespace IDASupport;

public class ClassInformation
{
    public string ClassName { get; set; }
    public IDictionary<string,ClassFieldInformation> Fields { get; set; }=new Dictionary<string,ClassFieldInformation>();
}

public class ClassFieldInformation
{
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public int FileIndex { get; set; }
    public bool IsTranslate { get; set; }
}
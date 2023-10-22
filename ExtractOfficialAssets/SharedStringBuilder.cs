using System.Text;

namespace ExtractOfficialAssets;

public class SharedStringBuilder
{
    private static readonly StringBuilder builder = new StringBuilder();

    public static StringBuilder Get()
    {
        SharedStringBuilder.builder.Clear();
        return SharedStringBuilder.builder;
    }
}
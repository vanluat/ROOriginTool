using System.Collections;

namespace AssetsUpdate.Features.TableUpdate;

public class TableDefine
{
    public string Name { get; set; }
    public IList<TableFieldDefine> Fields { get; set; }
    public string CsvPath { get; set; }
}
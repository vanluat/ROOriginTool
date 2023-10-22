using ExtractOfficialAssets;

namespace AssetsUpdate.Features.TableUpdate;

public interface ITableHandler
{
    public string Name { get; }
    public void Process(TableDumpData table,ListView ui);
}
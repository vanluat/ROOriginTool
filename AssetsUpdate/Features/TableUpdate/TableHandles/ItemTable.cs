using ExtractOfficialAssets;
using Newtonsoft.Json;
using NPOI.HSSF.Record.Chart;
using ToolLib.Excel;

namespace AssetsUpdate.Features.TableUpdate.TableHandles;

public class ItemTable:ITableHandler
{
    public string Name => "ItemTable";
    public void Process(TableDumpData table, ListView ui)
    {
        ui.BeginUpdate();
        var desRow=ui.Items[0];
        ui.Items.Clear();
        ui.Items.Add(desRow);

        var tableJsonInfo =
            JsonConvert.DeserializeObject<TableInfo>(
                File.ReadAllText($"{Program.Env.MoonClientConfigPath}\\Table\\Configs\\{Name}.json"));

        foreach (var f in tableJsonInfo.Fields)
        {
            foreach (ColumnHeader columnHeader in ui.Columns)
            {
                if (columnHeader.Text == f.FieldName)
                    columnHeader.Tag = f.ClientPosID;
            }
        }
        for (int i = 0; i < table.RowNumber; i++)
        {
            var r = new ListViewItem();
            for (int j = 0; j < ui.Columns.Count; j++)
            {
                var cellValue = "";
                if(ui.Columns[j].Tag!=null) 
                    cellValue=table.Body[i, table.FieldInfos[(int)ui.Columns[j].Tag].Hash].ToString();

                if (ui.Columns[j].Text == @"ItemName" || ui.Columns[j].Text == @"ItemDescription")
                {
                    if (Program.LanguageDic.TryGetValue(cellValue, out var newVal))
                    {
                        cellValue=newVal;
                    }
                }
                if (j == 0)
                {
                    r.Text = cellValue; 
                }
                else
                {
                    r.SubItems.Add(new ListViewItem.ListViewSubItem(r, cellValue));
                }
            }
            ui.Items.Add(r);
        }
        ui.EndUpdate();
    }
}
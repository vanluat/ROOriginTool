using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Windows.Forms;
using ExtractOfficialAssets;
using Newtonsoft.Json;
using NPOI.HSSF.Record.Chart;
using ToolLib.Excel;
using System.Collections.Generic;

namespace AssetsUpdate.Features.TableUpdate
{
    public partial class TableUpdateCtrl : UserControl, IFeatureControl
    {
        private const string TableLuaDefine = "\\Table\\Ro_Tabel_result\\result.lua";
        private const string CsvPath = "\\Table\\CSV";
        private const string ROOT = "Assets/Resources/Table/";
        private IList<TableDefine> _tables = new List<TableDefine>();
        SynchronizationContext _syncContext;
        private Task? _delayCheckSearchTask;
        private Dictionary<string, ITableHandler> _tableHandlers = new Dictionary<string, ITableHandler>();
        private int _columnSelect = -1;
        private const string ConfigTablePath = "Configs/Tables";

        private Dictionary<string, IList<FieldTranslateDefine>?> _translateDefine =
            new Dictionary<string, IList<FieldTranslateDefine>?>();
        readonly Color _tabDesRowColor = Color.Bisque;
        readonly Color _tabTransRowColor = Color.Aquamarine;
        public TableUpdateCtrl()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            if (!Directory.Exists(ConfigTablePath))
            {
                Directory.CreateDirectory(ConfigTablePath);
            }
        }

        public string Display => "Tables";
        public UserControl Me => this;

        protected override void OnLoad(EventArgs e)
        {
            // load Table Name and field define 
            var lines = File.ReadAllLines($"{Program.Env.MoonClientConfigPath}{TableLuaDefine}");
            // load all Csv

            TableDefine table = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("---@class"))
                {
                    if (table != null)
                    {
                        // end table define
                        _tables.Add(table);
                    }

                    table = new TableDefine()
                    {
                        Name = line.Split(' ').Last(),
                        Fields = new List<TableFieldDefine>()
                    };
                }

                if (table != null && line.StartsWith("---@field"))
                {
                    var sp = line.Split(' ');
                    if (sp.Length < 3)
                    {
                        Debug.WriteLine($"Table field define error {table.Name} - {line}");
                        continue;
                    }
                    table.Fields.Add(new TableFieldDefine()
                    {
                        Name = sp[1],
                        StrType = sp[2]
                    });
                }
            }
            var files = Directory.GetFiles($"{Program.Env.MoonClientConfigPath}{CsvPath}", "*.csv",
                System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileName = file.Replace("/", "\\").Split('\\').Last().Split('.').First().ToLower();
                table = _tables.FirstOrDefault(m => m.Name.ToLower() == fileName);
                if (table != null)
                {
                    table.CsvPath = file;
                }


            }
            // display to list view
            DisplayTableList();

            foreach (var type in GetType().Assembly.GetTypes().Where(m => m.IsClass && typeof(ITableHandler).IsAssignableFrom(m)))
            {
                var handler = (ITableHandler)Activator.CreateInstance(type);
                _tableHandlers[handler.Name.ToLower()] = handler;
            }
        }

        private void DisplayTableList()
        {
            uiTableList.Items.Clear();
            uiTableList.BeginUpdate();
            foreach (var table in _tables.Where(m => m.Name.ToLower().Contains(uiTableSearch.Text.ToLower())))
            {
                uiTableList.Items.Add(table.Name);
            }
            uiTableList.EndUpdate();
            if (uiTableList.Items.Count > 0)
                uiTableList.SelectedIndex = 0;

            uiTableCount.Text = $"{uiTableList.Items.Count:D}";
        }

        private void uiTableSearch_TextChanged(object sender, EventArgs e)
        {
            _delayCheckSearchTask ??= Task.Factory.StartNew(DelayCheck);
        }

        private async void DelayCheck()
        {
            await Task.Delay(200);
            _delayCheckSearchTask = null;
            _syncContext.Post(m =>
            {
                DisplayTableList();
            }, null);
        }

        private void uiTableList_SelectedValueChanged(object sender, EventArgs e)
        {
            var item = uiTableList.SelectedItem;
            if (item == null)
            {
                return;
            }

            uiTableList.Enabled = false;

            var defineTable = _tables.FirstOrDefault(m => m.Name == item.ToString());
            using var g = Graphics.FromHwnd(IntPtr.Zero);
            if (defineTable != null)
            {
                uiTableView.Items.Clear();
                uiTableView.Columns.Clear();
                uiTableView.BeginUpdate();
                var transDefinePath = $@"{ConfigTablePath}/{item}.json";
                List<FieldTranslateDefine> trans = new List<FieldTranslateDefine>();
                if (File.Exists(transDefinePath))
                {
                    _translateDefine[item.ToString().ToLower()] = trans =
                        JsonConvert.DeserializeObject<List<FieldTranslateDefine>>(File.ReadAllText(transDefinePath));
                }
                using (TextFieldParser parser = new TextFieldParser(defineTable.CsvPath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    var idx = 0;
                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[] fields = parser.ReadFields();

                        if (idx == 0)
                        {
                            foreach (var field in fields)
                            {
                                var w = (int)g.MeasureString(field, uiTableView.Font).Width + 30;
                                _ = uiTableView.Columns.Add(new ColumnHeader()
                                {
                                    Text = field,
                                    Width = w
                                });
                            }

                        }
                        else
                        {

                            var r = new ListViewItem();
                            r.UseItemStyleForSubItems = false;

                            for (int j = 0; j < fields.Length; j++)
                            {
                                var color = Color.WhiteSmoke;
                                if (idx == 1)
                                {
                                    color = _tabDesRowColor;
                                    r.BackColor = color;
                                }

                                if (idx > 1)
                                {
                                    if (trans.FirstOrDefault(m => m.FieldName == uiTableView.Columns[j].Text) != null)
                                    {
                                        color = _tabTransRowColor;
                                    }
                                }

                                if (j == 0)
                                {
                                    r.Text = fields[j];
                                }
                                else
                                {
                                    r.SubItems.Add(new ListViewItem.ListViewSubItem(r, fields[j])
                                    {
                                        BackColor = color
                                    });
                                }
                            }

                            uiTableView.Items.Add(r);
                        }

                        idx++;

                    }
                }

                uiRowCount.Text = $"{uiTableView.Items.Count:D}";
                uiTableView.EndUpdate();
            }
            // load old data 
            uiTableList.Enabled = true;

        }

        private void setColumnTranslateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadOfficialTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var resPath = $"{ROOT}{uiTableList.SelectedItem}.bytes";
            var id = GetHash(resPath);
            string officalPath =
                $"{Program.Env.OfficialPath}/bytesblock/{id}.bytes";
            if (!File.Exists(officalPath))
            {
                MessageBox.Show(@"File not found");
                return;
            }
            var tab = TableDataDumper.DumpTable(officalPath);



            ProcessBinaryTable(uiTableList.SelectedItem.ToString(), tab, uiTableView);
            uiRowCount.Text = $"{uiTableView.Items.Count:D}";

        }

        public void ProcessBinaryTable(string tableName, TableDumpData table, ListView ui)
        {
            ui.BeginUpdate();
            var desRow = ui.Items[0];
            ui.Items.Clear();
            ui.Items.Add(desRow);

            var tableJsonInfo =
                JsonConvert.DeserializeObject<TableInfo>(
                    File.ReadAllText($"{Program.Env.MoonClientConfigPath}\\Table\\Configs\\{tableName}.json"));

            // remap column
            foreach (var f in tableJsonInfo.Fields)
            {
                foreach (ColumnHeader columnHeader in ui.Columns)
                {
                    if (columnHeader.Text == f.FieldName)
                        columnHeader.Tag = f.ClientPosID;
                }
            }

            if (!_translateDefine.TryGetValue(tableName.ToLower(), out var fields))
            {
                fields = new List<FieldTranslateDefine>();
            }

            for (int i = 0; i < table.RowNumber; i++)
            {
                var r = new ListViewItem();
                for (int j = 0; j < ui.Columns.Count; j++)
                {
                    var cellValue = "";
                    var field = fields.FirstOrDefault(m => m.FieldName == ui.Columns[j].Text);
                    if (ui.Columns[j].Tag != null)
                    {
                        var pos = (int)ui.Columns[j].Tag;
                        if (field != null)
                        {
                            if (field.Index >= 0)
                            {
                                pos = field.Index;
                            }
                        }
                        if (pos < 0 || table.FieldInfos.Length <= pos)
                        {

                        }
                        else
                            cellValue = table.Body[i, table.FieldInfos[pos].Hash].ToString();
                    }

                    var isTranslate = false;
                    // define translate
                    if (field != null)
                    {
                        isTranslate = true;
                        if(field.FieldType==FileTranslateType.SingleId)
                            if (Program.LanguageDic.TryGetValue(cellValue, out var newVal))
                            {
                                cellValue = newVal;
                            }

                        if (field.FieldType == FileTranslateType.ArrayId)
                        {
                            var sp = cellValue.Split('|');
                            cellValue = "";
                            for (int k = 0; k < sp.Length; k++)
                            {
                                if (k > 0)
                                    cellValue += "|";
                                if (Program.LanguageDic.TryGetValue(sp[k], out var newVal))
                                {
                                    cellValue += newVal;
                                }
                            }
                        }
                    }

                    var color = Color.WhiteSmoke;
                    if (isTranslate)
                    {
                        color = _tabTransRowColor;
                    }

                    r.UseItemStyleForSubItems = false;
                    if (j == 0)
                    {
                        r.Text = cellValue;
                        r.BackColor = color;
                    }
                    else
                    {

                        r.SubItems.Add(new ListViewItem.ListViewSubItem(r, cellValue)
                        {
                            BackColor = color
                        });
                    }
                }

                ui.Items.Add(r);
            }

            ui.EndUpdate();
        }

        public uint GetHash(string str)
        {
            if (str == null)
                return 0;
            uint hash = 0;
            for (int index = 0; index < str.Length; ++index)
                hash = (hash << 5) + hash + (uint)str[index];
            return hash;
        }

        private void uiMenuSetSingleIdTransle_Click(object sender, EventArgs e)
        {
            SetupColumnTranslate(FileTranslateType.SingleId);
        }

        private void SetupColumnTranslate(FileTranslateType type,int index=-1)
        {
            if (uiTableList.SelectedItem == null)
            {
                return;
            }

            if (_columnSelect == -1)
                return;
            var tabName = uiTableList.SelectedItem.ToString();
            var columnName = uiTableView.Columns[_columnSelect].Text;
            if (MessageBox.Show($@"Do you want set ""{columnName}"" column to translate", "Warning",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                uiTableView.BeginUpdate();
                foreach (ListViewItem item in uiTableView.Items)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[_columnSelect].BackColor = _tabTransRowColor;
                }

                if (!_translateDefine.TryGetValue(tabName.ToLower(), out var define))
                {
                    define = new List<FieldTranslateDefine>();
                    _translateDefine.Add(tabName.ToLower(), define);
                }

                var field = define.FirstOrDefault(m => m.FieldName == columnName);
                if (field != null)
                {
                    if (type != FileTranslateType.DontCare)
                        field.FieldType = type;
                    field.Index = index;
                }
                else
                {
                    define.Add(new FieldTranslateDefine()
                    {
                        FieldName = columnName,
                        FieldType = type,
                        Index = index
                    });
                }

                File.WriteAllText($"{ConfigTablePath}/{tabName}.json",
                    JsonConvert.SerializeObject(define, Formatting.Indented));
                uiTableView.EndUpdate();
            }
        }

        private void uiTableView_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            _columnSelect = e.Column;
            uictxColumnMenu.Show(uiTableView, uiTableView.PointToClient(Cursor.Position));
        }

        private void uiMenuOverrideColumnIndex_Click(object sender, EventArgs e)
        {
            using (var dlg = new InputDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetupColumnTranslate(FileTranslateType.None, dlg.Value);
                }
            }
        }

        private void uiMenuSetArrayIdTranslate_Click(object sender, EventArgs e)
        {
            SetupColumnTranslate(FileTranslateType.ArrayId);
        }
    }
}

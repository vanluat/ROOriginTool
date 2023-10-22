using AssetsUpdate.Features;

namespace AssetsUpdate
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            uiMenuChooseLangChina_Click(null, null);
        }

        protected override void OnLoad(EventArgs e)
        {
            uiTableControl.TabPages.Clear();
            foreach (var type in GetType().Assembly.GetTypes().Where(m => typeof(IFeatureControl).IsAssignableFrom(m) && m.IsClass))
            {
                var ctrl = (IFeatureControl)Activator.CreateInstance(type);
                ctrl.Me.Dock = DockStyle.Fill;
                uiTableControl.TabPages.Add(new TabPage(ctrl.Display)
                {
                    Controls = { ctrl.Me }
                });

            }
        }

        private void uiMenuChooseLangChina_Click(object sender, EventArgs e)
        {
            uiMenuChooseLangChina.Checked = true;
            uiMenuChooseLangEnglish.Checked = false;
            Program.LoadLanguage(MGameLanguage.Chinese);
        }

        private void uiMenuChooseLangEnglish_Click(object sender, EventArgs e)
        {
            uiMenuChooseLangChina.Checked=false;
            uiMenuChooseLangEnglish.Checked=true;
            Program.LoadLanguage(MGameLanguage.English);

        }
    }
}
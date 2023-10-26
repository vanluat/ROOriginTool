using ExtractOfficialAssets;
using Newtonsoft.Json;

namespace AssetsUpdate
{
    internal static class Program
    {
        public static EnvironmentConfig? Env { get; private set; }
        public static Dictionary<string, string> LanguageDic { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            LoadConfig();
            Application.Run(new Main());
        }

        private static void LoadConfig()
        {
            Env = JsonConvert.DeserializeObject<EnvironmentConfig>(File.ReadAllText("sys_env.json"));
            // load language 
            // default is china

        }

        public static void LoadLanguage(MGameLanguage language)
        {
            var tmp=
                JsonConvert.DeserializeObject<LanguageWrap>(
                    File.ReadAllText($"{Env.OfficialPath}/{language}_lang.json"));
            LanguageDic =tmp

                .data.ToDictionary(m=>m.id.ToString(),m=>m.val);
            
        }
        public static uint GetKey(string key)
        {
            key = GetOneLineString(key);
            return GetHash(key);
        }
        static string GetOneLineString(string source)
        {
            return source
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t").TrimEnd();
        }
        public static uint GetHash(string str)
        {
            if (str == null) return 0;

            uint hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash << 5) + hash + str[i];
            }

            return hash;
        }
    }

}


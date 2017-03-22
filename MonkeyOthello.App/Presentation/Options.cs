using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Presentation
{
    class Options
    {
        public string Name { get; set; } = "MonkeyOthello V3.0";
        public GameLevel Level { get; set; } = GameLevel.Hard;
        public GameMode Mode { get; set; } = GameMode.HumanVsComputer;

        public readonly static Options Default = new Options();

        private const string fileName = "monkey.lee";

        public void Save()
        {
            File.WriteAllText(fileName, ToText());
        }

        public static Options Load()
        {
            if (File.Exists(fileName))
            {
                try
                {
                    return Parse( File.ReadAllText(fileName) );
                }
                catch
                {
                    //do nothing, ignore the error file
                }
            }
            return Default;
        }

        private string ToText()
        {
            return $"Name={Name}\r\nLevel={Level}\r\nMode={Mode}\r\n";
        }

        private static Options Parse (string text)
        {
            var option = new Options();
            var lines= text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim('\r')).ToArray();
            foreach(var line in lines)
            {
                var sp = line.Split('=');
                if( sp[0].Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    option.Name = sp[1];
                }
                else if (sp[0].Equals("Level", StringComparison.OrdinalIgnoreCase))
                {
                    option.Level = (GameLevel)Enum.Parse(typeof(GameLevel), sp[1]);
                }
                else if (sp[0].Equals("Mode", StringComparison.OrdinalIgnoreCase))
                {
                    option.Mode = (GameMode)Enum.Parse(typeof(GameMode), sp[1]);
                }
            }

            return option;
        }
    }
}

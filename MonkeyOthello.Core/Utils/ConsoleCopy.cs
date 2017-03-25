using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Utils
{
    public class ConsoleCopy
    {
        public static TextWriter Create(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                if (!Directory.Exists("logs\\"))
                {
                    Directory.CreateDirectory("logs\\");
                }
                filePath = $@"logs\{DateTime.Today:yyyy-MM-dd}.log";
            }

            var fileWriter = File.AppendText(filePath);
            fileWriter.AutoFlush = true;
            var doubleWriter = new DoubleWriter(fileWriter, Console.Out);
            // Console.SetOut(doubleWriter);

            return doubleWriter;
        }

        class DoubleWriter : TextWriter
        {
            TextWriter fileWriter;
            TextWriter consoleWriter;

            public DoubleWriter(TextWriter fileWriter, TextWriter consoleWriter)
            {
                this.fileWriter = fileWriter;
                this.consoleWriter = consoleWriter;
                Console.SetOut(this);
            }

            public override Encoding Encoding
            {
                get { return fileWriter.Encoding; }
            }

            public override void Flush()
            {
                fileWriter.Flush();
                consoleWriter.Flush();
            }

            public override void Write(char value)
            {
                fileWriter.Write(value);
                consoleWriter.Write(value);
            }

            protected override void Dispose(bool disposing)
            {
                if (fileWriter != null)
                {
                    fileWriter.Flush();
                    fileWriter.Close();
                    fileWriter.Dispose();
                }

                Console.SetOut(consoleWriter);
                base.Dispose(disposing);
            }
        }

    }
}

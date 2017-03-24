using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Utils
{
    public class ConsoleCopy : IDisposable
    {
        FileStream fileStream;
        StreamWriter fileWriter;
        TextWriter doubleWriter;
        TextWriter oldOut;

        class DoubleWriter : TextWriter
        {

            TextWriter one;
            TextWriter two;

            public DoubleWriter(TextWriter one, TextWriter two)
            {
                this.one = one;
                this.two = two;
            }

            public override Encoding Encoding
            {
                get { return one.Encoding; }
            }

            public override void Flush()
            {
                one.Flush();
                two.Flush();
            }

            public override void Write(char value)
            {
                one.Write(value);
                two.Write(value);
            }

        }

        public ConsoleCopy(string filePath=null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                if (!Directory.Exists("logs\\"))
                {
                    Directory.CreateDirectory("logs\\");
                }
                filePath = $@"logs\{DateTime.Today:yyyy-MM-dd}.log";
            }

            oldOut = Console.Out;

            fileStream = File.Create(filePath);

            fileWriter = new StreamWriter(fileStream);
            fileWriter.AutoFlush = true;

            doubleWriter = new DoubleWriter(fileWriter, oldOut);

            Console.SetOut(doubleWriter);
        }

        public void Flush()
        {
            if (fileWriter != null)
            {
                fileWriter.Flush();
            }
        }

        public void Dispose()
        {
            Console.SetOut(oldOut);
            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
                fileWriter = null;
            }
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }

    }
}

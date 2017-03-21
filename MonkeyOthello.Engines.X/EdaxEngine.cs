using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MonkeyOthello.Engines.X
{
    public class EdaxEngine : BaseEngine
    {
        public const int EndGameDepth = 30;
        public const int WinLoseDepth = 34;

        public override SearchResult Search(BitBoard board, int depth)
        {
            var pattern = board.Draw(ownSymbol: "O", oppSymbol: "X", emptySymbol: "-");

            var empties = board.EmptyPiecesCount();

            var r = (empties <= WinLoseDepth && empties > EndGameDepth ? CallEdax("endgame-search", pattern, -1, 1, empties) :
                (empties <= EndGameDepth ? CallEdax("endgame-search", pattern, -64, 64, empties) :
                (CallEdax("midgame-search", pattern, -64, 64, depth)))
                );

            return r;
        }

        private SearchResult CallEdax(string gameMode, string pattern, int alpha, int beta, int depth)
        {
            var sw = Stopwatch.StartNew();
            var process = new Process();

            var edaxPath = Path.Combine(Environment.CurrentDirectory, @"Tools\edax");
            var edaxFile = Path.Combine(edaxPath, "wEdax-x64.exe");

            process.StartInfo.FileName = "cmd";
            //process.StartInfo.WorkingDirectory = edaxPath;
            //process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            // process.OutputDataReceived += Process_OutputDataReceived;
            //process.OutputDataReceived += (s, e) =>
            //{
            //    if (!string.IsNullOrWhiteSpace(e.Data) && e.Data.Contains("move "))
            //    {
            //        r = ParseResult(e.Data);
            //    }
            //};
            //process.BeginOutputReadLine();

            var input = process.StandardInput;
            var output = process.StandardOutput;

            input.WriteLine($"cd /d \"{edaxPath}\"\n\n");
            input.WriteLine($"wEdax-x64.exe -cassio\n\n");
            while (true)
            {
                var line = output.ReadLine();
                //Console.WriteLine(line);
                if (line.Contains("ready."))
                {
                    break;
                }
                Thread.Sleep(50);
            }

            //input.WriteLine($"-q -cassio");
            //input.Flush();

            var cmd = $"ENGINE-PROTOCOL {gameMode} {pattern}O {alpha} {beta} {depth} 100";
            input.WriteLine(cmd);
            input.Flush();


            var result = string.Empty;

            var foundResult = false;
            while (true)
            {
                var line = output.ReadLine();
                //Console.WriteLine(line);
                if (!string.IsNullOrWhiteSpace(line) && !line.Contains("ready."))
                {
                    result = line;
                    foundResult = true;
                }
                if (foundResult && line.Contains("ready."))
                {
                    break;
                }

                Thread.Sleep(100);
            }


            input.WriteLine($"ENGINE-PROTOCOL quit");
            input.Flush();

            //process.WaitForExit();

            process.Close();

            sw.Stop();
            //analyze output
            var r = ParseResult(result);
            r.TimeSpan = sw.Elapsed;
            r.Message = $" [{gameMode}] {r.Message}";
            r.Process = 1;

            return r;
        }

        private static SearchResult ParseResult(string result)
        {
            var rs = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
            var move = rs[1].Substring(5).ToIndex();
            var score = (int)double.Parse(rs[4].Substring(1).Split(' ')[0]);
            var nodes = int.Parse(rs[6].Substring(5));
            var time = rs[rs.Length - 1];

            return new SearchResult
            {
                Move = move.Value,
                Score = score,
                Nodes = nodes,
                Message = $"{time} | {result}",
            };
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data) && e.Data.Contains("move "))
            {
                var r = ParseResult(e.Data);
            }
        }
    }
}

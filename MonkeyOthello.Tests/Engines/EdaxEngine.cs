using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MonkeyOthello.Tests.Engines
{
    public class EdaxEngine : BaseEngine
    {
        public const int EndGameDepth = 30;

        public override SearchResult Search(BitBoard board, int depth)
        {
            var pattern = board.Draw(ownSymbol: "O", oppSymbol: "X", emptySymbol: "-");

            depth = board.EmptyPiecesCount();
            var gameMode = depth <= EndGameDepth ? "endgame-search" : "midgame-search";
            var r = CallEdax(gameMode, pattern);

            return r;
        }

        private SearchResult CallEdax(string gameMode, string pattern)
        {
            var sw = Stopwatch.StartNew();
            var process = new Process();

            var edaxPath = Path.Combine(Environment.CurrentDirectory, @"edax\");
            var edaxFile = Path.Combine(edaxPath, "wEdax-x64.exe");

            process.StartInfo.FileName = edaxFile;
            process.StartInfo.WorkingDirectory = edaxPath;
            process.StartInfo.Arguments = "-q -cassio";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            var input = process.StandardInput;
            var output = process.StandardOutput;
  


            //input.WriteLine($"cd /d \"{edaxPath}\"\n\n");
            //input.WriteLine($"wEdax-x64.exe -cassio\n\n");
            /*while (true)
            {
                var line = output.ReadLine();
                //Console.WriteLine(line);
                if (line.Contains("ready."))
                {
                    break;
                }
                Thread.Sleep(50);
            }*/ 

            var alpha = -64;
            var beta = 64;
            var depth = 4;
             
            input.WriteLine($"ENGINE-PROTOCOL {gameMode} {pattern}O {alpha} {beta} {depth} 100");
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
            process.Close();

            sw.Stop();
            //analyze output
            var rs = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>x.Trim())
                .ToArray();
            var move = rs[1].Substring(5).ToIndex();
            var score = (int) double.Parse(rs[4].Substring(1).Split(' ')[0]);
            var nodes = ulong.Parse(rs[6].Substring(5));

            return new SearchResult
            {
                Move = move.Value,
                Score = score,
                Nodes = nodes,
                TimeSpan = sw.Elapsed,
                Message = $"{gameMode} {result}",
            }; 
        }
    }
}
